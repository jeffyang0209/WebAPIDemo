using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPIDemo.Models;

namespace WebAPIDemo.Controllers
{
    [RoutePrefix("prod")]
    public class ProductsController : ApiController
    {
        private FabricsEntities db = new FabricsEntities();

        /* 1.GET
         * 2.GetProduct
         * 3.GetProduct(id)
         * 4.GetProduct1
         * 
         * 1、2是一樣的東西
         * 3是拿單筆資料
         * 4加入時會與1或2衝突
         */

        /* 如何關閉swagge cs1591 遺漏可見XML
         */

        public ProductsController()
        {
            db.Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// 取得所有商品
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IQueryable<Product> GetProduct()
        {
            return db.Product.OrderByDescending(p => p.ProductId).Take(10);
        }

        /// <summary>
        /// 取得單筆商品
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <returns></returns>
        // 屬性路由
        // http://localhost:40098/prod/1555
        // 無屬性路由
        // http://localhost:40098/api/products/1556
        //[Route("prod/{id:int}")]
        // Route("{id}")] 全域套用[RoutePrefix("prod")]
        [Route("{id}", Name = "GetProductById")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// 更新商品
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="product">商品資料</param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("{id}")]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// 新增商品
        /// </summary>
        /// <param name="product">商品資料</param>
        /// <returns></returns>
        [ResponseType(typeof(Product))]
        [Route("")]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Product.Add(product);
            db.SaveChanges();

            // 使路由資訊可以對應到GET BY ID
            // ACTION NAME = GetProduct
            return CreatedAtRoute("GetProductById", new { id = product.ProductId }, product);
        }

        /// <summary>
        /// 刪除商品
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <returns></returns>
        [ResponseType(typeof(Product))]
        [Route("{id}")]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Product.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 依據商品ID查詢商品是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ProductExists(int id)
        {
            return db.Product.Count(e => e.ProductId == id) > 0;
        }

        [Route("{id:int}/orderlines")]
        public IHttpActionResult GetProductOrderLines(int id)
        {
            Product product = db.Product.Include("OrderLine").FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.OrderLine.ToList());
        }

        [Route("{id}/httpResponse")]
        public HttpResponseMessage GetHttpResponse(int id)
        {
            var result = db.Product.Where(p => p.ProductId == id).First();
            if (result == null) return new HttpResponseMessage(HttpStatusCode.NoContent);

            return new HttpResponseMessage
            {
                ReasonPhrase = "OKK",
                Content = new ObjectContent<Product>(result, GlobalConfiguration.Configuration.Formatters.JsonFormatter)
            };
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.DTO;
using WebApi.Models;

namespace WebApi.Controllers
{
    //localhost:5000/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly SocialContext _context;
        public ProductsController(SocialContext context)
        {
            _context = context;
        }


        //localhost:5000/api/products
        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            var produts = await _context
                                    .Products
                                    .Select(p => ProductToDTO(p))
                                    .ToListAsync(); //await ile burada satırı bekletiyoruz ki burada product bilgileri geldikten sonra return ok yapmış oluyor.
            //await kısmını eklememiş olsaydık üst satırda bilgi gelmeden önce return geriye dönecekti ve null olacaktı 
            return Ok(produts);
        }

        //localhost:5000/api/products/2
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context
                                    .Products
                                    .FirstOrDefaultAsync(x => x.ProductId == id); //bu satır için veritabanından verileri getirmesi için bu satırda kodu bekletiyoruz ve veriler geldikten sonra bir sonraki satıra geçmektedir.
            if (product == null)
            {
                return NotFound(); //404 durum kodu oluşur
            }
            else
            {
                return Ok(ProductToDTO(product)); //200 durum kodu oluşturulur
            }
        }

        //localhost:5000/api/products  bu url e post sorgusu gönderilirse bu metot çalışır //product bilgisi requestin bodysi içerisinden gönderilecektir.
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product entity)
        {
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = entity.ProductId }, ProductToDTO(entity)); //201 created bilgisini döndürür.
        }

        //localhost:5000/api/products/2   //aynı urlye biz bu sefer update işlemi yapılacaktır.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product entity)
        {
            if (id != entity.ProductId)
                return BadRequest();

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            product.Name = entity.Name;
            product.Price = entity.Price;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return NotFound();
            }

            return NoContent(); //200 durum kodu ile karşımıza geliyor //client tarafına gönderilecek olan bir bilgi yoktur. //204 döner
        }


        //localhost:5000/api/products/2  //aynı şekilde 2 numaralı kaydı siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
        
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static ProductDTO ProductToDTO(Product p)
        {
            return new ProductDTO()
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive
            };
        }
    }
}

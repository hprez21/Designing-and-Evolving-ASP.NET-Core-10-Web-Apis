using Globomantics.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Globomantics.API.Antipatterns
{
    [ApiController]
    [Route("api/productsdemo")]
    //[Route("v1/productsdemo")]
    public class BrittleController : ControllerBase
    {        
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = new List<BrittleProductDto>();

            return Ok(products);
        }

        [HttpGet("all")]
        public IActionResult GetAllUnpaginated()
        {
            var allProducts = Enumerable.Range(1, 10000)
                .Select(i => new BrittleProductDto { ProductId = i });
            return Ok(allProducts);
        }


        [HttpGet("search")]
        public IActionResult Search(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {            
            var allProducts = Enumerable.Range(1, 10000)
                .Select(i => new BrittleProductDto { ProductId = i });
            var paged = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            Response.Headers["X-Total-Count"] = allProducts.Count().ToString();
            Response.Headers["X-Page"] = page.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(paged);
        }



        //[HttpGet("all")]
        //public IActionResult GetAllUnpaginated([FromQuery] int page = 1, 
        //    [FromQuery] int pageSize = 20)
        //{
        //    var allProducts = Enumerable.Range(1, 10000)
        //        .Select(i => new BrittleProductDto { ProductId = i });
        //    return Ok(allProducts.Skip((page - 1) * pageSize).Take(pageSize));
        //}


    }
}

using Globomantics.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Globomantics.API.Antipatterns
{
    [ApiController]
    [Route("api/productsdemo")]    
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
    }
}

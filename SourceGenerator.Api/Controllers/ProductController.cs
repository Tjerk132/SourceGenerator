using Microsoft.AspNetCore.Mvc;
using SourceGenerator.Domain;
using System;
using GeneratedMappers;

namespace SourceGenerator.Api.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpPost("save")]
        public DateTime Save([FromBody] Product product)
        {
            var custom = new CustomProductRecord();
            var dto = new ProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                //Description = product.Description,
            };

            return DateTime.Now; 
        }
    }
}

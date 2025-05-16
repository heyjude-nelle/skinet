using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);

        return await CreatePagedResult(repo, spec, specParams.PageIndex, specParams.PageSize);  
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        await repo.Add(product);
        
        if (await repo.SaveAllAsync())
        {
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        return BadRequest(new { message = "Cannot update this product" });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
    {
       if (product.Id != id || !ProductExists(id))
            return BadRequest(new { message = "Cannot update this product" });

        await repo.Update(product);

        if (await repo.SaveAllAsync())
        {
            return NoContent();
        }

         return BadRequest(new { message = "Problem updating product" });

    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Product>> DeleteProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        await repo.Remove(product);

        if (await repo.SaveAllAsync())
        {
            return NoContent();
        }

         return BadRequest(new { message = "Problem deleting product" });
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {

        var spec = new BrandListSpecification();

        return Ok(await repo.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();
        
        return Ok(await repo.ListAsync(spec));
    }

    private bool ProductExists(int id)
    {
        return repo.Exists(id);
    }

}

using Entities;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Contexts;

namespace Repository.InMemory;

public class ProductGroupRepository : BaseRepository<ProductGroup>, IProductGroupRepository
{
    public ProductGroupRepository(ProductContext ctx) : base(ctx)
    {

    }
    public Task<IQueryable<ProductGroup?>> GetProductGroupsAsync(int productID)
    {
        var query = Context.ProductGroupProducts
           .Include(p => p.ProductGroup)
           .Where(p => p.ProductID == productID)
           .Select(p => p.ProductGroup);
        return Task.FromResult(query);
    }

    public Task<IQueryable<Product?>> GetProductsAsync(int productGroupID)
    {
        var query = Context.ProductGroupProducts
            .Include(p=>p.Product)
            .ThenInclude(b => b.Brand)
            .Where(p => p.ProductGroupID == productGroupID)
            .Select(p => p.Product);
        return Task.FromResult(query);
    }
}
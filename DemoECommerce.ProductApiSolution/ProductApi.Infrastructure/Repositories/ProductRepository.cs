using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                var product = await GetByAsync(_ => _.Name!.Equals(entity.Name));

                if (product is not null && !string.IsNullOrEmpty(product.Name))
                    return new Response(false, $"{entity.Name} already added");

                var result = _context.Add(entity).Entity;
                await _context.SaveChangesAsync();

                if (result is not null && result.Id > 0)
                    return new Response(true, $"{entity.Name} is added successfully");

                return new Response(false, $"Error occurred while adding {entity.Name}");
            }
            catch(Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error occurred while adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);

                if (product is null)
                    return new Response(false, $"{entity.Name} not found");

                _context.Products.Remove(entity);
                await _context.SaveChangesAsync();

                return new Response(true, $"{entity.Name} is deleted successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error occurred while deleting product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occurred while retrieving product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await _context.Products.AsNoTracking().ToListAsync();

                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new InvalidOperationException("Error occurred while retrieving product");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await _context.Products.Where(predicate).FirstOrDefaultAsync();

                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new InvalidOperationException("Error occurred while retrieving product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);

                if (product is null)
                    return new Response(false, $"{entity.Name} not found");

                _context.Entry(product).State = EntityState.Detached;
                _context.Products.Update(entity);
                await _context.SaveChangesAsync();

                return new Response(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false ,"Error occurred while updating product");
            }
        }
    }
}

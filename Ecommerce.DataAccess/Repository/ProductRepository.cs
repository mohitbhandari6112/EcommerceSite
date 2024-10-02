using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public void Update(Product obj)
        {
            var ProductFromDb = _db.Products.FirstOrDefault(x => x.Id==obj.Id);
            if (ProductFromDb != null)
            {
                ProductFromDb.ISBN = obj.ISBN;
                ProductFromDb.Title=obj.Title;
                ProductFromDb.Author = obj.Author;
                ProductFromDb.Description = obj.Description;
                ProductFromDb.ListPrice = obj.ListPrice;
                ProductFromDb.Price50 = obj.Price50;
                ProductFromDb.Price100 = obj.Price100;
                ProductFromDb.Price = obj.Price;
                ProductFromDb.CategoryId = obj.CategoryId;
                if(obj.ImageUrl != null)
                {
                    ProductFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}

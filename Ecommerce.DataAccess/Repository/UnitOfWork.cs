﻿using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork

    {
        public IProductRepository Product { get;  private set; }
        public ICategoryRepository Category { get; private set; }

        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category=new CategoryRepository(db);
            Product=new ProductRepository(db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
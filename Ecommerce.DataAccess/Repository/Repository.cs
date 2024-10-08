using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ecommerce.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> DbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.DbSet=_db.Set<T>();
            _db.Products.Include(u => u.Category);
        }
        public void Add(T item)
        {
            DbSet.Add(item);
        }

        public T Get(Expression<Func<T, bool>> filter, string? IncludeProperties = null,bool tracked=false)
        {
            IQueryable<T> query;

            if (tracked)
            {
                query = DbSet;
            }
            else {
                query = DbSet.AsNoTracking();
            }
            query = query.Where(filter);
            if (!String.IsNullOrEmpty(IncludeProperties))
            {
                foreach (var IncludeProp in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(IncludeProp);

                }
            }
           
            return query.FirstOrDefault();
        }

        //Category,covertype
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null,string? IncludeProperties=null)
        {
   
            IQueryable<T> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!String.IsNullOrEmpty(IncludeProperties))
            {
                foreach (var IncludeProp  in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(IncludeProp);

                }
            }
            return query.ToList();
        }

        public void Remove(T item)
        {
            DbSet.Remove(item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            DbSet.RemoveRange(items);
        }
    }
}

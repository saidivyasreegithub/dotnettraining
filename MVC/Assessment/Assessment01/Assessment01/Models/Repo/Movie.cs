
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _2_Code_first_Approach.Models;
using System.Data.Entity;
using Assessment01.Models;

namespace _2_Code_first_Approach.Models.Repository
{
    public class MovieRepo<T> : IMovieRepo<T> where T : class
    {
        MovieContext db;
        DbSet<T> dbset;

        public MovieRepo()
        {
            db = new MovieContext();
            dbset = db.Set<T>();
        }
        public void Delete(Object Id)
        {
            T getmodel = dbset.Find(Id);
            dbset.Remove(getmodel);
        }

        public IEnumerable<T> GetAll()
        {
            return dbset.ToList();
        }

        public T GetById(object Id)
        {
            return dbset.Find(Id);
        }

        public void Insert(T obj)
        {
            dbset.Add(obj);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void Update(T obj)
        {
            db.Entry(obj).State = EntityState.Modified;
        }

    }
}
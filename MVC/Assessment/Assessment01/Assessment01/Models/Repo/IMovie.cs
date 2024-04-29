using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assessment01.Models.Repo
{
   
        public interface IMovieRepository<T> where T : class
        {
            IEnumerable<T> GetAll();
            T GetById(int id);
            void Insert(T obj);
            void Update(T obj);
            void Delete(int id);
            void Save();
        }
    

}

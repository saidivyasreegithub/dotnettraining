using MVC_CodeFirst.Models;

namespace _2_Code_first_Approach.Models.Repository
{
    public interface IMovieRepo<T> where T : class
    {
        object GetAll();
        void Insert(Movie p);
        void Save();
        object GetById(int id);
        void Update(Movie p);
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment01
{
    // IMovieRepository.cs
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetAllMovies();
        Movie GetMovieById(int id);
        void AddMovie(Movie movie);
        void UpdateMovie(Movie movie);
        void DeleteMovie(int id);
        IEnumerable<Movie> GetMoviesReleasedInYear(int year);
    }

    // MovieRepository.cs
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext _context;

        public MovieRepository(MovieDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Movie> GetAllMovies()
        {
            return _context.Movies.ToList();
        }

        public Movie GetMovieById(int id)
        {
            return _context.Movies.Find(id);
        }

        public void AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            _context.SaveChanges();
        }

        public void UpdateMovie(Movie movie)
        {
            _context.Entry(movie).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            _context.Movies.Remove(movie);
            _context.SaveChanges();
        }

        public IEnumerable<Movie> GetMoviesReleasedInYear(int year)
        {
            return _context.Movies.Where(m => m.DateOfRelease.Year == year).ToList();
        }
    }

}

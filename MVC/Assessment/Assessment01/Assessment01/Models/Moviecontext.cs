using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Assessment01.Models.Repo;
using Assessment01.Models;
using MVC_CodeFirst.Models;

namespace Assessment01.Models
{
    public class MovieContext : DbContext
    {





        public MovieContext() : base("connectstr")
        { }
        public DbSet<Movie> Movies { get; set; }

    }
}


       
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MVC_CodeFirst.Models
{
    public class Movie
    {
        [Key]
        public int Mid { get; set; }

        [Required(ErrorMessage = "Movie name is required")]
        public string MovieName { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        [Display(Name = "Date of Release")]
        public DateTime DateOfRelease { get; set; }
    }
}

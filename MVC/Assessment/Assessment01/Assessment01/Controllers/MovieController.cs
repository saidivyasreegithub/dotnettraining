using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2_Code_first_Approach.Models.Repository;
using Assessment01.Models;
using Assessment01.Models.Repo;
using MVC_CodeFirst.Models;

namespace _2_Code_first_Approach.Controllers
{
    public class MovieController : Controller
    {
        IMovieRepo<Movie> _movrepo = null;

        public MovieController()
        {
            _movrepo = new MovieRepo<Movie>();
        }
        // GET: Products
        public ActionResult Index()
        {
            var product = _movrepo.GetAll();
            return View(product);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Movie p)
        {
            if (ModelState.IsValid)
            {
                _movrepo.Insert(p);
                _movrepo.Save();
                return RedirectToAction("Index");
            }
            return View(p);
        }

        public ActionResult Edit(int Id)
        {
            var product = _movrepo.GetById(Id);
            return View(product);
        }

        [HttpPost]

        public ActionResult Edit(Movie p)
        {
            if (ModelState.IsValid)
            {
                _movrepo.Update(p);
                _movrepo.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(p);
            }
        }

        public ActionResult Details(int id)
        {
            var product = _movrepo.GetById(id);
            return View(product);
        }

        public ActionResult Delete(int Id)
        {
            var product = _movrepo.GetById(Id);
            return View(product);
        }
    }
}
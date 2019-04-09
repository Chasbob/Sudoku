using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sudoku.View_Models;
namespace Sudoku.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        // GET: Admin/Details/
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            SudokuOutputView model = new SudokuOutputView();
            string[] OutputSudoku = new string[80];

            Sudo.Sudo NewSudoku = new Sudo.Sudo();
            NewSudoku.start();
           OutputSudoku = NewSudoku.FullSudoku;
            model.Values = OutputSudoku;
            return View("sudoku",model);
        }
        // GET: Admin/Edit/
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}

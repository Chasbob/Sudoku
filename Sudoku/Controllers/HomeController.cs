using Sudoku.View_Models;
using Sudo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sudoku.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            SudokuListView model = GetListViewModel();

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult SudokuFail()
        {
            return View();
        }
        public ActionResult SudokuSuccess()
        {
            var temp = View();
            return View("SudokuSuccess");
        }


        /// <summary>
        /// Gets all times with the SudokuID of 'ID' and stores them in a 'Leaderboard'
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Leaderboard(int id)
        {
            SudoCRUD GetTimes = new SudoCRUD();
            List<SudoCRUD.LeaderboardEntry> LeaderboardList = GetTimes.GetLeaderboard(id);
            SudokuLeaderboard Output = new SudokuLeaderboard();
            foreach (var item in LeaderboardList)
            {
                LeaderboardEntry temp = new LeaderboardEntry();
                temp.SudokuID = item.SudokuID;
                temp.TimeTaken = TimeSpan.Parse(item.TimeTaken);
                temp.UserName = item.UserName;
                Output.AddEntryToList(temp);
            }

            return View(Output);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Test()
        {
            ViewBag.message = "A test page";
            return View();
        }
        /// <summary>
        /// declares 'StartTime' within the session as equal to the current time
        /// and passes ID and Difficulty to SudokuForView
        /// then passes the returned string to StringToArray
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Difficulty"></param>
        /// <returns></returns>
        public ActionResult Sudoku(int id, string Difficulty)
        {
            Session["StartTime"] = DateTime.Now;

            SudokuOutputView model = new SudokuOutputView();
            SudoCRUD s = new SudoCRUD();
            Sudo.Sudo s2 = new Sudo.Sudo();
            string[] temp = s.SudokuForView(id, Difficulty);

            model.Values = s2.StringToArray(temp[1]);
            model.SudokuID = id;
            return View(model);
        }
        /// <summary>
        /// takes the html table from the sudoku view and joins all the values into a csv called solution
        /// it then runs CheckSudoku with solution and SudokuID
        /// if it returns true then it passes the timespan of the users session, SudokuID and the username to SaveTime
        /// and redirects them to SudokuSuccess view
        /// if it returns false then it redirects them to SudokuFailure
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="SudokuID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitSudoku(int[] ID, int SudokuID)
        {
            string Solution = string.Join(",", ID); ;
            bool Result;
            SudoCRUD s = new SudoCRUD();
            //Solution = s.ArrayToString(ID);

            Result = s.CheckSudoku(Solution, SudokuID);
            if (Result)
            {
                DateTime StartTime = (DateTime)Session["StartTime"];
                TimeSpan Duration = DateTime.Now - StartTime;
                string UserName = User.Identity.Name;
                s.SaveTime(SudokuID, Duration, UserName);
                return RedirectToAction("SudokuSuccess");
            }
            else
            {
                return RedirectToAction("SudokuFail");
            }

        }
        /// <summary>
        /// gets all sudokus and their creation date and stores them in an IDictionary which is a list of integers and DateTimes
        /// and returns the IDictionary
        /// </summary>
        /// <returns></returns>
        private SudokuListView GetListViewModel()
        {
            string userID = User.Identity.Name;

            SudokuListView List = new SudokuListView();
            SudoCRUD s = new SudoCRUD();
            IDictionary<int, DateTime> DBList = s.SudokuList();
            foreach (var item in DBList)
            {
                SudokuModel temp = new SudokuModel();
                temp.ID = item.Key;
                temp.CreationDate = item.Value;
                List.AddSudoToList(temp);
            }

            return List;
        }
    }
}
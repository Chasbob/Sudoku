using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.View_Models
{
    public class SudokuLeaderboard
    {
        public List<LeaderboardEntry> ListOfEntries { get { return _ListOfEntries; } }
        private List<LeaderboardEntry> _ListOfEntries = new List<LeaderboardEntry>();
        public void AddEntryToList(LeaderboardEntry Entry)
        {
            _ListOfEntries.Add(Entry);
        }
    }
}
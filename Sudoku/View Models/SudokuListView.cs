using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.View_Models
{
    public class SudokuListView
    {
        List<SudokuModel> _list = new List<SudokuModel>();
        public List<SudokuModel> ListOfSudokus { get { return _list; } }
        public void AddSudoToList(SudokuModel sudokumodel)
        {
            _list.Add(sudokumodel);

        }
    }
}
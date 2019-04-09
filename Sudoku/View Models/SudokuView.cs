using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sudo;

namespace Sudoku.View_Models
{
    public class SudokuOutputView
    {
        public string[] Values = new string[81];
        public int SudokuID { get; set; }
    }

}
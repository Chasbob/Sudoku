using Sudoku.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class SudokuModel
    {
        int ID;
        DateTime CreationDate;
        string CellValues;

        public SudokuOutputView GetCells()
        {
            SudokuOutputView Output = new SudokuOutputView();

            int counter = new int();
            string[] cells = CellValues.Split(',');
            foreach (var Number in cells)
            {
                
                Output.Values[counter] = Number;

            }
            return Output;
        }
    }
    
}
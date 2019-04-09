using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Sudo;

namespace Sudo
{
    public class Sudo
    {
        
        private string[] _fullSudoko = new string[81];
        public string[] FullSudoku { get { return _fullSudoko; } }

        private string[] _problemSudoko = new string[81];
        public string[] ProblemSudoku { get { return _problemSudoko; } }

        private string[] _EasySudoku = new string[81];
        public string[] EasySudoku { get { return _EasySudoku; } }

        private string[] _MediumSudoku = new string[81];
        public string[] MediumSudoku { get { return _MediumSudoku; } }

        private string[] _HardSudoku = new string[81];
        public string[] HardSudoku { get { return _HardSudoku; } }

        private IDictionary<int, DateTime> _SudokuDBList = new Dictionary<int, DateTime>();
        public IDictionary<int,DateTime> SudokuDBList { get { return _SudokuDBList; } }
        List<Square> tempProblem = new List<Square>();
        List<Square> tempFull = new List<Square>();


        public List<Square> Sudoku = new List<Square>();

        Random r = new Random();

        private Square Clearvalues(Square Temp, int index)
        {
            Temp.Column = 0;
            Temp.Row = 0;
            Temp.Value = 0;
            Temp.Index = index;
            Temp.Region = 0;
            return Temp;
        }
        /// <summary>
        /// It will take a list of squares and pass the value property of each item in the list and pass it into the corresponding element of an array
        /// The array will then be returned
        /// </summary>
        /// <param name="InputGrid"></param>
        /// <returns></returns>
        public string[] SquareToArray(List<Square> InputGrid)
        {
            string[] OutputGrid=new string[81];
            for (int i = 0; i < 81; i++)
            {
                OutputGrid[i] = InputGrid[i].Value.ToString();
                //_fullSudoko[i] = tempFull[i].Value.ToString();

            }
            return OutputGrid;
        }
        
        /// <summary>
        /// used to generate a solution and problem then save them to the database
        /// </summary>
        public void start()
        {
            List<Square> temp = GenerateGrid();
            //SquareToArray();
            SudoCRUD CRUD = new SudoCRUD();
          CRUD.SaveToDatabase( ArrayToString(_fullSudoko) , ArrayToString(_EasySudoku),ArrayToString(_MediumSudoku),ArrayToString(_HardSudoku));
            
        }
        /// <summary>
        /// Takes a string and splits it at every comma into an array of strings
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public string[] StringToArray(string InputString)
        {
            string[] splitter = { "," };
            string[] OutputArray = InputString.Split(splitter,StringSplitOptions.RemoveEmptyEntries);
            return OutputArray;
        }
        /// <summary>
        /// Takes an array of strings and concatenates the elements into one string while seperating them with commas
        /// </summary>
        /// <param name="StringArray"></param>
        /// <returns></returns>
        public string ArrayToString(string[] StringArray)
        {
            return string.Join(",", StringArray);
        }

        /// <summary>
        /// Creates a grid in the form of a list of squares that produces the numbers for a sudoku
        /// </summary>
        /// <returns></returns>
        public List<Square> GenerateGrid()
        {
            
            Sudoku.Clear();
            //Clear();
            //List<Square> Squares = new List<Square>();
            for (int i = 0; i <= 80; i++)
            {
                Square temp = new Square();
                Clearvalues(temp, i);
                tempFull.Add(temp);
            }
            //an arraylist of squares: see line 86
            List<int>[] Available = new List<int>[81];
            //an arraylist of generic lists (nested lists)
            //we use this to keep track of what numbers we can still use in what squares
            int c = 0;
            //use this to count the square we are up to

            for (int x = 0; x <= Available.Length - 1; x++)
            {
                Available[x] = new List<int>();
                for (int i = 1; i <= 9; i++)
                {
                    Available[x].Add(i);
                }
            }

            //we want to fill every square object with values
            while (c < 81)
            {

                //if every number has been tried and failed then backtrack
                if (!(Available[c].Count == 0))
                {
                    int i = GetRan(0, Available[c].Count - 1);
                    int z = Available[c][i];
                    //do a check with the proposed number
                    if (Conflicts(tempFull, Item(c, z)) == false)
                    {
                        tempFull[c] = Item(c, z);
                        //this number works so we add it to the list of numbers
                        Available[c].RemoveAt(i);
                        //we also remove it from its individual list
                        c += 1;
                        //move to the next number
                    }
                    else
                    {
                        Available[c].RemoveAt(i);
                        // this number conflicts so we remove it from its list
                    }
                }
                else
                {
                    //forget anything about the current square
                    for (int y = 1; y <= 9; y++)
                    {
                        Available[c].Add(y);
                        //by resetting its available numbers
                    }
                    Clearvalues(tempFull[c - 1], c - 1);
                    //go back and retry a different number 
                    c -= 1;
                    //in the previous square
                }
            }
            int j;
            // this produces the output  list of squares
            for (j = 0; j <= 80; j++)
            {
                Sudoku.Add(tempFull[j]);
            }
            List<Square> tempEasy = new List<Square>();
            tempEasy.AddRange(tempFull.ToList());

            List<Square> tempMedium = new List<Square>();
            tempMedium.AddRange(tempFull.ToList());

            List<Square> tempHard = new List<Square>();
            tempHard.AddRange(tempFull.ToList());
            
            
            _fullSudoko = SquareToArray(tempFull);
            //tempProblem = CreateProblem(tempFull);
          
            _HardSudoku = SquareToArray(CreateHardProblem(tempHard));
            
            _MediumSudoku = SquareToArray(CreateMediumProblem(tempMedium));

            _EasySudoku = SquareToArray(CreateEasyProblem(tempEasy));

            return Sudoku;
            
// originaly passed to the site in the squares structure but was unnessiary. So it was restructured into an array of strings and then returned

        }

        
     
        /// <summary>
        /// removes 20 numbers from the solution to create an easy problem
        /// </summary>
        /// <param name="Completed"></param>
        /// <returns></returns>
        public List<Square> CreateEasyProblem(List<Square> Completed)
        {
            int Pos;
            List<int> removed = new List<int>();
            Square testing = new Square();

            int i;
            for (i = 0; i <= 19; i++)
            {
                NewNumber:
                Pos = GetRan(0, 80);
                if (removed.Contains(Pos))
                {
                    goto NewNumber;
                }
                testing.Value = 0;
                Completed[Pos] = testing;
                removed.Add(Pos);

            }
            return Completed;
        }
        /// <summary>
        /// removes 30 numbers from solution to create a medium problem
        /// </summary>
        /// <param name="Completed"></param>
        /// <returns></returns>
        public List<Square> CreateMediumProblem(List<Square> Completed)
        {
            int Pos;
            List<int> removed = new List<int>();
            Square testing = new Square();

            int i;
            for (i = 0; i <= 29; i++)
            {
                NewNumber:
                Pos = GetRan(0, 80);
                if (removed.Contains(Pos))
                {
                    goto NewNumber;
                }
                testing.Value = 0;
                Completed[Pos] = testing;
                removed.Add(Pos);

            }
            return Completed;
        }
        /// <summary>
        /// removes 40 numbers from solution to create a hard problem
        /// </summary>
        /// <param name="Completed"></param>
        /// <returns></returns>
        public List<Square> CreateHardProblem(List<Square> Completed)
        {
            int Pos;
            List<int> removed = new List<int>();
            Square testing = new Square();

            int i;
            for (i = 0; i <= 39; i++)
            {
                NewNumber:
                Pos = GetRan(0, 80);
                if (removed.Contains(Pos))
                {
                    goto NewNumber;
                }
                testing.Value = 0;
                Completed[Pos] = testing;
                removed.Add(Pos);

            }
            return Completed;
        }


        public void Clear()
        {
            Sudoku.Clear();
        }

        /// <summary>
        /// returns a random number between "lower" and "upper"
        /// </summary>
        /// <param name="lower">lower bound for result</param>
        /// <param name="upper">upper bound for result</param>
        /// <returns></returns>
        private int GetRan(int lower, int upper)
        {
            return r.Next(lower, upper + 1);
        }
        /// <summary>
        /// Loops through a list squares to check if the 'Column', 'Row' or 'region' properties 
        /// of that element match that of the test square as long as they are not 0
        /// If any of the criteria are met then it will check if the value properties are equal and return 'True'
        /// Else it will return 'False'
        /// </summary>
        /// <param name="CurrentValues"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public bool Conflicts(List<Square> CurrentValues, Square test)
        {

            foreach (Square s in CurrentValues)
            {

                if ((s.Column != 0 & s.Column == test.Column) || (s.Row != 0 & s.Row == test.Row) || (s.Region != 0 & s.Region == test.Region))
                {
                    if (s.Value == test.Value)
                    {
                        return true;
                    }
                }
            }

            return false;
            //return false;
        }
        /// <summary>
        /// The structure squre stores 5 different integer values to represent one cell of a sudoku grid
        /// </summary>
        public class Square
        {
            public int Column;
            public int Row;
            public int Region;
            public int Value;
            public int Index;
        }
        /// <summary>
        /// Takes two integers n and v and returns a square with attributes:
        ///  -across, down and region calculated from n
        ///  -value of v
        ///  -index of n - 1
        /// </summary>
        /// <param name="n"> the index of the square </param>
        /// <param name="v"> the value of the square</param>
        /// <returns></returns>
        public Square Item(int n, int v)
        {
            Square temp = new Square();
            n += 1;
            temp.Column = GetColumn(n);
            temp.Row = GetRow(n);
            temp.Region = GetRegion(n);
            temp.Value = v;
            temp.Index = n - 1;
            return temp;
        }

        /// <summary>
        /// Divides n by 9 and returns the remainder to one digit unless it is 0, then it returns 9
        /// this is used to check what row it falls in a sudoku grid
        /// This is to calculate the Column that this cell falls in
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int GetColumn(int n)
        {
            int k;
            k = n % 9;
            if (k == 0)
                return 9;
            else
                return k;
        }
        /// <summary>
        /// If n is 9 it calculates how many times 9 goes into it. Otherwise it calulates how many times 9 goes into it and then adds 1
        /// 
        /// This is to calculate the row that this cell falls in
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int GetRow(int n)
        {
            int k;
            if (GetColumn(n) == 9)
            {
                k = n / 9;
            }
            else
            {
                k = n / 9 + 1;
            }
            return k;
        }
        /// <summary>
        /// Calculates which Row and Column n is in to calulate the 3x3 region it is in, within the 9x9 grid
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int GetRegion(int n)
        {
            int k = 0;
            int a = GetColumn(n);
            int d = GetRow(n);

            if (1 <= a & a < 4 & 1 <= d & d < 4)
            {
                k = 1;
            }
            else if (4 <= a & a < 7 & 1 <= d & d < 4)
            {
                k = 2;
            }
            else if (7 <= a & a < 10 & 1 <= d & d < 4)
            {
                k = 3;
            }
            else if (1 <= a & a < 4 & 4 <= d & d < 7)
            {
                k = 4;
            }
            else if (4 <= a & a < 7 & 4 <= d & d < 7)
            {
                k = 5;
            }
            else if (7 <= a & a < 10 & 4 <= d & d < 7)
            {
                k = 6;
            }
            else if (1 <= a & a < 4 & 7 <= d & d < 10)
            {
                k = 7;
            }
            else if (4 <= a & a < 7 & 7 <= d & d < 10)
            {
                k = 8;
            }
            else if (7 <= a & a < 10 & 7 <= d & d < 10)
            {
                k = 9;
            }
            return k;
        }
    }
}


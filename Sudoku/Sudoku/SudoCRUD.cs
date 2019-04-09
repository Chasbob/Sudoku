using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudo
{
    public class SudoCRUD
    {
        /// <summary>
        /// Inserts newly created easy,medium,hard and full sudoku into "Sudokus" table with current time as creation time
        /// and auto ID
        /// </summary>
        /// <param name="fullSudokoString"></param>
        /// <param name="problemSudokoString"></param>
        /// <returns></returns>
        public int SaveToDatabase(string fullSudokoString, string EasySudokoString,string MediumSudokuString,string HardSudokuString)
        {
            int SudoID = 0;
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);
            using (cnn)
            {
                try
                {
                    cnn.Open();
                    SqlCommand cmd = cnn.CreateCommand();
                    cmd.CommandText = "AddNewSudoku";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FullSudoku", fullSudokoString);
                    cmd.Parameters.AddWithValue("@EasySudoku", EasySudokoString);
                    cmd.Parameters.AddWithValue("@MediumSudoku", MediumSudokuString);
                    cmd.Parameters.AddWithValue("@HardSudoku", HardSudokuString);
                    cmd.Parameters.Add("@ID", SqlDbType.Int);
                    cmd.Parameters["@ID"].Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    SudoID = (int)cmd.Parameters["@ID"].Value;

                }
                catch (SqlException ex)
                {
                    string error = ex.Message;
                }
            }
            return SudoID;
        }
        /// <summary>
        /// inserts SudokuID,Time and Username into SudokuTimes with Time being converted into a string
        /// </summary>
        /// <param name="SudokuID"></param>
        /// <param name="Time"></param>
        /// <param name="UserName"></param>
        public void SaveTime(int SudokuID, TimeSpan Time, string UserName)
        {
            string StringTime = Time.ToString();
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);
            SqlCommand cmd = cnn.CreateCommand();
            cmd.CommandText = "StoreTime";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SudokuID", SudokuID);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            TimeSpan TooLong = TimeSpan.FromHours(24) -TimeSpan.FromSeconds(1);
            
                cmd.Parameters.AddWithValue("@Time", StringTime);
            try
            {
                cnn.Open();
                cmd.ExecuteNonQuery();

                
            }
            catch (SqlException error)
            {
                
                

                throw;
            }

            //throw new NotImplementedException();
        }
        public struct LeaderboardEntry
        {
            public int SudokuID;
            public string UserName;
            public string TimeTaken;
        }
        /// <summary>
        /// selects from SudokuTimes where SudokuId == ID and stores them in a list of <leaderboardEntries> called Leaderboard
        /// it then returns Leaderboard
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<LeaderboardEntry> GetLeaderboard(int ID)
        {
            List<LeaderboardEntry> Leaderboard= new List<LeaderboardEntry>();
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);
            try
            {
                cnn.Open();
                SqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "GetLeaderboard";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SudokuID", ID);
                SqlDataReader d = cmd.ExecuteReader();
                if (d.HasRows)
                {
                    while (d.Read())
                    {
                        LeaderboardEntry temp;
                        temp.SudokuID = (int)d["SudokuID"];
                        temp.UserName = (string)d["UserName"];
                        temp.TimeTaken = (string)d["TimeTaken"];
                        Leaderboard.Add(temp);
                    }
                    
                }
            }
            catch (Exception)
            {

                
            }
            return Leaderboard;
        }
        /// <summary>
        /// selects from Sudoku and stores ID and CreationDate in an IDictionary called List
        /// </summary>
        /// <returns></returns>
        public IDictionary<int, DateTime> SudokuList()
        {
            IDictionary<int, DateTime> List = new Dictionary<int, DateTime>();
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);
            try
            {
                cnn.Open();
                SqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "ListSudokus";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader d = cmd.ExecuteReader();
                if (d.HasRows)
                {
                    while (d.Read())
                    {
                        List.Add((int)d["Id"], (DateTime)d["CreationDate"]);
                    }
                }

            }
            catch (SqlException ex)
            {
                string error = ex.Message;
            }
            return List;
        }
        /// <summary>
        /// difficulty has Sudoku appended to the end and then '(difficulty)+Sudoku' is selected where SudokuID == ID
        /// it then returns the string that was selected
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Difficulty"></param>
        /// <returns></returns>
        public string[] SudokuForView(int ID,string Difficulty)
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);
            string[] Output = new string[2];

            try
            {
                string DifficultyForDB = Difficulty + "Sudoku";
                cnn.Open();
                SqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "SelectSudoku3";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", ID);
                cmd.Parameters.AddWithValue("@ProblemSudoku", DifficultyForDB);
                SqlDataReader d = cmd.ExecuteReader();
                
                if (d.HasRows)
                {
                    while (d.Read())
                    {
                        Output[0] = (string)d["FullSudoku"];
                        Output[1] = (string)d[DifficultyForDB];
                    }
                }
            }
            catch (SqlException ex)
            {
                string error = ex.Message;
            }

            return Output;

        }
        /// <summary>
        /// selects from SudokTimes where SudokuID==SudokuID and where solution==FullSudoku
        /// since there will only ever be one or zero, 1 is intrepreted as true and 0 as false
        /// this boolean is returned
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="sudokuID"></param>
        /// <returns></returns>
        public bool CheckSudoku(string solution, int sudokuID)
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);
            bool Result = false;
            SqlCommand cmd = cnn.CreateCommand();
            cmd.CommandText = "CheckSudoku";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", sudokuID);
            cmd.Parameters.AddWithValue("@Solution", solution);
            cmd.Parameters.Add("@Result", SqlDbType.Bit);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            try 
            {
                cnn.Open();
                cmd.ExecuteNonQuery();
                Result = (bool)cmd.Parameters["@Result"].Value;
            }
            catch (Exception)
            {

                throw;
            }
            return Result;
        }

        public void ListTimes(int id)
        {
            throw new NotImplementedException();
        }
    }
}

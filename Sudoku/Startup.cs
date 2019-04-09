using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Sudoku.Models;
using Owin;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Diagnostics;
using System;

[assembly: OwinStartupAttribute(typeof(Sudoku.Startup))]
namespace Sudoku
{
    public partial class Startup
    {
        public int SudoID { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
			createRolesandUsers();
            //InitSudoku();
		}

        private void InitSudoku()
        {
            Console.WriteLine("Init sudoku");
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);
            using (cnn) 
            try
            {
                cnn.Open();
                SqlCommand cmd = cnn.CreateCommand();
                //cmd.CommandText = "AddNewSudoku";
                cmd.CommandType = CommandType.Text;

                    //string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    //string fileName = "sql.sql";
                    //string path = Path.Combine(Environment.CurrentDirectory,  fileName);
                    string text = @"if not exists (select * from sysobjects where name='Sudokus' and xtype='U')
    CREATE TABLE [dbo].[Sudokus] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [CreationDate] DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    [FullSudoku]   NVARCHAR (200) NOT NULL,
    [EasySudoku]   NVARCHAR (200) NOT NULL,
    [MediumSudoku] NVARCHAR (200) NOT NULL,
    [HardSudoku]   NVARCHAR (200) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([EasySudoku] ASC),
    UNIQUE NONCLUSTERED ([FullSudoku] ASC)
);
if not exists (select * from sysobjects where name='SudokuTimes' and xtype='U')
CREATE TABLE [dbo].[SudokuTimes] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [SudokuID]  INT           NOT NULL,
    [UserName]  NVARCHAR (50) NOT NULL,
    [TimeTaken] NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
go
CREATE PROCEDURE [dbo].[AddNewSudoku]
	@FullSudoku NVARCHAR(200),
	@EasySudoku NVARCHAR(200),
	@MediumSudoku nvarchar(200),
	@HardSudoku nvarchar(200),
	@ID INT OUT
AS
	INSERT INTO dbo.Sudokus(FullSudoku,EasySudoku,MediumSudoku,HardSudoku)
	VALUES(@FullSudoku,@EasySudoku,@MediumSudoku,@HardSudoku)

	SET @ID=SCOPE_IDENTITY()
go
CREATE PROCEDURE [dbo].[CheckSudoku]
	@ID int,
	@Solution nvarchar(200),
	@Result bit out
	
AS
declare @TempID int = ISNULL((SELECT ID FROM Sudokus WHERE ID=@ID AND FullSudoku = @Solution),0)
	if (@TempID = 0)
	 begin
		set @result=0
	 End
	 else
	 begin
		set @Result=1
	 end
go
CREATE PROCEDURE [dbo].[GetLeaderboard]
	@SudokuID int
AS
	SELECT * FROM SudokuTimes WHERE SudokuID=@SudokuID
RETURN 0
go
CREATE PROCEDURE [dbo].[ListSudokus]
	
AS
	SELECT Id, CreationDate FROM dbo.Sudokus
go
CREATE PROCEDURE [dbo].[SelectSudoku]
	@ID int,
	@ProblemSudoku nvarchar(50)
AS
	Declare @sql nvarchar(1000)
	Set @sql = 'SELECT FullSudoku, ' + @ProblemSudoku + ' from dbo.Sudokus where Id=' + CAST(@ID as nvarchar(5))
	EXECUTE sp_executesql @sql
go
Create PROCEDURE [dbo].[SelectSudoku3]
	@ID int,
	@ProblemSudoku nvarchar(50)
AS
	Declare @sql nvarchar(1000)
	Set @sql = 'SELECT FullSudoku, ' + @ProblemSudoku + ' from dbo.Sudokus where Id=' + CAST(@ID as nvarchar(5))
	EXECUTE sp_executesql @sql
go
CREATE PROCEDURE [dbo].[StoreTime]
	@SudokuID int,
	@Time nvarchar(20),
	@UserName nvarchar(50)
AS
	INSERT INTO SudokuTimes (SudokuID,TimeTaken,UserName) values(@SudokuID,@Time,@UserName)
RETURN 0";          //string text = File.ReadAllText(path + @"\sql.sql");

                    cmd.CommandText = text;
                cmd.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {
                string error = ex.Message;
                    Console.WriteLine(error);
                }
        }
		// In this method we will create default User roles and Admin user for login
		private void createRolesandUsers()
		{
			ApplicationDbContext context = new ApplicationDbContext();

			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
			var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


			// In Startup iam creating first Admin Role and creating a default Admin User 
			    if (!roleManager.RoleExists("Admin"))
			{

				// first we create Admin rool
				var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
				role.Name = "Admin";
				roleManager.Create(role);

               
                var user = new ApplicationUser();
                user.UserName = "admin";
                user.Email = "ypph279@gmail.com";

                string userPWD = "Password1!";


                var chkUser = UserManager.Create(user, userPWD);

				//Add default User to Role Admin
				if (chkUser.Succeeded)
				{
					var result1 = UserManager.AddToRole(user.Id, "Admin");

				}
			}

			// creating Creating Manager role 
			if (!roleManager.RoleExists("Daily"))
			{
				var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
				role.Name = "Daily";
				roleManager.Create(role);

			}

			// creating Creating Employee role 
			if (!roleManager.RoleExists("Weekly"))
			{
				var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
				role.Name = "Weekly";
				roleManager.Create(role);

			}
		}
	}
}

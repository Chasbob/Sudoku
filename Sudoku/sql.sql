if not exists (select * from sysobjects where name='Sudokus' and xtype='U')
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

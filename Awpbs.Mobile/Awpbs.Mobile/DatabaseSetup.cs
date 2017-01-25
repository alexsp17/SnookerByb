using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
    /// <summary>
    /// Takes care of setting up the local database
    /// </summary>
    public class DatabaseSetup
    {
        public Exception Exception { get; private set; }

        private string getDbFileName()
        {
            string databaseFileName = System.IO.Path.Combine(App.Files.GetWritableFolder(), Config.DatabaseFileName);
            return databaseFileName;
        }

        public bool DoesDatabaseExist()
        {
            try
            {
                string databaseFileName = this.getDbFileName();
                if (App.Files.DoesFileExist(databaseFileName) == true)
                    return true;
                return false;
            }
            catch (Exception exc)
            {
                Exception = exc;
                return false;
            }
        }

        public bool Install()
        {
            try
            {
                string databaseFileName = this.getDbFileName();

                if (Config.CleanUpTheDatabaseOnStart == true && this.DoesDatabaseExist())
                    App.Files.DeleteFile(databaseFileName);

                if (this.DoesDatabaseExist() == false)
                {
					Mono.Data.Sqlite.SqliteConnection.CreateFile(databaseFileName);
                    //App.Files.CreateDatabaseFile(databaseFileName);
                    createDataTables();
                }

                this.upgradeDataTablesIfNecessary();

                return true;
            }
            catch (Exception exc)
            {
                Exception = exc;
                return false;
            }
        }

        private void upgradeDataTablesIfNecessary()
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;

            try
            {
                conn = new Mono.Data.Sqlite.SqliteConnection("Data Source=" + this.getDbFileName());
                conn.Open();

                if (!this.checkIfColumnExists(conn, "Score", "ExtraData"))
                    this.createColumn(conn, "Score", "ExtraData", "ntext");
            }
            catch (Exception exc)
            {
                throw new Exception("Failed to update data table(s)", exc);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        private bool checkIfColumnExists(Mono.Data.Sqlite.SqliteConnection conn, string tableName, string columnName)
        {
            var command = conn.CreateCommand();
            command.CommandText = string.Format("PRAGMA table_info({0})", tableName);
            var reader = command.ExecuteReader();
            int nameIndex = reader.GetOrdinal("Name");
            while (reader.Read())
            {
                if (reader.GetString(nameIndex).Equals(columnName))
                {
                    return true;
                }
            }
            return false;
        }

        void createColumn(Mono.Data.Sqlite.SqliteConnection conn, string tableName, string columnName, string type)
        {
            var command = conn.CreateCommand();
            command.CommandText = "ALTER TABLE " + tableName + " ADD COLUMN " + columnName + " " + type;
            command.ExecuteNonQuery();
        }

        private void createDataTables()
        {
            // note: sqlite tables always have a hidden "rowid" field

            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = new Mono.Data.Sqlite.SqliteConnection("Data Source=" + this.getDbFileName());
                conn.Open();

                var command = conn.CreateCommand(); // note that AthleteID is not auto-incremented
                command.CommandText = @"
CREATE TABLE [Athlete] (
    [AthleteID] int PRIMARY KEY NOT NULL,
    [EMail] ntext,
    [Name] ntext,
    [IsPro] bit,
    [DOB] date NULL,
    [Gender] int,
    [Country] ntext,
    [MetroID] int,
    [Picture] ntext,
    [FacebookId] ntext,
    [TimeCreated] datetime,
    [TimeModified] datetime,
    [SnookerAbout] ntext
)";
                command.ExecuteNonQuery();

                command.CommandText = @"
CREATE TABLE [Result] (
    [ResultID] integer PRIMARY KEY NOT NULL,
    [AthleteID] int NOT NULL,
    [ResultTypeID] int,
    [Time] float NULL,
    [Distance] float NULL,
    [Count] int NULL,
    [Count2] int NULL,
    [Date] date NULL,
    [Notes] ntext,
    [TimeModified] datetime,
    [Guid] nvarchar(50),
    [IsDeleted] bit NOT NULL,
    [VenueID] int NULL,
    [OpponentAthleteID] int NULL,
    [OpponentConfirmation] int,
    [Type1] int NULL,
    [Details1] ntext
)";
                command.ExecuteNonQuery();

                command.CommandText = @"
CREATE TABLE [Score] (
    [ScoreID] integer PRIMARY KEY NOT NULL,
    [AthleteAID] int NOT NULL,
    [AthleteBID] int NOT NULL,
    [Date] date NOT NULL,
    [IsUnfinished] int NOT NULL,
    [TimeModified] date NOT NULL,
    [Guid] nvarchar(50),
    [IsDeleted] bit NOT NULL,
    [SportID] int NOT NULL,
    [VenueID] int NULL,
    [Type1] int NULL,
    [OpponentConfirmation] int,

    [PointsA] int NOT NULL,
    [PointsB] int NOT NULL,

    [InnerPoints1A] int NOT NULL,
    [InnerPoints1B] int NOT NULL,
    [InnerPoints2A] int NOT NULL,
    [InnerPoints2B] int NOT NULL,
    [InnerPoints3A] int NOT NULL,
    [InnerPoints3B] int NOT NULL,
    [InnerPoints4A] int NOT NULL,
    [InnerPoints4B] int NOT NULL,
    [InnerPoints5A] int NOT NULL,
    [InnerPoints5B] int NOT NULL,
    [InnerPoints6A] int NOT NULL,
    [InnerPoints6B] int NOT NULL,
    [InnerPoints7A] int NOT NULL,
    [InnerPoints7B] int NOT NULL,
    [InnerPoints8A] int NOT NULL,
    [InnerPoints8B] int NOT NULL,
    [InnerPoints9A] int NOT NULL,
    [InnerPoints9B] int NOT NULL,
    [InnerPoints10A] int NOT NULL,
    [InnerPoints10B] int NOT NULL,

    [ExtraData] ntext
)";
                command.ExecuteNonQuery();

                command.CommandText = @"
CREATE TABLE [Singular] (
    [ID] int,
    [DateDbCreated] datetime,
    [MyAthleteID] int,
    [AccessToken] ntext,
    [UserWantsToBeGuest] int
)";
                command.ExecuteNonQuery();

                command.CommandText = @"
INSERT INTO [Singular] (ID,DateDbCreated,MyAthleteID,AccessToken,UserWantsToBeGuest) VALUES (0,@Date,0,'',0)
";
				command.Parameters.Add(new Mono.Data.Sqlite.SqliteParameter("@Date", DateTime.Now.ToUniversalTime()));
                command.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                throw new Exception("Failed to create tables. Message: " + TraceHelper.ExceptionToString(exc));
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

            CreateEmptyAthleteRecord();
        }

        public void CreateEmptyAthleteRecord()
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = new Mono.Data.Sqlite.SqliteConnection("Data Source=" + this.getDbFileName());
                conn.Open();

                var command = conn.CreateCommand();

                // note that AthleteID is not auto-incremented
                command.CommandText = @"
INSERT INTO [Athlete] (AthleteID,EMail,Name,IsPro,Gender,Country,MetroID,TimeModified,TimeCreated,SnookerAbout) 
VALUES 
(0,'','',0,0,'',0,@TimeModified,@TimeCreated,'')
";
                DateTime now = DateTimeHelper.GetUtcNow();
                command.Parameters.Add(new Mono.Data.Sqlite.SqliteParameter("@TimeModified", now));
                command.Parameters.Add(new Mono.Data.Sqlite.SqliteParameter("@TimeCreated", now));
                command.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                throw new Exception("Failed to create Athlete record. Message: " + TraceHelper.ExceptionToString(exc));
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
    }
}

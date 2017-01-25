using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;

namespace Awpbs.Mobile
{
    /// <summary>
    /// The data-access-layer for the local mobile database
    /// </summary>
    public class Repository
    {
        private string getDbFileName()
        {
            string databaseFileName = System.IO.Path.Combine(App.Files.GetWritableFolder(), Config.DatabaseFileName);
            return databaseFileName;
        }

        private SqliteConnection openDbConnection()
        {
            var conn = new Mono.Data.Sqlite.SqliteConnection("Data Source=" + this.getDbFileName());
            conn.Open();
            return conn;
        }

        public void DeleteAllData()
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            Mono.Data.Sqlite.SqliteTransaction trans = null;

            try
            {
                conn = openDbConnection();
                trans = conn.BeginTransaction();

                this.executeNonqueryCommand("DELETE FROM [Athlete]", conn, trans);
                this.executeNonqueryCommand("DELETE FROM [Result]", conn, trans);
                this.executeNonqueryCommand("DELETE FROM [Score]", conn, trans);

                trans.Commit();
            }
            catch (Exception exc)
            {
                if (trans != null)
                    trans.Rollback();
                throw exc;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

            new DatabaseSetup().CreateEmptyAthleteRecord();
        }

        public void SaveSecurity(int myAthleteID, bool userWantsToBeGuest, DateTime timeAthleteCreated)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            Mono.Data.Sqlite.SqliteTransaction trans = null;
            try
            {
                conn = openDbConnection();

                int myOldAthleteID = this.getMyAthleteID(conn);
                bool myOldAthleteRecordAlreadyExists = int.Parse(this.executeScalarCommand("SELECT COUNT(*) FROM Athlete WHERE AthleteID=" + myOldAthleteID, conn, null).ToString()) == 1;
                bool myNewAthleteRecordAlreadyExists = int.Parse(this.executeScalarCommand("SELECT COUNT(*) FROM Athlete WHERE AthleteID=" + myAthleteID, conn, null).ToString()) == 1;

                trans = conn.BeginTransaction();

                // update Singular row
                var command = conn.CreateCommand();
                command.Transaction = trans;
                command.CommandText = "UPDATE Singular SET MyAthleteID=" + myAthleteID + ", UserWantsToBeGuest=" + (userWantsToBeGuest ? "1" : "0");
                //command.CommandText = "UPDATE Singular SET AccessToken=@AccessToken, MyAthleteID=" + myAthleteID + ", UserWantsToBeGuest=" + (userWantsToBeGuest ? "1" : "0");
                //command.Parameters.Add(new SqliteParameter() { ParameterName = "@AccessToken", Value = Crypto.Encrypt(accessToken, "$EFK#$RF!#$#SDFwefasdWE@") });
                command.ExecuteNonQuery();
                command.Dispose();

                if (myAthleteID != myOldAthleteID)
                {
                    // create Athlete row
                    if (myNewAthleteRecordAlreadyExists == false)
                    {
                        createAhlete(myAthleteID, timeAthleteCreated, conn, trans);
                    }

                    // move results and scores
                    command = conn.CreateCommand();
                    command.Transaction = trans;
                    command.CommandText = "UPDATE Result SET AthleteID=" + myAthleteID + " WHERE AthleteID=" + myOldAthleteID;
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE Score SET AthleteAID=" + myAthleteID + " WHERE AthleteAID=" + myOldAthleteID;
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE Score SET AthleteBID=" + myAthleteID + " WHERE AthleteBID=" + myOldAthleteID;
                    command.ExecuteNonQuery();

                    if (myOldAthleteRecordAlreadyExists == true)
                    {
                        this.executeNonqueryCommand("DELETE FROM [Athlete] WHERE AthleteID=" + myOldAthleteID, conn, trans);
                    }
                }

                trans.Commit();
            }
            catch (Exception exc)
            {
                if (trans != null)
                    trans.Rollback();
                throw exc;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        // id will be 0 by default
        // id will be non-0 when the user is registered
        private int getMyAthleteID(Mono.Data.Sqlite.SqliteConnection conn)
        {
            int id = int.Parse(this.executeScalarCommand("SELECT MyAthleteID FROM Singular", conn, null).ToString());
            return id;
        }

        public int GetMyAthleteID()
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();
                return getMyAthleteID(conn);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        private void createAhlete(int athleteID, DateTime timeCreated, SqliteConnection conn, SqliteTransaction trans)
        {
            var command = conn.CreateCommand();
            if (trans != null)
                command.Transaction = trans;
            command.CommandText = "INSERT INTO [Athlete] (AthleteID,Name,EMail,Gender,TimeModified,TimeCreated) VALUES (" + athleteID + ",\"\",\"\",0,@TimeModified,@TimeCreated)";
            command.Parameters.Add(new SqliteParameter("@TimeModified", DateTimeHelper.GetUtcNow()));
			command.Parameters.Add(new SqliteParameter("@TimeCreated", timeCreated));
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public Athlete GetMyAthlete()
        {
            return GetAthlete(GetMyAthleteID());
        }

        public Athlete GetAthlete(int athleteID)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM [Athlete] WHERE AthleteID=" + athleteID;
                var reader = command.ExecuteReader();

                if (!reader.Read())
                    throw new Exception("no records for this athleteID: " + athleteID);

                Athlete athlete = new Athlete();
                athlete.AthleteID = int.Parse(reader["AthleteID"].ToString());
                athlete.UserName = reader["EMail"].ToString();
                athlete.Name = reader["Name"].ToString();
                athlete.Gender = (int)reader["Gender"];
                var country = reader["Country"];
                if (country != null && country != DBNull.Value)
                    athlete.Country = country.ToString();
                athlete.MetroID = (int)reader["MetroID"];
                athlete.DOB = null;
                var dob = reader["DOB"];
                if (dob != DBNull.Value)
                    athlete.DOB = (DateTime)dob;
                athlete.TimeCreated = (DateTime)reader["TimeCreated"];
                athlete.TimeModified = (DateTime)reader["TimeModified"];
                var facebookId = reader["FacebookId"];
                if (facebookId != DBNull.Value)
                    athlete.FacebookId = (string)facebookId;
                var picture = reader["Picture"];
                if (picture != DBNull.Value)
                    athlete.Picture = (string)picture;

                var snookerAbout = reader["SnookerAbout"];
                if (snookerAbout != DBNull.Value)
                    athlete.SnookerAbout = (string)snookerAbout;

                return athlete;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void UpdateAthlete(Athlete athlete)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();

                if (athlete.Name == null)
                    athlete.Name = "";
                if (athlete.UserName == null)
                    athlete.UserName = "";

                var command = conn.CreateCommand();
                command.CommandText = 
                    "UPDATE [Athlete] SET " +
                    " Name=@Name,EMail=@EMail,Gender=" + ((int)athlete.Gender).ToString() + ",DOB=@DOB,Picture=@Picture,FacebookId=@FacebookId,TimeModified=@TimeModified" + 
                    ",Country=@Country,MetroID=" + athlete.MetroID +
                    ",SnookerAbout=@SnookerAbout" +
                    " WHERE AthleteID=" + athlete.AthleteID;
                command.Parameters.Add(new SqliteParameter() { ParameterName = "@Name", Value = athlete.Name });
                command.Parameters.Add(new SqliteParameter() { ParameterName = "@EMail", Value = athlete.UserName });
                if (athlete.Country == null)
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@Country", Value = DBNull.Value });
                else
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@Country", Value = athlete.Country });
                if (athlete.DOB == null)
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@DOB", Value = DBNull.Value });
                else
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@DOB", Value = athlete.DOB.Value });
                if (string.IsNullOrEmpty(athlete.FacebookId))
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@FacebookId", Value = DBNull.Value });
                else
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@FacebookId", Value = athlete.FacebookId });
                if (string.IsNullOrEmpty(athlete.Picture))
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@Picture", Value = DBNull.Value });
                else
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@Picture", Value = athlete.Picture });
                if (athlete.SnookerAbout == null)
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@SnookerAbout", Value = DBNull.Value });
                else
                    command.Parameters.Add(new SqliteParameter() { ParameterName = "@SnookerAbout", Value = athlete.SnookerAbout });
                command.Parameters.Add(new SqliteParameter("@TimeModified", athlete.TimeModified));
                command.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public Result GetMyBestResult()
        {
            int myAthleteID = this.GetMyAthleteID();
            var results = getResults("SELECT * FROM [Result] WHERE AthleteID=" + myAthleteID.ToString() + " ORDER BY Count desc LIMIT 1").ToList();
            if (results == null || results.Count == 0)
                return null;
            return results[0];
        }

        public Result GetResult(int id)
        {
            var results = getResults("SELECT * FROM [Result] WHERE ResultID=" + id);
            return results[0];
        }

        public Result GetLatestResult()
        {
            var results = getResults("SELECT * FROM [Result] ORDER BY Date desc LIMIT 1");
            if (results.Count == 0)
                return null;
            return results[0];
        }

        public void GetSnookerBests(int athleteID, out int? bestBreakPoints, out int? bestBreakBalls, out int? bestFrameScore)
        {
            bestBreakPoints = null;
            bestBreakBalls = null;
            bestFrameScore = null;

            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();

                string sql = "SELECT Count FROM [Result] WHERE AthleteID=" + athleteID + " ORDER BY Count DESC";
                var obj1 = this.executeScalarCommand(sql, conn, null);
                if (obj1 != null && obj1 != DBNull.Value)
                    bestBreakPoints = (int)obj1;

                sql = "SELECT Count2 FROM [Result] WHERE AthleteID=" + athleteID + " ORDER BY Count2 DESC";
                var obj2 = this.executeScalarCommand(sql, conn, null);
                if (obj2 != null && obj2 != DBNull.Value)
                    bestBreakBalls = (int)obj2;

                bestFrameScore = null;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void DeleteResult(int resultID)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();
                this.executeNonqueryCommand("DELETE FROM [Result] WHERE [ResultID]=" + resultID, conn, null);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public List<Result> GetResultsForOtherAthletes()
        {
            int myAthleteID = this.GetMyAthleteID();

            string sql = "SELECT * FROM [Result] WHERE AthleteID!=" + myAthleteID;
            sql += " ORDER BY [Date] DESC, [TimeModified] DESC";

            return getResults(sql);
        }

        public List<Result> GetResults(bool includeDeleted)
        {
            string sql = "SELECT * FROM [Result]";
            if (includeDeleted == false)
                sql += " WHERE IsDeleted=0";

            return getResults(sql);
        }

        public List<int> GetAthleteNamesFromResults(bool includeDeleted)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();

                string sql = "SELECT AthleteID FROM [Result]";
                if (includeDeleted == false)
                    sql += " WHERE IsDeleted=0";
                sql += " ORDER BY [Date] DESC";

                List<int> ids = new List<int>();

                var command = new SqliteCommand(sql, conn);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = (int)reader["AthleteID"];
                    ids.Add(id);
                }

                return ids;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public List<Result> GetResults(int? athleteID, bool includeDeleted, int? resultTypeID = null, int? maxRowsCount = null)
        {
            string sql = "SELECT * FROM [Result] WHERE AthleteID=" + athleteID;
            if (resultTypeID != null)
                sql += " AND ResultTypeID=" + resultTypeID.Value.ToString();
            if (includeDeleted == false)
                sql += " AND IsDeleted=0";
            if (maxRowsCount != null)
                sql += " AND [Date]>1";
            sql += " ORDER BY [Date] DESC, [TimeModified] DESC";
            if (maxRowsCount != null)
                sql += " LIMIT " + maxRowsCount.Value.ToString();

            return getResults(sql);
        }

        private List<Result> getResults(string sql)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();

                var command = conn.CreateCommand();
                command.CommandText = sql;

                var reader = command.ExecuteReader();

                List<Result> results = new List<Result>();
                while (reader.Read())
                {
                    Result result = new Result();
                    result.AthleteID = (int)reader["AthleteID"];
                    result.ResultID = int.Parse(reader["ResultID"].ToString());
                    result.ResultTypeID = (int)reader["ResultTypeID"];
                    result.Date = null;
                    if (reader["Date"] != DBNull.Value)
                        result.Date = (DateTime)reader["Date"];
                    result.Time = null;
                    if (reader["Time"] != DBNull.Value)
                        result.Time = (double)reader["Time"];
                    result.Count = null;
                    if (reader["Count"] != DBNull.Value)
                        result.Count = (int)reader["Count"];
                    result.Count2 = null;
                    if (reader["Count2"] != DBNull.Value)
                        result.Count2 = (int)reader["Count2"];
                    result.Notes = (string)reader["Notes"];
                    result.TimeModified = (DateTime)reader["TimeModified"];
                    result.Guid = Guid.Parse(reader["Guid"].ToString());
                    string isDeleted = reader["IsDeleted"].ToString().ToLower();
                    if (isDeleted == "true")
                        result.IsDeleted = true;
                    else if (isDeleted == "false")
                        result.IsDeleted = false;
                    else
                        result.IsDeleted = int.Parse(isDeleted) > 0;

                    if (reader["OpponentAthleteID"] != DBNull.Value)
                        result.OpponentAthleteID = (int)reader["OpponentAthleteID"];
                    result.OpponentConfirmation = (int)reader["OpponentConfirmation"];

                    if (reader["Type1"] != DBNull.Value)
                        result.Type1 = (int)reader["Type1"];
                    if (reader["Details1"] != DBNull.Value)
                        result.Details1 = (string)reader["Details1"];

                    if (reader["VenueID"] != DBNull.Value)
                        result.VenueID = (int)reader["VenueID"];

                    results.Add(result);
                }

                return results;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void AddResult(Result result)
        {
            if (result.Guid == null || result.Guid == Guid.Empty)
            {
                result.Guid = Guid.NewGuid();
            }

            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();
                this.addResult(conn, null, result);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void UpdateResult(Result result)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();
                this.updateResult(conn, null, result);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void SetIsDeletedOnResult(int resultID, bool isDeleted)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();

                var command = conn.CreateCommand();
                command.CommandText = "UPDATE [Result] SET TimeModified=@TimeModified,IsDeleted=" + (isDeleted ? "1" : "0") + " WHERE ResultID=" + resultID;
                command.Parameters.Add(new SqliteParameter("@TimeModified", DateTimeHelper.GetUtcNow()));
                command.ExecuteNonQuery();
                command.Dispose();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void UpdateAllAthleteResults(int athleteID, List<Result> results)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            Mono.Data.Sqlite.SqliteTransaction trans = null;
            try
            {
                conn = openDbConnection();
                trans = conn.BeginTransaction();

                var command = conn.CreateCommand();
                command.Transaction = trans;
                command.CommandText = "DELETE FROM [Result] WHERE AthleteID=" + athleteID;
                command.ExecuteNonQuery();
                command.Dispose();

                foreach (var result in results)
                {
                    result.AthleteID = athleteID;
                    addResult(conn, trans, result);
                }

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        private void addResult(Mono.Data.Sqlite.SqliteConnection conn, Mono.Data.Sqlite.SqliteTransaction trans, Result result)
        {
            var command = conn.CreateCommand();
            command.Transaction = trans;
            command.CommandText = @"INSERT INTO [Result] ([AthleteID],[ResultTypeID],[Time],[Distance],[Count],[Count2],[Date],[Notes],[TimeModified],[Guid],[IsDeleted],[VenueID],[OpponentAthleteID],[Type1],[Details1],[OpponentConfirmation])
VALUES (@AthleteID,@ResultTypeID,@Time,@Distance,@Count,@Count2,@Date,@Notes,@TimeModified,@Guid,@IsDeleted,@VenueID,@OpponentAthleteID,@Type1,@Details1,@OpponentConfirmation)";
            command.Parameters.Add(new SqliteParameter("@AthleteID", result.AthleteID));
            command.Parameters.Add(new SqliteParameter("@ResultTypeID", result.ResultTypeID));
            if (result.Time != null)
                command.Parameters.Add(new SqliteParameter("@Time", result.Time.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Time", DBNull.Value));
            if (result.Distance != null)
                command.Parameters.Add(new SqliteParameter("@Distance", result.Distance.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Distance", DBNull.Value));
            if (result.Count != null)
                command.Parameters.Add(new SqliteParameter("@Count", result.Count.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Count", DBNull.Value));
            if (result.Count2 != null)
                command.Parameters.Add(new SqliteParameter("@Count2", result.Count2.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Count2", DBNull.Value));
            if (result.Date != null)
                command.Parameters.Add(new SqliteParameter("@Date", result.Date.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Date", DBNull.Value));
            command.Parameters.Add(new SqliteParameter("@Notes", result.Notes ?? ""));
            command.Parameters.Add(new SqliteParameter("@TimeModified", result.TimeModified));
            command.Parameters.Add(new SqliteParameter("@Guid", result.Guid.ToString()));
            command.Parameters.Add(new SqliteParameter("@IsDeleted", result.IsDeleted));
            if (result.VenueID != null)
                command.Parameters.Add(new SqliteParameter("@VenueID", result.VenueID.Value));
            else
                command.Parameters.Add(new SqliteParameter("@VenueID", DBNull.Value));

            if (result.OpponentAthleteID != null)
                command.Parameters.Add(new SqliteParameter("@OpponentAthleteID", result.OpponentAthleteID.Value));
            else
                command.Parameters.Add(new SqliteParameter("@OpponentAthleteID", DBNull.Value));
            if (result.Type1 != null)
                command.Parameters.Add(new SqliteParameter("@Type1", result.Type1.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Type1", DBNull.Value));
            if (result.Details1 != null)
                command.Parameters.Add(new SqliteParameter("@Details1", result.Details1));
            else
                command.Parameters.Add(new SqliteParameter("@Details1", DBNull.Value));
            command.Parameters.Add(new SqliteParameter("@OpponentConfirmation", result.OpponentConfirmation));

            command.ExecuteNonQuery();
            command.Dispose();
        }

        private void updateResult(Mono.Data.Sqlite.SqliteConnection conn, Mono.Data.Sqlite.SqliteTransaction trans, Result result)
        {
            var command = conn.CreateCommand();
            command.Transaction = trans;
            command.CommandText = @"UPDATE [Result] SET [AthleteID]=@AthleteID,[ResultTypeID]=@ResultTypeID,[Time]=@Time,[Distance]=@Distance,[Count]=@Count,[Count2]=@Count2,[Date]=@Date,[Notes]=@Notes,[TimeModified]=@TimeModified,[Guid]=@Guid,[VenueID]=@VenueID,[OpponentAthleteID]=@OpponentAthleteID,[Type1]=@Type1,[Details1]=@Details1,[OpponentConfirmation]=@OpponentConfirmation WHERE ResultID=" + result.ResultID;
            command.Parameters.Add(new SqliteParameter("@AthleteID", result.AthleteID));
            command.Parameters.Add(new SqliteParameter("@ResultTypeID", result.ResultTypeID));
            if (result.Time != null)
                command.Parameters.Add(new SqliteParameter("@Time", result.Time.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Time", DBNull.Value));
            if (result.Distance != null)
                command.Parameters.Add(new SqliteParameter("@Distance", result.Distance.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Distance", DBNull.Value));
            if (result.Count != null)
                command.Parameters.Add(new SqliteParameter("@Count", result.Count.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Count", DBNull.Value));
            if (result.Count2 != null)
                command.Parameters.Add(new SqliteParameter("@Count2", result.Count2.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Count2", DBNull.Value));
            if (result.Date != null)
                command.Parameters.Add(new SqliteParameter("@Date", result.Date.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Date", DBNull.Value));
            command.Parameters.Add(new SqliteParameter("@Notes", result.Notes ?? ""));
            command.Parameters.Add(new SqliteParameter("@TimeModified", result.TimeModified));
            command.Parameters.Add(new SqliteParameter("@Guid", result.Guid.ToString()));
            if (result.VenueID != null)
                command.Parameters.Add(new SqliteParameter("@VenueID", result.VenueID.Value));
            else
                command.Parameters.Add(new SqliteParameter("@VenueID", DBNull.Value));

            if (result.OpponentAthleteID != null)
                command.Parameters.Add(new SqliteParameter("@OpponentAthleteID", result.OpponentAthleteID.Value));
            else
                command.Parameters.Add(new SqliteParameter("@OpponentAthleteID", DBNull.Value));
            if (result.Type1 != null)
                command.Parameters.Add(new SqliteParameter("@Type1", result.Type1.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Type1", DBNull.Value));
            if (result.Details1 != null)
                command.Parameters.Add(new SqliteParameter("@Details1", result.Details1));
            else
                command.Parameters.Add(new SqliteParameter("@Details1", DBNull.Value));
            command.Parameters.Add(new SqliteParameter("@OpponentConfirmation", result.OpponentConfirmation));

            command.ExecuteNonQuery();
            command.Dispose();
        }

        public Score GetScore(int scoreID)
        {
            List<Score> scores = this.getScores("SELECT * FROM [Score] WHERE ScoreID=" + scoreID);
            return scores[0];
        }

        public Score GetLatestScore()
        {
            List<Score> scores = this.getScores("SELECT * FROM [Score] ORDER BY Date desc LIMIT 1");
            if (scores.Count == 0)
                return null;
            return scores[0];
        }

        public List<Score> GetScores(bool includeDeleted, int? sportID = null)
        {
            string sql = "SELECT * FROM [Score]";

            int whereClauses = 0;
            if (sportID != null)
            {
                if (whereClauses == 0)
                    sql += " WHERE ";
                else
                    sql += " AND ";
                sql += " SportID=" + sportID.Value.ToString();
                whereClauses++;
            }
            if (includeDeleted == false)
            {
                if (whereClauses == 0)
                    sql += " WHERE ";
                else
                    sql += " AND ";
                sql += " IsDeleted=0";
                whereClauses++;
            }
            sql += " ORDER BY [Date] DESC, [TimeModified] DESC";

            return this.getScores(sql);
        }

        private List<Score> getScores(string sql)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();

                var command = conn.CreateCommand();
                command.CommandText = sql;

                var reader = command.ExecuteReader();

                List<Score> scores = new List<Score>();
                while (reader.Read())
                {
                    Score score = new Score();
                    score.ScoreID = int.Parse(reader["ScoreID"].ToString());
                    score.AthleteAID = int.Parse(reader["AthleteAID"].ToString());
                    score.AthleteBID = int.Parse(reader["AthleteBID"].ToString());
                    score.Date = (DateTime)reader["Date"];
                    score.IsUnfinished = ((int)reader["IsUnfinished"]) > 0;
                    score.TimeModified = (DateTime)reader["TimeModified"];

                    string guid = reader["Guid"].ToString();
                    score.Guid = Guid.Parse(guid);
                    string isDeleted = reader["IsDeleted"].ToString().ToLower();
                    if (isDeleted == "true")
                        score.IsDeleted = true;
                    else if (isDeleted == "false")
                        score.IsDeleted = false;
                    else
                        score.IsDeleted = int.Parse(isDeleted) > 0;

                    score.SportID = int.Parse(reader["SportID"].ToString());
                    if (reader["VenueID"] != DBNull.Value)
                        score.VenueID = int.Parse(reader["VenueID"].ToString());
                    if (reader["Type1"] != DBNull.Value)
                        score.Type1 = int.Parse(reader["Type1"].ToString());

                    score.AthleteBConfirmation = int.Parse(reader["OpponentConfirmation"].ToString());

                    score.PointsA = int.Parse(reader["PointsA"].ToString());
                    score.PointsB = int.Parse(reader["PointsB"].ToString());

                    score.InnerPoints1A = int.Parse(reader["InnerPoints1A"].ToString());
                    score.InnerPoints1B = int.Parse(reader["InnerPoints1B"].ToString());
                    score.InnerPoints2A = int.Parse(reader["InnerPoints2A"].ToString());
                    score.InnerPoints2B = int.Parse(reader["InnerPoints2B"].ToString());
                    score.InnerPoints3A = int.Parse(reader["InnerPoints3A"].ToString());
                    score.InnerPoints3B = int.Parse(reader["InnerPoints3B"].ToString());
                    score.InnerPoints4A = int.Parse(reader["InnerPoints4A"].ToString());
                    score.InnerPoints4B = int.Parse(reader["InnerPoints4B"].ToString());
                    score.InnerPoints5A = int.Parse(reader["InnerPoints5A"].ToString());
                    score.InnerPoints5B = int.Parse(reader["InnerPoints5B"].ToString());
                    score.InnerPoints6A = int.Parse(reader["InnerPoints6A"].ToString());
                    score.InnerPoints6B = int.Parse(reader["InnerPoints6B"].ToString());
                    score.InnerPoints7A = int.Parse(reader["InnerPoints7A"].ToString());
                    score.InnerPoints7B = int.Parse(reader["InnerPoints7B"].ToString());
                    score.InnerPoints8A = int.Parse(reader["InnerPoints8A"].ToString());
                    score.InnerPoints8B = int.Parse(reader["InnerPoints8B"].ToString());
                    score.InnerPoints9A = int.Parse(reader["InnerPoints9A"].ToString());
                    score.InnerPoints9B = int.Parse(reader["InnerPoints9B"].ToString());
                    score.InnerPoints10A = int.Parse(reader["InnerPoints10A"].ToString());
                    score.InnerPoints10B = int.Parse(reader["InnerPoints10B"].ToString());

                    object extraData = reader["ExtraData"];
                    if (extraData != DBNull.Value)
                        score.ExtraData = (string)extraData;

                    scores.Add(score);
                }

                return scores;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void AddScore(Score score)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();
                this.addScore(conn, null, score);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void SetIsDeletedOnScore(int scoreID, bool isDeleted)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();

                var command = conn.CreateCommand();
                command.CommandText = "UPDATE [Score] SET TimeModified=@TimeModified,IsDeleted=" + (isDeleted ? "1" : "0") + " WHERE ScoreID=" + scoreID;
                command.Parameters.Add(new SqliteParameter("@TimeModified", DateTimeHelper.GetUtcNow()));
                command.ExecuteNonQuery();
                command.Dispose();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        private void addScore(Mono.Data.Sqlite.SqliteConnection conn, Mono.Data.Sqlite.SqliteTransaction trans, Score score)
        {
            string sql = @"
INSERT INTO [Score] ([AthleteAID],[AthleteBID],[Date],[IsUnfinished],[TimeModified],[Guid],[IsDeleted],[SportID],[VenueID],[Type1],[PointsA],[PointsB],[InnerPoints1A],[InnerPoints1B],[InnerPoints2A],[InnerPoints2B],[InnerPoints3A],[InnerPoints3B],[InnerPoints4A],[InnerPoints4B],[InnerPoints5A],[InnerPoints5B],[InnerPoints6A],[InnerPoints6B],[InnerPoints7A],[InnerPoints7B],[InnerPoints8A],[InnerPoints8B],[InnerPoints9A],[InnerPoints9B],[InnerPoints10A],[InnerPoints10B],[OpponentConfirmation],[ExtraData])
VALUES (@AthleteAID,@AthleteBID,@Date,@IsUnfinished,@TimeModified,@Guid,@IsDeleted,@SportID,@VenueID,@Type1,@PointsA,@PointsB,@InnerPoints1A,@InnerPoints1B,@InnerPoints2A,@InnerPoints2B,@InnerPoints3A,@InnerPoints3B,@InnerPoints4A,@InnerPoints4B,@InnerPoints5A,@InnerPoints5B,@InnerPoints6A,@InnerPoints6B,@InnerPoints7A,@InnerPoints7B,@InnerPoints8A,@InnerPoints8B,@InnerPoints9A,@InnerPoints9B,@InnerPoints10A,@InnerPoints10B,@OpponentConfirmation,@ExtraData)";
            var command = createCommandForScore(sql, conn, trans, score);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void UpdateScore(Score score)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            try
            {
                conn = openDbConnection();
                string sql = @"
UPDATE [Score] 
SET [AthleteAID]=@AthleteAID,[AthleteBID]=@AthleteBID,[Date]=@Date,[IsUnfinished]=@IsUnfinished,[TimeModified]=@TimeModified,[Guid]=@Guid,[IsDeleted]=@IsDeleted,[SportID]=@SportID,[VenueID]=@VenueID,[Type1]=@Type1,[PointsA]=@PointsA,[PointsB]=@PointsB,[OpponentConfirmation]=@OpponentConfirmation,[ExtraData]=@ExtraData,
[InnerPoints1A]=@InnerPoints1A,[InnerPoints1B]=@InnerPoints1B,[InnerPoints2A]=@InnerPoints2A,[InnerPoints2B]=@InnerPoints2B,[InnerPoints3A]=@InnerPoints3A,[InnerPoints3B]=@InnerPoints3B,[InnerPoints4A]=@InnerPoints4A,[InnerPoints4B]=@InnerPoints4B,[InnerPoints5A]=@InnerPoints5A,[InnerPoints5B]=@InnerPoints5B,[InnerPoints6A]=@InnerPoints6A,[InnerPoints6B]=@InnerPoints6B,[InnerPoints7A]=@InnerPoints7A,[InnerPoints7B]=@InnerPoints7B,[InnerPoints8A]=@InnerPoints8A,[InnerPoints8B]=@InnerPoints8B,[InnerPoints9A]=@InnerPoints9A,[InnerPoints9B]=@InnerPoints9B,[InnerPoints10A]=@InnerPoints10A,[InnerPoints10B]=@InnerPoints10B
WHERE ScoreID=" + score.ScoreID;
                var command = createCommandForScore(sql, conn, null, score);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void UpdateAllScores(List<Score> scores)
        {
            Mono.Data.Sqlite.SqliteConnection conn = null;
            Mono.Data.Sqlite.SqliteTransaction trans = null;
            try
            {
                conn = openDbConnection();
                trans = conn.BeginTransaction();

                var command = conn.CreateCommand();
                command.Transaction = trans;
                command.CommandText = "DELETE FROM [Score]";
                command.ExecuteNonQuery();
                command.Dispose();

                foreach (var score in scores)
                {
                    this.addScore(conn, trans, score);
                }

                trans.Commit();
            }
            catch (Exception exc)
            {
                trans.Rollback();
                throw exc;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        private SqliteCommand createCommandForScore(string sql, Mono.Data.Sqlite.SqliteConnection conn, Mono.Data.Sqlite.SqliteTransaction trans, Score score)
        {
            var command = conn.CreateCommand();
            command.Transaction = trans;
            command.CommandText = sql;
            command.Parameters.Add(new SqliteParameter("@AthleteAID", score.AthleteAID));
            command.Parameters.Add(new SqliteParameter("@AthleteBID", score.AthleteBID));
            command.Parameters.Add(new SqliteParameter("@Date", score.Date));
            command.Parameters.Add(new SqliteParameter("@IsUnfinished", (int)(score.IsUnfinished ? 1 : 0)));
            command.Parameters.Add(new SqliteParameter("@TimeModified", score.TimeModified));
            command.Parameters.Add(new SqliteParameter("@Guid", score.Guid.ToString()));
            command.Parameters.Add(new SqliteParameter("@IsDeleted", score.IsDeleted));
            command.Parameters.Add(new SqliteParameter("@SportID", score.SportID));
            if (score.VenueID == null)
                command.Parameters.Add(new SqliteParameter("@VenueID", DBNull.Value));
            else
                command.Parameters.Add(new SqliteParameter("@VenueID", score.VenueID));
            if (score.Type1 == null)
                command.Parameters.Add(new SqliteParameter("@Type1", DBNull.Value));
            else
                command.Parameters.Add(new SqliteParameter("@Type1", score.Type1));

            command.Parameters.Add(new SqliteParameter("@PointsA", score.PointsA));
            command.Parameters.Add(new SqliteParameter("@PointsB", score.PointsB));

            for (int i = 1; i <= 10; ++i)
            {
                command.Parameters.Add(new SqliteParameter("@InnerPoints" + i.ToString() + "A", score.InnerPointsA[i - 1]));
                command.Parameters.Add(new SqliteParameter("@InnerPoints" + i.ToString() + "B", score.InnerPointsB[i - 1]));
            }

            command.Parameters.Add(new SqliteParameter("@OpponentConfirmation", score.AthleteBConfirmation));

            if (score.ExtraData == null)
                command.Parameters.Add(new SqliteParameter("@ExtraData", DBNull.Value));
            else
                command.Parameters.Add(new SqliteParameter("@ExtraData", score.ExtraData));

            return command;
        }

        private object executeScalarCommand(string sql, Mono.Data.Sqlite.SqliteConnection conn, Mono.Data.Sqlite.SqliteTransaction trans)
        {
            var command = conn.CreateCommand();
            if (trans != null)
                command.Transaction = trans;
            command.CommandText = sql;
            object result = command.ExecuteScalar();
            command.Dispose();
            return result;
        }

        private void executeNonqueryCommand(string sql, Mono.Data.Sqlite.SqliteConnection conn, Mono.Data.Sqlite.SqliteTransaction trans)
        {
            var command = conn.CreateCommand();
            if (trans != null)
                command.Transaction = trans;
            command.CommandText = sql;
            command.ExecuteNonQuery();
            command.Dispose();
        }
    }
}

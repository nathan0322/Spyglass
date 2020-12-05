using System;
using System.Data.OleDb;
using System.IO;

namespace SpyglassApp.SQL
{

    public class SQLHandler
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\miles\source\repos\SQL-TestApp\Observer Database_2013_backend_RELedits_be.accdb";
        private OleDbConnection dbConnection;
        private Int32 tripID;
        private Int32 dropID;
        private Int32 fishID;
        private Int32 dropNum;
        private bool local;

        public SQLHandler(bool local)
        {
            if (local) {
                this.local = local;

                String localFileName = "local-" + "asdasd" + ".accdb";

                File.Copy("local-template.accdb", localFileName);
                setPath(localFileName);
                this.tripID = 0;
                this.dropID = 0;
                this.fishID = 0;
            } else {
                this.dbConnection = new OleDbConnection(connectionString);
            }
        }

        public void setPath(string path)
        {
            this.connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path;
            this.dbConnection = new OleDbConnection(connectionString);
        }

        public void open()
        {
            if (!local) {
                dbConnection.Open();
                OleDbCommand comm = new OleDbCommand("Select max([Simplified Trip #]) from [Trip Data]");
                tripID = Convert.ToInt32(comm.ExecuteScalar());
                comm = new OleDbCommand("Select max([ID]) from [Drop Data]");
                dropID = Convert.ToInt32(comm.ExecuteScalar());
                comm = new OleDbCommand("Select max([ID]) from [Species Data]");
                fishID = Convert.ToInt32(comm.ExecuteScalar());
            }
        }

        public int insertTrip(string anglers, string date, string year, string port, string vessel, string numObv, string numAnglers, string cap, string cond_sea, string cond_wind, string cond_swell, string notes, string time_depart, string time_return)
        {
            tripID += 1;
            dropNum = 0;

            OleDbCommand comm = new OleDbCommand(
                "INSERT INTO [Trip Data] ([Simplified Trip #], [Observers], [Date], [Year], [Port], [Vessel], [Obs Anglers], [Total Anglers], [Captain], [Conditions- Sea], [Conditions- Sky], [Conditions- Wind], [Conditions- Swell], [Trip Notes], [Time of Departure], [Time of Return])" +
                "VALUES (@tripNum, @anglers, @date, @year, @port, @vessel, @numObv, @numAnglers, @cap, @cond_sea, @cond_wind, @cond_swell, @notes, @time_depart, @time_return)",
                dbConnection
            );
            comm.Parameters.AddRange(new OleDbParameter[] {
                new OleDbParameter("@tripNum", tripID.ToString()),
                new OleDbParameter("@anglers", anglers),
                new OleDbParameter("@date", date),
                new OleDbParameter("@year", year),
                new OleDbParameter("@port", port),
                new OleDbParameter("@vessel", vessel),
                new OleDbParameter("@numObv", numObv),
                new OleDbParameter("@numAnglers", numAnglers),
                new OleDbParameter("@cap", cap),
                new OleDbParameter("@cond_sea", cond_sea),
                new OleDbParameter("@cond_wind", cond_wind),
                new OleDbParameter("@cond_swell", cond_swell),
                new OleDbParameter("@notes", notes),
                new OleDbParameter("@time_depart", time_depart),
                new OleDbParameter("@time_return", time_return)
            });
            comm.ExecuteNonQuery();
            return tripID;
        }

        public void modifyTrip(int mod_tripID, string anglers, string date, string year, string port, string vessel, string numObv, string numAnglers, string cap, string cond_sea, string cond_sky, string cond_wind, string cond_swell, string notes, string time_depart, string time_return)
        {

            OleDbCommand comm = new OleDbCommand(
                "UPDATE [Trip Data] SET [Observers]=@anglers, [Date]=@date, [Year]=@year, [Port]=@port, [Vessel]=@vessel, [Obs Anglers]=@numObv, [Total Anglers]=@numAnglers, [Captain]=@cap, [Conditions- Sea]=@cond_sea, [Conditions- Sky]=@cond_sky, [Conditions- Wind]=@cond_wind, [Conditions- Swell]=@cond_swell, [Trip Notes]=@notes, [Time of Departure]=@time_depart, [Time of Return]=@time_return " +
                "WHERE Simplified Trip #]=@tripNum",
                dbConnection
            );
            comm.Parameters.AddRange(new OleDbParameter[] {
                new OleDbParameter("@tripNum", mod_tripID.ToString()),
                new OleDbParameter("@anglers", anglers),
                new OleDbParameter("@date", date),
                new OleDbParameter("@year", year),
                new OleDbParameter("@port", port),
                new OleDbParameter("@vessel", vessel),
                new OleDbParameter("@numObv", numObv),
                new OleDbParameter("@numAnglers", numAnglers),
                new OleDbParameter("@cap", cap),
                new OleDbParameter("@cond_sea", cond_sea),
                new OleDbParameter("@cond_sky", cond_sky),
                new OleDbParameter("@cond_wind", cond_wind),
                new OleDbParameter("@cond_swell", cond_swell),
                new OleDbParameter("@notes", notes),
                new OleDbParameter("@time_depart", time_depart),
                new OleDbParameter("@time_return", time_return)
            });
            comm.ExecuteNonQuery();
        }

        public void deleteTrip(int del_tripID){
            OleDbCommand comm = new OleDbCommand(
                "DELETE [Trip Data] WHERE [Simplified Trip #]=@tripNum",
                dbConnection
            );
            comm.Parameters.AddRange(new OleDbParameter[] {
                new OleDbParameter("@tripNum", del_tripID.ToString())
            });
            comm.ExecuteNonQuery();
        }

        public int insertDrop(string numObservers, string startLat, string startLon, string endLat, string endLon, string notes, string timeDown, string timeUp, string depth)
        {
            dropID += 1;
            dropNum += 1;

            OleDbCommand comm = new OleDbCommand(
                "INSERT INTO [Drop Data] ([ID], [Trip #], [Drop #], [Obs Fishers], [Start Lat DD], [Start Lon DD], [End Lat DD], [End Lon DD], [DropNotes], [Time Down], [Time Up], [Start Depth(ft)])" +
                "VALUES (@id, @tripNum, @dropNum, @numObservers, @startLat, @startLon, @endLat, @endLon, @notes, @timeDown, @timeUp, @depth)",
                dbConnection
                );
            comm.Parameters.AddRange(new OleDbParameter[]
            {
                new OleDbParameter("@id", dropID.ToString()),
                new OleDbParameter("@tripNum", tripID.ToString()),
                new OleDbParameter("@dropNum", dropNum.ToString()),
                new OleDbParameter("@numObservers", numObservers),
                new OleDbParameter("@startLat", startLat),
                new OleDbParameter("@startLon", startLon),
                new OleDbParameter("@endLat", endLat),
                new OleDbParameter("@endLon", endLon),
                new OleDbParameter("@notes", notes),
                new OleDbParameter("@timeDown", timeDown),
                new OleDbParameter("@timeUp", timeUp),
                new OleDbParameter("@depth", depth),
            });
            comm.ExecuteNonQuery();
            return dropID;
        }

        public void updateDrop(int mod_dropID, string numObservers, string startLat, string startLon, string endLat, string endLon, string notes, string timeDown, string timeUp, string depth)
        {

            OleDbCommand comm = new OleDbCommand(
                "UPDATE [Drop Data] SET [Obs Fishers]=@numObservers, [Start Lat DD]=@startLat, [Start Lon DD]=@startLon, [End Lat DD]=@endLat, [End Lon DD]=@endLon, [DropNotes]=@notes, [Time Down]=@timeDown, [Time Up]=@timeUp, [Start Depth(ft)]=@depth " +
                "WHERE [ID]=@id",
                dbConnection
                );
            comm.Parameters.AddRange(new OleDbParameter[]
            {
                new OleDbParameter("@id", mod_dropID.ToString()),
                new OleDbParameter("@numObservers", numObservers),
                new OleDbParameter("@startLat", startLat),
                new OleDbParameter("@startLon", startLon),
                new OleDbParameter("@endLat", endLat),
                new OleDbParameter("@endLon", endLon),
                new OleDbParameter("@notes", notes),
                new OleDbParameter("@timeDown", timeDown),
                new OleDbParameter("@timeUp", timeUp),
                new OleDbParameter("@depth", depth),
            });
            comm.ExecuteNonQuery();
        }
        public void deleteDrop(int del_dropID){
            OleDbCommand comm = new OleDbCommand(
                "DELETE [Drop Data] WHERE [ID]=@dropNum",
                dbConnection
            );
            comm.Parameters.AddRange(new OleDbParameter[] {
                new OleDbParameter("@dropNum", del_dropID.ToString())
            });
            comm.ExecuteNonQuery();
        }

        public int insertFish(string speciesCode, string len, string fate, string sex, string notes)
        {
            fishID += 1;

            OleDbCommand comm = new OleDbCommand(
                "INSERT INTO [Species Data] ([ID], [Trip #], [Drop #], [Species Code], [Length], [Fate], [Sex], [Notes])" +
                "VALUES (@id, @tripNum, @dropNum, @speciesCode, @len, @fate, @sex, @notes)",
                dbConnection
                );
            comm.Parameters.AddRange(new OleDbParameter[]
            {
                new OleDbParameter("@id", fishID.ToString()),
                new OleDbParameter("@tripNum", tripID.ToString()),
                new OleDbParameter("@dropNum", dropNum.ToString()),
                new OleDbParameter("@speciesCode", speciesCode),
                new OleDbParameter("@len", len),
                new OleDbParameter("@fate", fate),
                new OleDbParameter("@sex", sex),
                new OleDbParameter("@notes", notes),
            });
            comm.ExecuteNonQuery();
            return fishID;
        }

        public void updateFish(int mod_fishID, string speciesCode, string len, string fate, string sex, string notes)
        {
            OleDbCommand comm = new OleDbCommand(
                "UPDATE [Species Data] SET ([Species Code]=@speciesCode, [Length]=@len, [Fate]=@fate, [Sex]=@sex, [Notes]=@notes)" +
                "WHERE [ID]=@id",
                dbConnection
                );
            comm.Parameters.AddRange(new OleDbParameter[]
            {
                new OleDbParameter("@id", mod_fishID.ToString()),
                new OleDbParameter("@speciesCode", speciesCode),
                new OleDbParameter("@len", len),
                new OleDbParameter("@fate", fate),
                new OleDbParameter("@sex", sex),
                new OleDbParameter("@notes", notes),
            });
            comm.ExecuteNonQuery();
        }

        public void deleteFish(int del_fishID){
            OleDbCommand comm = new OleDbCommand(
                "DELETE [Species Data] WHERE [ID]=@id",
                dbConnection
            );
            comm.Parameters.AddRange(new OleDbParameter[] {
                new OleDbParameter("@id", del_fishID.ToString())
            });
            comm.ExecuteNonQuery();
        }

        public void Close() {
            this.dbConnection.Close();
        }
    }
}
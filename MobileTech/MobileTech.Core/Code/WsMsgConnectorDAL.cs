using System.Data;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;
using Mono.Data.Sqlite;
using System;

namespace MobileTech {
    public static class WsMsgConnectorDAL {

        //private static string AssemblyDirectory
        //{
          //  get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName); }
        //}

		static string documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
		static string CONNECTION_STRING = "URI=file:"+Path.Combine(documents,"WsMsgConnectorQueue.sqlite");


        // wsMsgConnector common components
        private static readonly SqliteConnection connection;
//        private static SqliteCommand insertCommand;
//        private static SqliteCommand deleteCommand;
//        private static SqliteCommand updateCommand;


        static WsMsgConnectorDAL() {
            connection = new SqliteConnection(CONNECTION_STRING);
            connection.Open();
        }

		public static void InsertWsMessageConnector(int entityAction, int entityType, string id, string subID,int entityStatus)
		{
			SqliteTransaction transaction = null;
			try
			{
				transaction = connection.BeginTransaction();
//				insertCommand = null;

				using (SqliteCommand sqlQry = connection.CreateCommand()) {
					sqlQry.CommandText = "INSERT INTO WsMsgConnectorQueue (EntityAction, EntityType, EntityKey,EntitySubKey, EntityStatus) " +
						"VALUES (@EntityAction, @EntityType, @EntityKey, @EntityStatus)";
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityAction", DbType = DbType.Int16, Value = entityAction });
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityType", DbType = DbType.Int16, Value = entityType });
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityKey", DbType = DbType.String, Value = id });
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityStatus", DbType = DbType.Int16, Value = entityStatus });
					sqlQry.ExecuteNonQuery();
				}

				transaction.Commit();
			}
			catch(Exception ex)
			{
				//Author/Date/Issue - M.D.Prasad/14th May 2014/ISS - 5395 or CRR - 1338  Upload iGotIt device logs
				string strLogMessage = string.Format("{0} InsertWsMessageConnector Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);

				transaction.Rollback();
				//Console.WriteLine("InsertWsMessageConnector: "+ex.Message);
			}
		}

		public static void InsertWsMessageConnector(int entityAction, int entityType, string id, string eSK)
        {
			using (SqliteCommand sqlQry = connection.CreateCommand()) {
				sqlQry.CommandText = "INSERT INTO WsMsgConnectorQueue (EntityAction, EntityType, EntityKey,EntitySubKey,EntityStatus) VALUES (@EntityAction, @EntityType, @EntityKey,@EntitySubKey,@EntityStatus)";
				sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityAction", DbType = DbType.Int16, Value = entityAction });
				sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityType", DbType = DbType.Int16, Value = entityType });
				sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityKey", DbType = DbType.String, Value = id });
				sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntitySubKey", DbType = DbType.String, Value = eSK });
				sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityStatus", DbType = DbType.Int16, Value = 0 });
				sqlQry.ExecuteNonQuery();
			}

        }

        public static void UpdateWsMessageConnector(int id, int entityStatus)
        {
			using (SqliteCommand sqlQry = connection.CreateCommand()) {
				sqlQry.CommandText = "UPDATE WsMsgConnectorQueue SET EntityStatus = @EntityStatus WHERE Id = @Id ";
				sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityStatus", DbType = DbType.Int16, Value = entityStatus });
				sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@Id", DbType = DbType.Int16, Value = id });
				sqlQry.ExecuteNonQuery();
			}
        }



		/// <summary>
        /// This Method is used to check for successful Messages in the connector Queue
        /// </summary>
        /// <returns>returns true if there exist successful messages, else returns false</returns>
        public static bool CheckWsMessageConnector()
        {
            bool exist = false;
            const string sqlStatement = @" select count(EntityKey) from WsMsgConnectorQueue Where EntityStatus <> 2 ";

            using (SqliteCommand sqlQuery = connection.CreateCommand()){
                sqlQuery.CommandText = sqlStatement;
                object result = sqlQuery.ExecuteScalar();

                if (result != null){
                    if (System.Convert.ToInt32(result) > 0){
                        exist = true;
                    }
                }
            }
            return exist;
        }

        public static void DeleteWsMessageConnector(int id) {
			using (SqliteCommand sqlQry = connection.CreateCommand()) {
				sqlQry.CommandText = "DELETE FROM WsMsgConnectorQueue WHERE Id = @id";
				sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@id", DbType = DbType.Int16, Value = id });
				sqlQry.ExecuteNonQuery ();
			}
        }

		//Added by Srikanth Nuvvula on 14th Oct'2014 for CRR-1411/ISS-4326, Update WorkerSchedule  Start
		public static void InsertUpdateWsMsgConnectorRequestsAvailability(int entityAction, int entityType, string id, string subId, int entityStatus)
		{
			SqliteTransaction transaction = null;
			try
			{
				transaction = connection.BeginTransaction();
				//				insertCommand = null;

				using (SqliteCommand sqlQry = connection.CreateCommand()) {
					sqlQry.CommandText = "DELETE FROM WsMsgConnectorQueue WHERE EntityType = @EntityType";
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityType", DbType = DbType.Int16, Value = entityType });
					sqlQry.ExecuteNonQuery ();
				}


				using (SqliteCommand sqlQry = connection.CreateCommand()) {
					sqlQry.CommandText = "INSERT INTO WsMsgConnectorQueue (EntityAction, EntityType, EntityKey, EntitySubKey, EntityStatus) " +
						"VALUES (@EntityAction, @EntityType, @EntityKey, @EntitySubKey, @EntityStatus)";
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityAction", DbType = DbType.Int16, Value = entityAction });
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityType", DbType = DbType.Int16, Value = entityType });
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityKey", DbType = DbType.String, Value = id });
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntitySubKey", DbType = DbType.String, Value = subId });
					sqlQry.Parameters.Add(new SqliteParameter { ParameterName = "@EntityStatus", DbType = DbType.Int16, Value = entityStatus });
					sqlQry.ExecuteNonQuery();
				}

				transaction.Commit();
			}
			catch(Exception ex)
			{

				string strLogMessage = string.Format("{0} InsertUpdateWsMsgConnectorRequestsAvailability Exception: {1}", DateTime.Now, ex.Message);
				MobileTech.Consts.HandleExceptionLog(strLogMessage);

				transaction.Rollback();
				//Console.WriteLine("InsertWsMessageConnector: "+ex.Message);
			}
		}
    }
}
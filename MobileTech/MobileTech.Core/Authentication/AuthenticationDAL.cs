using System;
using System.Data;
using System.Text;
//using System.Data.SqlClient;
using MobileTech.Core;
using Mono.Data.Sqlite;

namespace MobileTechService
{
	public class AuthenticationDAL
	{
		public AuthenticationDAL ()
		{
		}

		private static String stringFromBytes(SqliteDataReader myReader) {
			const int bufferSize = 32;
			byte[] outbyte = new byte[bufferSize]; // The BLOB byte[] buffer to be filled by GetBytes.
			string str = "";
			const int startValue = 128;
			char[] HexChars = {'€', '', '‚', 'ƒ', '„', '…', '†', '‡'}; //{w,x,y,z,{,|,}, ~ }

			while (myReader.Read()) {
				const long startIndex = 0;
				// Read the bytes into outbyte[] and retain the number of bytes returned.
				myReader.GetBytes(0, startIndex, outbyte, 0, bufferSize);
				Encoding enc = Encoding.ASCII;
				for (int t = 0; t < outbyte.Length; t++) {
					if (outbyte[t] > 127) {
						int ArrVal = outbyte[t] - startValue;
						str = str + HexChars[ArrVal];
					} else {
						Byte[] tempBytes = new[] {outbyte[t]};
						str = str + enc.GetString(tempBytes, 0, tempBytes.Length);
					}
				}
			}
			return str;
		}

		private static bool isPasswordCorrect(string userPass, string dbPass) {
			string strPasswd = "";
			string UserPassword = userPass.PadRight(32, ' ');
			string DBPassword = dbPass;
			const int startValue = 128;
			char[] HexChars = {'€', '', '‚', 'ƒ', '„', '…', '†', '‡'}; //{w,x,y,z,{,|,}, ~ }

			try {
				if (UserPassword.Length == DBPassword.Length) {
					int intIndex = 0;
					while (intIndex < UserPassword.Length) {
						if (UserPassword.Substring(intIndex, 1) == " ") {
							strPasswd = strPasswd + (UserPassword.Substring(intIndex, 1));
						} else {
							//convert each letter in byte array
							Byte[] tempBytes = Encoding.ASCII.GetBytes(UserPassword.Substring(intIndex, 1));
							//add 9 to that byte
							tempBytes = new[] {Convert.ToByte((Convert.ToInt32(tempBytes[0]) + 9))};

							if (tempBytes[0] > 127) {
								int ArrVal = tempBytes[0] - startValue;
								strPasswd = strPasswd + HexChars[ArrVal];
							} else {
								strPasswd = strPasswd + Encoding.ASCII.GetString(tempBytes, 0, tempBytes.Length);
							}
						}
						intIndex = intIndex + 1;
					}

					if (strPasswd == DBPassword) {
						return true;
					}

					return false;
				}

				return false;
			} catch (Exception ex) {
				MobileTech.Consts.LogException (ex);

				throw new Exception(ex.Message);
			}
		}

		public static string AuthenticateUser(String username, String password) {
			bool result;
			// loginPwd = string.Empty;
			try {
				int totalTableRecords = 0;
				string sqlQry = "Select Count(SECPrimaryid) as Count from Security";
				//string sqlQry = "Select usernm from Security";

				SqliteConnection conn = null;
				try
				{

					conn = MobileTech.Consts.GetDBConnection();
					using (SqliteCommand command = conn.CreateCommand()){
						command.CommandType = CommandType.Text;
						command.CommandText = sqlQry;

						SqliteDataReader reader = command.ExecuteReader();
						while (reader.Read()){
							totalTableRecords = Convert.ToInt32(reader.IsDBNull(reader.GetOrdinal("Count")) ? 0 : reader.GetValue(reader.GetOrdinal("Count")));
						}
						reader.Close();
						reader.Dispose();
					}
				}
				finally
				{
					if (conn != null)
						MobileTech.Consts.ReleaseDBConnection(conn);
				}                


				if (totalTableRecords == 0)
				{
					return "EmtypDB";
				}

				string sql = "Select Security.[PASSWORD] from Security Where lower(Security.[UserNm]) = lower(trim(@Username))";

				// end any open sessions for the user first
				//SqlCeConnection conn = null;
				try
				{
					conn = MobileTech.Consts.GetDBConnection();
					using (SqliteCommand command = conn.CreateCommand()){
						command.CommandType = CommandType.Text;
						command.CommandText = sql;
						command.Parameters.Add(new SqliteParameter { ParameterName = "@Username", DbType = DbType.String, Value = username });

						SqliteDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
						String dpPwd = stringFromBytes(reader);

						reader.Close();
						reader.Dispose();

						//loginPwd = dpPwd;

						result = isPasswordCorrect(password, dpPwd);
					}
				}
				finally
				{
					if (conn != null)
						MobileTech.Consts.ReleaseDBConnection(conn);
				}                

			} catch (Exception e) {
				MobileTech.Consts.LogException (e);

				//Console.WriteLine("AuthenticateUser: "+ex.Message);
				result = false;
			}

			return result == true ? "true" : "false";
		}
	}
}


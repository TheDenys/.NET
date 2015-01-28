using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class DbTest
    {
        [Run(false)]
        public static void ConnectionStringBuilderTest()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            //builder.DataSource = "DEN-DELL\\SQLEXPRESS";
            builder.DataSource = "10.140.107.245";
            builder.PersistSecurityInfo = false;
            builder.UserID = "sa";
            builder.Password = "Password01";
            builder.InitialCatalog = "EOC";
            ConsolePrint.print("connestion string = [{0}]", builder.ConnectionString);

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["DatabaseParameters"];
            Console.WriteLine("sett [" + settings + "]");
        }

        [Run(false)]
        public static void TestConnection()
        {
            IDbConnection con = null;
            try
            {
                con = DbUtils.getConnection();
                con.Open();
                SqlCommand command = (SqlCommand)DbUtils.makeCommand("select 1", con);
                var res = command.ExecuteScalar() as int?;
                ConsolePrint.print("res: {0}", res);
                ConsolePrint.print("Connected!!!!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }

        [Run(false)]
        public static void TestSelect()
        {
            IDbConnection con = null;
            try
            {
                con = DbUtils.getConnection();
                con.Open();
                SqlCommand command = (SqlCommand)DbUtils.makeCommand("select FileData from MapStudioFiles", con);
                byte[] res = (byte[])command.ExecuteScalar();
                ConsolePrint.print("res: {0}", res);

                var fiBin = new FileInfo("data.bin");
                using (var stream = fiBin.OpenWrite())
                {
                    stream.Write(res, 0, res.Length);
                    stream.Flush();
                }

                string sbytes = string.Join(null, res.Select(b => b.ToString("X2")));
                ConsolePrint.print("bytes\n{0}", sbytes);

                var fi = new FileInfo("dataHex.bin");
                using (var stream = new StreamWriter(fi.OpenWrite()))
                {
                    stream.Write(sbytes);
                    stream.Flush();
                }

                for (int pos = 0; pos < sbytes.Length - 1; pos += 2)
                {
                    string sbuf = string.Format("{0}{1}", sbytes[pos], sbytes[pos + 1]);
                    byte b = byte.Parse(sbuf);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }


    }

    class DbUtils
    {
        private static String getConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "DEN-DELL\\SQLEXPRESS";
            builder.PersistSecurityInfo = false;
            builder.UserID = "test";
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["DatabaseParameters"];
            //builder.Password = "sa";
            //builder.InitialCatalog = "master";
            Console.WriteLine("sett [" + settings + "]");
            Console.WriteLine("connection string [" + settings.ConnectionString + "]");
            //return builder.ConnectionString;
            return settings.ConnectionString;
        }

        public static IDbConnection getConnection()
        {
            IDbConnection connection = new SqlConnection(getConnectionString());
            return connection;
        }

        public static IDbCommand makeCommand(String queryString, IDbConnection con)
        {
            IDbCommand command = new SqlCommand(queryString, (SqlConnection)con);
            return command;
        }
    }

    class DBApp
    {
        public static void ModifyData()
        {
            IDbConnection con = null;
            try
            {
                con = DbUtils.getConnection();
                con.Open();
                SqlCommand command = (SqlCommand)DbUtils.makeCommand("insert into table1(name, descr) values (@name, @descr);", con);
                //command.Parameters.Add("@table", SqlDbType.Char);
                ((SqlCommand)command).Parameters.Add("@name", SqlDbType.VarChar);
                ((SqlCommand)command).Parameters.Add("@descr", SqlDbType.NText);
                //command.Parameters["@table"].Value = "table1";
                command.Parameters["@name"].Value = "param";
                command.Parameters["@descr"].Value = "pampam";
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }


        public static void SelectData()
        {
            IDbConnection con = null;
            try
            {
                con = DbUtils.getConnection();
                con.Open();
                IDbCommand command = DbUtils.makeCommand("select * from table1;", con);
                //((SqlCommand) command).Parameters.Add("@table", SqlDbType.VarChar);
                //((SqlCommand) command).Parameters["@table"].Value = "table1";
                IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int r_length = reader.FieldCount;
                    Console.WriteLine("depth=" + r_length);
                    for (int i = 0; i < r_length; i++)
                    {
                        Console.Write("col[" + i + "] type[" + reader[i].GetType() + "]: [" + reader[i].ToString() + "]\t");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }

        public static void InsertHugeData(object oLimit)
        {
            int limit = (int)oLimit;
            IDbConnection con = null;
            try
            {
                con = DbUtils.getConnection();
                con.Open();
                IDbCommand command = DbUtils.makeCommand("insert into huge_table(foo,bar,foobar) values(@data1,@data2,@data3);", con);
                ((SqlCommand)command).Parameters.Add("@data1", SqlDbType.NChar);
                ((SqlCommand)command).Parameters["@data1"].Value = "table1";
                ((SqlCommand)command).Parameters.Add("@data2", SqlDbType.NChar);
                ((SqlCommand)command).Parameters["@data2"].Value = getHugeString(500);
                ((SqlCommand)command).Parameters.Add("@data3", SqlDbType.NChar);
                ((SqlCommand)command).Parameters["@data3"].Value = getHugeString(500);
                for (int i = 0; i < limit; i++)
                {
                    int res = command.ExecuteNonQuery();
                    if (i % 500 == 0)
                    {
                        Console.WriteLine(i + " items added");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        private static String getHugeString(int limit)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < limit; i++)
            {
                sb.Append("f");
            }
            return sb.ToString();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ООО_Ткани_Сессия1_2
{
    public class DataBaseClass
    {
        public static string USERS_ID = "null", Password = "null", App_name = "ООО Ткани";
        public static string ConnectionString = "Data Source = KANDREY1604\\MYSERVERSQL; Initial Catalog = OOO_Tkani_Session_1; Persist Security Info = true; USER ID = {0}; Password = '{1}';";
        public SqlConnection connection = new SqlConnection(ConnectionString);
        public SqlCommand command = new SqlCommand();
        public DataTable resultTable = new DataTable();
        public SqlDependency dependency = new SqlDependency();

        public enum act { select, manipulation}
        public void sqlExecute(string query, act actionType)
        {
            command.Connection = connection;
            command.CommandText = query;
            command.Notification = null;

            switch(actionType)
            {
                case act.select:
                    dependency.AddCommandDependency(command);
                    SqlDependency.Start(connection.ConnectionString);
                    connection.Open();
                    resultTable.Load(command.ExecuteReader());
                    connection.Close();
                    break;
                case act.manipulation:
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    break;
            }
        }
    }
}

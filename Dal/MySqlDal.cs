namespace DbCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data;
    using MySql.Data.MySqlClient;
    using System.Configuration;
    using System.Globalization;

    internal sealed class MySqlDal : IDal
    {

        public string ServerName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }

        public int RunSql(string sql, string connectionString)
        {
            MySqlConnection myConnection = new MySqlConnection(connectionString);
            MySqlCommand myCommand = new MySqlCommand(sql, myConnection);

            myConnection.Open();

            int rowsAffected = myCommand.ExecuteNonQuery();

            myConnection.Close();

            return rowsAffected;
        }

        public int RunSqlReturnId(string sql, string connectionString)
        {
            MySqlConnection myConnection = new MySqlConnection(connectionString);
            MySqlCommand myCommand = new MySqlCommand(sql, myConnection);

            myConnection.Open();

            // Execute the sql
            myCommand.ExecuteNonQuery();

            // Return the ID
            DataTable myID = new DataTable();
            myID.Locale = CultureInfo.InvariantCulture;
            MySqlDataAdapter myAdapter = new MySqlDataAdapter("SELECT @@IDENTITY", myConnection);

            myAdapter.Fill(myID);

            myConnection.Close(); 

            // Return ID
            return Convert.ToInt32(myID.Rows[0][0], CultureInfo.InvariantCulture);
        }


        public DataTable RunSqlReturnData(string sql, string connectionString)
        {
            MySqlConnection myConnection = new MySqlConnection(connectionString);

            DataTable myData = new DataTable();
            myData.Locale = CultureInfo.InvariantCulture;

            MySqlDataAdapter myAdapter = new MySqlDataAdapter(sql, myConnection);

            myAdapter.Fill(myData);
            myConnection.Close();

            return myData;
        }

    }

}

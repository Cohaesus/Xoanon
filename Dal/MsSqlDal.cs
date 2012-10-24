//-----------------------------------------------------------------------
// <copyright file="MsSqlDal.cs" company="Cohaesus Projects Ltd">
//     Copyright (c) Cohaesus Projects Ltd. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DbCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data;
    using System.Data.SqlClient;
    using System.Configuration;
    using System.Globalization;

    internal sealed class MsSqlDal : IDal
    {
        public string ServerName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }

        public int RunSql(string sql, string connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();

            int rowsAffected = command.ExecuteNonQuery();

            connection.Close();

            return rowsAffected;
        }

        public int RunSqlReturnId(string sql, string connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();

            // Execute the sql
            command.ExecuteNonQuery();

            // Return the ID
            DataTable myID = new DataTable();
            myID.Locale = CultureInfo.InvariantCulture;
            SqlDataAdapter myAdapter = new SqlDataAdapter("SELECT @@IDENTITY", connection);

            myAdapter.Fill(myID);

            connection.Close(); 

            // Return ID
            return Convert.ToInt32(myID.Rows[0][0], CultureInfo.InvariantCulture);
        }


        public DataTable RunSqlReturnData(string sql, string connectionString)
        {
            SqlConnection myConnection = new SqlConnection(connectionString);

            DataTable myData = new DataTable();
            myData.Locale = CultureInfo.InvariantCulture;

            SqlDataAdapter myAdapter = new SqlDataAdapter(sql, myConnection);

            myAdapter.Fill(myData);
            myConnection.Close();

            return myData;
        }

    }

}

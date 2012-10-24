//-----------------------------------------------------------------------
// <copyright file="IDal.cs" company="Cohaesus Projects Ltd">
//     Copyright (c) Cohaesus Projects Ltd. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DbCompiler
{
    using System;
    using System.Data;

    internal interface IDal
    {
        string ServerName { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string AdminUsername { get; set; }
        string AdminPassword { get; set; }

        int RunSql(string sql, string connectionString);
        DataTable RunSqlReturnData(string sql, string connectionString);
        int RunSqlReturnId(string sql, string connectionString);
    }
}

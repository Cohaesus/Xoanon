﻿//-----------------------------------------------------------------------
// <copyright file="DalFactory.cs" company="Cohaesus Projects Ltd">
//     Copyright (c) Cohaesus Projects Ltd. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DbCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal sealed class DalFactory
    {
        public IDal CreateDal(string databaseType)
        {
            switch (databaseType)
            {
                case "MSSQL":
                    return new MsSqlDal();

                case "MYSQL":
                    return new MySqlDal();

                default:
                    throw new Exception("Unknown Database Type Requested");
            }
        }

    }
}

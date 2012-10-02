namespace DbCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.IO;
    using System.Data;
    using System.Globalization;
    using System.Reflection;

    public sealed class DbCompilerMain
    {
        private static string Server = string.Empty;
        private static string Username = string.Empty;
        private static string Password = string.Empty;
        private static string SAPassword = string.Empty;
        private static string DatabaseName = string.Empty;
        private static string ScriptPath = string.Empty;
        private static string DatabaseType = string.Empty;

        private static int NumberOfErrors = 0;
        private static int NumberOfWarnings = 0;

        private static string connectionString = String.Empty;

        private static IDal dal;

        private DbCompilerMain() { }

        [STAThread]
        static void Main(string[] args)
        {

            Console.WriteLine(string.Format("Foundation Database Build Tool -- Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            Console.WriteLine("Copyright 2012 Cohaesus Projects Limited");
            Console.WriteLine();

            // Check number of arguments
            if (args.Length != 8)
            {
                Console.WriteLine("incorrect number of arguments passed");
                Console.WriteLine("expecting: ServerName, UserName, Password, AdminUserName, AdminPassword, DatabaseName, ScriptPath, DatabaseType (MSSQL|MYSQL)");
                System.Environment.ExitCode = 1;
                return;
            }

            DatabaseName = args[5];
            ScriptPath = args[6];
            DatabaseType = args[7];

            dal = new DalFactory().CreateDal(DatabaseType);

            dal.ServerName = args[0];
            dal.Username = args[1];
            dal.Password = args[2];
            dal.AdminUsername = args[3];
            dal.AdminPassword = args[4];

            connectionString = "server=" + dal.ServerName + ";user id=" + dal.AdminUsername + "; password=" + dal.AdminPassword + "; pooling=false;";

            List<string> scriptsDatabase = GetScripts(@"\schema");
            List<string> scriptsDefaults = GetScripts(@"\defaults");
            List<string> scriptsTypes = GetScripts(@"\types");
            List<string> scriptsTable = GetScripts(@"\tables");
            List<string> scriptsView = GetScripts(@"\views");
            List<string> scriptsProcedures = GetScripts(@"\procedures");
            List<string> scriptsTriggers = GetScripts(@"\triggers");
            List<string> scriptsFunctions = GetScripts(@"\functions");
            List<string> scriptsSynonyms = GetScripts(@"\synonyms");
            List<string> scriptsDataBase = GetScripts(@"\data\base");
            List<string> scriptsDataImplementation = GetScripts(@"\data\implementation");
            List<string> scriptsDataTest = GetScripts(@"\data\test");

            RunSchema(scriptsDatabase);

            if (NumberOfErrors > 0)
            {
                // Problem building schema, so exit
                Console.WriteLine("unable to create database");
                System.Environment.ExitCode = 1;
                return;
            }

            // Update the connection string with the database now its created.
            connectionString = "server=" + dal.ServerName + ";user id=" + dal.Username + "; password=" + dal.Password + "; database=" + DatabaseName + "; pooling=false;";

            int cycle = 0;

            do
            {
                Console.WriteLine("starting cycle " + cycle);

                NumberOfErrors = 0;

                // Create structure
                scriptsSynonyms = RunScripts(scriptsSynonyms);

                scriptsTable = RunScripts(scriptsTable);
                scriptsFunctions = RunScripts(scriptsFunctions);
                scriptsView = RunScripts(scriptsView);
                scriptsProcedures = RunScripts(scriptsProcedures);

                // Base Data
                scriptsDataBase = RunScripts(scriptsDataBase);

                // Implementation Data
                scriptsDataImplementation = RunScripts(scriptsDataImplementation);

                // Test Data
                scriptsDataTest = RunScripts(scriptsDataTest);

                scriptsTriggers = RunScripts(scriptsTriggers);

                cycle++;

            } while (cycle < 7);

            // Write out the final result
            Console.WriteLine("Completed with " + NumberOfErrors + " Errors, " + NumberOfWarnings + " Warnings");

            // Exit with errors
            System.Environment.ExitCode = NumberOfErrors;

        }

        private static void RunSchema(List<string> scriptsDatabase)
        {
            // try to execute schema
            try
            {
                RunScripts(scriptsDatabase);
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed");
                Console.WriteLine(ex.Message);
                NumberOfErrors = NumberOfErrors + 1;
            }
        }

        /// <summary>
        /// Runs a list of passed in scripts against the database
        /// </summary>
        /// <param name="scriptsDatabase">A list of paths to the scripts</param>
        private static List<string> RunScripts(List<string> scriptsDatabase)
        {
            // Scripts executed
            List<string> scriptsExecuted = new List<string>();

            foreach (string scriptPath in scriptsDatabase)
            {
                using (StreamReader streamReader = new StreamReader(scriptPath, true))
                {
                    StringBuilder scriptText = new StringBuilder(streamReader.ReadToEnd());
                    //scriptText.Replace("##PREFIX##", Prefix);
                    scriptText.Replace("##DATABASE_NAME##", DatabaseName);
                    scriptText.Replace("##DATABASE_USER##", Username);
                    scriptText.Replace("##BASE_PATH##", ScriptPath);
                    scriptText.Replace("##DATETIME_NOW##", DateTime.Now.ToString("dd MMMM yyyy hh:mm:ss", CultureInfo.InvariantCulture));

                    Console.Write("Trying: " + scriptPath + "...");

                    try
                    {
                        // Factory here to connect via the right DAL (MySql/MS SQL)
                        dal.RunSql(scriptText.ToString(), connectionString);
                        Console.WriteLine("done");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("failed");
                        Console.WriteLine(ex.Message);
                        NumberOfErrors = NumberOfErrors + 1;
                        // Rerun this next time
                        scriptsExecuted.Add(scriptPath);
                    }

                }
            }

            return scriptsExecuted;
        }

        /// <summary>
        /// Gets all the scripts for a given folder
        /// </summary>
        /// <param name="pathToScripts">The top level folder to iterate for scripts</param>
        /// <returns>A list of paths to the scripts</returns>
        private static List<string> GetScripts(string pathToScripts)
        {
            List<string> scriptList = new List<string>();
            if (Directory.Exists(ScriptPath + pathToScripts))
            {
                Console.WriteLine("searching " + ScriptPath + pathToScripts);
                scriptList.AddRange(Directory.GetFiles(ScriptPath + pathToScripts, "*.sql", SearchOption.AllDirectories));
            }
            return scriptList;
        }
    }
}

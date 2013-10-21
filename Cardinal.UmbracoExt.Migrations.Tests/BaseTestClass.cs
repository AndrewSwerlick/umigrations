using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardinal.UmbracoExt.Migrations.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Cardinal.UmbracoExt.Migrations.Tests
{
    public class BaseTestClass
    {
        private static string FILE_NAME = Directory.GetCurrentDirectory() + "\\" + "Umbraco.sdf";
        static string CONNECTION_STRING = string.Format("DataSource=\"{0}\";", FILE_NAME);
        
        public static ApplicationContext Context { get; set; }
        public static MigrationContext MigrationContext { get; set; }
        public static TestApplicationBase Application { get; set; }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            try
            {
                File.Delete(FILE_NAME);
                var en = new SqlCeEngine(CONNECTION_STRING);
                en.CreateDatabase();
            }
            catch (SqlCeException) { }
            if (ApplicationContext.Current == null)
            {
                Application = new TestApplicationBase();
                Application.Start(Application, new EventArgs());
            }
            Context = ApplicationContext.Current;            
            Context.DatabaseContext.Database.CreateDatabaseSchema();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            MigrationContext = new MigrationContext(Context, new MigrationsSettings());
        }
        [ClassCleanup]
        public static void Cleanup()
        {
            File.Delete(FILE_NAME);
        }

        [TestCleanup]
        public void TestCleanup()
        {            
            File.Delete(FILE_NAME);
            var en = new SqlCeEngine(CONNECTION_STRING);
            en.CreateDatabase();
            Context.DatabaseContext.Database.CreateDatabaseSchema();
        }
    }
}

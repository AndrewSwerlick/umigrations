using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cardinal.UmbracoExt.Migrations.Tests.MigrationScripts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cardinal.UmbracoExt.Migrations.Tests
{
    [TestClass]
    public class MigrationContextTests : BaseTestClass
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            BaseTestClass.Initialize(context);
        }

        [TestMethod]
        public void Ensure_That_We_Can_Create_A_MigrationContext()
        {
            var settings = new MigrationsSettings()
            {
                Assembly = Assembly.GetExecutingAssembly().FullName,
                ScriptsNameSpace = typeof(Script1).Namespace,
                TargetVersion = "1.0"
            };
            var context = new MigrationContext(Context,settings);

            Assert.IsNotNull(context);
        }

        [TestMethod]
        public void Ensure_That_The_Migration_Context_Sets_The_From_Value_To_Zero_With_A_New_Database()
        {
            var settings = new MigrationsSettings()
            {
                Assembly = Assembly.GetExecutingAssembly().FullName,
                ScriptsNameSpace = typeof(Script1).Namespace,
                TargetVersion = "1.0"
            };
            var context = new MigrationContext(Context,settings);
            Assert.AreEqual(new VersionNumber("0"), context.From);
        }

        [TestMethod]
        public void
            If_There_Is_Already_A_Migration_Record_In_The_Database_Ensure_The_Context_Sets_The_From_Value_To_The_Last_Migration
            ()
        {
            var settings = new MigrationsSettings()
                {
                    Assembly = Assembly.GetExecutingAssembly().FullName,
                    ScriptsNameSpace = typeof (Script1).Namespace,
                    TargetVersion = "1.0"
                };
            Context.DatabaseContext.Database.Insert(new Migration() {VersionString = "1.0"});
            var context = new MigrationContext(Context,settings);
            Assert.AreEqual(new VersionNumber("1.0"), context.From);            
        }

        [TestMethod]
        public void
            If_The_To_Value_Is_Set_In_The_Settings_Ensure_The_Migration_Context_Set_The_To_Value_To_SameSetting
            ()
        {
            var settings = new MigrationsSettings()
                {
                    Assembly = Assembly.GetExecutingAssembly().FullName,
                    ScriptsNameSpace = typeof (Script1).Namespace,
                    TargetVersion = "1.0"
                };
            var context = new MigrationContext(Context,settings);

            Assert.AreEqual(context.To, new VersionNumber(settings.TargetVersion));
        }
    }
}

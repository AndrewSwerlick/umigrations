using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Reflection;
using Cardinal.UmbracoExt.Migrations.Tests.DependentScripts;
using Cardinal.UmbracoExt.Migrations.Tests.MigrationScripts;
using Cardinal.UmbracoExt.Migrations.Tests.OneScript;
using Cardinal.UmbracoExt.Migrations.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Core.Services;
using File = System.IO.File;

namespace Cardinal.UmbracoExt.Migrations.Tests
{    
    [TestClass]
    public class MigrationManagerTests : BaseTestClass
    {     
        [ClassInitialize]
        public static void Initialize(TestContext MigrationContext)
        {
            BaseTestClass.Initialize(MigrationContext);
        }

        [TestMethod]
        public void Ensure_We_Can_Create_A_Migration_Manager()
        {
            var manager = new MigrationManager(MigrationContext);
        }

        [TestMethod]
        public void Ensure_We_Prepare_A_Database_For_Migrations()
        {

            var manager = new MigrationManager(MigrationContext);

            Assert.IsTrue(MigrationContext.AppContext.DatabaseContext.Database.TableExist("Migrations"));
        }

        [TestMethod]
        public void Ensure_We_Can_Run_A_Deployment_Script_Which_Adds_A_Document_Type()
        {
            var manager = new MigrationManager(MigrationContext);
            manager.RunScript(new InlineScript(Context =>
                {
                    var contentType = new ContentType(-1) {Name = "Test", Alias = "Test"};
                    Context.Services.ContentTypeService.Save(contentType);
                }));

            var testExists = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Any(c => c.Name == "Test");
            Assert.IsTrue(testExists);
        }

        [TestMethod]
        public void Ensure_We_Can_Run_A_Deployment_Script_Which_Adds_A_Custom_Table()
        {            
            var manager = new MigrationManager(MigrationContext);
            manager.RunScript(new InlineScript((Context) =>
                {
                    Context.DatabaseContext.Database.CreateTable<CustomModel>();  
                }));

            Assert.IsTrue(MigrationContext.AppContext.DatabaseContext.Database.TableExist("CustomModel"));
        }

        [TestMethod]
        public void Ensure_We_Can_Register_Scripts_By_Namespace()
        {
            var manager = new MigrationManager(MigrationContext);
            var scriptsNamespace = typeof (Script1).Namespace;
            
            manager.RegisterScripts(scriptsNamespace, Assembly.GetExecutingAssembly());

            Assert.AreEqual(manager.Scripts.Count(), 2);
        }

        [TestMethod]
        public void Ensure_We_Can_Run_A_Migration_With_The_Scripts_In_The_Migration_Scripts_Folders()
        {
            var manager = new MigrationManager(MigrationContext);
            var scriptsNamespace = typeof(Script1).Namespace;

            manager.RegisterScripts(scriptsNamespace, Assembly.GetExecutingAssembly());
            manager.Migrate(new VersionNumber("0"), new VersionNumber(("2.0")));

            Assert.IsTrue(MigrationContext.AppContext.DatabaseContext.Database.TableExist("CustomModel")); 
            var testExists = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Any(c => c.Name == "Test");
            Assert.IsTrue(testExists);
        }
        
        [TestMethod]
        public void Ensure_That_Scripts_That_Depend_On_Each_Other_Are_Run_In_Order()
        {
            var manager = new MigrationManager(MigrationContext);
            var scriptsNamespace = typeof(DependentScript1).Namespace;

            manager.RegisterScripts(scriptsNamespace, Assembly.GetExecutingAssembly());
            manager.Migrate(new VersionNumber("0"), new VersionNumber(("2.0")));

            var test = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Single(c => c.Name == "Test");
            Assert.IsNotNull(test);
            Assert.IsTrue(test.PropertyTypeExists("test"));
        }

        [TestMethod]
        public void Ensure_That_We_Can_Run_A_Migration_Using_The_Parameterless_Method()
        {
            MigrationContext.Settings.ScriptsNameSpace = typeof (Script1).Namespace;
            MigrationContext.Settings.Assembly = Assembly.GetExecutingAssembly().FullName;
            var manager = new MigrationManager(MigrationContext);
            manager.Migrate();

            Assert.IsTrue(MigrationContext.AppContext.DatabaseContext.Database.TableExist("CustomModel"));
            var testExists = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Any(c => c.Name == "Test");
            Assert.IsTrue(testExists);
        }

        [TestMethod]
        public void
            Ensure_That_If_The_Settings_Say_To_Re_Run_The_Last_Script_The_Migration_Manager_Will_Run_The_Last_Script_Only_During_A_Second_Migration
            ()
        {
            MigrationContext.Settings.ScriptsNameSpace = typeof(Script1).Namespace;
            MigrationContext.Settings.Assembly = Assembly.GetExecutingAssembly().FullName;
            MigrationContext.Settings.ReRunLastScript = true;
            var manager = new MigrationManager(MigrationContext);
            manager.Migrate();
            new MigrationManager(new MigrationContext(MigrationContext.AppContext, MigrationContext.Settings)).Migrate();

            Assert.IsTrue(MigrationContext.AppContext.DatabaseContext.Database.TableExist("CustomModel"));
            var countOfTest = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Count(c => c.Name == "Test");
            Assert.AreEqual(1, countOfTest);
            Assert.AreEqual(2,MigrationContext.AppContext.DatabaseContext.Database.Query<CustomModel>("SELECT * FROM CustomModel").Count());
        }

        [TestMethod]
        public void Ensure_The_ReRun_Setting_Works_When_There_Is_Only_One_Migration_Script()
        {
            MigrationContext.Settings.ScriptsNameSpace = typeof(SingleScript).Namespace;
            MigrationContext.Settings.Assembly = Assembly.GetExecutingAssembly().FullName;
            MigrationContext.Settings.ReRunLastScript = true;
            var manager = new MigrationManager(MigrationContext);
            manager.Migrate();

            var countOfTest = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Count(c => c.Name == "Test");
            Assert.AreEqual(1, countOfTest);
        }


        [TestMethod]
        public void Ensure_That_Running_A_Migration_Twice_Doesnt_Result_In_The_Scripts_Running_Twice()
        {
            MigrationContext.Settings.ScriptsNameSpace = typeof(Script1).Namespace;
            MigrationContext.Settings.Assembly = Assembly.GetExecutingAssembly().FullName;
            var manager = new MigrationManager(MigrationContext);
            manager.Migrate();
            new MigrationManager(new MigrationContext(MigrationContext.AppContext,MigrationContext.Settings)).Migrate();
                
            Assert.IsTrue(MigrationContext.AppContext.DatabaseContext.Database.TableExist("CustomModel"));
            var countOfTest = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Count(c => c.Name == "Test");
            Assert.AreEqual(1,countOfTest);
        }


        [ClassCleanup]
        public static void Cleanup()
        {
            BaseTestClass.Cleanup();
        }
    }
}

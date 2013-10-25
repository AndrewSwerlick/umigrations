using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardinal.UmbracoExt.Migrations.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umbraco.Core.Models;

namespace Cardinal.UmbracoExt.Migrations.Tests
{
    [TestClass]
    class ContentTypeBuilderTests : BaseTestClass
    {
        [ClassInitialize]
        public static void Initialize(TestContext MigrationContext)
        {
            BaseTestClass.Initialize(MigrationContext);
        }

        [TestMethod]
        public void Ensure_We_Can_Build_A_Content_Type_Builder()
        {
            var builder = new ContentTypeBuilder("Test", "Test");
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void Ensure_When_We_Call_The_Build_Method_On_The_Builder_It_Creates_The_Content_Type_If_It_Doesnt_Exist()
        {
            var contentType = new ContentTypeBuilder("Test", "Test").Build(Context);
            var testExists = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Any(c => c.Name == "Test");
            Assert.IsTrue(testExists);
        }

        [TestMethod]
        public void Ensure_When_We_Call_The_Build_Method_On_The_Builder_Twice_It_Doesnt_Create_The_Type_Again()
        {
            var builder = new ContentTypeBuilder("Test", "Test");
            var contentType = builder.Build(Context);
            builder.Build(Context);
            var numberOfTest = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Count(c => c.Name == "Test");
            Assert.AreEqual(1, numberOfTest);
        }

        [TestMethod]
        public void Ensure_When_We_Call_The_Build_Method_On_The_Builder_With_A_Property_It_Creates_The_Property()
        {
            var contentType = new ContentTypeBuilder("Test", "Test")
                .AddProperty("Textstring","myProp","My Property")
                .Build(Context);
            var test = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Single(c => c.Name == "Test");
            Assert.IsTrue(test.PropertyTypeExists("myProp"));
        }

        [TestMethod]
        public void Ensure_When_We_Call_The_Build_Method_On_The_Builder_With_The_Same_Property_Twice_It_Creates_The_Property_Once()
        {
            var contentType = new ContentTypeBuilder("Test", "Test")
                .AddProperty("Textstring", "myProp", "My Property")
                .AddProperty("Textstring", "myProp", "My Property")
                .Build(Context);
            var test = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Single(c => c.Name == "Test");
            Assert.AreEqual(1, test.PropertyTypes.Count(p=> p.Alias == "myProp"));
        }

        [TestMethod]
        public void Ensure_If_We_Call_The_Create_Template_Method_It_Creates_A_Template_And_Sets_It_As_A_Default()
        {
            var contentType = new ContentTypeBuilder("Test", "Test")
                .CreateTemplate()
                .Build(Context);

            var test = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Single(c => c.Name == "Test");
            Assert.IsNotNull(test.DefaultTemplate);
        }

        [TestMethod]
        public void Ensure_If_Call_Add_Allowed_ContentTypes_The_Othertypes_Are_Registered_As_Allowed_Types()
        {
            var contentType = new ContentTypeBuilder("Test", "Test").Build(Context);

            var contentType2 = new ContentTypeBuilder("Test2", "Test2")
               .AddAllowedContentTypes(new List<IContentType>(){contentType})
               .Build(Context);

            var test = MigrationContext.AppContext.Services.ContentTypeService.GetAllContentTypes().Single(c => c.Name == "Test2");
            Assert.IsTrue(test.AllowedContentTypes.Any(c=> c.Alias == contentType.Alias));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            BaseTestClass.Cleanup();
        }
    }
}

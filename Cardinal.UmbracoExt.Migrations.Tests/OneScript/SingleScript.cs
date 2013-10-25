using Umbraco.Core;
using Umbraco.Core.Models;

namespace Cardinal.UmbracoExt.Migrations.Tests.OneScript
{
    [VersionNumber("1.0")]
    class SingleScript : IMigrationScript
    {
        public void Execute(ApplicationContext context)
        {
            var contentType = new ContentType(-1) { Name = "Test", Alias = "Test" };
            context.Services.ContentTypeService.Save(contentType);
        }
    }
}

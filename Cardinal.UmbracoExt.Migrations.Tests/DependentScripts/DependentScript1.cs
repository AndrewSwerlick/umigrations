using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Cardinal.UmbracoExt.Migrations.Tests.DependentScripts
{
    [VersionNumber("1.0")]
    class DependentScript1 : IMigrationScript 
    {
        public void Execute(ApplicationContext context)
        {
            var contentType = new ContentType(-1) { Name = "Test", Alias = "Test" };
            context.Services.ContentTypeService.Save(contentType);
        }
    }
}

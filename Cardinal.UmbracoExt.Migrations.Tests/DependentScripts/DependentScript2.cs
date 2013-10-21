using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Cardinal.UmbracoExt.Migrations.Tests.DependentScripts
{
    [VersionNumber("2.0")]
    class DependentScript2 : IMigrationScript
    {
        public void Execute(ApplicationContext context)
        {
            var type = context.Services.ContentTypeService.GetContentType("Test");
            var dataType =
                context.Services.DataTypeService.GetAllDataTypeDefinitions().SingleOrDefault(d => d.Name == "Label");
            type.AddPropertyType(new PropertyType(dataType){Name = "test", Alias = "test"});
            
            context.Services.ContentTypeService.Save(type);
        }
    }
}

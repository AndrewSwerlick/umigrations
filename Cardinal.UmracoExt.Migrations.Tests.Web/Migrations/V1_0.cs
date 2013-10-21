using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cardinal.UmbracoExt.Migrations;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Cardinal.UmbracoExt.Migrations.Tests.Web.Migrations
{
    [VersionNumber("1.0")]
    public class V1_0 : IMigrationScript
    {
        public void Execute(ApplicationContext context)
        {
            context.Services.ContentTypeService.Save(new ContentType(-1){Name = "Test", Alias = "Test"});
        }
    }
}
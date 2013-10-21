using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardinal.UmbracoExt.Migrations.Tests.TestInfrastructure;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Cardinal.UmbracoExt.Migrations.Tests.MigrationScripts
{
    [VersionNumber("2.0")]
    class Script2 : IMigrationScript 
    {
        public void Execute(ApplicationContext context)
        {
            context.DatabaseContext.Database.CreateTable<CustomModel>();  
        }
    }
}

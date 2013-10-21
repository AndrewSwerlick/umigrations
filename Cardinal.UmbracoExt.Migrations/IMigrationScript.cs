using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardinal.UmbracoExt.Migrations;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace Cardinal.UmbracoExt.Migrations
{
    public interface IMigrationScript
    {
        void Execute(ApplicationContext context);
    }
}

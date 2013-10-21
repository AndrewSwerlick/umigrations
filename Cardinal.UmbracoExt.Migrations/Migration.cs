using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;

namespace Cardinal.UmbracoExt.Migrations
{
    [PrimaryKey("Id", autoIncrement = true)]
    [TableName("Migrations")]
    public class Migration
    {
        public string VersionString { get; set; }
        
        [Ignore]
        public VersionNumber Version
        {
            get { return new VersionNumber(VersionString); }
        }
    }
}

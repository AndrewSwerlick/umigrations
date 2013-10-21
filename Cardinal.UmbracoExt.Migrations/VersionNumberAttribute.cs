using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardinal.UmbracoExt.Migrations
{
    /// <summary>
    /// Attribute to allow <see cref="IMigrationScript">IMigrationScript</see> to be decorated with a version number
    /// </summary>
    public class VersionNumberAttribute : Attribute
    {
        public VersionNumberAttribute(string version)
        {
            VersionProperty = new VersionNumber(version);
        }
        public VersionNumber VersionProperty { get; set; }
    }
}

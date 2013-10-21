using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;

namespace Cardinal.UmbracoExt.Migrations
{
    /// <summary>
    /// uSync Settings - 
    /// 
    /// reads the uSync bit of the Web.Config
    /// 
    /// <umigrations 
    ///             targetVersion="2.0"                              (optional) the version to migrate to on startup
    ///             scriptsNamespace="MyNamespace.Migrations"        The namespace that the migration scripts are in for your application    
    ///             asssembly="MyAssembly"                           The assembly the scripts are in.        
    ///             />
    ///    
    /// 
    /// </summary>
    public class MigrationsSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("targetVersion", DefaultValue = "", IsRequired = false)]
        public string VersionNumber
        {
            get { return (string)this["targetVersion"]; }
            set { this["targetVersion"] = value; }
        }

        [ConfigurationProperty("scriptsNamespace", DefaultValue = "", IsRequired = true)]
        public string ScriptsNamespace
        {
            get { return (string)this["scriptsNamespace"]; }
            set { this["scriptsNamespace"] = value; }
        }

        [ConfigurationProperty("assembly", DefaultValue = "", IsRequired = true)]
        public string Assembly
        {
            get { return (string)this["assembly"]; }
            set { this["assembly"] = value; }
        }  
    }    
      
    public class MigrationsSettings
    {

        private static string _settingfile = "umigrations.config";
        
        public static MigrationsSettings LoadFromFile()
        {           
            var fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = IOHelper.MapPath(string.Format("~/config/{0}", _settingfile));

            // load the settings file
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                                                                                    ConfigurationUserLevel.None);

            return new MigrationsSettings((MigrationsSettingsSection)config.GetSection("umigrations"));                     
        }

        private MigrationsSettings(MigrationsSettingsSection settings)
        {
            TargetVersion = settings.VersionNumber;
            ScriptsNameSpace = settings.ScriptsNamespace;
            Assembly = settings.Assembly;
        }

        public MigrationsSettings()
        {
            
        }

        public string TargetVersion { get; set; }

        public string ScriptsNameSpace { get; set; }

        public string Assembly { get; set; }
    }
}


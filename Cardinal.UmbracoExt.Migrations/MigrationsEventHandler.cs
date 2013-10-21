using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Cardinal.UmbracoExt.Migrations
{
    public class MigrationsEventHandler : ApplicationEventHandler
    {
        private static bool _hasRun;

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if(_hasRun)
                base.ApplicationStarted(umbracoApplication, applicationContext);

            MigrationsSettings settings = null;
            try
            {
                settings = MigrationsSettings.LoadFromFile();
            }          
             catch (Exception ex)
            {
                LogHelper.Error<MigrationsSettings>("Error loading umigrations settings file", ex);                
            }            
            var migrationContext = new MigrationContext(applicationContext, settings);
            var manager = new MigrationManager(migrationContext);
            manager.Migrate();
            _hasRun = true;
            base.ApplicationStarted(umbracoApplication, applicationContext);
        }     
   
        public void ForceRunEventHandler(UmbracoApplicationBase application, ApplicationContext context)
        {
            ApplicationStarted(application,context);
        }
    }
}

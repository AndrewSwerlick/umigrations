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
        private static bool _hasRun = false;
        private static object _syncObj = new object(); 

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (!_hasRun)
            {
                lock (_syncObj)
                {
                    if (!_hasRun)
                    {
                        RunMigration(applicationContext);
                    }
                }
            }            
            base.ApplicationStarted(umbracoApplication, applicationContext);
        }     
   
        public void ForceRunEventHandler(UmbracoApplicationBase application, ApplicationContext context)
        {
            ApplicationStarted(application,context);
        }

        private void RunMigration(ApplicationContext applicationContext)
        {
            MigrationsSettings settings = null;
            try
            {
                settings = MigrationsSettings.LoadFromFile();
            }
            catch (Exception ex)
            {
                LogHelper.Error<MigrationsSettings>("Error loading umigrations settings file", ex);
            }

            try
            {
                var migrationContext = new MigrationContext(applicationContext, settings);
                var manager = new MigrationManager(migrationContext);
                manager.Migrate();
                _hasRun = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error<MigrationsSettings>("Error performing migrations", ex);
            }
        }
    }
}

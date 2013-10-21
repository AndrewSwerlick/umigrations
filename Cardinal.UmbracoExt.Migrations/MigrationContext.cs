using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Core.Services;

namespace Cardinal.UmbracoExt.Migrations
{
    public class MigrationContext
    {

        public VersionNumber To { get; set; }
        public VersionNumber From { get; set; }
        public ApplicationContext AppContext { get; private set; }
        public MigrationsSettings Settings { get; private set; }

        public MigrationContext(ApplicationContext context, MigrationsSettings settings)
        {
            Settings = settings;
            AppContext = context;
            InitializeMigrationsFramework();
            var migrations = context.DatabaseContext.Database.Query<Migration>(
                new Sql().Select("*").From<Migration>());

            var lastMigration = migrations.OrderByDescending(m=> m.Version).FirstOrDefault();
            From = lastMigration == null ? new VersionNumber("0") : lastMigration.Version;
            if(!string.IsNullOrEmpty(settings.TargetVersion))
                To = new VersionNumber(settings.TargetVersion);
        }

        private void InitializeMigrationsFramework()
        {
            if (!AppContext.DatabaseContext.Database.TableExist("Migrations"))
                AppContext.DatabaseContext.Database.CreateTable<Migration>(false);
        }
    }
}

using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Cardinal.UmbracoExt.Migrations.Tests.TestInfrastructure
{
    [PrimaryKey("Id", autoIncrement = true)]
    public class CustomModel
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

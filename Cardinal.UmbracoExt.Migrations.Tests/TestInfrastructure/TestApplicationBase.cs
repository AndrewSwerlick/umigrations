using System;
using Umbraco.Core;

namespace Cardinal.UmbracoExt.Migrations.Tests.TestInfrastructure
{
    public class TestApplicationBase : UmbracoApplicationBase
    {
        protected override IBootManager GetBootManager()
        {
            return new TestBootManager(this);
        }

        public void Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);
        }
    }
}

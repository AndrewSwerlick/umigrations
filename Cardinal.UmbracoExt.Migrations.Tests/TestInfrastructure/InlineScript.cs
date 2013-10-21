using System;
using Umbraco.Core;

namespace Cardinal.UmbracoExt.Migrations.Tests.TestInfrastructure
{
    /// <summary>
    /// Script for testing purposes. Used so the action of the script can be written inline with the test for better readability
    /// </summary>
    class InlineScript : IMigrationScript 
    {
        private readonly Action<ApplicationContext> _executeMethod;

        public InlineScript(Action<ApplicationContext> executeMethod)
        {
            _executeMethod = executeMethod;
        }

        public void Execute(ApplicationContext context)
        {
            _executeMethod.Invoke(context);
        }
    }
}

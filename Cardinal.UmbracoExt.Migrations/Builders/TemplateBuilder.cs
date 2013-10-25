using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;

namespace Cardinal.UmbracoExt.Migrations.Builders
{
    public class TemplateBuilder
    {
        private readonly string _templateName;
        private readonly string _templateAlias;
        private IFileSystem _viewsFileSystem;

         public TemplateBuilder(string templateName, string templateAlias)
         {
             _templateName = templateName;
             _templateAlias = templateAlias;
             _viewsFileSystem = new PhysicalFileSystem(SystemDirectories.MvcViews);
         }

        public ITemplate Build(ApplicationContext context)
        {
            return CreateTemplate(context, _templateName, _templateAlias);
        }

        private ITemplate CreateTemplate(ApplicationContext context, string templateName, string templateAlias)
        {
            var fileName = templateAlias + ".cshtml";
            var path = _viewsFileSystem.GetRelativePath(fileName);
            string content = string.Empty;

            if (_viewsFileSystem.FileExists(path))
            {
                using (var stream = _viewsFileSystem.OpenFile(fileName))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(bytes, 0, (int)stream.Length);
                    content = Encoding.UTF8.GetString(bytes);
                }
            }
            var loginTemplate = context.Services.FileService.GetTemplate(templateAlias);

            if (loginTemplate == null)
                loginTemplate = new Template(path, templateName, templateAlias);

            loginTemplate.Content = content;

            context.Services.FileService.SaveTemplate(loginTemplate);
            return loginTemplate;
        }
    }
}

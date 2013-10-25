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
    public class ContentTypeBuilder
    {
        private readonly string _alias;
        private readonly string _name;
        private readonly IList<PropertyDTO> _properties;
        private bool _createAssociatedTemplate;
        private IFileSystem _viewsFileSystem;
        private readonly List<IContentType> _allowedTypes;
        private readonly List<ITemplate> _allowedTemplates; 
        private bool _allowedAtRoot;
        private IContentType _parentType;
        private bool _allowSelfAsChild;

        public ContentTypeBuilder(string alias, string name)
        {
            _alias = alias;
            _name = name;
            _properties = new List<PropertyDTO>();
            _viewsFileSystem = new PhysicalFileSystem(SystemDirectories.MvcViews);
            _allowedTemplates = new List<ITemplate>();
            _allowedTypes = new List<IContentType>();
        }

        public IContentType Build(ApplicationContext context)
        {
            IContentType contentType;
            if (_createAssociatedTemplate)
            {
                var template = CreateTemplate(context);
                _allowedTemplates.Add(template);
                contentType = CreateContentType(context, _name, _alias, template, _allowedTemplates, false);
            }
            else
                contentType = CreateContentType(context, _name, _alias, null, _allowedTemplates, false);

            foreach (var propertyDto in _properties)
            {
                if (!contentType.PropertyTypeExists(propertyDto.Alias))
                {
                    var datatype = context
                        .Services.DataTypeService.GetAllDataTypeDefinitions()
                        .First(d => d.Name == propertyDto.Type);
                    contentType.AddPropertyType(new PropertyType(datatype)
                    {
                        Alias = propertyDto.Alias,
                        Name = propertyDto.Name
                    });
                }
            }
            var allowedTypes = _allowedTypes
                .Select((c, i) => new ContentTypeSort(new Lazy<int>(() => c.Id), i, c.Alias)).ToList();
            if(_allowSelfAsChild)
                allowedTypes.Add(new ContentTypeSort(new Lazy<int>(()=> contentType.Id), allowedTypes.Count, contentType.Alias));
            contentType.AllowedContentTypes = allowedTypes;

            if (_allowedAtRoot)
                contentType.AllowedAsRoot = true;

            if (_parentType != null)
                contentType.ParentId = _parentType.Id;
            context.Services.ContentTypeService.Save(contentType);

            return contentType;
        }

        public ContentTypeBuilder AddProperty(string type, string propAlias, string name)
        {
            _properties.Add(new PropertyDTO{Alias = propAlias,Name = name,Type = type});
            return this;
        }

        public ContentTypeBuilder CreateTemplate()
        {
            _createAssociatedTemplate = true;
            return this;
        }

        private IContentType CreateContentType(ApplicationContext context, string contentTypeName,
                                            string contentTypeAlias, ITemplate defaultTemplate, IEnumerable<ITemplate> allowedTemplates, bool allowedAtRoot)
        {
            var type = context.Services.ContentTypeService.GetContentType(contentTypeAlias) ?? new ContentType(-1)
            {
                Alias = contentTypeAlias,
                Name = contentTypeName,
                AllowedTemplates = allowedTemplates,
                AllowedAsRoot = allowedAtRoot
            };

            if (defaultTemplate != null)
                type.SetDefaultTemplate(defaultTemplate);


            return type;
        }

        public ITemplate CreateTemplate(ApplicationContext context)
        {
            var fileName = _alias + ".cshtml";
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
            var loginTemplate = context.Services.FileService.GetTemplate(_alias);

            if (loginTemplate == null)
                loginTemplate = new Template(path, _name, _alias);

            loginTemplate.Content = content;

            context.Services.FileService.SaveTemplate(loginTemplate);
            return loginTemplate;
        }

        public ContentTypeBuilder AddAllowedContentTypes(List<IContentType> contentTypes)
        {
            _allowedTypes.AddRange(contentTypes);
            return this;
        }

        public ContentTypeBuilder AddAllowedTemplates(List<ITemplate> templates)
        {
            _allowedTemplates.AddRange(templates);
            return this;
        }

        public ContentTypeBuilder AddAllowedTemplates(ITemplate template)
        {
            _allowedTemplates.Add(template);
            return this;
        }


        public ContentTypeBuilder AllowedAtRoot()
        {
            _allowedAtRoot = true;
            return this;
        }

        public ContentTypeBuilder SetParentType(IContentType type)
        {
            _parentType = type;
            return this;
        }

        public ContentTypeBuilder AllowSelfAsChild()
        {
            _allowSelfAsChild = true;
            return this;
        }
    }
}

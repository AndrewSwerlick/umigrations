using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Cardinal.UmbracoExt.Migrations.Builders
{
    public class ContentBuilder
    {
        private int _parentId;
        private readonly string _name;
        private readonly string _contentTypeAlias;
        private readonly ApplicationContext _context;
        private readonly List<ContentBuilder> _children;
        private readonly IDictionary<string, object> _properties;

        public ContentBuilder(IContent parent, string name, string contentTypeAlias, ApplicationContext context)
            : this(parent.Id, name, contentTypeAlias,context)
        {
            
        }

        public ContentBuilder(string name, string contentTypeAlias, ApplicationContext context)
            : this(-1, name, contentTypeAlias, context)
        {
            
        }

        public ContentBuilder(int parentId, string name, string contentTypeAlias, ApplicationContext context)
        {
            _parentId = parentId;
            _name = name;
            _contentTypeAlias = contentTypeAlias;
            _context = context;
            _children = new List<ContentBuilder>();
            _properties = new Dictionary<string, object>();
        }

        public IContent Build()
        {
            var service = _context.Services.ContentService;
            var page = _parentId != -1
                                   ? service.GetById(_parentId).Children().FirstOrDefault(c => c.Name == _name)
                                   : service.GetRootContent().FirstOrDefault(c => c.Name == _name);
            if (page == null)
            {
                page = _context.Services.ContentService.CreateContent(_name, _parentId, _contentTypeAlias);
                _context.Services.ContentService.SaveAndPublish(page);
            }
            foreach (var property in _properties)
            {
                page.SetValue(property.Key,property.Value);
            }
            foreach (var contentBuilder in _children)
            {
                contentBuilder.SetParentId(page.Id).Build();
            }
            return page;
        }

        public ContentBuilder AddChild(string name, string contentTypeAlias)
        {
            _children.Add(new ContentBuilder(name,contentTypeAlias,_context));
            return this;
        }

        public ContentBuilder AddChild(ContentBuilder builder)
        {
            _children.Add(builder);
            return this;
        }

        public ContentBuilder SetParentId(int id)
        {
            _parentId = id;
            return this;
        }

        public ContentBuilder SetProperty(string propertyAlias, object value)
        {
            if (_properties.ContainsKey(propertyAlias))
                _properties[propertyAlias] = value;
            else
                _properties.Add(propertyAlias,value);

            return this;
        }        
    }
}

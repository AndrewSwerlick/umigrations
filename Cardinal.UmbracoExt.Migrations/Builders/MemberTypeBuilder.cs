using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.member;

namespace Cardinal.UmbracoExt.Migrations.Builders
{
    public class MemberTypeBuilder
    {
        private readonly string _alias;
        private readonly IList<PropertyDTO> _properties;

        public MemberTypeBuilder(string alias)
        {
            _alias = alias;
            _properties = new List<PropertyDTO>();
        }

        public MemberTypeBuilder AddProperty(string dataType, string propAlias, string propName)
        {
            _properties.Add(new PropertyDTO { Alias = propAlias, Name = propName, Type = dataType });
            return this;
        }

        public MemberType Build(ApplicationContext context)
        {
            var type = MemberType.GetByAlias("Member");
            if (type == null)
                type = MemberType.MakeNew(User.GetUser(0), "Member");

            foreach (var propertyDto in _properties)
            {
                var prop = type.getPropertyType(propertyDto.Alias);
                if (prop == null)
                {
                    var dataType =
                        context.Services.DataTypeService.GetAllDataTypeDefinitions().SingleOrDefault(d => d.Name == propertyDto.Type);
                    var legacyDt = DataTypeDefinition.GetDataTypeDefinition(dataType.Id);
                    type.AddPropertyType(legacyDt, propertyDto.Alias, propertyDto.Name);
                }
            }

            type.Save();
            return type;
        }
    }
}

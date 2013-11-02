using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cardinal.UmbracoExt.Migrations;
using Cardinal.UmbracoExt.Migrations.Builders;
using Umbraco.Core;
using Umbraco.Core.Models;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.member;

namespace Cardinal.UmbracoExt.Migrations.Tests.Web7.Migrations
{
    [VersionNumber("1.0")]
    public class V1_0 : IMigrationScript
    {
        public void Execute(ApplicationContext context)
        {
            new MemberTypeBuilder("Member").AddProperty("True/false", "hasVerifiedEmail", "Has Verified Email")
                                           .AddProperty("Textstring", "profileUrl", "Profile Url")
                                           .AddProperty("Textstring", "emailVerifyGUID", "Email Verify Guid")
                                           .AddProperty("Textstring", "joinedDate", "Joined Date")
                                           .AddProperty("Numeric", "numberOfLogins", "Number of Logins")
                                           .AddProperty("Textstring", "lastLoggedIn", "Last Logged In")
                                           .AddProperty("Textstring", "hostNameOfLastLogin", "Host of Last Login")
                                           .AddProperty("Textstring", "IPofLastLogin", "IP of Last Login");

            var group = MemberGroup.GetByName("Members");
            if(group == null)
                MemberGroup.MakeNew("Members", User.GetUser(0));

            new TemplateBuilder("SharedMaster", "SharedMaster").Build(context);         

            var generic = new ContentTypeBuilder("_genericProperties", "_GenericProperties")
                .AllowedAtRoot()
                .AddProperty("Textstring", "meta", "Meta")
                .Build(context);               

            var standard = new ContentTypeBuilder("standardPage", "Standard Page")
                .AllowedAtRoot()
                .AllowSelfAsChild()
                .SetParentType(generic) 
                .AddAllowedTemplates(new TemplateBuilder("LoginPage","LoginPage").Build(context))
                .CreateTemplate().Build(context);

            var homeType = new ContentTypeBuilder("Home", "Home")
                .CreateTemplate()
                .Build(context);

            var site = new ContentTypeBuilder("site", "Site")
                .AllowedAtRoot()
                .AddAllowedContentTypes(new List<IContentType> {homeType, standard})
                .AddProperty("Content Picker", "umbracoInternalRedirectId", "Umbraco Internal Redirect Id")
                .Build(context);

            var siteRoot = new ContentBuilder("Site", site.Alias, context).Build();
            var homePage = new ContentBuilder(siteRoot, "Home", homeType.Alias, context).Build();
            siteRoot.SetValue("umbracoInternalRedirectId",homePage.Id);
            context.Services.ContentService.SaveAndPublish(siteRoot);
            context.Services.ContentService.RePublishAll();
        }
    }
}
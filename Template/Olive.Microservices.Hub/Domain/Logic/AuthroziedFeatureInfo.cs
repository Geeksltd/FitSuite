namespace Olive.Microservices.Hub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using Olive;

    partial class AuthroziedFeatureInfo
    {
        public static XElement RenderMenu(Feature currentFeature)
        {
            var items = FeatureSecurityFilter.GetAuthorizedFeatures(Context.Current.User());
            //items = AddEverythingItem(items);
            return RenderMenu(currentFeature, items);
        }

        static XElement RenderMenu(Feature currentFeature, IEnumerable<AuthroziedFeatureInfo> items)
        {
            if (items.None()) return null;

            var rootMEnuId = Guid.NewGuid();

            if (currentFeature != null)
            {
                rootMEnuId = currentFeature.ID;
            }

            var nav = new XElement("div", new XAttribute("class", "nav"), new XAttribute("id", rootMEnuId));

            var rootUl = new XElement("ul", new XAttribute("class", ""), new XAttribute("id", rootMEnuId));
            rootUl.AddTo(nav);

            var scopeLi = new XElement("li", new XAttribute("class", "nav-blue"), new XAttribute("id", "ScopeLi"));
            var scopeH4 = new XElement("h4", new XAttribute("class", "nav-blue-title"), new XAttribute("id", "scopeH4"), "Scope");
            var scopeUl = new XElement("ul", new XAttribute("id", "scopeUl"));
            scopeH4.AddTo(scopeLi);
            scopeUl.AddTo(scopeLi);
            scopeLi.AddTo(rootUl);


            var functionalLi = new XElement("li", new XAttribute("class", "nav-orange"), new XAttribute("id", "functionalLi"));
            var functionalH4 = new XElement("h4", new XAttribute("class", "nav-orange-title"), new XAttribute("id", "functionalH4"), "Functional Design");
            var functionalUl = new XElement("ul", new XAttribute("id", "functionalUl"));
            functionalH4.AddTo(functionalLi);
            functionalUl.AddTo(functionalLi);
            functionalLi.AddTo(rootUl);


            var technicalLi = new XElement("li", new XAttribute("class", "nav-green"), new XAttribute("id", "technicalLi"));
            var technicalH4 = new XElement("h4", new XAttribute("class", "nav-green-title"), new XAttribute("id", "technicalH4"), "Technical Design");
            var technicalUl = new XElement("ul", new XAttribute("id", "technicalUl"));
            technicalH4.AddTo(technicalLi);
            technicalUl.AddTo(technicalLi);
            technicalLi.AddTo(rootUl);

            var logo = new XElement("img", new XAttribute("class", "logo-main-menu"), new XAttribute("src", "/img/logo/Logo.png"));
            logo.AddTo(technicalH4);
            var dropdown = new XElement("div", new XAttribute("class", "dropdown"));
            dropdown.AddTo(technicalH4);
            var dropbtn = new XElement("button", new XAttribute("class", "dropbtn"), "Hi ");
            dropbtn.AddTo(dropdown);
            var name = new XElement("b", new XAttribute("id", "accountName"), " ");
            name.AddTo(dropbtn);
            var caret = new XElement("i", new XAttribute("class", "fa fa-caret-down"), " ");
            caret.AddTo(dropbtn);
            var dropdown_content = new XElement("div", new XAttribute("class", "dropdown-content"));
            dropdown_content.AddTo(dropdown);
            var linkAccount = new XElement("a", new XAttribute("href", Config.Get("Website:url") + "/My-Account/Profile/Edit.aspx"), "Account");
            linkAccount.AddTo(dropdown_content);
            var linkLogOff = new XElement("a", new XAttribute("href", Config.Get("Website:url") + "/Logout.aspx"), "Log off");
            linkLogOff.AddTo(dropdown_content);

            foreach (var item in items)
            {
                var feature = item.Feature;

                var li = new XElement("li",
                    new XAttribute("id", feature.ID));

                var link = new XElement("a",
                    new XAttribute("href", item.AddQueryString()),
                    new XAttribute("data-badgeurl", feature.GetBadgeUrl().OrEmpty()),
                    new XAttribute("data-badge-optional", feature.IsBadgeOptional()),
                    new XAttribute("data-service", (feature.Service?.Name).OrEmpty()),
                    feature.Title
                ).AddTo(li);

                if (!item.IsDisabled && !feature.UseIframe)
                    link.Add(new XAttribute("data-redirect", "ajax"));

                if (feature.Title == "Features" || feature.Title == "Guesstimate")
                {
                    li.AddTo(scopeUl);
                }
                if (feature.Title == "Plan" || feature.Title == "Discover" ||
                    feature.Title == "Designer" || feature.Title == "Refine" || feature.Title == "Estimator")
                {
                    li.AddTo(functionalUl);
                }
                if (feature.Title == "Stories" || feature.Title == "Entities"
                                               || feature.Title == "Microservices"
                                               || feature.Title == "DeliveryPlan")
                {
                    li.AddTo(technicalUl);
                }

            }

            return nav;
        }

        string AddQueryString()
        {
            if (IsDisabled) return string.Empty;

            var query = Context.Current.Request().Query;

            var result = Feature.LoadUrl;

            if (Feature.Pass.HasAny() && query.Any())
            {

                var queryStringItems = (from key in Feature.Pass.Split(",")
                                        where query.ContainsKey(key)
                                        select key + "=" + query[key]).ToString("&");

                return $"{result.TrimEnd("/")}{queryStringItems.WithPrefix("?")}";
            }

            return result;
        }





        public class JsonMenu
        {
            public Guid ID { get; set; }
            public string Title { get; set; }
            public string Icon { get; set; }
            public string LoadUrl { get; set; }
            public string LogicalPath { get; set; }
            public bool UseIframe { get; set; }
            public HashSet<JsonMenu> Children { get; set; }
        }
    }
}
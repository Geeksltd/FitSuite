﻿using System.Security.Claims;
using System.Threading.Tasks;

using Olive;
using Olive.GlobalSearch;
using Olive.Microservices.Hub;

namespace Olive.Microservices.Hub
{
    public class GlobalSearchSource : SearchSource
    {
        public override async Task Process(ClaimsPrincipal user)
        {
            Add("Logout", "/logout");

            foreach (var feature in Feature.All)
            {
                if (feature.GetFullPath().Contains("WIDGETS")) continue;
                if (!MatchesKeywords(feature, x => x.GetFullPath(), x => x.Description)) continue;

                if (user.CanSee(feature))
                {
                    Add(new SearchResult
                    {
                        Title = feature.ToString(),
                        Url = feature.GetHubUrl() == "Hub" ? feature.LoadUrl : $"/{feature.GetHubUrl()}",
                        Description = feature.Description,
                        Colour = "#54AAAA",
                        IconUrl = "fas fa-arrows-alt"
                    });
                }
            }
        }
    }
}
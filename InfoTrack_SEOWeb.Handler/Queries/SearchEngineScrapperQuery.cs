using InforTrack_SEOWeb.Common.Configs;
using InfoTrack_SEOWeb.Handler.Infrastructure;
using InfoTrack_SEOWeb.Providers.Scrapper;
using InfoTrack_SEOWeb.Providers.Scrapper.Models;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Xml.XPath;

namespace InfoTrack_SEOWeb.Handler.Queries;

public record SearchEngineScrapperQuery([Required] string Url, [Required] string Keyword) : IQuery<string>;

public class SearchEngineScrapperQueryHandler : IQueryHandler<SearchEngineScrapperQuery, string>
{
    private readonly SEOConfig config;
    private readonly IScrapperProvider scrapperProvider;
    public SearchEngineScrapperQueryHandler(IOptions<SEOConfig> seoConfig, IScrapperProvider scrapperProvider)
    {
        this.config = seoConfig.Value;
        this.scrapperProvider = scrapperProvider;
    }
    public async Task<string> Handle(SearchEngineScrapperQuery request, CancellationToken cancellationToken)
    {
        var url = config.SearchURL.Replace("{keyword}", HttpUtility.UrlEncode(request.Keyword));
        using var client = new HttpClient();
        var contentHtml = await client.GetStringAsync(url, cancellationToken);

        var titleXPath = "//div//div//a//div//div//h3//div";
        var urlXPath = "//div//div//a";

        var data = scrapperProvider.ExtractObject(new ScrapperRequest()
        {
            ContentHTML = contentHtml,
            XPathParent = config.XPathParentList,
            XPathChildren = config.XPathItemNode,
            ChildrenNodeConfigs = new List<ChildrenNodeConfig>()
            {
            new ("Title", titleXPath),
            new ("Url", urlXPath, false, "href")
            },
            ChildValidWhen = (context) =>
            {
                if(context.XElement == null)
                {
                    return false;
                }

                if (context.XElement.XPathSelectElement(context.ChildXPath + titleXPath) == null)
        {
                    return false;
                }

                return true;
            }
        });

        List<int> founds = new List<int>();

        int index = 1;

        foreach (var item in data)
        {
            if (!item.TryGetValue("Title", out var _))
            {
                continue;
            }
            if (item.TryGetValue("Url", out var webUrl) && !string.IsNullOrEmpty(webUrl))
            {
                if (webUrl.Contains(request.Url))
                {
                    founds.Add(index);
                }
            }
            else
            {
                continue;
            }
            index++;
        }

        return string.Join(',', founds.Select(x => x.ToString()));

    }
}


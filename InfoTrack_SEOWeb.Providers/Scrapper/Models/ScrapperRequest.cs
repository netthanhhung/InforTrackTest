namespace InfoTrack_SEOWeb.Providers.Scrapper.Models;

public record ScrapperRequest(string ContentHTML, string XPathParent, string XPathChildren, IList<ChildrenNodeConfig> ChildrenNodeConfigs);
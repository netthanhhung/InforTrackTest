namespace InfoTrack_SEOWeb.Providers.Scrapper.Models;

public record ChildrenNodeConfig(string Name = "", string XPath = "", bool GetValueByContent = true, string AttributeValue = "");
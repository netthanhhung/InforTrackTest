using System.Xml.Linq;

namespace InfoTrack_SEOWeb.Providers.Scrapper.Models;

public class ChildValidWhenContext
{
    public XElement? XElement { get; set; }
    public string ChildXPath { get; set; } = "";
}
public delegate bool ChildValidWhenMethod(ChildValidWhenContext context);
public class ScrapperRequest
{
    public string ContentHTML { get; set; } = "";
    public string XPathParent { get; set; } = "";
    public string XPathChildren { get; set; } = "";
    public IList<ChildrenNodeConfig> ChildrenNodeConfigs { get; set; } = new List<ChildrenNodeConfig>();
    public ChildValidWhenMethod? ChildValidWhen {  get; set; }


}

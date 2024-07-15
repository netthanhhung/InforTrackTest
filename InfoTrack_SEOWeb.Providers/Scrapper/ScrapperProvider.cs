using InfoTrack_SEOWeb.Providers.Scrapper.Exceptions;
using InfoTrack_SEOWeb.Providers.Scrapper.Models;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace InfoTrack_SEOWeb.Providers.Scrapper;

public interface IScrapperProvider
{
    public IEnumerable<IDictionary<string, string>> ExtractObject(ScrapperRequest request);
}

public class ScrapperProvider : IScrapperProvider
{
    private readonly static string[] UnclosedTags = { "area", "base", "br", "col", "embed", "hr", "img", "input", "keygen", "link", "menuitem", "meta", "param", "source", "track", "wbr" };
    private readonly static string[] UnnecessaryTags = { "script", "noscript", "style" };
    public IEnumerable<IDictionary<string, string>> ExtractObject(ScrapperRequest request)
    {
        string html = NormalizeHtml(request.ContentHTML);
        XDocument document = XDocument.Parse(html);

        var parentNode = document.XPathSelectElement(request.XPathParent);

        if (parentNode == null)
        {
            throw new ParentNodeNotFoundException();
        }

        var childNodes = parentNode.XPathSelectElements(request.XPathParent + request.XPathChildren);

        int index = 1;

        foreach(var childNode in childNodes)
        {
            var childFields = new Dictionary<string, string>();
            foreach (var config in request.ChildrenNodeConfigs)
            {
                var fieldNode = childNode.XPathSelectElement(request.XPathParent + request.XPathChildren + $"[{index}]" + config.XPath);
                if (fieldNode == null)
                {
                    continue;
                }
                if (config.GetValueByContent)
                {
                    childFields.Add(config.Name, fieldNode.Value);
                }
                else
                {
                    childFields.Add(config.Name, fieldNode.Attribute(config.AttributeValue)?.Value ?? "");
                }
            }

            index++;

            yield return childFields;
        }
    }

    private string NormalizeHtml(string html)
    {
        // extract body tag
        var closeTag = "</body>";
        int firstIndex = html.IndexOf("<body ");
        int lastIndex = html.LastIndexOf(closeTag);
        string content = html.Substring(firstIndex, lastIndex - firstIndex + closeTag.Length);

        // remove unnecessary tag
        foreach (var tag in UnnecessaryTags)
        {
            Regex rRem = new Regex(@$"<{tag}[^>]*>[\s\S]*?</{tag}>");
            content = rRem.Replace(content, string.Empty);
        }

        // Remove not closing tag
        foreach (var tag in UnclosedTags)
        {
            content = Regex.Replace(content, $"<{tag}.*?>", string.Empty);
        }

        content = content.Replace("&nbsp;", string.Empty);

        return content;
    }
}

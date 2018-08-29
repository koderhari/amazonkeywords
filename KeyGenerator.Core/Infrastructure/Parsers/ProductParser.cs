using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using KeyGenerator.Core.Parsers.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeyGenerator.Core.Parsers
{
    public class ProductParser : IProductParser
    {
        private List<string> _selectors;

        public ProductParser()
        {
            _selectors = new List<string>()
            {
                "#productTitle"
            };
        }

        public string[] ParseProductCompletion(string json)
        {
            try
            {
                var jobject = JArray.Parse(json);
                return jobject[1].ToObject<string[]>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error while parse completion json {json}", e);
            }

        }

        public string ParseProductPage(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var document = doc.DocumentNode;
            var result = new StringBuilder();
            var titleNode = document.QuerySelector("#productTitle");
            result.AppendLine(titleNode?.InnerText.Trim());
            var features = document.QuerySelectorAll("#feature-bullets .a-list-item");
            foreach (var feature in features)
            {
                result.AppendLine(feature.InnerText.Trim());
            }

            result.AppendLine(document.QuerySelector("#productDescription p")?.InnerText.Trim());
            //product detail сложная инфа
            //#detail-bullets
            return result.ToString();
        }
    }
}

using KeyGenerator.Core.Infrastructure.Helpers;
using KeyGenerator.Core.Parsers.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KeyGenerator.Core.Infrastructure
{
    public class KeywordsService : IKeywordsService
    {
        //to-do move to settings
        private static readonly string urlTemplate = "https://completion.amazon.com/search/complete?mkt=1&l=en_US&sv=desktop&search-alias=aps&q={0}";
        private IProductParser _productParser;
        private IKeywordsExtraction _keyWordExtractor;
        private IHttpClientFactory _clientFactory;

        public KeywordsService(
            IProductParser productParser,
            IKeywordsExtraction keyWordExtractor,
            IHttpClientFactory clientFactory)
        {
            _productParser = productParser;
            _keyWordExtractor = keyWordExtractor;
            _clientFactory = clientFactory;
        }

        public async Task<List<string>> ExctractKeywordsForProductPageAsync(string url)
        {
            var client = _clientFactory.CreateClient();
                //to-do move to factory
            client.DefaultRequestHeaders.Add(HttpHeader.UserAgent, HttpHeader.UserAgent);
            var productPage = await client.GetStringAsync(url);
            var productDescription = _productParser.ParseProductPage(productPage);
            return _keyWordExtractor.Exctract(productDescription, 10);
        }

        public async Task<List<string>> GetSuggestionsAsync(string[] seeds)
        {
            var tasks = new List<Task>(seeds.Length*37);
            var result = new ConcurrentQueue<string>();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //var oneClient = _clientFactory.CreateClient();
            foreach (var seed in seeds)
            {
                foreach (var item in GetUrlsEnum(seed.Trim()))
                {
                    tasks.Add(Task.Run(() => ProccessAsync(_clientFactory.CreateClient(), item, result)));
                }
            }

            await Task.WhenAll(tasks);
            stopWatch.Stop();

            return result.ToHashSet().ToList();
        }

        private IEnumerable<string> GetUrlsEnum(string queryWord)
        {
            
            yield return string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode(queryWord));

            for (int i = (int)'a'; i <= (int)'z'; i++)
            {

                yield return string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode($"{queryWord}{(char)i}"));
            }

            for (int i = (int)'0'; i <= (int)'9'; i++)
            {
                yield return string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode($"{queryWord}{(char)i}"));
            }
            yield break;
        }

        private async Task ProccessAsync(HttpClient client, string url, ConcurrentQueue<string> container)
        {
            try
            {
                var response = await client.GetStringAsync(url);
                foreach (var item in _productParser.ParseProductCompletion(response))
                {
                    container.Enqueue(item);
                }
            }
            catch (Exception e)
            {
                //to-do add log и спросить что делать
                //Console.WriteLine($" {e}");
            }
        }

    }
}

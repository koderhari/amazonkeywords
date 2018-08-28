using KeyGenerator.Core.Infrastructure.Helpers;
using KeyGenerator.Core.Parsers.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KeyGenerator.Core.Infrastructure
{
    public class KeywordsService : IKeywordsService
    {
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
            using (var client = _clientFactory.CreateClient())
            {
                //to-do move to factory
                client.DefaultRequestHeaders.Add(HttpHeader.UserAgent, HttpHeader.UserAgent);
                var productPage = await client.GetStringAsync(url);
                var productDescription = _productParser.Parse(productPage);
                return _keyWordExtractor.Exctract(productDescription, 10);
            }
        }
    }
}

using KeyGenerator.Core.Infrastructure;
using KeyGenerator.Core.Parsers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeyGenerator.Tests
{
    [TestFixture]
    class KeywordsServiceTest
    {


        [Test]
        public void KeyWordsGenerateTest()
        {
            var rake = new RakeAlgorithm(TestEnvironment.STOPLIST,3,2);
            var parser = new ProductParser();
            var mock = new Mock<IHttpClientFactory>();
            var testClient = new HttpClient(new TestProductPageHandler());
            mock.Setup(x => x.CreateClient(string.Empty)).Returns(testClient);
            var keyWordsService = new KeywordsService(parser, rake, mock.Object);
            var keyWords= keyWordsService.ExctractKeywordsForProductPageAsync("https://www.amazon.com/dp/B079P5Q8T6/ref=sspa_dk_detail_2?psc=1https://www.amazon.com/dp/B079P5Q8T6/ref=sspa_dk_detail_2?psc=1").Result;
            Assert.AreEqual(keyWords.Count, 10, "Invalid count keyWords");
            var msg = "There are not keyword {0}";
            var listKeyWords = new List<string>()
            {
                "e5 men",
                "ncaa hoodie",
                "machine washable",
                "school pride",
                "hoodie features",
                "embroidered school",
                "game day",
                "hands cozy",
                "season long",
                "imported"
            };
            foreach (var keyWord in listKeyWords)
            {
                Assert.True(keyWords.Contains(keyWord), string.Format(msg,keyWord));
            }
           
        }

        [Test]
        public void GetSuggestionsSuccess()
        {
            var parser = new ProductParser();
            var original_seeds = new List<string>
            {
                "1seed",
                "2seed",
                "3seed"
            };
            var urls = new ConcurrentQueue<string>();
            var jsonStr = @"[""suit"",[""suitcase"",""suits season 8""],[{""nodes"":[{""name"":""Luggage & Travel Gear"",""alias"":""fashion-luggage""}]},{},{},{},{},{},{},{},{},{}],[],""3XPFVRN05ZSW""]";
            var mock = new Mock<IHttpClientFactory>();
            var testClient = new HttpClient(new TestCompletionHandler(urls, jsonStr));
            mock.Setup(x => x.CreateClient(string.Empty)).Returns(testClient);
            var rake = new RakeAlgorithm(TestEnvironment.STOPLIST, 3, 2);
            var keyWordsService = new KeywordsService(parser, rake, mock.Object);

            var suggests = keyWordsService.GetSuggestionsAsync(original_seeds.ToArray()).Result;

            Assert.AreEqual(suggests.Count,2, "Invalid count suggests");
            var msg = "Suggests does not contains";
            Assert.True(suggests.Contains("suitcase"), msg + "suitcase");
            Assert.True(suggests.Contains("suits season 8"), msg + "suits season 8");
            Assert.AreEqual(urls.Count, 3 * 37, "Invalid count urls");
        }

        class TestProductPageHandler : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(File.ReadAllText(TestEnvironment.PRODUCT1HTML))
                };

                return Task.FromResult(responseMessage);
            }
        }

        class TestCompletionHandler : HttpClientHandler
        {
            private ConcurrentQueue<string> _urls;
            private string _response;

            public TestCompletionHandler(ConcurrentQueue<string> urls,string response)
            {
                _urls = urls;
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                _urls.Enqueue(request.RequestUri.ToString());
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_response)
                };

                return Task.FromResult(responseMessage);
            }
        }
    }
}

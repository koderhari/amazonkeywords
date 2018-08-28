using KeyGenerator.Core.Infrastructure;
using KeyGenerator.Core.Parsers;
using Moq;
using NUnit.Framework;
using System;
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
        class TestHandler : HttpClientHandler
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

        [Test]
        public void KeyWordsGenerateTest()
        {
            var rake = new RakeAlgorithm(TestEnvironment.STOPLIST,3,2);
            var parser = new ProductParser();
            var mock = new Mock<IHttpClientFactory>();
            var testClient = new HttpClient(new TestHandler());
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
    }
}

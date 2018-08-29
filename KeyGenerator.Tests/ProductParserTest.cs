using KeyGenerator.Core.Parsers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KeyGenerator.Tests
{
    [TestFixture]
    public class ProductParserTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParseProductAllSuccess()
        {
            var productParser = new ProductParser();
            
            var description = productParser.ParseProductPage(File.ReadAllText(TestEnvironment.PRODUCT1HTML));

            Assert.True(description.Contains("E5 Men's NCAA Hoodie"),"Invalid product title");
            var invalidProductFeature = "Invalid product feature";
            Assert.True(description.Contains("70% Cotton/30% Polyester"), invalidProductFeature);
            Assert.True(description.Contains("Machine Washable"), invalidProductFeature);
            Assert.True(description.Contains("Embroidered/Tackle Twill logos & Front Pouch"), invalidProductFeature);
            Assert.True(description.Contains("Officially Licensed Collegiate Product"), invalidProductFeature);
            Assert.True(description.Contains("Imported"), invalidProductFeature);
            var des = "Show off your school pride in this officially licensed NCAA pullover hoodie! This hoodie features a double layer tackle twill and embroidered school name arched over logo . The additional school logo on the sleeve and contrasting team color hood lining make it perfect for game day. Keep your hands cozy in the front pouch pocket all season long!";
            Assert.True(description.Contains(des),"Invalid product description");
        }

        [Test]
        public void ParseCompletionSuccess()
        {
            var productParser = new ProductParser();
            var origSuggestions = new List<string>()
            {
                "suitcase",
                "suits season 8",
                "suitcases with wheels",
                "suit bag",
                "suit travel bag",
                "suitcase lock",
                "suitcase organizer",
                "suit bags for men",
                "suits",
                "suit hanger"
            };
            var jsonStr = @"[""suit"",[""suitcase"",""suits season 8"",""suitcases with wheels"",""suit bag"",""suit travel bag"",""suitcase lock"",""suitcase organizer"",""suit bags for men"",""suits"",""suit hanger""],[{""nodes"":[{""name"":""Luggage & Travel Gear"",""alias"":""fashion-luggage""}]},{},{},{},{},{},{},{},{},{}],[],""3XPFVRN05ZSW""]";
            var suggestion = productParser.ParseProductCompletion(jsonStr).ToHashSet();
            foreach (var item in origSuggestions)
            {
                Assert.True(suggestion.Contains(item),$"{item} is not in result");
            }
        }

        [Test]
        public void ParseCompletionInvalidJson()
        {
            var productParser = new ProductParser();
            var jsonStr = @"[""suit""]";
            Assert.Throws<InvalidOperationException>(() => productParser.ParseProductCompletion(jsonStr));
        }


    }
}

using KeyGenerator.Core.Parsers;
using NUnit.Framework;
using System;
using System.IO;

namespace KeyGenerator.Tests
{
    [TestFixture]
    public class ProductParserTest
    {
        [SetUp]
        public void Setup()
        {
        }
        //todo проверить связи
        //некорректный формат
        [Test]
        public void ParseProductAllSuccess()
        {
            var productParser = new ProductParser();
            
            var description = productParser.Parse(File.ReadAllText(TestEnvironment.PRODUCT1HTML));

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
    }
}

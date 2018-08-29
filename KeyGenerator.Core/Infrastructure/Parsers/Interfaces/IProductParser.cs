using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KeyGenerator.Core.Parsers.Interfaces
{
    public interface IProductParser
    {
        string ParseProductPage(string html);

        string[] ParseProductCompletion(string json);
    }
}

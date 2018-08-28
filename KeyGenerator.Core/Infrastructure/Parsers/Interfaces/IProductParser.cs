using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KeyGenerator.Core.Parsers.Interfaces
{
    public interface IProductParser
    {
        string Parse(string html);
    }
}

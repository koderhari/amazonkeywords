using System;
using System.Collections.Generic;
using System.Text;

namespace KeyGenerator.Core.Infrastructure
{
    public interface IKeywordsExtraction
    {
        List<string> Exctract(string text,int? top = null);
    }
}

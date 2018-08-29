using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KeyGenerator.Core.Parsers.Interfaces
{
    public interface IKeywordsService
    {
        Task<List<string>> ExctractKeywordsForProductPageAsync(string url);

        Task<List<string>> GetSuggestionsAsync(string[] seeds);
    }
}

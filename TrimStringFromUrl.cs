using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace azuredCreateClient
{
    class TrimStringFromUrl
    {
        string urlString;

        public TrimStringFromUrl(string urlString)
        {
            this.urlString = urlString;
        }

        public string returnCode()
        {
            // Create a pattern for a word that starts with letter "M"  
            string pattern = @"code=(.*?)&state=";
            char[] charsToTrimStart = { 'c', 'o', 'd', 'e', '=' };
            char[] charsToTrimEnd = { '&', 's', 't', 'a', 't', 'e', '=' };
            // Create a Regex  
            Regex rg = new Regex(pattern);

            MatchCollection matchedAuthors = rg.Matches(this.urlString);
            Console.WriteLine(matchedAuthors.Count);
            string initialCode = matchedAuthors[0].ToString().TrimStart(charsToTrimStart).TrimEnd(charsToTrimEnd);

            return initialCode;
        }
    }
}

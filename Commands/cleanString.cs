using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class CleanString
    {
        //private static readonly Regex sWhitespace = new Regex(@"\s+");
        
        public static string RegexClean(string strIn)
        {
            return Regex.Replace(strIn, "[_]", " ");
        }
        public static string RemoveSpace(string strIn)
        {
            return Regex.Replace(strIn, @"\s+", string.Empty);
        }
        public static string RemoveCharacters(string strIn)
        {
            return Regex.Replace(strIn, @"[!?\,\.']", string.Empty);
        }
    }
}
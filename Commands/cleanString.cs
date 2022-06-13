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
        public static string RegexClean(string strIn)
        {
            return Regex.Replace(strIn, "[_]", " ");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoreLinq; // http://stackoverflow.com/questions/15265588/how-to-find-item-with-max-value-using-linq

namespace GeoReVi
{
    public class CsvHelper
    {
        public static Dictionary<LineSeparator, Func<string, string[]>> DictionaryOfLineSeparatorAndItsFunc = new Dictionary<LineSeparator, Func<string, string[]>>();

        static CsvHelper()
        {
            DictionaryOfLineSeparatorAndItsFunc[LineSeparator.Unknown] = ParseLineNotSeparated;
            DictionaryOfLineSeparatorAndItsFunc[LineSeparator.Tab] = ParseLineTabSeparated;
            DictionaryOfLineSeparatorAndItsFunc[LineSeparator.Semicolon] = ParseLineSemicolonSeparated;
            DictionaryOfLineSeparatorAndItsFunc[LineSeparator.Comma] = ParseLineCommaSeparated;
        }

        // ******************************************************************
        public enum LineSeparator
        {
            Unknown = 0,
            Tab,
            Semicolon,
            Comma
        }

        // ******************************************************************
        public static LineSeparator GuessCsvSeparator(string oneLine)
        {
            List<Tuple<LineSeparator, int>> listOfLineSeparatorAndThereFirstLineSeparatedValueCount = new List<Tuple<LineSeparator, int>>();

            listOfLineSeparatorAndThereFirstLineSeparatedValueCount.Add(new Tuple<LineSeparator, int>(LineSeparator.Tab, CsvHelper.ParseLineTabSeparated(oneLine).Count()));
            listOfLineSeparatorAndThereFirstLineSeparatedValueCount.Add(new Tuple<LineSeparator, int>(LineSeparator.Semicolon, CsvHelper.ParseLineSemicolonSeparated(oneLine).Count()));
            listOfLineSeparatorAndThereFirstLineSeparatedValueCount.Add(new Tuple<LineSeparator, int>(LineSeparator.Comma, CsvHelper.ParseLineCommaSeparated(oneLine).Count()));

            Tuple<LineSeparator, int> bestBet = listOfLineSeparatorAndThereFirstLineSeparatedValueCount.MaxBy((n) => n.Item2).Select(x=>x).First();

            if (bestBet != null && bestBet.Item2 > 1)
            {
                return bestBet.Item1;
            }

            return LineSeparator.Unknown;
        }

        // ******************************************************************
        public static string[] ParseLineCommaSeparated(string line)
        {
            // CSV line parsing : From "jgr4" in http://www.kimgentes.com/worshiptech-web-tools-page/2008/10/14/regex-pattern-for-parsing-csv-files-with-embedded-commas-dou.html
            var matches = Regex.Matches(line, @"\s?((?<x>(?=[,]+))|""(?<x>([^""]|"""")+)""|""(?<x>)""|(?<x>[^,]+)),?",
                                        RegexOptions.ExplicitCapture);

            string[] values = (from Match m in matches
                               select m.Groups["x"].Value.Trim().Replace("\"\"", "\"")).ToArray();

            return values;
        }

        // ******************************************************************
        public static string[] ParseLineTabSeparated(string line)
        {
            var matchesTab = Regex.Matches(line, @"\s?((?<x>(?=[\t]+))|""(?<x>([^""]|"""")+)""|""(?<x>)""|(?<x>[^\t]+))\t?",
                            RegexOptions.ExplicitCapture);

            string[] values = (from Match m in matchesTab
                               select m.Groups["x"].Value.Trim().Replace("\"\"", "\"")).ToArray();

            return values;
        }

        // ******************************************************************
        public static string[] ParseLineSemicolonSeparated(string line)
        {
            // CSV line parsing : From "jgr4" in http://www.kimgentes.com/worshiptech-web-tools-page/2008/10/14/regex-pattern-for-parsing-csv-files-with-embedded-commas-dou.html
            var matches = Regex.Matches(line, @"\s?((?<x>(?=[;]+))|""(?<x>([^""]|"""")+)""|""(?<x>)""|(?<x>[^;]+));?",
                                        RegexOptions.ExplicitCapture);

            string[] values = (from Match m in matches
                               select m.Groups["x"].Value.Trim().Replace("\"\"", "\"")).ToArray();

            return values;
        }

        // ******************************************************************
        public static string[] ParseLineNotSeparated(string line)
        {
            string[] lineValues = new string[1];
            lineValues[0] = line;
            return lineValues;
        }

        // ******************************************************************
        public static List<string[]> ParseText(string text)
        {
            string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            return ParseString(lines);
        }

        // ******************************************************************
        public static List<string[]> ParseString(string[] lines)
        {
            List<string[]> result = new List<string[]>();

            LineSeparator lineSeparator = LineSeparator.Unknown;
            if (lines.Any())
            {
                lineSeparator = GuessCsvSeparator(lines[0]);
            }

            Func<string, string[]> funcParse = DictionaryOfLineSeparatorAndItsFunc[lineSeparator];

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                result.Add(funcParse(line));
            }

            return result;
        }

        // ******************************************************************
    }
}
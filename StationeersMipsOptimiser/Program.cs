using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace StationeersMipsOptimiser
{
   class Program
   {
      static void Main(string[] args)
      {
         string file = Encoding.UTF8.GetString(File.ReadAllBytes(args[0]));
         file = RemoveInactiveLines(file);
         file = RemoveWhitespace(file);
         file = RemoveAlias(file);
         file = RemoveJumpAlias(file);

         Console.WriteLine(file);
         return;
      }

      private static string RemoveWhitespace(string file)
      {
         List<string> lines = new List<string>(file.Split(new char[1] { '\n' }, StringSplitOptions.None));
         for (int i = 0; i < lines.Count; i++)
         {
            lines[i] = lines[i].Trim();
         }
         return string.Join("\n", lines);
      }

      private static string RemoveAlias(string file)
      {
         Dictionary<string, string> aliasList = new Dictionary<string, string>();
         List<string> lines = new List<string>(file.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
         for (int i = 0; i < lines.Count; i++)
         {
            string line = lines[i].TrimStart();

            if (line.StartsWith("alias"))
            {

               char[] seperators = new char[2] { '\t', ' ' };
               string[] alias = line.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
               lines.RemoveAt(i);
               aliasList[alias[1]] = alias[2];
               i--; // Changed offset.
            }
         }
         file = string.Join("\n", lines);
         // Second pass
         foreach (var key in aliasList.Keys)
         {
            file = Regex.Replace(file, $"\\b{key}\\b", aliasList[key]);
         }
         return file;
      }

      private static string RemoveJumpAlias(string file)
      {
         Dictionary<string, int> aliasList = new Dictionary<string, int>();
         List<string> lines = new List<string>(file.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
         for (int i = 0; i < lines.Count; i++)
         {
            string line = lines[i].TrimEnd();
            int coloIndex = line.IndexOf(':');
            int hashIndex;
            if ((coloIndex > 0) && ((((hashIndex = line.IndexOf('#')) > 1) && hashIndex > coloIndex) || hashIndex == -1))
            {
               lines.RemoveAt(i);
               
               string alias = line.Substring(0, line.IndexOf(':'));
               aliasList[alias] = i;
               i--; // Changed offset.
            }
         }
         file = string.Join("\n", lines);
         // Second pass
         foreach (var key in aliasList.Keys)
         {
            file = Regex.Replace(file, $"\\b{key}\\b", $"{aliasList[key]}");
         }
         return file;
      }

      // Removes lines that provide no action
      private static string RemoveInactiveLines(string file)
      {
         List<string> lines = new List<string>(file.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
         for (int i = 0; i < lines.Count; i++)
         {
            string line = lines[i].TrimStart();

            if (line.Length == 0 || line.StartsWith("#") || line.StartsWith("\r"))
            {
               lines.RemoveAt(i);
               i--; // Removed line, Index shift.
            }
            else if (line.Contains("#"))
            {
               lines[i] = line.Substring(0, line.IndexOf("#"));
            }
            lines[i] = lines[i].Replace("\r", "");
         }
         return string.Join("\n", lines);
      }
   }
}

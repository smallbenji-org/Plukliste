using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plukliste
{
    internal static class Template
    {
        public static string SelectTemplate(ConsoleColor standardColor, ConsoleColor highlight)
        {
            int index = 0;

            List<string> files = new List<string>();
            files = Directory.EnumerateFiles("templates").ToList();

            char readKey = ' ';
            while (readKey != 'R')
            {               
                ViewFrames.Templates(standardColor, highlight, files, index);

                readKey = Console.ReadKey().KeyChar;
                if (readKey >= 'a') readKey -= (char)('a' - 'A'); //HACK: To upper
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                switch (readKey)
                {
                    case 'N':
                        if (index < files.Count - 1) index++;
                        break;
                    case 'F':
                        index--;
                        break;
                    case 'G':
                        Console.WriteLine("Genindlæst templates");
                        files = Directory.EnumerateFiles("templates").ToList();
                        break;
                    case 'V':
                        return files[index];
                }

                if (index == -1) index = 0;

                Console.ForegroundColor = standardColor; //reset color
            }

            return "";
        }
    }
}

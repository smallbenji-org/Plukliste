using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plukliste
{
    internal static class Dialog
    {
        public static void PartlyColorLine(int colorLength, string text, ConsoleColor color)
        // colorLength is for how many letters of the text we are coloring.
        {
            int top = Console.CursorTop;
            ConsoleColor standardColor = Console.ForegroundColor;

            foreach (char c in text)
            {
                if (colorLength >= 1) { Console.ForegroundColor = color; } else
                {
                    Console.ForegroundColor = standardColor;
                }

                Console.Write(c);

                colorLength -= 1;
            }
            Console.WriteLine(""); // add the last new line.
        }

        public static void ColorLine(string text, ConsoleColor color)
        {
            ConsoleColor standardColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = standardColor;
        }

        public static void FormatLinesColored(string format, string coloredText, string uncoloredText, ConsoleColor color) 
        {
            ConsoleColor standardColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.Write(format, coloredText);
            Console.ForegroundColor = standardColor;
            Console.Write("{0}", uncoloredText+"\n");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plukliste
{
    internal static class ViewFrames
    {
        private static void Options(ConsoleColor standardColor)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nOptions:");
            Console.ForegroundColor = standardColor;
        }

        public static void Plukliste(List<string> files, ConsoleColor highlight, ConsoleColor standardColor, int index, string templateFile = "")
        {
            if (files.Count == 0)
            {
                Console.WriteLine("No order files found.");

            }
            else
            {
                if (index == -1) index = 0;

                Dialog.ColorLine($"[Plukliste {index + 1} af {files.Count}]", ConsoleColor.White);
                Console.WriteLine();
                Dialog.FormatLinesColored("{0, -18}", "Viewing file:", files[index], ConsoleColor.Yellow);
                Dialog.FormatLinesColored("{0, -18}", "Current template:", templateFile, ConsoleColor.Yellow);
                Console.WriteLine();

                //read file
                FileStream file = File.OpenRead(files[index]);
                System.Xml.Serialization.XmlSerializer xmlSerializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
                Pluklist plukliste = (Pluklist?)xmlSerializer.Deserialize(file);

                //print plukliste
                if (plukliste != null && plukliste.Lines != null)
                {
                    Dialog.FormatLinesColored("{0, -13}", "Name:", plukliste.Name, ConsoleColor.White);
                    Dialog.FormatLinesColored("{0, -13}", "Forsendelse:", plukliste.Forsendelse, ConsoleColor.White);
                    //TODO: Add adresse to screen print

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
                    Console.ForegroundColor = standardColor;
                    foreach (var item in plukliste.Lines)
                    {
                        Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                    }
                }
                file.Close();
            }

            ViewFrames.Options(standardColor);
            Console.ForegroundColor = ConsoleColor.White;
            Dialog.PartlyColorLine(1, "Quit", highlight);          
            if (index >= 0)
            {
                Dialog.PartlyColorLine(1, "Afslut plukseddel", highlight);
            }
            if (index > 0)
            {
                Dialog.PartlyColorLine(1, "Forrige plukseddel", highlight);
            }
            if (index < files.Count - 1)
            {
                Dialog.PartlyColorLine(1, "Næste plukseddel", highlight);
            }
            Dialog.PartlyColorLine(1, "Genindlæs pluksedler", highlight);

            Dialog.PartlyColorLine(1, "Vælg template", highlight);
            if (templateFile != "") { Dialog.PartlyColorLine(1, "Eksporter HTML", highlight); }
            Console.ForegroundColor = standardColor;
        }

        public static void Templates(ConsoleColor standardColor, ConsoleColor highlight, List<string> files, int index)
        {
            Console.Clear();
            Console.ForegroundColor = standardColor;

            if (files.Count == 0) 
            {
                Console.WriteLine("No template files found.");
            }
            else
            {
                Dialog.ColorLine($"[Template {index + 1} af {files.Count}]", ConsoleColor.White);
                Console.WriteLine();
                Dialog.FormatLinesColored("{0, -18}", "Viewing template:", files[index], ConsoleColor.Yellow);
            }

            Options(standardColor);
            Console.ForegroundColor = ConsoleColor.White;           
            if (index < files.Count - 1)
            {
                Dialog.PartlyColorLine(1, "Næste template", highlight);
            }
            if (index > 0)
            {
                Dialog.PartlyColorLine(1, "Forrige template", highlight);
            }            
            
            Dialog.PartlyColorLine(1, "Genindlæs templates", highlight);
            Dialog.PartlyColorLine(1, "Vælg template", highlight);
            Dialog.PartlyColorLine(1, "Return", highlight);
            Console.ForegroundColor = standardColor;
        }
    }
}

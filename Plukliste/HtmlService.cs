using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Plukliste
{
    public class HtmlService
    {
        public static void SaveHtmlFile(string content)
        {
            string[] temp = Directory.GetDirectories(Directory.GetCurrentDirectory());

            bool printDirectoryExists = temp.Any(dir => dir == Directory.GetCurrentDirectory() + "print");

            if (!printDirectoryExists)
            {
                Console.WriteLine("Directory \"print\" not found, creating folder");
                Directory.CreateDirectory("print");
            }

            string fileName = Guid.NewGuid().ToString();
            File.Create(Path.Combine(Directory.GetCurrentDirectory(), "print", $"{fileName}.html")).Close();
            
            using (StreamWriter sw = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "print", $"{fileName}.html")))
            {
                sw.WriteLine("<!DOCTYPE html>");
                sw.WriteLine("<html lang=\"en\">");
                sw.WriteLine("<head>");
                sw.WriteLine("<meta charset=\"UTF-8\">");
                sw.WriteLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                sw.WriteLine("<title>Plukliste</title>");
                sw.WriteLine("</head>");
                sw.WriteLine("<body>");
                sw.WriteLine(content);
                sw.WriteLine("</body>");
                sw.WriteLine("</html>");
            }
        }
    }
}

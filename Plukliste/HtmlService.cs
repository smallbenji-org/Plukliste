using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Plukliste
{
    public class HtmlService
    {
        public string? GenerateHTML(Pluklist pluklist, string templateFileName)
        {
            string templateData = "";

            string dir = Path.Combine(Directory.GetCurrentDirectory(), "templates");

            bool fileExists = Directory.GetFiles(dir).Any(file => file == Path.Combine(dir, templateFileName));

            string templatePath = Path.Combine(dir, templateFileName);

            if (!fileExists)
            {
                Console.WriteLine($"Template file \"{templateFileName}\" not found in templates directory.");
                return null;
            }

            using (StreamReader sr = new StreamReader(templatePath))
            {
                templateData = sr.ReadToEnd();
            }

            templateData = templateData.Replace("[Name]", pluklist.Name);
            templateData = templateData.Replace("[Adresse]", pluklist.Adresse);

            foreach (var item in pluklist.Lines)
            {
                templateData = templateData.Replace("[Plukliste]", $"<tr><td>{item.Amount}</td><td>{item.Type}</td><td>{item.ProductID}</td><td>{item.Title}</td></tr>");
            }

            return templateData;
        }

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
                sw.WriteLine(content);
            }
        }
    }
}

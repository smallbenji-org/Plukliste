//Eksempel på funktionel kodning hvor der kun bliver brugt et model lag
namespace Plukliste;

class PluklisteProgram
{

    static void Main()
    {
        //Arrange
        char readKey = ' ';
        List<string> files;
        var index = 0;
        var standardColor = Console.ForegroundColor;
        string templateFile = "";
        Directory.CreateDirectory("templates");

        Directory.CreateDirectory("import");


        if (!Directory.Exists("export"))
        {
            Console.WriteLine("Directory \"export\" not found");
            Console.ReadLine();
            return;
        }
        files = Directory.EnumerateFiles("export").ToList();

        //ACT
        while (readKey != 'Q')
        {
            // Plukliste view
            ConsoleColor highlight = ConsoleColor.Green;
            ViewFrames.Plukliste(files, highlight, standardColor, index, templateFile);

            readKey = Console.ReadKey().KeyChar;
            if (readKey >= 'a') readKey -= (char)('a' - 'A'); //HACK: To upper
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red; //status in red
            switch (readKey)
            {
                case 'G':
                    files = Directory.EnumerateFiles("export").ToList();
                    index = -1;
                    Console.WriteLine("Pluklister genindlæst");
                    break;
                case 'F':
                    if (index > 0) index--;
                    break;
                case 'N':
                    if (index < files.Count - 1) index++;
                    break;
                case 'A':
                    //Move files to import directory
                    var filewithoutPath = files[index].Substring(files[index].LastIndexOf('\\'));
                    File.Move(files[index], string.Format(@"import\\{0}", filewithoutPath));
                    Console.WriteLine($"Plukseddel {files[index]} afsluttet.");
                    files.Remove(files[index]);
                    if (index == files.Count) index--;
                    break;
                case 'V': // Vælg template
                    templateFile = Template.SelectTemplate(standardColor, highlight);
                    break;
                case 'E': // Eksporter fil med template

                    break;
            }
            Console.ForegroundColor = standardColor; //reset color
        }

    }

}

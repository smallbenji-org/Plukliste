//Eksempel på funktionel kodning hvor der kun bliver brugt et model lag
namespace Plukliste;

class PluklisteProgram { 

    static void Main()
    {
        //Arrange
        char readKey = ' ';
        List<string> files;
        var index = -1;
        var standardColor = Console.ForegroundColor;
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
            if (files.Count == 0)
            {
                Console.WriteLine("No files found.");

            }
            else
            {
                if (index == -1) index = 0;

                Console.WriteLine($"Plukliste {index + 1} af {files.Count}");
                Console.WriteLine($"\nfile: {files[index]}");

                //read file
                FileStream file = File.OpenRead(files[index]);
                System.Xml.Serialization.XmlSerializer xmlSerializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
                var plukliste = (Pluklist?)xmlSerializer.Deserialize(file);

                //print plukliste
                if (plukliste != null && plukliste.Lines != null)
                {
                    Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name);
                    Console.WriteLine("{0, -13}{1}", "Forsendelse:", plukliste.Forsendelse);
                    //TODO: Add adresse to screen print

                    Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
                    foreach (var item in plukliste.Lines)
                    {
                        Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                    }
                }
                file.Close();
            }

            //Print options
            Console.WriteLine("\n\nOptions:");
            Dialog.ColorLine(1, "Quit", ConsoleColor.Green);
            if (index >= 0)
            {
                Dialog.ColorLine(1, "Afslut plukseddel", ConsoleColor.Green);
            }
            if (index > 0)
            {
                Dialog.ColorLine(1, "Forrige plukseddel", ConsoleColor.Green);
            }
            if (index < files.Count - 1)
            {
                Dialog.ColorLine(1, "Næste plukseddel", ConsoleColor.Green);
            }
            Dialog.ColorLine(1, "Genindlæs pluksedler", ConsoleColor.Green);

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
            }
            Console.ForegroundColor = standardColor; //reset color
        }
    }
}

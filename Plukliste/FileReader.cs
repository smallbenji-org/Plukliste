using Plukliste.Models;
using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Plukliste

{
    internal static class FileReader
    {
        private static Dictionary<string, int> getHeaderCSV(string headerRow)
        {
            Dictionary<string, int> newDict = new Dictionary<string, int>();
            int indexCount = 0;
            foreach (string column in headerRow.Split(';')) 
            {             
                newDict.Add(column.Replace("\r", ""), indexCount);
                indexCount++;
            }
            return newDict;
        }
        public static Pluklist? ReadPluklist(string filePath)
        {
            Pluklist? plukliste = new Pluklist();
            if (filePath.EndsWith(".csv"))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string[] rows = sr.ReadToEnd().Split('\n');
                    if (rows.Length <= 0) return plukliste;

                    Dictionary<string, int> headerDict = getHeaderCSV(rows[0]);
                    foreach(string row in rows.Skip(1))
                    {
                        Item? item = new Item();
                        string[] columns = row.Split(';');
                        foreach (KeyValuePair<string, int> entry in headerDict)
                        {
                            if (columns.Count() < headerDict.Count()) continue;
                            switch (entry.Key)
                            {
                                case "productid":
                                    item.ProductID = columns[entry.Value];
                                    break;
                                case "type":
                                    string type = columns[entry.Value];
                                    if (type.ToUpper() == "FYSISK") 
                                    {
                                        item.Type = ItemType.Fysisk;
                                    }else if (type.ToUpper() == "PRINT")
                                    {
                                        item.Type = ItemType.Print;
                                    }
                                    break;
                                case "description":
                                    item.ProductID = columns[entry.Value];
                                    break;
                                case "amount":
                                    int.TryParse(columns[entry.Value], out item.Amount);
                                    break;
                            }
                        }                        
                        plukliste.AddItem(item);
                    }
                }
            }
            else            
            {
                FileStream file = File.OpenRead(filePath);
                XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(Pluklist));
                plukliste = (Pluklist?)xmlSerializer.Deserialize(file);
                file.Close();
            }

            return plukliste;
        }
    }
}
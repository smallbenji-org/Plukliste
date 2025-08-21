using Plukliste.Models;
using System;
using System.Xml.Serialization;

namespace Plukliste

{
    internal static class FileReader
    {
        public static Pluklist ReadPluklist(string filePath)
        {
            FileStream file = File.OpenRead(filePath);

            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(Pluklist));

            var plukliste = (Pluklist?)xmlSerializer.Deserialize(file);

            file.Close();

            return plukliste;
        }
    }
}
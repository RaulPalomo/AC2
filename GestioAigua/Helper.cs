using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GestioAigua
{
    public class Helper
    {
        public static void CsvToXml()
        {
            const string ConvertSuccess = "Csv convertido a Xml correctamente";

            var records = new List<Dictionary<string, string>>();

            using (var reader = new StreamReader("Consum_d_aigua_a_Catalunya_per_comarques_20240402.csv"))
            using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var record = new Dictionary<string, string>();

                    foreach (var header in csv.HeaderRecord)
                    {
                        var cleanedHeader = header.Replace(" ", "");
                        record[cleanedHeader] = csv.GetField(header);
                    }

                    records.Add(record);
                }
            }

            var root = new XElement("data");

            foreach (var record in records)
            {
                var recordElement = new XElement("record");

                foreach (var kvp in record)
                {
                    var elementName = kvp.Key;
                    elementName = Regex.Replace(elementName, @"[^\w.-]", "");


                    recordElement.Add(new XElement(elementName, kvp.Value));
                }

                root.Add(recordElement);
            }

            var xmlDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            xmlDocument.Save("GestioAigua.xml");
            Console.WriteLine(ConvertSuccess);
        }
        public static void BuscaPerComarca()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("GestioAigua.xml");
            XmlNodeList nodes = xmlDoc.SelectNodes("//record");
            List<string> list = new List<string>();
            foreach (XmlNode node in nodes)
            {
                XmlNode poblation = node.SelectSingleNode("Població");
                if (poblation != null && Convert.ToInt32(poblation.InnerText) > 200000)
                {

                    foreach (XmlNode singleNode in node)
                    {
                        if (singleNode.Name == "Comarca")
                        {
                            if (!list.Contains(singleNode.InnerText)) list.Add(singleNode.InnerText);
                        }

                    }


                }

            }
            foreach (string comarca in list)
            {
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine(comarca);
                Console.WriteLine("------------------------------------------------");
            }
        }
        public static void CalcConsum()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("GestioAigua.xml");
            XmlNodeList nodes = xmlDoc.SelectNodes("//record");
            foreach (XmlNode node in nodes)
            {
                Console.WriteLine("------------------------------------------------");
                foreach (XmlNode singleNode in node)
                {
                    Console.WriteLine($"{singleNode.Name}: {singleNode.InnerText}");
                }
                XmlNode poblation = node.SelectSingleNode("Població");
                double pValue = Convert.ToDouble(poblation.InnerText);
                XmlNode net = node.SelectSingleNode("Domèsticxarxa");
                double netValue = Convert.ToDouble(net.InnerText);
                XmlNode consum = node.SelectSingleNode("Consumdomèsticpercàpita");
                double cValue = Convert.ToDouble(consum.InnerText);
                Console.WriteLine("Consum doméstic mitjà: "+netValue/pValue);//no se quina formula s'ha de utilitzar
                Console.WriteLine("------------------------------------------------");
            }

        }
        public static void ConsDomesticAlt()
        {
            double top = 0;
            string comarca = "", year = "";
            int last=2022;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("GestioAigua.xml");
            XmlNodeList nodes = xmlDoc.SelectNodes("//record");
            
            foreach (XmlNode node in nodes)
            {
                XmlNode comarcaNode = node.SelectSingleNode("Comarca");
                XmlNode yearNode = node.SelectSingleNode("Any");
                XmlNode consum = node.SelectSingleNode("Consumdomèsticpercàpita");
                double cValue = Convert.ToDouble(consum.InnerText);
                if (cValue>top)
                {
                    top = cValue;
                    comarca = comarcaNode.InnerText;
                    year = yearNode.InnerText;
                }
                if (Convert.ToInt32(yearNode.InnerText) != last)
                {
                    Console.WriteLine(year + " " + comarca);
                    top = 0;
                }
                XmlNode lastYearNode = node.SelectSingleNode("Any");
                last = Convert.ToInt32(lastYearNode.InnerText);
            }
            
        }
        public static void ConsDomesticBaix()
        {
            double bot = 600000;
            string comarca = "", year = "";
            int last = 2022;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("GestioAigua.xml");
            XmlNodeList nodes = xmlDoc.SelectNodes("//record");
            
            foreach (XmlNode node in nodes)
            {
                XmlNode comarcaNode = node.SelectSingleNode("Comarca");
                XmlNode yearNode = node.SelectSingleNode("Any");
                XmlNode consum = node.SelectSingleNode("Consumdomèsticpercàpita");
                double cValue = Convert.ToDouble(consum.InnerText);
                if (cValue < bot)
                {
                    bot = cValue;
                    comarca = comarcaNode.InnerText;
                    year = yearNode.InnerText;
                }
                if (Convert.ToInt32(yearNode.InnerText) != last)
                {
                    Console.WriteLine(year + " " + comarca);
                    bot = 600000;
                }
                XmlNode lastYearNode = node.SelectSingleNode("Any");
                last = Convert.ToInt32(lastYearNode.InnerText);
            }

        }
        public static void Filtrar()
        {
            const string Msg="1-Filtrar per nom\n2-Filtrar per codi";
            int option;
            bool found = false;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("GestioAigua.xml");
            XmlNodeList nodes = xmlDoc.SelectNodes("//record");

            Console.WriteLine(Msg);
            do
            {
                option = Convert.ToInt32(Console.ReadLine());
            } while (option<1||option>2);
            if(option == 1)
            {
                Console.Write("Escriu el nom: ");
                string name=Console.ReadLine();
                foreach (XmlNode node in nodes)
                {
                    XmlNode comarca = node.SelectSingleNode("Comarca");
                    if (comarca != null && comarca.InnerText == name)
                    {

                        foreach (XmlNode singleNode in node)
                        {
                            Console.WriteLine("------------------------------------------------");
                            Console.WriteLine($"{singleNode.Name}: {singleNode.InnerText}");
                            Console.WriteLine("------------------------------------------------");
                        }
                        found= true;

                    }

                }

            }
            else
            {
                Console.Write("Escriu el codi: ");
                string cod = Console.ReadLine();
                foreach (XmlNode node in nodes)
                {
                    XmlNode codiComarca = node.SelectSingleNode("Codicomarca");
                    if (codiComarca != null && codiComarca.InnerText == cod)
                    {
                        Console.WriteLine("------------------------------------------------");
                        foreach (XmlNode singleNode in node)
                        {
                            
                            Console.WriteLine($"{singleNode.Name}: {singleNode.InnerText}");
                            
                        }
                        Console.WriteLine("------------------------------------------------");
                        found = true;

                    }

                }
            }
            
        }
    }
}

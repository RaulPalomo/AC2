using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using GestioAigua;

namespace GestioAiguaa
{
    public class Program
    {
        public static void Main()
        {
            const string ChooseMsg = "Que vols fer??\n1-Identificar les comarques amb una població superior a 200000\n2-Calcular el consum domèstic mitjà per comarca\n3-Mostrar les comarques amb el consum domèstic per càpita més alt\n4-Mostrar les comarques amb el consum domèstic per càpita més baix\n5-Filtrar les comarques per nom o codi\n6-Sortir";
            int option=0;
            bool correct;
            Helper.CsvToXml();
            while (option != 6)
            {
                correct= false;
                Console.WriteLine(ChooseMsg);
                while (!correct)
                {
                    option = Convert.ToInt32(Console.ReadLine());
                    if (option > 0 && option < 7)
                    {
                        correct = true;
                    }
                }
                switch (option)
                {
                    case 1:
                        Console.WriteLine();
                        Helper.BuscaPerComarca();
                        break;
                    case 2:
                        Console.WriteLine();
                        Helper.CalcConsum();
                        break;
                    case 3:
                        Console.WriteLine();
                        Helper.ConsDomesticAlt();
                        break;
                    case 4:
                        Console.WriteLine();
                        Helper.ConsDomesticBaix();
                        break;
                    case 5:
                        Console.WriteLine();
                        Helper.Filtrar();
                        break;
                    case 6:
                        Console.WriteLine();
                        break;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using FelicaLib;
using static FelicaLib.Felica;

namespace FelicaLib
{
    class Program
    {
        public static void Main()
        {
            try
            {
                using (Felica f = new Felica())
                {
                    readCard(f);
                }
                while (true) ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void printserviceinfo(ushort s)
        {
            string ident;

            switch ((s >> 1) & 0xf)
            {
                case 0: ident = "Area Code"; break;
                case 4: ident = "Random Access R/W"; break;
                case 5: ident = "Random Access Read only"; break;
                case 6: ident = "Cyclic Access R/W"; break;
                case 7: ident = "Cyclic Access Read only"; break;
                case 8: ident = "Purse (Direct)"; break;
                case 9: ident = "Purse (Cashback/decrement)"; break;
                case 10: ident = "Purse (Decrement)"; break;
                case 11: ident = "Purse (Read only)"; break;
                default: ident = "INVALID or UNKOWN"; break;
            }

            Console.Write("# Serivce code ={0:x4} : {1}", s, ident);
            if ((s & 0x1) == 0)
            {
                Console.Write(" (Protected)");
            }
            Console.Write("\n");
        }

        private static void readCard(Felica f)
        {
            int i, j, k;
            f.Polling((int)SystemCode.Any);

            Console.Write("# IDm: ");
            Console.Write(BitConverter.ToString(f.IDm()));
            Console.Write("\n");
            Console.Write("# PMm: ");
            Console.Write(BitConverter.ToString(f.PMm()));
            Console.Write("\n\n");

            felicat felicaf2 = new felicat();
            felicat felicaf1 = f.felica_enum_systemcode();

            for (i = 0; i < felicaf1.num_system_code; i++)
            {
                int syscode = ((felicaf1.system_code[i]) >> 8) & 0xff | ((felicaf1.system_code[i]) << 8) & 0xff00;
                Console.Write("# System code: {0:x4}\n", syscode);
                felicaf2 = f.felica_enum_service(syscode);

                Console.Write("# Number of area = {0}\n", felicaf2.num_area_code);
                for (j = 0; j < felicaf2.num_area_code; j++)
                {
                    Console.Write("# Area: {0:x4} - {1:x4}\n", felicaf2.area_code[j], felicaf2.end_service_code[j]);
                }

                Console.Write("# Number of service code = {0}\n", felicaf2.num_service_code);
                for (j = 0; j < felicaf2.num_service_code; j++)
                {
                    ushort service = felicaf2.service_code[j];
                    printserviceinfo(service);

                    for (k = 0; k < 255; k++)
                    {
                        byte[] data = f.ReadWithoutEncryption((int)felicaf2.service_code[j], k);
                        if (data == null) break;

                        Console.Write("{0:x4}:{1:x4} ", (int)felicaf2.service_code[j], k);
                        Console.Write(BitConverter.ToString(data));
                        Console.Write("\n");
                    }
                }
                Console.Write("\n");
            }
        }
    }
}

        
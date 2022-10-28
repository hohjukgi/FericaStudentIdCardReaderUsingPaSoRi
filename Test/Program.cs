using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FelicaLib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
			try
			{
				using (Felica f = new Felica())
				{
					readCard(f);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

        private static void readCard(Felica f)
        {
            int i, j, k;
            f.Polling((int)SystemCode.Any);

            Console.Write("# IDm: ");
            hexdump(f.IDm(), 8);
            Console.Write("\n");
            Console.Write("# PMm: ");
            hexdump(f.PMm(), 8);
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
                        data = f.ReadWithoutEncryption((int)felicaf2.service_code[j], k);
                        if (data == null) break;

                        Console.Write("{0:x4}:{1:x4} ", (int)felicaf2.service_code[j], k);
                        hexdump(data, 16);
                        Console.Write("\n");
                    }
                }
                Console.Write("\n");
            }
        }
    }
}

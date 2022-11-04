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

        private static string readCard(Felica f)
        {
            f.Polling((int)SystemCode.Any);

            felicat felicaf2 = new felicat();
            felicat felicaf1 = f.felica_enum_systemcode();

            int syscode = ((felicaf1.system_code[0]) >> 8) & 0xff | ((felicaf1.system_code[0]) << 8) & 0xff00;
            felicaf2 = f.felica_enum_service(syscode);

            byte[] data = f.ReadWithoutEncryption((int)felicaf2.service_code[1], 0);
            if (data == null) return null;

            List<Byte> vs = new List<Byte>();

            for(int i = 0; i < data.Length; i++)
            {
                vs.Add(data[i]);
            }

            vs.RemoveRange(0, 2);
            vs.RemoveRange(8, 6);

            string studentId = string.Empty;

            for(int i = 0; i < vs.Count; i++)
            {
                studentId += vs[i] & 0x0f;
            }

            Console.Write(studentId);

            return studentId;
            
        }
    }
}

        
using System;
using System.Collections.Generic;
using System.Text;
using FelicaLib;

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
                    f.Polling((int)SystemCode.Common);
                    //Console.WriteLine(f.IDm() BitCoverter);
                    Console.WriteLine(BitConverter.ToString(f.IDm()));
                }
                while (true) ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

        
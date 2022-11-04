using System;
using System.Collections.Generic;
using System.Text;
using FelicaLib;

namespace FelicaLib
{
    public class Nanaco
    {
        public static void Main()
        {
            try
            {
                using (Felica f = new Felica())
                {
                    f.Polling((int)SystemCode.Common);
                    Console.WriteLine(f.IDm());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

        
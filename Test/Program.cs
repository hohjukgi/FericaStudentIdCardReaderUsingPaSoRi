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
					readNanaco(f);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		} 
    }
}

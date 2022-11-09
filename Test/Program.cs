using System;
using System.Threading.Tasks;

namespace FelicaLib
{
    class Program
    {
        public static async Task Main()
        {
            while (true)
            {
                try
                {
                    using (Felica f = new Felica())
                    {
                        Console.WriteLine(FericaFunc.readStudentId(f));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                await Task.Delay(500);
            }
        }
    }
}

        
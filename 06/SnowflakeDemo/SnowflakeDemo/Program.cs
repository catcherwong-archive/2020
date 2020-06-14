using System;
using System.Threading.Tasks;

namespace SnowflakeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            IdGenerator generator = new IdGenerator();

            Parallel.For(0, 20, x =>
            {
                Console.WriteLine(generator.NextId()); 
            });

            Console.WriteLine("Hello World!");
            System.Threading.Thread.Sleep(1000 * 60);
        }
    }
}

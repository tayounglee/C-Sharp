using System;

namespace IntegerLiterals
{
    class MainApp
    {
        static void Main(string[] args)
        {
            byte a = 240;
            Console.WriteLine($"a={a}");

            byte b = 0b1111_0000;
            Console.WriteLine($"b={b}");

            byte c = 0xF0;
            Console.WriteLine($"c={c}");

            uint d = 0x1234_abcd;
            Console.WriteLine($"d={d}");
        }
    }
}

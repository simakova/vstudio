using System;
using System.Globalization;
using System.Xml.Schema;


namespace Num2
{
    public class Program
    {
        private static void Main()
        {
            int  a, b, c;
            string[] numbers = new string[3];
            numbers[0] = Console.ReadLine();
            numbers[1] = Console.ReadLine();
            numbers[2] = Console.ReadLine();

            a = int.Parse(numbers[0]);
            b = int.Parse(numbers[1]);
            c = int.Parse(numbers[2]);

            int count = 9;  
            int[] res = new int[count];
            res[0] = a - b * c;
            res[1] = a - b + c;
            res[2] = a - b - c;
            res[3] = a + b - c;
            res[4] = a + b * c;
            res[5] = a + b + c;
            res[6] = a * b - c;
            res[7] = a * b + c;
            res[8] = a * b * c;

            int min = res[0];
            for (int i = 1; i < count; i++)
            {
                if (res[i] < min)
                    min = res[i];
            }

            Console.WriteLine(min);
        }
    }
}


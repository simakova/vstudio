using System;

   public class Program
{
    private static void Main()
    {
        double sum = 0;
        string n = Console.ReadLine();
        int num = Convert.ToInt32(n);
        for (int i = 1; i <= num; i++)
            sum = sum + i;
        if (sum % 2 == 0) Console.WriteLine("black");
        else Console.WriteLine("grimy");
    }
}
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;


/* 1. Раскладываем заданное число N на множители делением на простые числа (массив prime) - получаем новый массив digits
 2. Циклом перебираем массив digits и максимально сворачиваем перемножением элементов (произведение <=10)
 */

namespace Num3
{
    public class Program
    {
        private static void Main()
        {
            int N, Q, count = 4; //счетчик массива простых чисел до 10
            int[] prime = new int[] {2, 3, 5, 7};
            //List<int> digits = new List<int>(); //динамический массив множителей заданного числа
            int[] counter = new int[10]; //массив для подсчета количества множителей

            String num = Console.ReadLine(); //вводим строку num и преобразуем ее в число N  
            N = int.Parse(num);
            Q = N;

            if (N == 0)
            {
                Console.Write(10);
                return;
            }

            if (N<10)
    	    {
           	    Console.Write(N);
        	    return;
    	    }
            //1 Цикл for перебирает простые числа из массива prime
            //2 Цикл while с предусловием, что число N делится без остатка
            //j - счетчик каждого простого числа, записывается на позицию соответствующую простому числу массива counter
            // результат = массив counter, содержащий количество простых множителей из которых можно собрать число
            for (int i = 0; i < count; i++)
            {
                int j = 0;
                while (N%prime[i] == 0 && N!=0)
                {
                    N = N/prime[i];
                    //digits.Add(prime[i]);
                    j++;
                }
                counter[prime[i]] = j;

                if (N == 1) break;
                
            }
            //3 сворачиваем простые числа для уменьшения разрядности искомого числа: 2*2*2=8, 3*3=9, 3*2=6, 2*2=4
            if (N == Q || N >= 10 || N==0)
            {
                Console.Write("-1");
                return;
            }
            
	        if (counter[2] != 0)
            {
                counter[8] = counter[2]/3;
                counter[2] = counter[2]%3;
            }
            if (counter[3] != 0)
            {
                counter[9] = counter[3]/2;
                counter[3] = counter[3]%2;
            }
            if (counter[2] != 0 && counter[3] != 0)
            {
                counter[6] = 1;
        	    counter[2]--;
        	    counter[3]--;
            }
            if (counter[2] != 0)
            {
                counter[4] = counter[2]/2;
                counter[2] = counter[2]%2;
            }
            //4 выводим все не пустые значения массива
          
            for (int i=0; i<10; i++) 
                if (counter[i]!=0)
		        {
                    for(int j=0;j<counter[i];j++)
            	    {
                	    Console.Write(i);
            	    }
		        }
                
        }
    }
}


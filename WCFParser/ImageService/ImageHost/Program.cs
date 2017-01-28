using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ImageHost
{
    class Program
    {
        static void Main()
        {
            using (var host = new ServiceHost(typeof(ImageService.ImageService)))
            {
                host.Open();

                Console.WriteLine("Host started...");
                Console.ReadLine();
            }
        }
    }
}

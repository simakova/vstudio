﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ImageHost
{
    class Program
    {
        static void Main(string[] args)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiService.Models
{
    public class Word
    {
        public string Wnid { get; set; }
        public string Name { get; set;}
        public string Category { get; set; }
        public string Description { get; set; }
        public string Count { get; set; }
        public string Popularity { get; set; }

    }
}
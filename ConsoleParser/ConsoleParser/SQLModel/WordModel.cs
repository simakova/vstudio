﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ConsoleParserLDB
{
    public class WordModel
    {
        
        public int Id { get; set; }
        public string Wnid { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Count { get; set; }
        public string Popularity { get; set; }
        public virtual List<HyponimModel> HiponimModels { get; set; }
        // public List<Hyponim> Hyponims { get; set; }
    }
}
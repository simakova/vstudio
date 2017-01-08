using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace ConsoleParserLDB
{
    class WordContext : DbContext
    {
        public WordContext()
            : base("DbConnection")
        { }

        public DbSet<WordModel> WordModels { get; set; }

    }
}

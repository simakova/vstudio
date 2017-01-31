using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiteDB;

namespace WebApiService.Models
{
    public class DbController
    {
        public static Word GetWord(string id)
        {
            //using (var db = new LiteDatabase(@"WordData4.db"))
            using (var db = new LiteDatabase(@"C:\Users\Ksenia\WordData5.db"))
            {
                var collection = db.GetCollection<Word>("words");
                var result = collection.FindOne(x => x.Wnid.Equals(id));
                return result;
            }
        }

        public static void SaveWord(Word newWord)
        {
            using (var db = new LiteDatabase(@"WordData.db"))
            {
                var collection = db.GetCollection<Word>("words");
                collection.Insert(newWord);
            }
        }
    }
}
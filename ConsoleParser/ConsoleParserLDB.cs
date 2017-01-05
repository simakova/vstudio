using System;
using System.Collections.Generic;

using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Xml;
using LiteDB;
using System.Xml.Linq;
using System.Data.Entity;

namespace ConsoleParserLDB
{
    class Program {

        static void Main(string[] args) {
           // using (var db = new LiteDatabase(@"MyData.db"))
            //{
                
                while (true) {
                    Console.Clear();
                    Console.WriteLine("1. Enter the WordNet ID to download the mapping between WordNet ID and words" + "\n" +
                                      "2. Enter the word to find the description and other details" + "\n" +
                                      "3. View collection" + "\n");

                    switch (Console.ReadLine()) {
                        case "1":
                            Console.Clear();
                            Console.Write("Enter any WNID (8-digits number): n");
                            string line1 = Console.ReadLine();
                            string wnid = "n" + line1;
                            GetInfoOfWord(GetWordOfID(line1), wnid);
                            Console.ReadKey();
                            break;

                        case "2":
                            Console.Clear();
                            Console.WriteLine("Enter any word: ");
                            string line2 = Console.ReadLine();
                            GetInfoOfWord(line2, GetIDOfWord(line2));
                            Console.ReadKey();
                            break;

                        case "3":
                        
                            Console.Clear();
                            ViewCollection();
                            Console.ReadKey();
                            break;

                        
                        default:
                                Console.WriteLine("Wrong request. Press any key");
                                Console.ReadKey();
                                break;
                 }
                continue;
            }
           // }
        }

        public static string GetWordOfID(string id) {
            using (var db = new LiteDatabase(@"WordData.db"))
            {
                var collection = db.GetCollection<Word>("words");
                //collection.EnsureIndex(x => x.Name);
                var result = collection.FindOne(x => x.Wnid.Equals("n"+id));

                if (result == null)
                {
                    try
                    {
                        WebRequest req = WebRequest.Create("http://www.image-net.org/api/text/wordnet.synset.getwords?wnid=n" + id);
                        req.Credentials = CredentialCache.DefaultCredentials;

                        HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                        Stream data = res.GetResponseStream();

                        StreamReader reader = new StreamReader(data);
                        string line = reader.ReadToEnd();

                        //Console.WriteLine(id + "\n" + line);

                        //var collection = db.GetCollection<Word>("words");

                        //var newWord = new Word { Name = line, Wnid = "n" + id };
                        //var collectionW = db.GetCollection<Word>("words");
                        //collection.Insert(newWord);
                        // collectionW.Update(newWord);
                        //collection.EnsureIndex(x => x.Name);
                        return line;
                    }

                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Error in Web Request" + "/n" + e);

                        //reader.Close();
                        //data.Close();
                    }
                }
                else
                    return GetInfoFromLDB(result, "word");
                   // Console.WriteLine(result.Name + " " + "(from DB)");
            }
            return "Error";
        }

        public static string GetIDOfWord(string word)
        {
            using (var db = new LiteDatabase(@"WordData.db"))
            {
                var collection = db.GetCollection<Word>("words");
                //collection.EnsureIndex(x => x.Name);
                var result = collection.FindOne(x => x.Name.Equals(word));

                if (result == null)
                {
                    try
                    {
                        XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
                        structure.WhitespaceHandling = WhitespaceHandling.None;

                        while (structure.Read())
                        {

                            if (structure.MoveToAttribute("words") && structure.Value.Contains(word))
                            {

                                structure.MoveToAttribute("wnid");
                                string wnid = structure.Value;
                                //Console.WriteLine("WNID" + "\t" + "\t" + "\t" + "| " + wnid);
                                //structure.MoveToAttribute("gloss");
                                //string gloss = structure.Value;
                                //Console.WriteLine("Description: " + "\t" + gloss);

                                //var collection = db.GetCollection<Word>("words");

                                //var newWord = new Word { Name = word, Wnid = wnid };
                                //var collectionW = db.GetCollection<Word>("words");
                                //collection.Insert(newWord);
                                return wnid;
                                // collectionW.Update(newWord);
                                //collection.EnsureIndex(x => x.Name);
                            }
                        }
                    }

                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Error in Web Request" + "/n" + e);
                       
                        //reader.Close();
                        //data.Close();
                    }
                }
                else {
                    return GetInfoFromLDB(result, "id");
                    
                    // Console.WriteLine(result.Name + " " + "(from DB)");
                }
           }
            return "Error";
        }

        public static void GetInfoOfWord(string word, string wnid)
        {
            using (var db = new LiteDatabase((@"WordData.db")))
            {
                var collection = db.GetCollection<Word>("words");
                //collection.EnsureIndex(x => x.Name);
                var result = collection.FindOne(x => x.Wnid.Equals(wnid));

                if (result == null)
                {
                    try
                    {
                        XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
                        structure.WhitespaceHandling = WhitespaceHandling.None;

                        while (structure.Read())
                        {

                            if (structure.MoveToAttribute("wnid") && structure.Value.Contains(wnid))
                            {

                                //structure.MoveToAttribute("wnid");
                                //string wnid = structure.Value;
                                //Console.WriteLine("WNID" + "\t" + "\t" + "\t" + "| " + wnid);
                                ////structure.MoveToAttribute("gloss");
                                ////string gloss = structure.Value;
                                ////Console.WriteLine("Description: " + "\t" + gloss);
                                Console.WriteLine("WNID" + "\t" + "\t" + "\t" + "| " + wnid);
                                //Console.WriteLine("\n" + word);
                                WebClient client = new WebClient();
                                //client.Encoding = Encoding.GetEncoding("utf-8");
                                string details = client.DownloadString("http://image-net.org/__viz/getControlDetails.php?wnid=" + wnid);

                                HtmlDocument doc = new HtmlDocument();
                                doc.LoadHtml(details);

                                HtmlNode catName = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[1]");
                                Console.WriteLine("Word is from category:" + "\t" + "| " + catName.InnerText);

                                HtmlNode description = doc.DocumentNode.SelectSingleNode("//table/tr[2]/td[1]");
                                Console.WriteLine("Description:" + "\t" + "\t" + "| " + description.InnerText);

                                HtmlNode count = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[2]");
                                Console.WriteLine("Count of pictures:" + "\t" + "| " + count.InnerText);

                                HtmlNode percent = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[3]");
                                Console.WriteLine("Popularity Percentile:" + "\t" + "| " + percent.InnerText);

                                //var collectionW = db.GetCollection<Word>("words");

                                var newWord = new Word
                                {
                                    Name = word,
                                    Wnid = wnid,
                                    Category = catName.InnerText,
                                    Description = description.InnerText,
                                    Count = count.InnerText,
                                    Popularity = percent.InnerText
                                };

                                collection.Insert(newWord);
                                //collection.Update(newWord);

                                Console.WriteLine("\n" + "Hyponims: " + "\n");

                                WebRequest req2 = WebRequest.Create("http://image-net.org/api/text/wordnet.structure.hyponym?wnid=" + wnid);
                                req2.Credentials = CredentialCache.DefaultCredentials;

                                HttpWebResponse res2 = (HttpWebResponse)req2.GetResponse();
                                Stream dataID = res2.GetResponseStream();

                                StreamReader reader2 = new StreamReader(dataID);

                                List<string> idStorage = new List<string>();

                                string lineID;


                                //заполнение массива

                                while ((lineID = reader2.ReadLine()) != null)
                                {
                                    string ids = lineID.Substring(0);
                                    idStorage.Add(ids);
                                }

                                int i = 1;
                                while (i < idStorage.Count)
                                {
                                    var idd = idStorage[i].Substring(2);
                                    Console.WriteLine("-" + GetWordOfID(idd));
                                    i++;
                                }

                                Console.WriteLine("More (press press any key)" + "\n" + "Menu (press esc)" + "\n");

                                if (Console.ReadKey().Key == ConsoleKey.Escape)
                                    break;

                                else
                                    continue;
                            }

                        }
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Error in XML Reader");
                    }
                }
                else
                    GetInfoFromLDB(result, "info");
                    //Console.WriteLine(result.Category+" " + result.Description + " " + result.Count+" " + result.Popularity+" " + "(from DB)");
            }
        }

        public static string GetInfoFromLDB(Word r, string key)
        {
            using (var db = new LiteDatabase((@"WordData.db")))
            {
                if (key == "word") {
                    Console.WriteLine("(from LDB)" + "\n" + r.Name + "\n");
                    return r.Name;
                }
                else if (key == "id") {
                    Console.WriteLine("(from LDB)" + "\n" + "WNID" + "\t" + "\t" + "\t" + "| " + r.Wnid + "\n");
                    return r.Wnid;
                }
                else  if (key=="info"){
                    Console.WriteLine("(from LDB)" + "\n" + "Word is from category:" + "\t" + "| " + r.Category + "\n" +
                    "Description:" + "\t" + "\t" + "| " + r.Description + "\n" +
                    "Count of pictures:" + "\t" + "| " + r.Count + "\n" +
                    "Popularity Percentile:" + "\t" + "| " + r.Popularity);
                }
            }
            return "Error";
        }

        public static void ViewCollection() {
            using (var db = new LiteDatabase((@"WordData.db"))) {
                var collection = db.GetCollection<Word>("words");
                var result = collection.FindAll();
                foreach (Word w in result)
                {
                    //Console.WriteLine(w.Name);
                    //foreach (Detail d in w.Details) {
                    Console.WriteLine("WNID" + "\t" + "\t" + "\t" + "| " + w.Wnid);
                    Console.WriteLine("Word is from category:" + "\t" + "| " + w.Category);
                    Console.WriteLine("Description:" + "\t" + "\t" + "| " + w.Description);
                    Console.WriteLine("Count of pictures:" + "\t" + "| " + w.Count);
                    Console.WriteLine("Popularity Percentile:" + "\t" + "| " + w.Popularity);
                    //}
                }
           }
        }
       
        //public static void ViewCollection()
        //{
        //    using (var db = new LiteDatabase(@"MyData.db"))
        //    {
        //        // Получаем коллекцию
        //        var col = db.GetCollection<Company>("companies");

        //        var microsoft = new Company { Name = "Microsoft" };
        //        microsoft.Users = new List<User> { new User { Name = "Bill Gates" } };

        //        // Добавляем компанию в коллекцию
        //        col.Insert(microsoft);

        //        // Обновляем документ в коллекции
        //        microsoft.Name = "Microsoft Inc.";
        //        col.Update(microsoft);


        //        var google = new Company { Name = "Google" };
        //        google.Users = new List<User> { new User { Name = "Larry Page" } };
        //        col.Insert(google);

        //        // Получаем все документы
        //        var result = col.FindAll();
        //        foreach (Company c in result)
        //        {
        //            Console.WriteLine(c.Name);
        //            foreach (User u in c.Users)
        //                Console.WriteLine(u.Name);
        //            Console.WriteLine();
        //        }

        //        // Индексируем документ по определенному свойству
        //        col.EnsureIndex(x => x.Name);

        //        col.Delete(x => x.Name.Equals("Google"));

        //        Console.WriteLine("После удаления Google");
        //        result = col.FindAll();
        //        foreach (Company c in result)
        //        {
        //            Console.WriteLine(c.Name);
        //            foreach (User u in c.Users)
        //                Console.WriteLine(u.Name);
        //            Console.WriteLine();
        //        }
        //    }
        //    Console.ReadKey();
            
        //}
    }
}


        //  //!!!!!!!!!!!!!!!!Вариант с txt!!!!!!!!!!!!!!!!!


//  /*//words.txt

//  WebRequest reqWords = WebRequest.Create("http://image-net.org/archive/words.txt");

//  reqWords.Credentials = CredentialCache.DefaultCredentials;

//  HttpWebResponse resWords = (HttpWebResponse)reqWords.GetResponse();

//  Stream dataWords = resWords.GetResponseStream();

//  StreamReader readerW = new StreamReader(dataWords);

//  // glossary.txt

//  WebRequest reqGloss = WebRequest.Create("http://image-net.org/archive/gloss.txt");

//  reqGloss.Credentials = CredentialCache.DefaultCredentials;

//  HttpWebResponse resGloss = (HttpWebResponse)reqGloss.GetResponse();

//  Stream dataGloss = resGloss.GetResponseStream();

//  StreamReader readerG = new StreamReader(dataGloss);

//  List<string> idStorage = new List<string>();

//  List<string> nameStorage = new List<string>();

//  List<string> glossStorage = new List<string>();

//  string lineWords;

//  string lineGloss;

//  //заполнение массивов

//  while ((lineWords = readerW.ReadLine()) != null && (lineGloss = readerG.ReadLine()) != null)

//  {
//      string ids = lineWords.Substring(1, 8);
//      string names = lineWords.Substring(10);
//      string glossary = lineGloss.Substring(10);
//      idStorage.Add(ids);
//      nameStorage.Add(names);
//      glossStorage.Add(glossary);
//  }

//  readerW.Close();
//  readerG.Close();
//  dataWords.Close();
//  dataGloss.Close();
//  resWords.Close();
//  resGloss.Close();

//  string name = Console.ReadLine();
//  int count = 0;
//  while (count < idStorage.Count)
//      if (nameStorage[count].Contains(name))
//          break;
//      else
//          count++;
//  string id = idStorage[count];

//  Console.WriteLine(glossStorage[count]);
//  Console.ReadKey();
//  /*int position = 0;

//  foreach (string word in nameStorage)
//  {
//      if (name == word) break;
//      position++;
//  }

//  string id = idStorage[position];*/


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

using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace ConsoleParserLDB
{
    class Program {

        static void Main(string[] args) {
            // using (var db = new LiteDatabase(@"MyData.db"))
            //{

            while (true) {
                Console.Clear();
                Console.WriteLine("1. Enter the WordNet ID to find the description and other details" + "\n" +
                                  "2. Enter the word to find the description and other details" + "\n" +
                                  "3. View objects of LDB" + "\n" +
                                  "4. View objects of SQL" + "\n");

                switch (Console.ReadLine()) {
                    case "1": //Поиск информации о группе изображений по id
                        Console.Clear();
                        Console.Write("Enter any WNID (8-digits number): n");
                        string line1 = Console.ReadLine();
                        string wnid = "n" + line1;
                        GetInfoOfWord(GetWordOfID(line1), wnid);
                        Console.ReadKey();
                        break;

                    case "2": //Поиск информации о группе изображений по слову
                        Console.Clear();
                        Console.WriteLine("Enter any word: ");
                        string line2 = Console.ReadLine();
                        GetInfoOfWord(line2, GetIDOfWord(line2));
                        Console.ReadKey();
                        break;

                    case "3": //Посмотреть имеющиеся в LDB данные
                        Console.Clear();
                        ViewCollection();
                        Console.ReadKey();
                        break;

                    case "4": //Посмотреть имеющиеся в SQL данные
                        Console.Clear();
                        ViewTable();
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

        public static string GetWordOfID(string id)
        { //Извлекаем слово по ID
            using (var db = new LiteDatabase(@"WordData.db"))
            {
                var collection = db.GetCollection<Word>("words");
                //collection.EnsureIndex(x => x.Name);
                var result = collection.FindOne(x => x.Wnid.Equals("n" + id));

                if (result == null)
                {//Если слова нет в коллекции
                    try
                    {
                        WebRequest req = WebRequest.Create("http://www.image-net.org/api/text/wordnet.synset.getwords?wnid=n" + id);
                        req.Credentials = CredentialCache.DefaultCredentials;

                        HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                        Stream data = res.GetResponseStream();

                        StreamReader reader = new StreamReader(data);
                        string line = reader.ReadToEnd();

                        return line;
                    }

                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Error in Web Request" + "/n" + e);
                        Console.WriteLine();
                        //reader.Close();
                        //data.Close();
                    }
                }
                else
                    return GetInfoFromLDB(result, "word");
            }
            return "Error";
        }

        public static string GetIDOfWord(string word) //Извлекаем ID по слову
        {
            using (var db = new LiteDatabase(@"WordData.db"))
            {
                var collection = db.GetCollection<Word>("words");
                //collection.EnsureIndex(x => x.Name);
                var result = collection.FindOne(x => x.Name.Equals(word));

                if (result == null)
                { //Если слова нет в коллекции
                    try
                    {
                        XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
                        structure.WhitespaceHandling = WhitespaceHandling.None;

                        while (structure.Read())
                        { //пока XML читается, то находим введенное слово и соответствующий к нему id
                            if (structure.MoveToAttribute("words") && structure.Value.Contains(word))
                            {

                                structure.MoveToAttribute("wnid");
                                string wnid = structure.Value;

                                return wnid;
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
                else
                {
                    return GetInfoFromLDB(result, "id");
                }
            }
            return "Error";
        }

        public static void GetInfoOfWord(string word, string wnid)
        { // извлечение дополнительной информации по каждому слову 
            using (var db = new LiteDatabase((@"WordData.db")))
            {
                var collection = db.GetCollection<Word>("words");
                //var collection2 = db.GetCollection<Hyponim>("hyponims");
                //collection.EnsureIndex(x => x.Name);
                var result = collection.FindOne(x => x.Wnid.Equals(wnid));

                if (result == null)
                { //если искомого слова нет в коллекции
                    try
                    {
                        XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
                        structure.WhitespaceHandling = WhitespaceHandling.None;

                        while (structure.Read())
                        {

                            if (structure.MoveToAttribute("wnid") && structure.Value.Contains(wnid))
                            {
                                Console.WriteLine("WNID" + "\t" + "\t" + "\t" + "| " + wnid);
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

                                
                                //var newWord = new Word
                                //{
                                //    Name = word,
                                //    Wnid = wnid,
                                //    Category = catName.InnerText,
                                //    Description = description.InnerText,
                                //    Count = count.InnerText,
                                //    Popularity = percent.InnerText
                                //};

                                //collection.Insert(newWord);
                                //collection.Update(newWord);

                                Console.WriteLine("\n" + "Hyponims: " + "\n");

                                WebRequest req2 = WebRequest.Create("http://image-net.org/api/text/wordnet.structure.hyponym?wnid=" + wnid);
                                req2.Credentials = CredentialCache.DefaultCredentials;

                                HttpWebResponse res2 = (HttpWebResponse)req2.GetResponse();
                                Stream dataID = res2.GetResponseStream();

                                StreamReader reader2 = new StreamReader(dataID);

                                List<string> idStorage = new List<string>();

                                string lineID;

                                //заполнение массива id Hyponims

                                while ((lineID = reader2.ReadLine()) != null)
                                {
                                    string ids = lineID.Substring(0);
                                    idStorage.Add(ids);
                                }

                                int i = 1;
                                //newWord.Hyponims = new List<Hyponim> { };

                                // поиск соответствующего id слова
                                List<string> wordStorage = new List<string>();
                                while (i < idStorage.Count)
                                {
                                    var idd = idStorage[i].Substring(2);
                                    string findWord = GetWordOfID(idd);
                                    wordStorage.Add(findWord);
                                    Console.WriteLine("- " + findWord);
                                    //var newHyp = new Hyponim() { Wnid = idd, Name = findWord };
                                    //newWord.Hyponims.Add(newHyp);

                                    //collection.EnsureIndex(x => x.Name);
                                    i++;
                                }
                                Console.WriteLine("Save to LDB");
                                SaveToLDB(word, wnid, catName.InnerText, description.InnerText, count.InnerText, percent.InnerText, idStorage, wordStorage);
                                // SaveToSQL(word, wnid, catName.InnerText, description.InnerText, count.InnerText, percent.InnerText, wordStorage);
                                //collection.Insert(newWord);
                                Console.WriteLine("Save to SQL");
                                SaveToSQL(word, wnid, catName.InnerText, description.InnerText, count.InnerText, percent.InnerText, idStorage, wordStorage);
                                Console.WriteLine("Press any key to return");
                                if (Console.ReadKey().Key == ConsoleKey.Escape)
                                    break;
                                //Console.WriteLine("More (press press any key)" + "\n" + "Menu (press esc)" + "\n");

                                //if (Console.ReadKey().Key == ConsoleKey.Escape)
                                //    break;

                                else
                                    break;
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
            }
        }

        public static string GetInfoFromLDB(Word r, string key)
        { //извлечение информации о слове из кэша
            using (var db = new LiteDatabase((@"WordData.db")))
            {
                if (key == "word")
                {
                    Console.WriteLine("(from LDB)" + "\n" + r.Name + "\n");
                    return r.Name;
                }
                else if (key == "id")
                {
                    Console.WriteLine("(from LDB)" + "\n" + "WNID" + "\t" + "\t" + "\t" + "| " + r.Wnid + "\n");
                    return r.Wnid;
                }
                else if (key == "info")
                {
                    Console.WriteLine("(from LDB)" + "\n" + "Word is from category:" + "\t" + "| " + r.Category + "\n" +
                    "Description:" + "\t" + "\t" + "| " + r.Description + "\n" +
                    "Count of pictures:" + "\t" + "| " + r.Count + "\n" +
                    "Popularity Percentile:" + "\t" + "| " + r.Popularity + "\n" +
                    "Hyponims:" + "\t" + "| ");
                    foreach (Hyponim h in r.Hyponims)
                        Console.WriteLine("- " + h.Wnid + " " + h.Name);
                    Console.WriteLine("Press press any key to return" + "\n");
                }
            }
            return "Error";
        }

        public static void ViewCollection()
        { // просмотр коллекции
            using (var db = new LiteDatabase((@"WordData.db")))
            {
                var collection = db.GetCollection<Word>("words");
                var result = collection.FindAll();
                collection.EnsureIndex(x => x.Wnid);
                foreach (Word w in result)
                {
                    Console.WriteLine("WNID" + "\t" + "\t" + "\t" + "| " + w.Wnid);
                    Console.WriteLine("Word is from category:" + "\t" + "| " + w.Category);
                    Console.WriteLine("Description:" + "\t" + "\t" + "| " + w.Description);
                    Console.WriteLine("Count of pictures:" + "\t" + "| " + w.Count);
                    Console.WriteLine("Popularity Percentile:" + "\t" + "| " + w.Popularity + "\n");
                    Console.WriteLine("Hyponims:" + "\t" + "| " + "\n");
                    foreach (Hyponim h in w.Hyponims)
                        Console.WriteLine("- " + h.Wnid + " " + h.Name);
                }
                Console.WriteLine("Press any key");
            }
        }

        public static void SaveToLDB(string word, string wnid, string cat, string gloss, string count, string percent, List<string> idStorage, List<string> wordStorage)
        {
            using (var db = new LiteDatabase((@"WordData.db")))
            {
                var collection = db.GetCollection<Word>("words");
                var newWord = new Word
                {
                    Name = word,
                    Wnid = wnid,
                    Category = cat,
                    Description = gloss,
                    Count = count,
                    Popularity = percent
                };
                newWord.Hyponims = new List<Hyponim> { };
                int i = 1;

                while (i < wordStorage.Count)
                {
                    var idd = idStorage[i].Substring(2);
                    var hyp = wordStorage[i].Substring(0);
                    var newHyp = new Hyponim() { Wnid = idd, Name = hyp };
                    newWord.Hyponims.Add(newHyp);

                    //collection.EnsureIndex(x => x.Name);
                    i++;
                }
                collection.Insert(newWord);
                Console.WriteLine("Save to Ldb is success" );
            }
        }

        public static void SaveToSQL(string word, string wnid, string cat, string gloss, string count, string percent, List<string> idStorage, List<string> wordStorage)
        {
            using (WordContext db = new WordContext())
            {
                WordModel newWd = new WordModel
                {
                    Name = word,
                    Wnid = wnid,
                    Category = cat,
                    Description = gloss,
                    Count = count,
                    Popularity = percent
                };

                //newWord.Hyponims = new List<Hyponim> { };
                //int i = 1;

                //while (i < wordStorage.Count)
                //{
                //    var idd = idStorage[i].Substring(2);
                //    var hyp = wordStorage[i].Substring(0);
                //    var newHyp = new Hyponim() { Wnid = idd, Name = hyp };
                //    newWord.Hyponims.Add(newHyp);

                //    //collection.EnsureIndex(x => x.Name);
                //    i++;
                //}

                db.WordModels.Add(newWd);
                db.SaveChanges();
                Console.WriteLine("Save to SQL is success");
            }
        }

        public static void ViewTable()
        {
            using (WordContext db = new WordContext())
            {
                
                // получаем объекты из бд и выводим на консоль
                var words = db.WordModels;
                Console.WriteLine("Список объектов:");
                foreach (WordModel w in words)
                {
                    Console.WriteLine("{0}.{1} - {3} ({4},{5},{6}) ", w.Id, w.Wnid, w.Name, w.Category, w.Description, w.Count, w.Popularity);
                }
            }
            Console.Read();
        }
    

    

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


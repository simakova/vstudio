using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
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
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Xml.Linq;
using LiteDB;


namespace ConsApp_HtmlParser
{
    
    //public class Word
    //{
    //    [Key]
    //    public string WNID { get; set; }
    //    public string NAME { get; set; }
        
    //    public virtual WordDetail WordDetails { get; set; }
    //}

    //public class WordDetail
    //{
    //    [Key]
    //    public string GLOSS { get; set; }
    //    public int COUNT { get; set; }
    //    public int PERCENT { get; set; }
    //    public string[] HYPONIM { get; set; }

    //    public virtual ICollection<Word> Words { get; set; }
    //}

    //public class WordsContext : DbContext
    //{
    //    public DbSet<Word> Words { get; set; }
    //    public DbSet<WordDetail> WordDetails { get; set; }
    //}
    
    //public class CachedWord
    //{
    //    public string WNID { get; set; }
    //    public string NAME { get; set; }
    //    public string WordDetail { get; set; }

    //    public void InfoOutput()
    //    {
    //        Console.WriteLine();
    //        Console.WriteLine("{0, 20}: {1}", "Name", this.NAME);
    //        Console.WriteLine("{0, 20}: {1}", "Details", this.WordDetail);
    //    }
    //}

    class Program
    {

        static void Main(string[] args)
        {

            while (true)
            {

                Console.Clear();
                Console.WriteLine("1. Enter the WordNet ID to download the mapping between WordNet ID and words" + "\n" +
                                    "2. Enter the word to find the description and other details" + "\n" +
                                    "Enter the number do you prefer" + "\n");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        Console.Write("Enter any WNID (8-digits number): n");
                        GetWordOfID(Console.ReadLine());
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("Enter any word: ");
                        GetInfoOfWord(Console.ReadLine());
                        Console.ReadKey();
                        break;

                    
                    default:
                        Console.WriteLine("Wrong request. Press any key");
                        Console.ReadKey();
                        break;
                }
                continue;
            }
        }


        //public static void GetWordsList()
        //{
        //    using (var db = new WordsContext())
        //    {
        //        var WordsList = db.WordDetails;
        //        if (WordsList != null)
        //        {
        //            Console.WriteLine("\nList of words:");
        //            int i = 0;
        //            Dictionary<int, string> WordsDictionary = new Dictionary<int, string>();
        //            foreach (var WordDetail in WordsList)
        //        }
        //    }
        //}





        public static void GetWordOfID(string id)
        {

            WebRequest req = WebRequest.Create("http://www.image-net.org/api/text/wordnet.synset.getwords?wnid=n" + id);

            req.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream data = res.GetResponseStream();

            StreamReader reader = new StreamReader(data);

            string line = reader.ReadToEnd();

            Console.WriteLine(id + "\n" + line);

            //reader.Close();

            //data.Close();
        }


        public static void GetInfoOfWord(string word)
        {
            XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
            structure.WhitespaceHandling = WhitespaceHandling.None;

            while (structure.Read())
            {
                if (structure.MoveToAttribute("words") && structure.Value.Contains(word))
                {
                    structure.MoveToAttribute("wnid");
                    string wnid = structure.Value;
                    Console.WriteLine("WNID" + "\t" + "\t" + "\t" + "| " + wnid);
                    //structure.MoveToAttribute("gloss");
                    //string gloss = structure.Value;
                    //Console.WriteLine("Description: " + "\t" + gloss);

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
                        GetWordOfID(idd);
                        i++;

                    }
                    Console.WriteLine("More (press press any key)" + "\n" + "Menu (press esc)" + "\n");

                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    else
                        continue;

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




        //  /////////////////////Задача 1. Вывод названия объекта по wnid(уникальный идентификатор 
        //  ///////////////////////////из словаря WordNet) в формате n********(8 значное число)

        //  Console.Write("Enter any 8-digits number(wnid): ");

        //  string id = Console.ReadLine();

        //  WebRequest req = WebRequest.Create("http://www.image-net.org/api/text/wordnet.synset.getwords?wnid=n" + id);

        //  req.Credentials = CredentialCache.DefaultCredentials;

        //  HttpWebResponse res = (HttpWebResponse)req.GetResponse();

        //  Stream data = res.GetResponseStream();

        //  StreamReader reader = new StreamReader(data);

        //  string line = reader.ReadToEnd();

        //  Console.WriteLine(line);

        // // reader.Close();

        ////  data.Close();

        //  ////////////Вариант 2 с xml файлом
        //  /* XmlTextReader reader = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
        //   reader.WhitespaceHandling = WhitespaceHandling.None;

        //   while (reader.Read())
        //   {
        //       if (reader.MoveToAttribute("wnid")&&reader.Value.Contains(id))
        //       {
        //           reader.MoveToAttribute("words");
        //           Console.WriteLine("Name" + "\t" + "\t" + reader.Value);
        //           reader.MoveToAttribute("gloss");
        //           Console.WriteLine("Description: " + "\t" + reader.Value);
        //           break;
        //       }
        //   }
        //   */
        //  /////////////////////////////////////////////////


        //  /////////////////////Задача 2. Вывод информации по введенному слову

        //  Console.Write("\n"+"Enter any word: ");
        //  string word = Console.ReadLine();

        //  XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
        //  structure.WhitespaceHandling = WhitespaceHandling.None;

        //  while (structure.Read())
        //  {
        //      if (structure.MoveToAttribute("words") && structure.Value.Contains(word))
        //      {
        //          //Console.WriteLine(reader2.Value);
        //          structure.MoveToAttribute("wnid");
        //          string wnid = structure.Value;
        //          Console.WriteLine("WNID= " + "\t" + "\t" + "\t" + wnid);
        //          structure.MoveToAttribute("gloss");
        //          string gloss = structure.Value;
        //          //Console.WriteLine("Description: " + "\t" + gloss);

        //          WebClient client = new WebClient();
        //          //client.Encoding = Encoding.GetEncoding("utf-8");
        //          string details = client.DownloadString("http://image-net.org/__viz/getControlDetails.php?wnid=" + wnid);  

        //          HtmlDocument doc = new HtmlDocument();
        //          doc.LoadHtml(details);

        //          HtmlNode catName = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[1]");
        //          Console.WriteLine("Word is from category:"  + "\t" + catName.InnerText);

        //          HtmlNode description = doc.DocumentNode.SelectSingleNode("//table/tr[2]/td[1]");
        //          Console.WriteLine("Description:" + "\t" + "\t" + description.InnerText);

        //          HtmlNode count = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[2]");
        //          Console.WriteLine("Count of pictures:" + "\t" + count.InnerText);

        //          HtmlNode percent = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[3]");
        //          Console.WriteLine("Popularity Percentile:" + "\t" + percent.InnerText);

        //          Console.WriteLine("\n" + "Hyponims: " + "\n");

        //          WebRequest req2 = WebRequest.Create("http://image-net.org/api/text/wordnet.structure.hyponym?wnid=" + wnid);

        //          req2.Credentials = CredentialCache.DefaultCredentials;

        //          HttpWebResponse res2 = (HttpWebResponse)req2.GetResponse();

        //          Stream dataID = res2.GetResponseStream();

        //          StreamReader reader2 = new StreamReader(dataID);

        //          List<string> idStorage = new List<string>();

        //          string lineID;


        //          //заполнение массива

        //          while ((lineID = reader2.ReadLine()) != null)

        //          {
        //              string ids = lineID.Substring(0);
        //              idStorage.Add(ids);
        //          }

        //          // reader2.Close();

        //          // dataID.Close();


        //          int i = 1;
        //          while (i < idStorage.Count)
        //          {
        //              id = idStorage[i].Substring(1);

        //              WebRequest req3 = WebRequest.Create("http://www.image-net.org/api/text/wordnet.synset.getwords?wnid=" + id);

        //          req3.Credentials = CredentialCache.DefaultCredentials;

        //          HttpWebResponse res3 = (HttpWebResponse)req3.GetResponse();

        //          Stream data3 = res3.GetResponseStream();

        //          StreamReader reader3 = new StreamReader(data3);

        //          string line3 = reader3.ReadLine();

        //              Console.WriteLine(id + "\t" + line3);
        //              i++;
        //          }


        //          Console.ReadKey();
        //      }

        //  }
        //  Console.ReadKey();


    }
}



      
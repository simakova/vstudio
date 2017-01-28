using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Configuration;
using HtmlAgilityPack;
using System.Xml;
using System.Net;
using System.IO;
using LiteDB;

namespace ImageService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "ImageService" в коде и файле конфигурации.
    public class ImageService : IImageService
    {
        public Word TestWord()
        {
            WordObj = new Word();
            WordObj.Wnid = "123456";
            WordObj.Name = "123456789";
           
            return WordObj;

        }

        public Word FindWord(string line, string key)
        {
            WordObj = new Word
            {
                Wnid = "",
                Name = "",
            };
            if (key == "fromId")
            {
                WordObj.Wnid = line;
                WordObj.Name = GetWordOfID(line);

            }
            else if (key == "fromWord")
            {
                WordObj.Name = line;
                WordObj.Wnid = GetIDOfWord(line);
                //return WordObj;
            }
            Tuple<string, string, string, string> info = GetInfoOfWord(WordObj.Name, WordObj.Wnid);
            WordObj.Category = info.Item1;
            WordObj.Description = info.Item2;
            WordObj.Count = info.Item3;
            WordObj.Popularity = info.Item4;

            return WordObj;

        }

        public Word WordObj { get; set; }

        public string GetWordOfID(string id)
        {
            using (var db = new LiteDatabase(@"WordData.db"))
            {
                var collection = db.GetCollection<Word>("words");
                //collection.EnsureIndex(x => x.Name);
                var result = collection.FindOne(x => x.Wnid.Equals(id));

                if (result == null)
                {
                    try
                    {
                        WebRequest req = WebRequest.Create("http://www.image-net.org/api/text/wordnet.synset.getwords?wnid=" + id);
                        req.Credentials = CredentialCache.DefaultCredentials;
                        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                        Stream data = res.GetResponseStream();
                        StreamReader reader = new StreamReader(data);
                        string line = reader.ReadToEnd();
                        // word_box.Text = line;
                        return line;
                    }
                    catch (System.Net.WebException ex)
                    {
                        return "Error";
                    }
                }
                else
                    return GetIDorWordFromLDB(result, "word");
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
                {
                    try
                    {
                        XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
                        structure.WhitespaceHandling = WhitespaceHandling.None;
                        while (structure.Read())
                        { //пока XML читается, то находим введенное слово и соответствующий ему id
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
                        return "Error";
                        //reader.Close();
                        //data.Close();
                    }
                    return "Error";
                }
                else
                    return GetIDorWordFromLDB(result, "id");
            }
            return "Error";
        }
        public static Tuple<string, string, string, string> GetInfoOfWord(string word, string wnid)
        { // извлечение дополнительной информации по каждому слову 
            using (var db = new LiteDatabase(@"WordData.db"))
            {
                var collection = db.GetCollection<Word>("words");
                //collection.EnsureIndex(x => x.Name);
                var result = collection.FindOne(x => x.Wnid.Equals(wnid));

                if (result == null)
                {
                    XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
                    structure.WhitespaceHandling = WhitespaceHandling.None;
                    while (structure.Read())
                    {
                        if (structure.MoveToAttribute("wnid") && structure.Value.Contains(wnid))
                        {
                            //Console.WriteLine("WNID" + "\t" + "\t" + "\t" + "| " + wnid);
                            WebClient client = new WebClient();
                            //client.Encoding = Encoding.GetEncoding("utf-8");
                            string details = client.DownloadString("http://image-net.org/__viz/getControlDetails.php?wnid=" + wnid);
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(details);
                            HtmlNode catName = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[1]");
                            HtmlNode description = doc.DocumentNode.SelectSingleNode("//table/tr[2]/td[1]");
                            HtmlNode count = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[2]");
                            HtmlNode percent = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[3]");
                            SaveToLDB(word, wnid, catName.InnerText, description.InnerText, count.InnerText, percent.InnerText);
                            return Tuple.Create(catName.InnerText, description.InnerText, count.InnerText, percent.InnerText);
                        }
                    }

                }
                else
                    return GetInfoFromLDB(result);
            }
            return Tuple.Create("Error", "Error", "Error", "Error");
        }


        public static string GetIDorWordFromLDB(Word r, string key)
        { //извлечение информации о слове из кэша
            using (var db = new LiteDatabase((@"WordData.db")))
            {
                if (key == "word")
                {
                    return r.Name;
                }
                else if (key == "id")
                {
                    return r.Wnid;
                }
            }
            return "Error";
        }

        public static Tuple<string, string, string, string> GetInfoFromLDB(Word r)
        {
            using (var db = new LiteDatabase((@"WordData.db")))
            {
                return Tuple.Create(r.Category, r.Description, r.Count, r.Popularity);
            }
        }
        public static void SaveToLDB(string word, string wnid, string cat, string gloss, string count, string percent)
        {
            using (var db = new LiteDatabase((@"WordData.db")))
            {
                try
                {
                    var collection = db.GetCollection<Word>("words");
                    var newWord = new Word
                    {
                        Wnid = wnid,
                        Name = word,
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
                    collection.Insert(newWord);
                    //MessageBox.Show("Save to LDB is success");
                }
                catch (ArgumentException e)
                {
                    return;
                }
            }
        }
    }
}



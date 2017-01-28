using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
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
        public void SetWord()
        {
            

            //сохранение полей экземпляра класса Word в XMl док-те
            //var file = ConfigurationManager.AppSettings["fileWord"];

            //var doc = XDocument.Load(file);

            //doc.Root.Add(new XElement("Word", /*new XAttribute("Id", w.Id),*/ new XElement("Wnid", w.Wnid),
            //    new XElement("Name", w.Name), new XElement("Category", w.Category), new XElement("Description", w.Description),
            //    new XElement("Count", w.Count), new XElement("Popularity", w.Popularity)));

            //doc.Save(file);


        }

        public Word GetWord(string line, string key)
        {

            //    //считывание из XML файла
            //    var file = ConfigurationManager.AppSettings["fileWord"];
            //    var result = new Word();

            //    var doc = XDocument.Load(file);

            //    var element = doc.Descendants("Word").FirstOrDefault(x => x.Element("Wnid").Value == wnid.ToString());

            //    //result.Id = int.Parse(element.Attribute("Id").Value);
            //    result.Wnid = element.Element("Wnid").Value;
            //    result.Name = element.Element("Name").Value;
            //    result.Category = element.Element("Category").Value;
            //    result.Description= element.Element("Description").Value;
            //    result.Count = element.Element("Count").Value;
            //    result.Popularity = element.Element("Popularity").Value;

            //    return result;
            //}

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
                return WordObj;
            }
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

        //public static Tuple<string, string, string, string> GetInfoFromLDB(Word r)
        //{
        //    using (var db = new LiteDatabase((@"WordData.db")))
        //    {
        //        return Tuple.Create(r.Category, r.Description, r.Count, r.Popularity);
        //    }
        //}
        public static void SaveToLDB(string word, string wnid/*, string cat, string gloss, string count, string percent*/)
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
                        //Category = cat,
                        //Description = gloss,
                        //Count = count,
                        //Popularity = percent
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

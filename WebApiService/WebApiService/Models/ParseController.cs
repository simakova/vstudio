using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using LiteDB;

using System.Xml;


namespace WebApiService.Models
{
    public class ParseController
    {
        public static Word GetWordFromId(string id)
        {
           
                try
                {
                    WebRequest req = WebRequest.Create("http://www.image-net.org/api/text/wordnet.synset.getwords?wnid=" + id);
                    req.Credentials = CredentialCache.DefaultCredentials;
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    Stream data = res.GetResponseStream();
                    StreamReader reader = new StreamReader(data);
                    string line = reader.ReadToEnd();
                    return GetInfo(line, id);
                    //newWord = new Word()
                    //{
                    //    Wnid = id,
                    //    Name = line
                    //};
                    //return newWord;
                }
                catch (Exception)
                {
                    return null;
                }
                
           
        }

        public static Word GetIDFromWord(string word) //Извлекаем ID по слову
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
                        //var result = new Word()
                        //{
                        //    Wnid = wnid,
                        //    Name = word
                        //};
                        return GetInfo(word, wnid);
                    }
                }
            }
            catch (ArgumentException e)
            {
                return null;
            }
            return null;
        }
           
        

        public static Word GetInfo(string line, string id)
        {
            var newWord = DbController.GetWord(id);
            if (newWord == null)
            {
                try
                {


                    XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
                    structure.WhitespaceHandling = WhitespaceHandling.None;
                    while (structure.Read())
                    {
                        if (structure.MoveToAttribute("wnid") && structure.Value.Contains(id))
                        {
                            //Console.WriteLine("WNID" + "\t" + "\t" + "\t" + "| " + wnid);
                            WebClient client = new WebClient();
                            //client.Encoding = Encoding.GetEncoding("utf-8");
                            string details = client.DownloadString("http://image-net.org/__viz/getControlDetails.php?wnid=" + id);
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(details);
                            HtmlNode catName = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[1]");
                            HtmlNode description = doc.DocumentNode.SelectSingleNode("//table/tr[2]/td[1]");
                            HtmlNode count = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[2]");
                            HtmlNode percent = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[3]");
                            //SaveToLDB(word, wnid, catName.InnerText, description.InnerText, count.InnerText, percent.InnerText);

                            newWord = new Word()
                            {
                                Wnid = id,
                                Name = line,
                                Category = catName.InnerText,
                                Description = description.InnerText,
                                Count = count.InnerText,
                                Popularity = percent.InnerText
                            };
                            DbController.SaveWord(newWord);
                            return newWord;
                        }

                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return newWord;
        }
    }
}

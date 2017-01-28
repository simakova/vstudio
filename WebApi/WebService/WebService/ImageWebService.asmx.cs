using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LiteDB;
using HtmlAgilityPack;
using System.Xml;
using System.Net;
using System.IO;

namespace WebService
{
    /// <summary>
    /// Сводное описание для ImageWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку. 
    // [System.Web.Script.Services.ScriptService]
    public class ImageWebService : System.Web.Services.WebService
    {

        [WebMethod]
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

        public static string GetIDOfWord(string word) //Извлекаем ID по слову
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
                return "Error";
            }
            catch (ArgumentException e)
            {
                return "Error";
                //reader.Close();
                //data.Close();
            }
        }
        public static Tuple<string, string, string, string> GetInfoOfWord(string word, string wnid)
        { // извлечение дополнительной информации по каждому слову 
            try
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
                       
                        return Tuple.Create(catName.InnerText, description.InnerText, count.InnerText, percent.InnerText);
                    }
                }
                return Tuple.Create("Error", "Error", "Error", "Error");
            }
            catch (ArgumentException e)
            {
                return Tuple.Create("Error", "Error", "Error", "Error");
            }
        }


      

    }
}

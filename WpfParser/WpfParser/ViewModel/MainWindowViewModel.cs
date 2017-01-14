using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfParser.Model;
using HtmlAgilityPack;
using System.Xml;
using System.Net;
using System.IO;
using System.Xml.Linq;
using WpfParser.Model;

namespace WpfParser.ViewModel
{
    class MainViewModel
    {
        public MainViewModel()
        {
            ClickCommand = new Command(arg => ClickMethod());
            ClearCommand = new Command(arg => ClearMethod());
            Word = new WordModel
            {
                Wnid = "",
                Name = "",
                Category = "",
                Description = "",
                Count = "",
                Popularity = "",
            };
        }
        public WordModel Word { get; set; }
        public ICommand ClickCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        private void ClickMethod()
        {
            if(Word.Wnid.Length!=0)
                Word.Name=GetWordOfID(Word.Wnid);
            else if(Word.Name.Length != 0)
                Word.Wnid = GetIDOfWord(Word.Name);
            Tuple<string, string, string, string> info=GetInfoOfWord(Word.Name, Word.Wnid);
            Word.Category = info.Item1;
            Word.Description = info.Item2;
            Word.Count = info.Item3;
            Word.Popularity = info.Item4;
        }

        private void ClearMethod()
        {
            Word.Wnid = "";
            Word.Name = "";
            Word.Category = "";
            Word.Description = "";
            Word.Count = "";
            Word.Popularity = "";
        }

        public string GetWordOfID(string id)
        {
            WebRequest req = WebRequest.Create("http://www.image-net.org/api/text/wordnet.synset.getwords?wnid=" + id);
            try
            {
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
                return "Error";
                //reader.Close();
                //data.Close();
            }
            return "Error";
        }
        public static Tuple<string, string, string, string> GetInfoOfWord(string word, string wnid)
        { // извлечение дополнительной информации по каждому слову 
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
                    HtmlNode description = doc.DocumentNode.SelectSingleNode("//table/tr[2]/td[1]");
                    HtmlNode count = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[2]");
                    HtmlNode percent = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[3]");
                    return Tuple.Create(catName.InnerText, description.InnerText, count.InnerText, percent.InnerText);
                }
            }
            return Tuple.Create("Error", "Error", "Error", "Error");
        }

    }
}


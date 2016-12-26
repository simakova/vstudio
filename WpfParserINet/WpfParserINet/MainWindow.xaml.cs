using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using HtmlAgilityPack;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;







namespace WpfParserINet
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            

        }

       

        //class MyTable
        //{
        //    public MyTable(int Wnid, string Word, string Count, int Population)
        //    {
        //        this.Wnid = Wnid;
        //        this.Word = Word;
        //        this.Count = Count;
        //        this.Population = Population;
        //    }
        //    public int Wnid { get; set; }
        //    public string Word { get; set; }
        //    public string Count { get; set; }
        //    public int Population { get; set; }
        //}
        //private void create_Window()
        //{
        //    Window win = new Window();
        //    Button button = new System.Windows.Forms.Button();
        //    win.Content = button;
        //}


        private void ok_but_Click(object sender, RoutedEventArgs e)
        {

            if (wnid_box.Text != String.Empty)
            {
                GetWordOfID(wnid_box.Text);
                GetInfo(wnid_box.Text);
            }

            else if (word_box.Text != String.Empty)
            {
                //cat_but.Visibility = Visibility.Visible;
                cat_block.Visibility = Visibility.Visible;
                GetIDOfWord(word_box.Text);
                GetInfo(wnid_box.Text);
            }
            else System.Windows.Forms.MessageBox.Show("No parameters for search");


            //string text = textbox1.text;
            //if (text != "")
            //{
            //    messagebox.show(text);
            //}
        }
     

        private void clear_but_Click(object sender, RoutedEventArgs e)
        {

            clearFields();


        }
        
        //private void cat_but_Click(object sender, RoutedEventArgs e)
        //{
        //    clearFields();

        //    искать следующее совпадение(?)
        //   
        //}


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        

        public void GetWordOfID(string id)
        {
            WebRequest req = WebRequest.Create("http://www.image-net.org/api/text/wordnet.synset.getwords?wnid=" + id);

            try {
                req.Credentials = CredentialCache.DefaultCredentials;

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                Stream data = res.GetResponseStream();

                StreamReader reader = new StreamReader(data);

                string line = reader.ReadToEnd();

                word_box.Text = line;

                reader.Close();

                data.Close();
                
            }

            catch (System.Net.WebException ex) {
                System.Windows.Forms.MessageBox.Show("Error");
            }
        }

       

        public void GetInfo(string id)
        {
            try
            {
                WebClient client = new WebClient();
                        //client.Encoding = Encoding.GetEncoding("utf-8");
                        string details = client.DownloadString("http://image-net.org/__viz/getControlDetails.php?wnid=" + id);

                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(details);

                        HtmlNode catName = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[1]");
                        cat_block.Text = catName.InnerText;

                        HtmlNode description = doc.DocumentNode.SelectSingleNode("//table/tr[2]/td[1]");
                        gloss_block.Text = description.InnerText;

                        HtmlNode count = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[2]");
                        count_block.Text = count.InnerText;

                        HtmlNode percent = doc.DocumentNode.SelectSingleNode("//table/tr[1]/td[3]");
                        pop_block.Text = percent.InnerText;

                        //clickEvent.Reset();
                        //clickEvent.WaitOne();
            }

            catch (System.Net.WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Error");

            }

        }

       

        public void GetIDOfWord(string word)
        {
            XmlTextReader structure = new XmlTextReader(@"http://www.image-net.org/api/xml/structure_released.xml");
            structure.WhitespaceHandling = WhitespaceHandling.None;

            
            try {
               
                while (structure.Read()) {
                    
                    if (structure.MoveToAttribute("words") && structure.Value.Contains(word)) {
                        
                        structure.MoveToAttribute("wnid");
                        string wnid = structure.Value;
                        wnid_box.Text = wnid;
                        
                    }
                }

            }
            catch (System.Net.WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Error");

            }
        }

        //private void info_but_Click(object sender, RoutedEventArgs e)
        //{

        //}
        public void clearFields()
        {
            wnid_box.Text = String.Empty;
            cat_block.Text = String.Empty;
            gloss_block.Text = String.Empty;
            count_block.Text = String.Empty;
            pop_block.Text = String.Empty;
        }


    }
}



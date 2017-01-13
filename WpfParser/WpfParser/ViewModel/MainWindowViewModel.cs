using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfParser.Model;

namespace WpfParser.ViewModel
{
    class MainViewModel
    {
        
        public MainViewModel()
        {
            ClickCommand = new Command(arg => ClickMethod());
            Word = new WordModel
            {
                Wnid = "Wnid",
                Name = "Name",
                Category = "Category",
                Description = "Description",
                Count = "Count",
                Popularity = "Popularity",
            };
        }

            public WordModel Word {get; set;}

            public ICommand ClickCommand { get; set; }

        private void ClickMethod()
        {
            MessageBox.Show("This is click command.");
        }
    }
}


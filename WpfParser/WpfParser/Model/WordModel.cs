using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WpfParser.Model
{
    class WordModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _Wnid;
        private string _Name;
        private string _Category;
        private string _Description;
        private string _Count;
        private string _Popularity;
        //[Key]
        // public int Id { get; set; }

        public string Wnid {
            get { return _Wnid; }
            set
            {
                if (_Wnid != value)
                {
                    _Wnid = value;
                    OnPropertyChanged("Wnid");
                }
            }
        }
        public string Name {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Category
        {
            get { return _Category; }
            set
            {
                if (_Category != value)
                {
                    _Category = value;
                    OnPropertyChanged("Category");
                }
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        public string Count
        {
            get { return _Count; }
            set
            {
                if (_Count != value)
                {
                    _Count = value;
                    OnPropertyChanged("Count");
                }
            }
        }

        public string Popularity
        {
            get { return _Popularity; }
            set
            {
                if (_Popularity != value)
                {
                    _Popularity = value;
                    OnPropertyChanged("Popularity");
                }
            }
        }

        // public List<HyponimModel> Hyponims { get; set; }
    }
}

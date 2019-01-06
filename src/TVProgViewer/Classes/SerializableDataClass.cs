using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TVProgViewer.TVProgApp
{
    [Serializable()]
    public class SerializableDataClass
    {
        private List<Tuple<string, Image>> _listFavsImages = new List<Tuple<string, Image>>();
        private List<Tuple<string, Image>> _listGenreImages = new List<Tuple<string, Image>>();
        public List<Tuple<string, Image>> FavsImageList
        {
            get { return _listFavsImages; }
            set { _listFavsImages = value; }
        }
        public List<Tuple<string, Image>> GenreImageList
        {
            get { return _listGenreImages; }
            set { _listGenreImages = value; }
        }
    }
}

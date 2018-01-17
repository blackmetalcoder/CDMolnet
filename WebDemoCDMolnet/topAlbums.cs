using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemoCDMolnet
{
    public class topAlbums
    {
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Coverart { get; set; }
        public string AlbunID { get; set; }

        public topAlbums() { }

        public topAlbums(string artist, string album, string coverart, string albunid)
        {
            Artist = artist;
            Album = album;
            Coverart = coverart;
            AlbunID = albunid;
        }

    }
}
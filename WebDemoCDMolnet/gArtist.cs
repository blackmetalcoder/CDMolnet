using ParkSquare.Gracenote;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebDemoCDMolnet
{
        
    public class Skiva
    {
        

        public Skiva(String id, String album, String ar, string coverart, string genre)
        {
            this.Album = album;
            this.Ar = ar;
            this.Id = id;
            this.Coverart = coverart;
            this.Genre = genre;
            this.Lat = new List<Latar>();
        }

       
        public string Id { get; set; }
        public string Album { get; set; }
        public string Ar { get; set; }
        public string Coverart { get; set; }
        public string Genre { get; set; }
        public List<Latar> Lat { get; private set; }

    }
    public class Latar
    {
        public Latar(String nr, String title, String id)
        {
            this.Id = id;
            this.Nr = nr;
            this.Title = title;
        }
        public string Nr { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
    }

    public sealed class SkivaDataSource
    {
        private const SearchOptions SearchOptions =
        ParkSquare.Gracenote.SearchOptions.Mood | ParkSquare.Gracenote.SearchOptions.Tempo | ParkSquare.Gracenote.SearchOptions.Cover |
        ParkSquare.Gracenote.SearchOptions.ArtistOriginEraType |
        ParkSquare.Gracenote.SearchOptions.ArtistImage | ParkSquare.Gracenote.SearchOptions.ArtistBiography;
        SearchResult result;
        private static SkivaDataSource _SkivaDataSource = new SkivaDataSource();
        private List<Skiva> _groups = new List<Skiva>();
        public static List<Skiva> GetInfo(string grupp)
        {
            return _SkivaDataSource.GetSkivData(grupp);
        }
        public static List<Skiva> GetAlbum(string artist, string album )
        {
            return _SkivaDataSource.DenSkivan(artist, album);
        }
        public List<Skiva> Groups
        {
            get { return this._groups; }
        }
        private List<Skiva> GetSkivData(string grupp)
        {

            Groups.Clear();
            var client = new GracenoteClient("687559541-407A6818314DF26BB17DC1E4AB57BD4E");
            var clientCover = new GracenoteClient("687559541-407A6818314DF26BB17DC1E4AB57BD4E");
            int iLoopar = 0;
            result = client.Search(new SearchCriteria
            {
                Artist = grupp,
                //SearchOptions = SearchOptions,
                Range = new Range(1, 500)
            });
            int iAntal = result.Count;
            if (iAntal > 100)
            {
                iLoopar = 8;
                //iLoopar = iAntal / 20;
            }
            else
            {
                iLoopar = iAntal / 20;
            }
            
            for (int i = 0; i <= iLoopar; i++)
            {
                var Album = client.Search(new SearchCriteria
                {
                    Artist = grupp,
                    SearchOptions = SearchOptions,
                    Range = new Range(i, 20)
                });
                foreach (var skiva in Album.Albums)
                {
                    string s = "";
                    try
                    {
                        s = skiva.Artwork.First().Uri.AbsoluteUri;
                    }
                    catch (Exception ex)
                    {
                        s = "";
                    }
                    Skiva NySkiva = new Skiva(skiva.Id, skiva.Title, skiva.Year.ToString(), s, skiva.Genre.First());
                    this.Groups.Add(NySkiva);   
                }

            }
            return Groups;
        }
        private List<Skiva> DenSkivan(string artist, string album)
        {
            List<Skiva> S = new List<Skiva>();
            var clientCover = new GracenoteClient("687559541-407A6818314DF26BB17DC1E4AB57BD4E");
            var Cover = BestMatchSearchWithOptions(clientCover, artist, album);

            foreach (var skiva in Cover.Albums)
            {
                string s = "";
                try
                {
                    //s = skiva.Artwork.First().Uri.AbsoluteUri;
                    s = Cover.Albums.First().Artwork.First().Uri.AbsoluteUri;
                }
                catch (Exception ex)
                {
                    s = "";
                }
                Skiva NySkiva = new Skiva(skiva.Id, skiva.Title, skiva.Year.ToString(), s, skiva.Genre.First());
                foreach (var track in skiva.Tracks)
                {
                    NySkiva.Lat.Add(new Latar(track.Number.ToString(), track.Title, track.Id));
                }
                S.Add(NySkiva);
            }
            return S;
        }

        private static SearchResult BestMatchSearchWithOptions(GracenoteClient client, string artist, string album)
        {
            return client.Search(new SearchCriteria
            {
                AlbumTitle = album,
                Artist = artist,
                SearchMode = SearchMode.BestMatch,
                SearchOptions = SearchOptions
            });
        }
        private static void DownloadArtwork(SearchResult result, string path)
        {
            result.Albums.First().Artwork.First().Download(path);
        }
    }
}


   
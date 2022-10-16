using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArtistSongSearch
{
    public class ArtistSongItem
    {
        public ArtistSongItem(string artist, string song)
        {
            Artist = artist;
            Song = song;
        }

        public string Artist { get; set; }

        public string Song { get; set; }

        public override string ToString()
        {
            return $"{Artist} - {Song}";
        }
    }


    class ArtistSongSearcher
    {
        private readonly List<ArtistSongItem> _items = new List<ArtistSongItem>();
        public int NumberSongsRead => _items.Count;

        public ArtistSongSearcher(string csvFile, char seperator = ';')
        {
            Console.WriteLine($"CSV File -> {csvFile}");

            if (string.IsNullOrEmpty(csvFile))
                throw new ArgumentException("No CSV File passed to searcher");

            if (!File.Exists(csvFile))
                throw new ArgumentException($"Configured CSV File not existing @ {csvFile} .terminating...");

            var currentArtist = string.Empty;

            foreach (var line in File.ReadAllLines(csvFile,Encoding.Default))
            {
                var parts = line.Split(seperator);
                if (parts.Length != 3)
                    continue;

                if (!string.IsNullOrEmpty(parts[1]))
                    if (!string.Equals(parts[1], currentArtist, StringComparison.OrdinalIgnoreCase))
                        currentArtist = parts[1];

                if (!string.IsNullOrEmpty(parts[2]))
                    _items.Add(new ArtistSongItem(currentArtist,parts[2]));
            }

        }

        public IEnumerable<ArtistSongItem> Search(string artist, string song)
        {
            List<ArtistSongItem> matches = new List<ArtistSongItem>();

            matches.AddRange(!string.IsNullOrEmpty(artist)
                ? _items.Where(item => item.Artist.Contains(artist))
                : _items);

            return !string.IsNullOrEmpty(song) 
                ? matches.Where(item => item.Song.Contains(song)) 
                : matches;


  
        }
    }
}

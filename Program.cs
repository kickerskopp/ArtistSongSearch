using System;
using System.Configuration;
using System.IO;
using System.Linq;
using ArtistSongSearch;
using ConsoleTables;
using Microsoft.Extensions.Configuration;

namespace AppSongSearch
{
    class Program
    {

        static void Main(string[] args)
        {
            IConfiguration config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            try
            {
                var csvFile = GetConfigItem<string>(config, "CSVFile", required: true);
                csvFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, csvFile));

                var seperator = GetConfigItem(config, "Seperator", ';');

                // create Searcher and execute
                var searcher = new ArtistSongSearcher(csvFile, seperator);

                Console.WriteLine($"{searcher.NumberSongsRead} Songs read");

                string artist = string.Empty;
                string song = string.Empty;
                do
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("\nPlease enter Artist :");
                    artist = Console.ReadLine();
                    Console.Write("Please enter Song :");
                    song = Console.ReadLine();

                    Search(artist,song,searcher);

                } while (!(string.IsNullOrEmpty(artist) && string.IsNullOrEmpty(song)));

                Console.WriteLine("Bye !!!");


            }
            catch (Exception e)
            {
                if (e is AggregateException aggregateException)
                    e = aggregateException.InnerExceptions.First();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.Gray;
                Environment.Exit(-2);
            }
        }


        #region DoWork


        private static int Search(string artist,string song, ArtistSongSearcher searcher)
        {

            var matchedSongs = searcher.Search(artist, song).ToList();

            // show results
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nSongs (as string)\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var matchedsong in matchedSongs)
            {
                Console.WriteLine(matchedsong);    
            }

            Console.WriteLine("\n-----");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nSongs (as properties) \n");


            Console.ForegroundColor = ConsoleColor.Yellow;
            ConsoleTable.From(matchedSongs.Select(matchedsong =>
                    new
                    {
                        matchedsong.Artist,
                        matchedsong.Song,
                    })).Configure(o => o.NumberAlignment = Alignment.Right)
                .Write(Format.Alternative);

            Console.WriteLine("\n-----");

            return 1;

        }

        #endregion

        private static T GetConfigItem<T>(IConfiguration config, string item, T defaultValue = default, bool required = false)
        {
            var value = config[item];
            if (required && value == null)
                throw new ConfigurationErrorsException($"Missing required appsetting '{item}'");

            if (value != null)
                return (T)Convert.ChangeType(value, typeof(T));

            return defaultValue;
        }
    }
}

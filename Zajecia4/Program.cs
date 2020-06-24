using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Zajecia4
{
    class Program
    {
        public class Team
        {
            public int teamId { get; set; }
            public string teamName { get; set; }

            public string teamMascot { get; set; }
            public int games { get; set; }

        }


        public class TeamContext : DbContext
        {
            public DbSet<Team> Teams { get; set; }
        }

        public static List<data> getData(string api)
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(api));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }
            List<data> items = JsonConvert.DeserializeObject<List<data>>(jsonString);
            return items;
        }
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();

            var google = new Website("https://google.pl");
            var ath = new Website("https://ath.bielsko.pl");
            var fb = new Website("https://facebook.com");
            var wiki = new Website("https://en.wikipedia.org");
            var anyapi = new Website("https://anyapi.com");
            var plany = new Website("https://plany.ath.bielsko.pl");

            var tasks = new List<Task<IRestResponse>>();

            stopwatch.Start();

            tasks.Add(google.DownloadAsync("/"));
            Console.WriteLine(stopwatch.Elapsed);
            tasks.Add(ath.DownloadAsync("/"));
            Console.WriteLine(stopwatch.Elapsed);
            tasks.Add(fb.DownloadAsync("/"));
            Console.WriteLine(stopwatch.Elapsed);
            tasks.Add(wiki.DownloadAsync("/wiki/.NET_Core"));
            Console.WriteLine(stopwatch.Elapsed);
            tasks.Add(anyapi.DownloadAsync("/wiki/.NET_Core"));
            Console.WriteLine(stopwatch.Elapsed);
            tasks.Add(plany.DownloadAsync("/plan.php?type=0&id=12647"));
            Console.WriteLine(stopwatch.Elapsed);
            tasks.Add(ath.DownloadAsync("/graficzne-formy-przekazu-informacji/"));
            Console.WriteLine(stopwatch.Elapsed);

            Console.WriteLine("----------------------------");
            Console.WriteLine(stopwatch.Elapsed);
            var htmlCodes = Task.WhenAll(tasks).Result;
            foreach (var site in htmlCodes)
            {
                Console.WriteLine(site.Content);
            }
            Console.WriteLine(stopwatch.Elapsed);
            

            Console.WriteLine(google.DownloadAsync("/").Result);
            Console.WriteLine(stopwatch.Elapsed);
            stopwatch.Stop();

            //football api v2

            List<data> test = getData("https://api.collegefootballdata.com/teams/");
            List<data> test2 = getData("https://api.collegefootballdata.com/records?year=2019");
            int xa = 0;
            for (int ia = 0; ia < test.Count; ia++)
            {
                if (test[ia].school == test2[xa].team)
                {

                    using (var db2 = new TeamContext())
                    {
                        Team x = new Team
                        {
                            teamId = test[ia].id,
                            teamName = test[ia].school,
                            teamMascot = test[ia].mascot,
                            games = test2[xa].total.games
                        };
                        db2.Teams.Add(x);
                        db2.SaveChanges();
                    }
                }

            }

            /*//FOOTBALL API
            var client = new RestClient("https://api.collegefootballdata.com/teams/"); //pobranie strony z druzynami
            var request = new RestRequest("/");
            var respose = client.Get(request);
            //Console.WriteLine(respose.Content);

            string[] data;
            data = respose.Content.Split('}').ToArray();
            data = data.Take(data.Length - 1).ToArray();

            var client2 = new RestClient("https://api.collegefootballdata.com/records?year=2019"); //pobranie strony z wynikami
            var request2 = new RestRequest("/");
            var respose2 = client2.Get(request2);


            string[] data2;
            data2 = respose2.Content.Split("year").ToArray();


            int i = 1;
            using (var db = new TeamContext())
            {
                foreach (string s in data)
                {

                    int found = s.IndexOf("id");
                    int found2 = s.IndexOf("school");
                    int lenght = found2 - found;

                    int afound = s.IndexOf(@"school");
                    int afound2 = s.IndexOf(@"mascot");
                    int alenght = afound2 - afound;

                    int bfound = s.IndexOf(@"mascot");
                    int bfound2 = s.IndexOf(@"abbrev");
                    int blenght = bfound2 - bfound;

                    int gamesIn2019 = 0;

                    int a = data2[i].IndexOf("team");
                    int b = data2[i].IndexOf("conf");
                    int c = b - a;
                    string teamName = data2[i].Substring(a + 7, c - 10);
                    if (s.Substring(afound + 9, alenght - 12) == teamName)
                    {
                        a = data2[i].IndexOf("games");
                        b = data2[i].IndexOf("wins");
                        c = b - a;
                        gamesIn2019 = int.Parse(data2[i].Substring(a + 7, c - 9));
                        if (i<data2.Length - 1) i++;
                    }

                    Team x = new Team
                    {
                        teamId = int.Parse(s.Substring(found + 4, lenght - 6)),
                        teamName = s.Substring(afound + 9, alenght - 12),
                        teamMascot = s.Substring(bfound + 9, blenght - 12),
                        games = gamesIn2019
                    }; 
                    db.Teams.Add(x);
                    db.SaveChanges();


                }
            }*/
        }
    }  


}

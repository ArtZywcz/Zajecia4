using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Nancy.Json;
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

        static void Main(string[] args)
        {
            

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
            }
        }
    }  


}

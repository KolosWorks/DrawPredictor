using System;

namespace KolosWorks.DrawPredicter.Model
{
    public class Match
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public byte HomeTeamDrawRate { get; set; }
        public byte AwayTeamDrawRate { get; set; }


        public string Location { get; set; }
        public string MatchDate { get; set; }
        public byte DaysToGo { get; set; }

        public Match(string location, string matchDate, byte daysToGo)
        {
            Location = location;
            MatchDate = matchDate;
            DaysToGo = daysToGo;
        }

    }
}

namespace KolosWorks.DrawPredicter.Model
{
    class Team
    {
        public int LeagueId { get; set; }
        public string Name { get; set; }

        public Team (int leagueId, string name)
        {
            LeagueId = leagueId;
            Name = name;
        }
    }
}

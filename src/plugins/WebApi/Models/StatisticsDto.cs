namespace WebApi.Models
{
    public class StatisticsDto
    {
        public long Uptime { get; set; }
        public int PlayersOnline { get; set; }

        public StatisticsDto()
        {
        }

        public StatisticsDto(long uptime, int playersOnline)
        {
            Uptime = uptime;
            PlayersOnline = playersOnline;
        }

    }
}

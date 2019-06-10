using BlubLib.Serialization;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class MixedTeamBriefingDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public TeamId TeamId { get; set; }

        public MixedTeamBriefingDto()
        {
        }

        public MixedTeamBriefingDto(ulong accountId, TeamId teamId)
        {
            AccountId = accountId;
            TeamId = teamId;
        }
    }
}

using BlubLib.Serialization;

namespace Netsphere.Network.Data.Auth
{
    [BlubContract]
    public class AeriaTokenDto
    {
        [BlubMember(0)]
        public string Token { get; set; }

        [BlubMember(1)]
        public string AccessToken { get; set; }

        [BlubMember(2)]
        public string RefreshToken { get; set; }

        [BlubMember(3)]
        public string ExpireDate { get; set; }

        public AeriaTokenDto()
        {
            Token = "";
            AccessToken = "";
            RefreshToken = "";
            ExpireDate = "";
        }

        public AeriaTokenDto(string token)
            : this()
        {
            Token = token;
        }

        public AeriaTokenDto(string accessToken, string refreshToken, string expireDate)
            : this()
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpireDate = expireDate;
        }
    }
}

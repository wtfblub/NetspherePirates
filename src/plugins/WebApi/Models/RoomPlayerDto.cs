using Netsphere;

namespace WebApi.Models
{
    public class RoomPlayerDto : PlayerDto
    {
        public TeamId TeamId { get; set; }
    }
}

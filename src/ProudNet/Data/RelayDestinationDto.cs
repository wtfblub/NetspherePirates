using BlubLib.Serialization;

namespace ProudNet.Data
{
    internal class RelayDestinationDto
    {
        [Serialize(0)]
        public uint HostId { get; set; }

        [Serialize(1)]
        public uint FrameNumber { get; set; }

        public RelayDestinationDto()
        { }

        public RelayDestinationDto(uint hostId, uint frameNumber)
        {
            HostId = hostId;
            FrameNumber = frameNumber;
        }
    }
}

using BlubLib.Serialization;

namespace ProudNet.Data
{
    [BlubContract]
    internal class RelayDestinationDto
    {
        [BlubMember(0)]
        public uint HostId { get; set; }

        [BlubMember(1)]
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

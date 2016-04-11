namespace Auth.ServiceModel
{
    public class RemoveServerDto
    {
        public ushort Id { get; set; }

        public RemoveServerDto()
        { }

        public RemoveServerDto(ushort id)
        {
            Id = id;
        }
    }
}

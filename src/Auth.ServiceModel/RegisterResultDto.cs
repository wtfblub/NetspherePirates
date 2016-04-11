namespace Auth.ServiceModel
{
    public class RegisterResultDto
    {
        public RegisterResult Result { get; set; }

        public RegisterResultDto()
        { }

        public RegisterResultDto(RegisterResult result)
        {
            Result = result;
        }
    }

    public enum RegisterResult
    {
        OK,
        AlreadyExists
    }
}

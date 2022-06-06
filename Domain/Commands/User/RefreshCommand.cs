namespace Domain.Commands.User
{
    public class RefreshCommand
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; }
    }
}

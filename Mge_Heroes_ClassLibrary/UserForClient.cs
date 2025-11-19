namespace Mge_Heroes_ClassLibrary
{
    public class UserForClient
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string EmailHash { get; set; }
        public string Password { get; set; }
        public UserForClient(string username, string emailHash, string password)
        {
            Username = username;
            EmailHash = emailHash;
            Password = password;
        }
    }
}

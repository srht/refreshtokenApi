namespace refreshtokenApi.ViewModels
{
    public class LoginReponseViewModel:ResponseViewModel
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Role { get; set; }
        public TokenResponseViewModel TokenResponse { get; set; }
    }

    public class TokenResponseViewModel
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireDate { get; set; }
    }
}

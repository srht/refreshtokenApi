using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace refreshtokenApi.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public TokenModel? TokenData { get; set; }
    }

    public class TokenModel
    {
        [Key]
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireDate { get; set; }
    }
}
/*
 *     id: number;
    username: string;
    company: string;
    password: string;
    firstName: string;
    lastName: string;
    token?: string;
    role: Role;
*/
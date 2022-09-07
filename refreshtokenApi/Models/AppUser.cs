using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace refreshtokenApi.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public TokenModel? TokenData { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
    }

    public class TokenModel
    {
        [Key]
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
    }

    public class RefreshToken
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? ReasonRevoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpireDate;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;
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
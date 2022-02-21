using Blog.Data.DbModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Data.Validation
{
    public class UserManager
    {
        /// <summary>
        /// Creates User object for database.
        /// This algorithm uses PBKDF2 with HMAC-SHA-256 and random 16-byte 'salt' to protect user password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns>User instance with filled Username, PasswordHash, Role and Salt (in base64 form)</returns>
        public User CreateUser(string username, string password, Role role = Role.User)
        {
            // generate random salt
            var salt = new byte[128 / 8];

            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(salt);

            // generate the salted and hashed password
            var saltedAndHashedPassword = SaltAndHashPassword(password, salt);

            return new User()
            {
                Username = username,
                PasswordHash = saltedAndHashedPassword,
                Role = role,
                Salt = Convert.ToBase64String(salt)
            };
        }

        public bool CheckCredentials(User user, string password)
        {
            var saltedAndHashedPassword = SaltAndHashPassword(password, Convert.FromBase64String(user.Salt));
            return saltedAndHashedPassword == user.PasswordHash;
        }

        public void CreateAuthenticationTicket(User user, ISession session)
        {
            var key = Encoding.ASCII.GetBytes(SiteKeys.Token);
            var JWToken = new JwtSecurityToken(
                issuer: SiteKeys.WebSiteDomain,
                audience: SiteKeys.WebSiteDomain,
                claims: GetUserClaims(user),
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature));

            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            session.SetString("JWToken", token);
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            var claims = new List<Claim>();
            var claim = new Claim(ClaimTypes.Name, user.Username);
            claims.Add(claim);

            claim = new Claim(ClaimTypes.Role, user.Role.ToString());
            claims.Add(claim);

            // if user has a specific role
            // add default 'User'
            if (user.Role != Role.User)
            {
                claims.Add(new Claim(ClaimTypes.Role, nameof(Role.User)));
            }

            return claims;
        }

        private static string SaltAndHashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000, // in 2022 better use >310000 iterations
                numBytesRequested: 256 / 8));
        }
    }
}

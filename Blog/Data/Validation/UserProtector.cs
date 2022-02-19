using Blog.Data.DbModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Data.Validation
{
    public static class UserProtector
    {
        /// <summary>
        /// Creates User object for database.
        /// This algorithm uses PBKDF2 with HMAC-SHA-256 and random 16-byte 'salt' to protect user password. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns>User instance with filled Username, PasswordHash, Role and Salt (in base64 form)</returns>
        public static User CreateUser(string username, string password, Role role = Role.User)
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

        public static bool CheckCredentials(User user, string password)
        {
            var saltedAndHashedPassword = SaltAndHashPassword(password, Convert.FromBase64String(user.Salt));
            return saltedAndHashedPassword == user.PasswordHash;
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

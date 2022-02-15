using Blog.Data.DbModels;
using Blog.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Data.Validation
{
    public static class UserProtector
    {
        /// <summary>
        /// Creates User object for database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns>User with filled Username, PasswordHash, Role and Salt</returns>
        public static User CreateUser(string username, string password, Role role)
        {
            // generate random salt
            var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            var saltText = Convert.ToBase64String(saltBytes);

            // generate the salted and hashed password
            var saltedAndHashedPassword = SaltAndHashPassword(password, saltText);

            return new User()
            {
                Username = username,
                PasswordHash = saltedAndHashedPassword,
                Role = role,
                Salt = saltText
            };
        }

        public static bool CheckCredentials(User user, string password)
        {
            var saltedAndHashedPassword = SaltAndHashPassword(password, user.Salt);
            return saltedAndHashedPassword == user.PasswordHash;
        }

        private static string SaltAndHashPassword(string password, string salt)
        {
            var sha = SHA256.Create();
            var saltedPassword = password + salt;
            return Convert.ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(saltedPassword)));
        }
    }
}

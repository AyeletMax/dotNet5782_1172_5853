

using DalApi;
using System.Text;

namespace Helpers;

internal static class VolunteerManager
{
    private string EncryptPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var encryptedPassword = EncryptPassword(enteredPassword);
        return encryptedPassword == storedPassword;
    }

    private string GenerateStrongPassword()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*";
        return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

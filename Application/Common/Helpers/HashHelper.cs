using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Helpers;

public static class HashHelper
{
    public static string ToHashedBase64String(this string plainText)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.ASCII.GetBytes(plainText)));
    }
}
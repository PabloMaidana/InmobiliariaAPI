using System.Security.Cryptography;

namespace Inmobiliaria.Api.Services;
public class PasswordService {
    public (string hashBase64, byte[] salt) HashPassword(string password, byte[]? salt = null) {
        salt ??= RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return (Convert.ToBase64String(hash), salt);
    }
    public bool Verify(string password, string storedBase64, byte[] salt) {
        var (hash, _) = HashPassword(password, salt);
        return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(storedBase64), Convert.FromBase64String(hash));
    }
}
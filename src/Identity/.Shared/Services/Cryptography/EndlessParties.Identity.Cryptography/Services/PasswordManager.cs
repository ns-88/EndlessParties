using System.Security.Cryptography;
using EndlessParties.Identity.Cryptography.Abstractions;
using EndlessParties.Identity.Cryptography.Abstractions.Models;

namespace EndlessParties.Identity.Cryptography.Services;

/// <inheritdoc />
internal class PasswordManager : IPasswordManager
{
    /// <summary>
    /// Количество итераций
    /// </summary>
    private const int Iterations = 100_000;

    /// <summary>
    /// Размер ключа в байтах
    /// </summary>
    private const int KeySize = 32;

    /// <summary>
    /// Размер соли в байтах
    /// </summary>
    private const int SaltSize = 16;


    /// <inheritdoc />
    public PasswordModel CreatePassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var keyBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA512, KeySize);

        var saltText = Convert.ToBase64String(saltBytes);
        var hashText = Convert.ToBase64String(keyBytes);

        return new PasswordModel(hashText, saltText);
    }

    /// <inheritdoc />
    public bool ValidatePassword(string password, PasswordModel userPassword)
    {
        var hashBytes = Convert.FromBase64String(userPassword.Hash);
        var saltBytes = Convert.FromBase64String(userPassword.Salt);
        var keyBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA512, KeySize);

        return CryptographicOperations.FixedTimeEquals(hashBytes, keyBytes);
    }
}
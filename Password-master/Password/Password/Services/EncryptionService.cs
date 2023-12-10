using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Essentials; // Ajoutez cette importation

namespace Password.Services
{
    public static class EncryptionService
    {
        private const string Key = "Qc4bEbZmnPKA68Ib5AKEQnkl3c4hkLGN";

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Le texte ne peut pas être null ou vide.");

            byte[] encrypted;

            Aes aes = null;
            try
            {
                aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(Key);

                // Utilisez le IV des préférences au lieu de le générer aléatoirement
                var storedIV = Preferences.Get("IV", "");
                aes.IV = Convert.FromBase64String(storedIV);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            finally
            {
                aes?.Dispose();
            }

            return Convert.ToBase64String(encrypted); // Seul le texte chiffré est renvoyé, pas le IV
        }

        public static string Decrypt(string cipherText, string iv)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            string plaintext = null;

            Aes aes = null;
            try
            {
                aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Convert.FromBase64String(iv);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                aes?.Dispose();
            }

            return plaintext;
        }
    }
}

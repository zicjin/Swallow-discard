using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Swallow.Service {
    public interface IEncryptorDecryptor {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public class EncryptorDecryptor : IEncryptorDecryptor {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEncryptorDecryptor"/> class.
        /// </summary>
        public EncryptorDecryptor() {
            var temp = new Rfc2898DeriveBytes("33swallow55", new byte[] { 8, 3, 39, 4, 68, 121, 7, 12 }, 10000);
            this._key = temp.GetBytes(32);
            this._iv = temp.GetBytes(16);
        }

        /// <summary>
        /// Encrypts the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns>The cipher text.</returns>
        public string Encrypt(string plainText) {
            using (var rm = new RijndaelManaged()) {
                using (var encryptor = rm.CreateEncryptor(this._key, this._iv)) {
                    var input = Encoding.UTF8.GetBytes(plainText);
                    var output = encryptor.TransformFinalBlock(input, 0, input.Length);

                    return Convert.ToBase64String(output);
                }
            }
        }

        /// <summary>
        /// Decrypts the specified cipher text.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <returns>plain text.</returns>
        public string Decrypt(string cipherText) {
            try {
                using (var rm = new RijndaelManaged()) {
                    using (var decryptor = rm.CreateDecryptor(this._key, this._iv)) {
                        var input = Convert.FromBase64String(cipherText);
                        var output = decryptor.TransformFinalBlock(input, 0, input.Length);

                        return Encoding.UTF8.GetString(output);
                    }
                }
            } catch (FormatException) {
                return String.Empty;
            } catch (CryptographicException) {
                return String.Empty;
            }
        }
    }
}

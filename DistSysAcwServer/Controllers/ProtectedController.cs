﻿using Microsoft.AspNetCore.Mvc;
using DistSysAcwServer.Controllers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.EntityFrameworkCore;
using DistSysAcwServer.Services;
using DistSysAcwServer.Models;
#region Task9
namespace DistSysAcwServer.Controllers
{

    [Authorize(Roles = "Admin, User")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        private readonly RSA rsaProvider;
        private readonly UserContext _dbContext;
        private readonly UserDatabaseAccess _userDatabaseAccess;

        public ProtectedController(UserDatabaseAccess userDatabaseAccess)
        {
            _userDatabaseAccess = userDatabaseAccess;
        }
        // GET: api/protected/hello
        [HttpGet("hello")]
        public IActionResult hello()
        {
            return Ok("Hello from the protected endpoint!");
        }

        // GET: api/protected/sha1/{message}
        [HttpGet("sha1/{message}")]
        public IActionResult sha1(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Message cannot be empty");
            }

            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(message));
                string hexHash = BitConverter.ToString(hashBytes).Replace("-", "");
                return Ok(hexHash);
            }
        }

        // GET: api/protected/sha256/{message}
        [HttpGet("sha256/{message}")]
        public IActionResult sha256(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Message cannot be empty");
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                string hexHash = BitConverter.ToString(hashBytes).Replace("-", "");
                return Ok(hexHash);
            }
        }
        #endregion Task9s


        public static class KeyStorageHelper
        {
            private const string ContainerName = "MyRSAKeyContainer";

            public static RSA GetOrCreateRSAProvider()
            {

#pragma warning disable CA1416 // Validate platform compatibility
                CspParameters cspParams = new CspParameters()
                {
                    KeyContainerName = ContainerName,
                    Flags = CspProviderFlags.UseMachineKeyStore
                };
#pragma warning restore CA1416 // Validate platform compatibility

                try
                {
                    // Try to retrieve the RSA key pair from the machine key store
                    using (var rsa = new RSACryptoServiceProvider(cspParams))
                    {
                        // Check if the key pair exists
                        if (rsa.PublicOnly)
                        {
                            throw new CryptographicException($"RSA key pair not found in the machine key store. Generate a new key pair.");
                        }

                        return rsa;
                    }
                }
                catch (CryptographicException)
                {

                    // RSA key pair not found, generate a new one and store it in the machine key store
                    using (var rsa = new RSACryptoServiceProvider(2048, cspParams))
                    {
                        return rsa;
                    }
                }
            }
        }

        public ProtectedController()
        {
            // Retrieve or create RSA provider
            rsaProvider = KeyStorageHelper.GetOrCreateRSAProvider();
        }




        // GET: api/Protected/GetPublicKey

        [HttpGet("GetPublicKey")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult GetPublicKey()
        {
            // Retrieve the public key from the RSA CSP
            RSA rsa = RSA.Create();
            //rsa.ExportParameters(/* Load public key parameters */);

            // Get the XML representation of the public key
            string publicKeyXml = rsa.ToXmlString(includePrivateParameters: false);

            // Return the public key as XML string
            return Ok(publicKeyXml);
        }

        [HttpPost("Sign")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult Sign([FromBody] string message)
        {
            // Check if the API key is in the database (assuming _dbContext is your database context)
            string apiKey = HttpContext.Request.Headers["ApiKey"];
            var user = _dbContext.Users.FirstOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
            {
                return Unauthorized();
            }

            // Digitally sign the message with the server's private RSA key
            RSA rsa = RSA.Create();
            //rsa.ImportParameters(/* Load private key parameters */);
            byte[] data = Encoding.ASCII.GetBytes(message);
            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

            // Convert the signature to hexadecimal format with dashes as delimiters
            string hexSignature = BitConverter.ToString(signature).Replace("-", "");

            // Return the signed message in hexadecimal format
            return Ok(hexSignature);
        }


        [HttpGet("Mashify")]
        [Authorize(Roles = "Admin")]
        public IActionResult Mashify(
            [FromHeader(Name = "EncryptedString")] string encryptedString,
            [FromHeader(Name = "EncryptedSymmetricKey")] string encryptedSymmetricKey,
            [FromHeader(Name = "EncryptedIV")] string encryptedIV)
        {
            try
            {
                // Check if API Key is in the database
                var apiKey = Request.Headers["ApiKey"].FirstOrDefault();
                var user = _userDatabaseAccess.GetUserByApiKey(apiKey);
                if (user == null)
                {
                    return Unauthorized("Invalid API Key");
                }

                // Decrypt the parameters using the server's private RSA key
                string decryptedString = DecryptRSA(encryptedString, user.Result.PrivateRSAKey);
                string decryptedSymmetricKey = DecryptRSA(encryptedSymmetricKey, user.Result.PrivateRSAKey);
                string decryptedIV = DecryptRSA(encryptedIV, user.Result.PrivateRSAKey);

                // Mashify the decrypted string
                string mashifiedString = MashifyString(decryptedString);

                // Encrypt the mashified string using the client's symmetric key and IV
                string encryptedMashifiedString = EncryptAES(mashifiedString, decryptedSymmetricKey, decryptedIV);

                return Ok(encryptedMashifiedString);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private string DecryptRSA(string encryptedString, object privateRSAKey)
        {
            throw new NotImplementedException();
        }

        
        private string DecryptRSA(string encryptedData, RSA rsa)
        {
            byte[] encryptedBytes = Convert.FromHexString(encryptedData);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA1);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private string EncryptAES(string plainText, string key, string iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromHexString(key);
                aesAlg.IV = Convert.FromHexString(iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToHexString(msEncrypt.ToArray());
                }
            }
        }

        private string MashifyString(string input)
        {
            var vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };
            var chars = input.Select(c => vowels.Contains(c) ? 'X' : c).Reverse();
            return new string(chars.ToArray());
        }
    }







}





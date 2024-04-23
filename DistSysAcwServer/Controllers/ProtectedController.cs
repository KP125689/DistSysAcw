using Microsoft.AspNetCore.Mvc;
using DistSysAcwServer.Controllers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System;
#region Task9
namespace DistSysAcwServer.Controllers
{

    [Authorize(Roles = "Admin, User")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        private readonly RSA rsaProvider;

        // GET: api/protected/hello
        [HttpGet("hello")]
        public IActionResult Hello()
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
        public string GetPublicKey()
        {
            // Get the XML string representation of the RSA public key
            string publicKeyXml = rsaProvider.ToXmlString(false);
            return publicKeyXml;
        }
    }
    

}




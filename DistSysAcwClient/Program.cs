using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


Console.WriteLine("helloworld");
#region Task 10 and beyond
namespace DistSysAcwClient
{
    public class YourClientClass
    {
        private static string publicKey;

        public static async Task GetPublicKey()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://yourserver/api/Protected/GetPublicKey");

                    if (response.IsSuccessStatusCode)
                    {
                        publicKey = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Public key retrieved successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve public key. Status code: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while retrieving the public key: " + ex.Message);
                }
            }
        }

        // Method to encrypt data using the stored public key
        public static string EncryptData(string data)
        {
            // Implement encryption logic using the stored public key
            RSA rsa = RSA.Create();
            rsa.FromXmlString(publicKey);
            byte[] encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(data), RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedData);

            //return "Encrypted data"; // Placeholder for encryption logic
        }
    }


    class Program
    {
        static HttpClient client = new HttpClient();
        private static readonly HttpClient httpClient = new HttpClient();


        public static class Constants
        {
            public const string BaseUrl = "http://localhost:44394/"; // Update <portnumber> with your local server's port
          //public const string BaseUrl = "http://150.237.94.9/<myUniqueCode>/"; // Uncomment this line to switch to the test server
        }

        static async Task Main(string[] args)
        {

            // Retrieve and store the public key
            await YourClientClass.GetPublicKey();

            // Example: Encrypt data using the stored public key
            string encryptedData = YourClientClass.EncryptData("Your sensitive data");
            Console.WriteLine("Encrypted data: " + encryptedData);


            Console.WriteLine("Hello. What would you like to do?");
            Console.WriteLine("1. TalkBack Hello");
            Console.WriteLine("2. TalkBack Sort");
            Console.WriteLine("3. User Get");
            Console.WriteLine("4. User Post");
            Console.WriteLine("5. User Set");
            Console.WriteLine("6. User Delete");
            Console.WriteLine("7. User Role");
            Console.WriteLine("8. Protected Hello");
            Console.WriteLine("9. Protected SHA1");
            Console.WriteLine("10. Protected SHA256");
            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    await TalkBackHello();
                    break;
                case 2:
                    await TalkBackSort();
                    break;
                case 3:
                    await UserGet();
                    break;
                case 4:
                    await UserPost();
                    break;
                case 5:
                    await UserSet();
                    break;
                case 6:
                    await UserDelete();
                    break;
                case 7:
                    await UserRole();
                    break;
                case 8:
                    await ProtectedHello();
                    break;
                case 9:
                    await ProtectedSHA1();
                    break;
                case 10:
                    await ProtectedSHA256();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }

           
        }

        static async Task TalkBackHello()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(Constants.BaseUrl + "api/talkback/hello");
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
        }

        static async Task TalkBackSort()
        {
            // Implement TalkBack Sort functionality
            Console.WriteLine("Enter integers separated by commas:");
            string input = Console.ReadLine();
            string[] numbers = input.Split(',');

            HttpResponseMessage response = await httpClient.PostAsync(Constants.BaseUrl + "api/talkback/sort", new StringContent(input, Encoding.UTF8, "application/json"));
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Sorted Numbers: {result}");
        }

        static async Task UserGet()
        {
            // Implement User Get functionality
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            HttpResponseMessage response = await client.GetAsync(Constants.BaseUrl + $"api/user/get?username={username}");
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }

        static async Task UserPost()
        {
            // Implement User Post functionality
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            StringContent content = new StringContent($"{{ \"username\": \"{username}\" }}", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(Constants.BaseUrl + "api/user/post", content);
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }

        static async Task UserSet()
        {
            // Implement User Set functionality
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter API Key:");
            string apiKey = Console.ReadLine();

            HttpResponseMessage response = await httpClient.PostAsync(Constants.BaseUrl + "api/user/set", new StringContent($"{{ \"username\": \"{username}\", \"apiKey\": \"{apiKey}\" }}", Encoding.UTF8, "application/json"));
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }

        static async Task UserDelete()
        {
            // Implement User Delete functionality
            HttpResponseMessage response = await httpClient.DeleteAsync(Constants.BaseUrl + "api/user/delete");
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }

        static async Task UserRole()
        {
            // Implement User Role functionality
            HttpResponseMessage response = await httpClient.GetAsync(Constants.BaseUrl + "api/user/role");
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }

        static async Task ProtectedHello()
        {
            // Implement Protected Hello functionality
            HttpResponseMessage response = await httpClient.GetAsync(Constants.BaseUrl + "api/protected/hello");
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }

        static async Task ProtectedSHA1()
        {
            // Implement Protected SHA1 functionality
            Console.WriteLine("Enter the message to hash with SHA1:");
            string message = Console.ReadLine();
            HttpResponseMessage response = await httpClient.GetAsync(Constants.BaseUrl + $"api/protected/sha1/{message}");
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"SHA1 Hash: {result}");
        }

        static async Task ProtectedSHA256()
        {
            // Implement Protected SHA256 functionality
            Console.WriteLine("Enter the message to hash with SHA256:");
            string message = Console.ReadLine();
            HttpResponseMessage response = await httpClient.GetAsync(Constants.BaseUrl + $"api/protected/sha256/{message}");
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"SHA256 Hash: {result}");
        }
    }


}
#endregion


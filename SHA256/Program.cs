using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace SHA256
{
    class Program
    {
        static readonly string path = "database.json";

        static void Main(string[] args)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();

            FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            if (User.checkUsername(username, fileStream))
            {
                Console.WriteLine("Username found!");
                Console.Write("Log in with your password: ");
                string password = null;
                while (User.checkPassword(password, fileStream) == false)
                {
                    while (true)
                    {
                        var key = System.Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                            break;
                        password += key.KeyChar;
                    }
                }
                Console.WriteLine("Logged in.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("No user with specified username found.");
                Console.WriteLine("Enter your a password for this new account: ");
                string pw = null;
                while (true)
                {
                    string password = null;
                    while (true)
                    {
                        var key = System.Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                            break;
                        password += key.KeyChar;
                    }

                    Console.WriteLine("Enter the same password again: ");

                    string repeatedPassword = null;
                    while (true)
                    {
                        var key = System.Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                            break;
                        repeatedPassword += key.KeyChar;
                    }

                    if (password == repeatedPassword)
                    {
                        pw = password;
                        break;
                    }

                    Console.WriteLine("You enetered one of them wrong");
                }

                User.save(username, pw, fileStream);
                Console.WriteLine("Logged in");
                Console.ReadKey();




                //SHA256 sha256 = new SHA256("password123");
                //Console.WriteLine(sha256.getHash());
                //Console.ReadKey();

            }

        }
    }

    class User
    {
        public static string path = "database.json";
        public string username { get; set; }
        public string password { get; set; }
        private static List<User> users = new List<User>();

        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public static bool checkUsername(string username, FileStream fileStream)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(fileStream))
            using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
            {
                jsonTextReader.CloseInput = false;
                jsonTextReader.SupportMultipleContent = true;

                while (jsonTextReader.Read())
                {
                    var users = jsonSerializer.Deserialize<List<User>>(jsonTextReader);

                    // Continue

                }
            }
               
            return false;
        }

        public static bool checkPassword(string password, FileStream fileStream)
        {

            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(fileStream))
            using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
            {
                jsonTextReader.CloseInput = false;
                jsonTextReader.SupportMultipleContent = true;

                while (jsonTextReader.Read())
                {
                    if (jsonSerializer.Deserialize<User>(jsonTextReader).password == password) return true;
                }
            }

            return false;
        }

        public static void save(string username, string password, FileStream fileStream)
        {
            users.Add(new User(username, password));

            JsonSerializer jsonSerializer = new JsonSerializer();

            using(StreamWriter sw = File.AppendText(path))
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(sw))
            {
                jsonSerializer.Formatting = Formatting.Indented;
                jsonSerializer.Serialize(jsonTextWriter, users);
            }
        }
    }

    public class ArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    // Operating in 32-bits
    class SHA256
    {
        string message;

        public SHA256(string message)
        {
            this.message = message;
        }

        uint RotR(uint A, byte n) // denotes the circular right shift of n bits of the binary word A.
        {
            // uint = UInt32 = 4 bytes = 4 * 8 bits = 32 bits
            Debug.Assert(n < 32); // Checks if n is less than 32 bits.
            return (A >> n) | (A << (32 - n));
        }

        uint ShR(uint A, byte n) // denotes the right shift of n bits of the binary word A.
        {
            Debug.Assert(n < 32);
            return A >> n;
        }

        uint Ch(uint X, uint Y, uint Z)
        {
            return (X & Y) ^ (~X & Z);
        }

        uint Maj(uint X, uint Y, uint Z)
        {
            return (X & Y) ^ (X & Z) ^ (Y & Z);
        }

        uint Sigma0(uint X)
        {
            return RotR(X, 2) ^ RotR(X, 13) ^ RotR(X, 22);
        }

        uint Sigma1(uint X)
        {
            return RotR(X, 6) ^ RotR(X, 11) ^ RotR(X, 25);
        }

        uint sigma0(string X)
        {
            uint binaryTextTobinary = Convert.ToUInt32(X, 2);
            Console.WriteLine("sigma0: {0}", RotR(binaryTextTobinary, 7) ^ RotR(binaryTextTobinary, 18) ^ ShR(binaryTextTobinary, 3));
            return (RotR(binaryTextTobinary, 7) ^ RotR(binaryTextTobinary, 18) ^ ShR(binaryTextTobinary, 3));
        }

        uint sigma1(string X)
        {
            uint binaryTextTobinary = Convert.ToUInt32(X, 2);
            Console.WriteLine("sigma1: {0}", RotR(binaryTextTobinary, 17) ^ RotR(binaryTextTobinary, 19) ^ ShR(binaryTextTobinary, 10));
            return (RotR(binaryTextTobinary, 17) ^ RotR(binaryTextTobinary, 19) ^ ShR(binaryTextTobinary, 10));
        }



        static readonly uint[] K = new uint[64] {
            0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5, 0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
            0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3, 0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
            0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC, 0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
            0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7, 0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
            0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13, 0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
            0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3, 0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
            0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5, 0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
            0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208, 0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2
        };

        UInt32[] H = new UInt32[8] {
            0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19
            };

        public string getHash()
        {

            int blockSize = 447;
            byte[] byteASCII = Encoding.UTF8.GetBytes(message);
            string byteBinary = "";

            for (var i = 0; i < byteASCII.Length; i++) // This for loop converts all ASCI code to binary and puts it in string array
            {
                string _ = Convert.ToString(byteASCII[i], 2); // Temporary variable _ that converts the input ASCII into base 2 binary
                while (_.Length < 8) _ = "0" + _; // While length < 8 add 0's in front of the string
                byteBinary += _; // Append the binary to the string
            }

            int remainder = byteBinary.Length % blockSize; // Check for the remainder
            int blockAmount = (byteBinary.Length / blockSize) + ((remainder > 0) ? 1 : 0); // Define blockAmount if reamined bigger than zero then add block amount
            remainder += (remainder > 0) ? 0 : blockSize; // If remained bigger than zero then nothing, but if not then dd blocksize

            string[] blocks = new string[blockAmount];

            for (int i = 0; i < blockAmount; i++)
            {
                int currentGroupSize = (i < (blockAmount - 1)) ? blockSize : remainder; // Defines the size of the current index array
                int offset = i * blockSize; // Define where to split the string

                string block = byteBinary.Substring(offset, currentGroupSize);

                if (block.Length > 0)
                {
                    blocks[i] = block;
                }
            }

            string[] paddedBlocks = Padding(blocks);
            // Padding Complete

            // Block decomposition and hash computation
            foreach(var block in paddedBlocks)
            {
                DecomposedBlocks(block);
            }

            foreach(var i in H)
            {
                Console.Write(i.ToString("X") + " ");
            }

            return "";
        }

        private string[] Padding(string[] blocks)
        {
            string[] tempBlocks = new string[blocks.Length];

            int index = 0;
            foreach (string block in blocks)
            {
                string M = block; // Placeholder for block, because it is read only
                int k = (447 - block.Length) % 512; // M.Length = l  / Checks how many 0's are going to be needed
                M += "1"; // Appends one bit
                while (k > 0)
                {
                    M += "0";
                    k--;
                }

                string _ = Convert.ToString(block.Length, 2); // Creating the 64 bit string
                int n = 64 - _.Length;
                for (var i = 0; i < n; i++) _ = "0" + _;
                M = M + _;

                tempBlocks[index] = M;
                index++;
            }

            return tempBlocks;
        }

        private void DecomposedBlocks(string block)
        {
            string[] W = new string[64];
            //Console.WriteLine(block[0]);
            for (int j = 0; j < 64; j++)
            {
                W[j] = (j < 16) ? block.Substring(j * 32, 32) : Convert.ToString(sigma1(W[j - 2]) + Convert.ToUInt32(W[j - 7], 2) + sigma0(W[j - 15]) + Convert.ToUInt32(W[j - 16], 2), 2).PadLeft(32, '0');
                Console.WriteLine(W[j] + ", Length: " + W[j].Length + ", Count: " + j);
            }  

            // Processing
            // 2. Initialize the eight working variables with the (i-1)-st hash value:
            UInt32 a = H[0],
                   b = H[1],
                   c = H[2],
                   d = H[3],
                   e = H[4],
                   f = H[5],
                   g = H[6],
                   h = H[7];

            // 3. For t=0 to 63:
            for (int t = 0; t < 64; ++t)
            {
                UInt32 T1 = h + Sigma1(e) + Ch(e, f, g) + K[t] + Convert.ToUInt32(W[t], 2);
                UInt32 T2 = Sigma0(a) + Maj(a, b, c);
                h = g;
                g = f;
                f = e;
                e = d + T1;
                d = c;
                c = b;
                b = a;
                a = T1 + T2;
            }

            // 4. Compute the intermediate hash value H:
            H[0] = a + H[0];
            H[1] = b + H[1];
            H[2] = c + H[2];
            H[3] = d + H[3];
            H[4] = e + H[4];
            H[5] = f + H[5];
            H[6] = g + H[6];
            H[7] = h + H[7];

        }


    }
}
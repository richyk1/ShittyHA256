﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHA256
{
    class Program
    {
        static void Main(string[] args)
        {
            SHA256 sha256 = new SHA256("password");
            Console.WriteLine(sha256.getHash());
            Console.ReadKey();
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
            Debug.Assert(n < sizeof(uint)); // Checks if n is less than 32 bits.
            return (A >> n) | (A << (sizeof(uint) - n));
        }

        uint ShR(uint A, byte n) // denotes the right shift of n bits of the binary word A.
        {
            Debug.Assert(n < sizeof(uint));
            return A >> n;
        }

        uint Ch(uint X, uint Y, uint Z)
        {
            return (X & Y) ^ (~X & Z);
        }
        
        uint Maj(uint X, uint Y, uint Z)
        {
            return (X & Y) ^ (X & Z) ^ (Y & Z );
        }

        uint Sigma0(uint X)
        {
            return RotR(X, 2) ^ RotR(X, 13) ^ RotR(X, 22);
        }

        uint Sigma1(uint X)
        {
            return RotR(X, 6) ^ RotR(X, 11) ^ RotR(X, 25);
        }

        uint sigma0(uint X)
        {
            return RotR(X, 7) ^ RotR(X, 18) ^ ShR(X, 3);
        }

        uint sigma1(uint X)
        {
            return RotR(X, 17) ^ RotR(X, 19) ^ ShR(X, 10);
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

        public string getHash()
        {
            // Padding
            // Padding the message so that it is fixed value
            byte[] byteASCII = Encoding.UTF8.GetBytes(message);

            string[] byteBinary = new string[message.Length];

            for(var i = 0; i < byteASCII.Length; i++)
            {
                string _ = Convert.ToString(byteASCII[i], 2);
                while (_.Length < 8) _ = "0" + _;
                byteBinary[i] = _;
            }

            string M = ""; // MESSAGE IN BINARY
            foreach(var s in byteBinary){
                Console.Write(s + " ");
                M += s;
            }

            int k = (447 - M.Length) % 512; // M.Length = l
            M += "1";
            while (k > 0)
            {
                M += "0";
                k--;
            }

            if (M.Length < 4294967296) // Check if input of message is bigger than 2 to the power of 64
            {
                string _ = Convert.ToString(M.Length, 2); // Creating the 64 bit string
                int n = 64 - _.Length;
                for (var i = 0; i < n; i++) _ = "0" + _;
                M = M + _;
            }
           // Padding complete



            return "";
        }

    }
}
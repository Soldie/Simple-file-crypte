using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCrypter
{
    public static class Security
    {
        static List<byte> defaultByteList = new List<byte> { 222, 199, 144, 79, 31, 43, 133, 28, 80, 75, 128, 50, 227, 95, 11, 208, 96, 232, 99, 69, 176, 16, 111, 64, 178, 41, 19, 125, 159, 89, 251, 0, 217, 25, 182, 214, 169, 158, 5, 241, 154, 1, 149, 127, 150, 86, 109, 122, 56, 141, 242, 163, 130, 101, 20, 218, 181, 206, 119, 55, 126, 33, 198, 138, 66, 220, 124, 44, 123, 196, 216, 168, 116, 6, 240, 134, 37, 106, 174, 54, 204, 36, 211, 226, 51, 100, 243, 197, 102, 255, 12, 65, 237, 221, 3, 186, 71, 10, 229, 201, 114, 58, 250, 13, 209, 207, 108, 78, 115, 188, 67, 225, 177, 85, 73, 239, 235, 97, 203, 81, 27, 234, 249, 151, 254, 104, 238, 59, 148, 228, 120, 92, 172, 87, 62, 152, 49, 143, 175, 53, 91, 244, 84, 103, 189, 145, 230, 179, 74, 135, 26, 8, 194, 45, 90, 252, 219, 167, 34, 63, 40, 18, 129, 146, 22, 72, 2, 24, 113, 82, 161, 14, 93, 131, 88, 17, 121, 4, 170, 202, 215, 21, 147, 236, 192, 185, 245, 60, 160, 180, 46, 223, 118, 30, 42, 164, 184, 157, 173, 117, 153, 29, 187, 213, 94, 70, 136, 39, 23, 183, 166, 165, 224, 52, 98, 35, 107, 195, 233, 190, 200, 156, 212, 140, 205, 162, 15, 142, 247, 137, 110, 83, 191, 139, 57, 155, 76, 61, 132, 253, 9, 77, 193, 47, 38, 68, 48, 248, 231, 32, 210, 7, 246, 112, 105, 171 };

        static string completionStringPercentage;


        public static void Encrypt(FileStream fileStream, string newPath, string password)
        {
            var substitutionByteList = GetSubstitutionByteList(password);
            completionStringPercentage = "";

            using (var newFileStream = new FileStream(newPath, FileMode.CreateNew))
            {
                var cpt = 0;

                var buffer = new byte[1];
                while (fileStream.Read(buffer, 0, 1) > 0)
                {
                    var index = defaultByteList.IndexOf(buffer[0]);
                    var encryptedData = new byte[] { substitutionByteList[index] };
                    newFileStream.Write(encryptedData, 0, 1);

                    DisplayCompletionMeter(cpt, (int)fileStream.Length - 1);
                    cpt++;
                }
            }
        }


        public static void Decrypt(FileStream fileStream, string newPath, string password)
        {
            var substitutionByteList = GetSubstitutionByteList(password);
            completionStringPercentage = "";

            using (var newFileStream = new FileStream(newPath, FileMode.CreateNew))
            {
                var cpt = 0;

                var buffer = new byte[1];
                while (fileStream.Read(buffer, 0, 1) > 0)
                {
                    var index = substitutionByteList.IndexOf(buffer[0]);
                    var decryptedData = new byte[] { defaultByteList[index] };
                    newFileStream.Write(decryptedData, 0, 1);

                    DisplayCompletionMeter(cpt, (int)fileStream.Length - 1);
                    cpt++;
                }
            }
        }



        static List<byte> GetSubstitutionByteList(string password)
        {
            var substitutionByteList = Encoding.UTF8.GetBytes(password).ToList();
            substitutionByteList.AddRange(defaultByteList);

            for (int i = 0; i <= 255; i++)
            {
                for(int y = i + 1; y <= 255; y++)
                {
                    if(substitutionByteList[i] == substitutionByteList[y])
                    {
                        substitutionByteList.RemoveAt(y);
                        y--;
                    }
                }
            }
            
            return substitutionByteList;
        }


        static void DisplayCompletionMeter(int progress, int total)
        {
            var completion = ((decimal)progress / (decimal)total) * (decimal)100;
            var completionToString = completion.ToString();
            var newCompletionStringPercentage = completionToString.IndexOf(',') != -1 ? completionToString.Substring(0, completionToString.IndexOf(',')) : completionToString;

            if (completion == 1)
            {
                Console.Write("\n");
            }
            else if(completionStringPercentage == "")
            {
                Console.Write("0%");
                completionStringPercentage = newCompletionStringPercentage;
            }
            else if(completionStringPercentage != newCompletionStringPercentage)
            {
                for(int i = 0; i < completionStringPercentage.Length + 1; i++)
                {
                    Console.Write("\b \b");
                }

                completionStringPercentage = newCompletionStringPercentage;
                Console.Write(completionStringPercentage);
                Console.Write('%');
            }
        }
    }
}

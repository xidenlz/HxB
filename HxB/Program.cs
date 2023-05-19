using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Diagnostics;
using AForge.Math.Geometry;
using System.Text;
using System.Drawing;
using AForge;
using System.Security.Cryptography;

namespace HexBuster
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "HxB";
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("$$\\   $$\\           $$$$$$$\\  ");
            Console.WriteLine("$$ |  $$ |          $$  __$$\\ ");
            Console.WriteLine("$$ |  $$ |$$\\   $$\\ $$ |  $$ |");
            Console.WriteLine("$$$$$$$$ |\\$$\\ $$  |$$$$$$$\\ |");
            Console.WriteLine("$$  __$$ | \\$$$$  / $$  __$$\\ ");
            Console.WriteLine("$$ |  $$ | $$  $$<  $$ |  $$ |");
            Console.WriteLine("$$ |  $$ |$$  /\\$$\\ $$$$$$$  |");
            Console.WriteLine("\\__|  \\__|\\__/  \\__|\\_______/");
            Console.ResetColor();
            Console.WriteLine("\n[UPDATES] github.com/Mes2d/HxB");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[INFO] Baisc Commands:");
            Console.WriteLine("[CMD] -E: Extract strings from the file and dump them to a text file.");
            Console.WriteLine("[CMD] -O: Open the file located at the given path.");
            Console.WriteLine("[INFO] Advanced Commands");
            Console.WriteLine("[CMD] -F: Find specific string.");
            Console.WriteLine("[GENERAL]");
            Console.WriteLine("[CMD] clear: Clear console.");
            Console.WriteLine("[CMD] help: Display full list of commands.");
            Console.WriteLine("[CMD] reload: Reload the tool");
            Console.WriteLine("[USAGE] HexBuster <command> <path>");
            while (true)
            {
                string environmentUserName = Environment.UserName;
                Console.Write($"{environmentUserName}@HexBuster => ");
                string input = Console.ReadLine();
                string[] inputParts = input.Split(' ');

                if (inputParts.Length >= 3)
                {
                    string command = inputParts[0];
                    string option = inputParts[1];
                    string path = inputParts[2];

                    if (command == "HexBuster")
                    {
                        if (option == "-E")
                        {
                            string hexData = ReadHex(path);
                            string[] strings = ExtractStringsFromHex(hexData);
                            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ExtractedStrings.txt");
                            File.WriteAllLines(outputPath, strings);
                            Console.WriteLine($"Extracted strings dumped to: {outputPath}");
                            continue;
                        }
                        else if (option == "-O")
                        {
                            Process.Start(path);
                            continue;
                        }
                        else if (option == "-F")
                        {
                            string hexData = ReadHex(path);
                            string[] strings = ExtractStringsFromHex(hexData);

                            Console.WriteLine("Enter the file path to save the specific strings:");
                            string outputPath = Console.ReadLine();

                            Console.WriteLine("Enter the specific strings (separated by commas):");
                            string specificStringsInput = Console.ReadLine();

                            string[] specificStrings = specificStringsInput.Split(',');

                            List<string> matchedStrings = new List<string>();

                            foreach (string str in strings)
                            {
                                foreach (string specificString in specificStrings)
                                {
                                    if (str.Contains(specificString.Trim()))
                                    {
                                        matchedStrings.Add(str);
                                        break;
                                    }
                                }
                            }

                            if (matchedStrings.Count > 0)
                            {
                                File.WriteAllLines(outputPath, matchedStrings);
                                Console.WriteLine($"Specific strings dumped to: {outputPath}");
                            }
                            else
                            {
                                Console.WriteLine("No matching specific strings found, try to dump the strings with -E command and search munualy");
                            }

                            continue;
                        }
                        else if (option == "-D")
                        {
                            try
                            {
                                byte[] ciphertext = File.ReadAllBytes(path);
                                bool isDESEncrypted = DetectDES(ciphertext);

                                if (isDESEncrypted)
                                {
                                    Console.WriteLine("The file appears to be encrypted using DES.");
                                }
                                else
                                {
                                    Console.WriteLine("The file does not appear to be encrypted using DES.");
                                }
                            }
                            catch(Exception DESex)
                            {
                                Console.WriteLine("Error occurred during image processing:");
                                Console.WriteLine(DESex.Message);
                            }
                        }
                    }
                }
                else if (input == "reload")
                {
                    Console.Clear();
                    Console.WriteLine("HxB reoloading");
                    Thread.Sleep(100);
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("$$\\   $$\\           $$$$$$$\\  ");
                    Console.WriteLine("$$ |  $$ |          $$  __$$\\ ");
                    Console.WriteLine("$$ |  $$ |$$\\   $$\\ $$ |  $$ |");
                    Console.WriteLine("$$$$$$$$ |\\$$\\ $$  |$$$$$$$\\ |");
                    Console.WriteLine("$$  __$$ | \\$$$$  / $$  __$$\\ ");
                    Console.WriteLine("$$ |  $$ | $$  $$<  $$ |  $$ |");
                    Console.WriteLine("$$ |  $$ |$$  /\\$$\\ $$$$$$$  |");
                    Console.WriteLine("\\__|  \\__|\\__/  \\__|\\_______/");
                    Console.WriteLine("Note: This tool version is v1 which is still development so do not forget to check on github.com/Mes2d/");
                    Console.WriteLine("\n[INFO] Baisc Commands:");
                    Console.WriteLine("[CMD] -E: Extract strings from the file and dump them to a text file.");
                    Console.WriteLine("[CMD] -O: Open the file located at the given path.");
                    Console.WriteLine("[INFO] Advanced Commands");
                    Console.WriteLine("[CMD] -F: Find specific string.");
                    Console.WriteLine("[GENERAL]");
                    Console.WriteLine("[CMD] clear: Clear console.");
                    Console.WriteLine("[CMD] help: Display full list of commands.");
                    Console.WriteLine("[CMD] reload: Reload the tool");
                    Console.WriteLine("[USAGE] HexBuster <command> <path>");
                }

                else if (input == "clear")
                {
                    Console.Clear();
                }
                else if (input == "help")
                {
                    Console.WriteLine("[INFO] Available Commands:");
                    Console.WriteLine("[CMD] -E: Extract strings from the photo and dump them to a text file.");
                    Console.WriteLine("[CMD] -O: Open the file located at the given path.");
                    Console.WriteLine("[CMD] -F: Find specific string.");
                    Console.WriteLine("[GENERAL]");
                    Console.WriteLine("[CMD] clear: Clear console");
                    Console.WriteLine("[USAGE] HexBuster <command> <path>");
                }

                else
                {
                    Console.WriteLine("Invalid command or path");
                }
            }
        }



        // Hex Function
        static string ReadHex(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            StringBuilder hexBuilder = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                hexBuilder.AppendFormat("{0:X2}", b);
            }

            return hexBuilder.ToString();
        }
        // DES Detection 
        private static bool DetectDES(byte[] ciphertext)
        {
            int count = 0;
            byte[] plaintext = new byte[ciphertext.Length];

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                for (long key = 0; key < Math.Pow(2, 64); key++)
                {
                    byte[] keyBytes = BitConverter.GetBytes(key);

                    Array.Resize(ref keyBytes, 8);

                    des.Key = keyBytes;
                    des.IV = keyBytes;

                    using (ICryptoTransform decryptor = des.CreateDecryptor())
                    {
                        decryptor.TransformBlock(ciphertext, 0, ciphertext.Length, plaintext, 0);
                    }

                    if (HasExpectedPattern(plaintext))
                    {
                        count++;
                    }
                }
            }

            int threshold = 10; 
            return count > threshold;
        }
        // Checking for DES 
        private static bool HasExpectedPattern(byte[] plaintext)
        {
            if (plaintext.Length >= 8 && plaintext[0] == 0x80 && plaintext[1] == 0x01 && plaintext[2] == 0x02 && plaintext[3] == 0x03 && plaintext[4] == 0x04 && plaintext[5] == 0x05 && plaintext[6] == 0x06 && plaintext[7] == 0x07)
            {

                return true;
            }

            for (int i = 0; i < plaintext.Length; i++)
            {
                if (plaintext[i] == 0x01 || plaintext[i] == 0x02 || plaintext[i] == 0x03 || plaintext[i] == 0x04 || plaintext[i] == 0x05 || plaintext[i] == 0x06 || plaintext[i] == 0x07)
                {

                    return true;
                }
            }

            for (int i = 0; i < plaintext.Length; i++)
            {
                if (plaintext[i] == 0x00 || plaintext[i] == 0x08 || plaintext[i] == 0x09 || plaintext[i] == 0x0A || plaintext[i] == 0x0B || plaintext[i] == 0x0C || plaintext[i] == 0x0D || plaintext[i] == 0x0E || plaintext[i] == 0x0F)
                {
                    return true;
                }
            }
            return false;
        }

 
        // Strings Dump

        static string[] ExtractStringsFromHex(string hexData)
        {
            List<string> strings = new List<string>();
            int startIndex = 0;

            while (startIndex < hexData.Length)
            {
                int nullIndex = hexData.IndexOf("00", startIndex);

                if (nullIndex == -1)
                    break;

                int stringLength = (nullIndex - startIndex) / 2;
                byte[] stringBytes = new byte[stringLength];

                for (int i = 0; i < stringLength; i++)
                {
                    stringBytes[i] = Convert.ToByte(hexData.Substring(startIndex + (i * 2), 2), 16);
                }

                string extractedString = Encoding.UTF8.GetString(stringBytes);
                string filteredString = string.Concat(extractedString.Where(c => c >= 32 && c <= 126));

                if (!string.IsNullOrWhiteSpace(filteredString))
                {
                    strings.Add(filteredString);
                }

                startIndex = nullIndex + 2;
            }

            return strings.ToArray();
        }

    }
}

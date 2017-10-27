using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileCrypter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                ConsoleManager.Init();

                var path = args[0];
                var files = new List<string>();

                Security.ProcessTypes method = PromptMethod(args[1] == "folder");
                var password = PromptPassword(false, method);
                bool destructFiles = PromptDestruct(args[1] == "folder");


                Console.Write("\nTarget:  ");
                ConsoleManager.WriteLine(path, ConsoleManager.Colors.File);
                if (args[1] == "folder")
                {
                    Console.Write("Warning : the folder encryption / decryption is ");
                    ConsoleManager.WriteLine("RECURSIVE", ConsoleManager.Colors.Important);
                }

                if (args[1] == "file")
                {
                    if (File.Exists(path))
                        files.Add(path);
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var file in GetFilesInFolder(path))
                            files.Add(file);
                    }
                }

                Console.WriteLine("\nAre you ready to launch the process ? (Press ENTER... or CTRL + C to quit)");
                Console.ReadLine();
                Console.Clear();
                if (files.Count != 1)
                    Console.WriteLine("Processed files :");
                else
                    Console.WriteLine("Progress :");

                Security.totalFiles = files.Count;
                List<Task> tasks = new List<Task>();
                foreach (var file in files)
                {
                    tasks.Add(Task.Run(() => { Security.ProcessFile(new FileInfo(file), password, method, destructFiles, files.Count == 1); }));
                }

                Task.WaitAll(tasks.ToArray());

                Console.WriteLine("\n\n----------\nDone !");
                Console.ReadLine();
            }
        }


        static string PromptPassword(bool reType, Security.ProcessTypes method)
        {
            var password = "";
            var reTypePassword = "";

            while(password == "")
            {
                Console.Write("\n{0} :{1}", reType ? "Type password again" : "Password", reType ? "     " : "                ");
                var input = new ConsoleKeyInfo();

                while (input.Key != ConsoleKey.Enter)
                {
                    input = Console.ReadKey(true);

                    if (input.Key == ConsoleKey.Backspace)
                    {
                        if (password.Length != 0)
                        {
                            password = password.Substring(0, password.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    else if (input.Key != ConsoleKey.Enter)
                    {
                        password += input.KeyChar;
                        ConsoleManager.Write("*", ConsoleManager.Colors.Password);
                    }
                }

                if(password == "")
                {
                    ConsoleManager.Write("Please enter a password...", ConsoleManager.Colors.Error);
                }
                else
                {
                    if (!reType)
                    {
                        while (password != reTypePassword && method == Security.ProcessTypes.encrypt)
                        {
                            if (password != reTypePassword && reTypePassword != "")
                                ConsoleManager.Write("\nPassword don't match", ConsoleManager.Colors.Error);

                            reTypePassword = PromptPassword(true, method);
                        }
                    }
                }
            }            

            return password;
        }


        static Security.ProcessTypes PromptMethod(bool isFolder)
        {
            Console.Write("Encrypt or decrypt file{0} ? (E / d)  ", isFolder ? "s in this folder" : "");
            var input = Console.ReadLine();
            var result = input == "d" || input == "D" ? Security.ProcessTypes.decrypt : Security.ProcessTypes.encrypt;

            Console.Write("Chosen method : ");
            ConsoleManager.WriteLine(result == Security.ProcessTypes.encrypt ? "Encrypt" : "Decrypt", ConsoleManager.Colors.Important);
            return result;
        }


        static bool PromptDestruct(bool isFolder)
        {
            Console.Write("\n\nDestruct original processed file{0} ? (y / N)  ", isFolder ? "s" : "");
            var input = Console.ReadLine();
            var result = input == "y" || input == "Y";

            Console.Write("Original processed file{0} ", isFolder ? "s" : "");
            ConsoleManager.Write((result ? "WILL" : "WON'T") + " be destroyed", ConsoleManager.Colors.Important);
            Console.WriteLine(" after processing");

            return result;
        }


        static IEnumerable<string> GetFilesInFolder(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                foreach (string subDir in Directory.GetDirectories(path))
                {
                    queue.Enqueue(subDir);
                }

                string[] files = null;
                files = Directory.GetFiles(path);
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }
    }
}

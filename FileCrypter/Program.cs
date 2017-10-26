using System;
using System.Collections.Generic;
using System.IO;

namespace FileCrypter
{
    class Program
    {
        enum Methods { encrypt, decrypt }


        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                ConsoleColorHelper.Init();

                var path = args[0];
                var files = new List<string>();

                Methods method = PromptMethod(args[1] == "folder");
                var password = PromptPassword(false, method);
                bool destructFiles = PromptDestruct(args[1] == "folder");


                Console.Write("\nTarget:  ");
                ConsoleColorHelper.WriteLine(path, ConsoleColorHelper.Colors.File);
                if (args[1] == "folder")
                {
                    Console.Write("Warning : the folder encryption / decryption is ");
                    ConsoleColorHelper.WriteLine("RECURSIVE", ConsoleColorHelper.Colors.Important);
                }
                Console.WriteLine("\nAre you ready to launch the process ? (Press any key... or CTRL + C to quit)");
                Console.Read();

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

                foreach(var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    ConsoleColorHelper.Write("\n" + fileInfo.Name, ConsoleColorHelper.Colors.File);
                    Console.Write("  ->  ");

                    using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open))
                    {
                        try
                        {
                            if (method == Methods.encrypt)
                                Security.Encrypt(fileStream, fileInfo.FullName + ".crypted", password);
                            else
                                Security.Decrypt(fileStream, fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf('.')), password);
                        }
                        catch (IOException)
                        {
                            Console.WriteLine("\nIO error... unlucky");
                        }
                        catch (Exception)
                        {
                            ConsoleColorHelper.WriteLine("\nSomething went wrong... unlucky", ConsoleColorHelper.Colors.Error);
                        }
                    }

                    if (destructFiles)
                        File.Delete(file);
                }
                
                
                Console.WriteLine("\n----------\nDone !");
                Console.ReadLine();
            }
        }


        static string PromptPassword(bool reType, Methods method)
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
                        ConsoleColorHelper.Write("*", ConsoleColorHelper.Colors.Password);
                    }
                }

                if(password == "")
                {
                    ConsoleColorHelper.Write("Please enter a password...", ConsoleColorHelper.Colors.Error);
                }
                else
                {
                    if (!reType)
                    {
                        while (password != reTypePassword && method == Methods.encrypt)
                        {
                            if (password != reTypePassword && reTypePassword != "")
                                ConsoleColorHelper.Write("\nPassword don't match", ConsoleColorHelper.Colors.Error);

                            reTypePassword = PromptPassword(true, method);
                        }
                    }
                }
            }            

            return password;
        }


        static Methods PromptMethod(bool isFolder)
        {
            Console.Write("Encrypt or decrypt file{0} ? (E / d)  ", isFolder ? "s in this folder" : "");
            var input = Console.ReadLine();
            var result = input == "d" || input == "D" ? Methods.decrypt : Methods.encrypt;

            Console.Write("Chosen method : ");
            ConsoleColorHelper.WriteLine(result == Methods.encrypt ? "Encrypt" : "Decrypt", ConsoleColorHelper.Colors.Important);
            return result;
        }


        static bool PromptDestruct(bool isFolder)
        {
            Console.Write("\n\nDestruct origin processed file{0} ? (y / N)  ", isFolder ? "s" : "");
            var input = Console.ReadLine();
            var result = input == "y" || input == "Y";

            Console.Write("Origin processed file{0} ", isFolder ? "s" : "");
            ConsoleColorHelper.Write((result ? "WILL" : "WON'T") + " be destroyed", ConsoleColorHelper.Colors.Important);
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

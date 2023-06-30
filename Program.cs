using System;
using System.IO;
using System.Diagnostics;

namespace fswt
{
    class fswt
    {
        static string[] commandLineArgs = Environment.GetCommandLineArgs();

        static void Exec(String str_cmd)
        {
            Process process = new Process();

            // 设置要执行的命令和参数
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C {str_cmd}"; // 在此处替换为您要执行的命令

            // 配置进程启动信息
            process.StartInfo.UseShellExecute = false; // 设置为 false 以重定向输入和输出
            process.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            process.StartInfo.RedirectStandardError = true; // 重定向标准错误
            process.StartInfo.CreateNoWindow = true; // 不创建新窗口

            // 启动进程
            process.Start();

            // 读取输出
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // 等待进程完成
            process.WaitForExit();

            // 输出结果
            Console.WriteLine("Command Stdout Output:");
            Console.WriteLine(output);

            if (error.Length != 0)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(error);
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 1)
            {
                Console.WriteLine("FileSystem Watcher Utilty. V0.1\n");
                Console.WriteLine("usage: fswt  <command>");
                Console.WriteLine("Description: watch you current directory EXE file created/changed event and execute your specified command.\n");

                Console.WriteLine(
                                  $"<command>: you want to execute script or command \n \n" +
                                  $"for example: fswt 'dir *.exe' \n" +
                                  $"Test Github action \n" +
                                  $"Project WebSite: https://github.com/epmpub/fswt.git"
                                  );

                return;
            }


            var watcher = new FileSystemWatcher(@".");

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*.exe";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press enter to exit.");

            string arg1 = commandLineArgs[1];

            Console.ReadLine();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
            ExecAction();

        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"File {e.FullPath} Created";
            Console.WriteLine(value);
            ExecAction();

        }

        private static void ExecAction()
        {
            string arg1 = commandLineArgs[1];
            string cmd = $"{arg1}";
            Exec(cmd);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"File {e.FullPath} Deleted");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
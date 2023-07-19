using System;
using System.IO;
using System.Diagnostics;

namespace fswt
{
    class fswt
    {
        static string[] Args = Environment.GetCommandLineArgs();
        static string cmd_str = null;


        static void Exec(String cmdex)
        {
            Process process = new Process();

            // 设置要执行的命令和参数
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C {cmdex}"; // 在此处替换为您要执行的命令

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

            if (Args.Length == 0 || Args.Length != 4)
            {
                Console.WriteLine("FileSystem Watcher Utilty. V0.2\n");
                Console.WriteLine("usage: fswt  <FileType> <FolderPath> <Command>");
                Console.WriteLine("Description: Monitor you specify path of directory and  fileType created/changed event and execute your specified command.\n");

                Console.WriteLine(
                                  $"<command>: you want to execute script or command \n \n" +
                                  $"for example: fswt *.exe c:\\Windows 'dir *.exe' \n" +
                                  $"Project WebSite: https://github.com/epmpub/fswt.git"
                                  );

                return;
            }


            string command = Args[3];
            string folderPath = Args[2];
            string fileExt = Args[1];

            var watcher = new FileSystemWatcher(folderPath);

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

            watcher.Filter = fileExt;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Moniter is running.Press enter to exit.");

            cmd_str = command;

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
            Exec(cmd_str);
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
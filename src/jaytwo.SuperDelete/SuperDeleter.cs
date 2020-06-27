using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace jaytwo.SuperDelete
{
    public static class SuperDeleter
    {
        // inspired by https://stackoverflow.com/a/8521573/12358195

        internal const int IterationSleepMs = 50;
        internal const int DefaultTimeoutSeconds = 10;

        public static void SuperDelete(string path) => SuperDelete(path, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

        public static void SuperDelete(string path, TimeSpan timeout)
        {
            var quittinTime = DateTime.Now.Add(timeout);

            while (DateTime.Now < quittinTime)
            {
                try
                {
                    Delete(path);
                    return;
                }
                catch
                {
                    if (DateTime.Now > quittinTime)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(IterationSleepMs);
                    }
                }
            }
        }

        public static async Task SuperDeleteAsync(string path) => await SuperDeleteAsync(path, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

        public static async Task SuperDeleteAsync(string path, TimeSpan timeout)
        {
            var quittinTime = DateTime.Now.Add(timeout);

            while (DateTime.Now < quittinTime)
            {
                try
                {
                    Delete(path);
                    return;
                }
                catch
                {
                    if (DateTime.Now > quittinTime)
                    {
                        throw;
                    }
                    else
                    {
                        await Task.Delay(IterationSleepMs);
                    }
                }
            }
        }

        internal static void Delete(string path)
        {
            if (Directory.Exists(path))
            {
                DeleteDirectory(path);
            }
            else if (File.Exists(path))
            {
                DeleteFile(path);
            }
        }

        internal static void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (UnauthorizedAccessException)
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
            catch (IOException)
            {
                Thread.Sleep(1);
                File.Delete(path);
            }
        }

        internal static void DeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                foreach (string subDirectory in Directory.GetDirectories(path))
                {
                    DeleteDirectory(subDirectory);
                }

                if (ex is UnauthorizedAccessException)
                {
                    foreach (var file in Directory.GetFiles(path))
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                    }

                    Directory.Delete(path, true);
                }
                else if (ex is IOException)
                {
                    Thread.Sleep(1);
                    Directory.Delete(path, true);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

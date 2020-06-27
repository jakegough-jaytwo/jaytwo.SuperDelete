using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace jaytwo.SuperDelete.Tests
{
    public class SuperDeleterTests
    {
        [Fact]
        public void SuperDelete_deletes_a_file()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            using (var fileStream = File.Create(fileName))
            {
            }

            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            SuperDeleter.SuperDelete(fileName);

            // assert
            Assert.False(File.Exists(fileName), "Assert file no longer exists");
        }

        [Fact]
        public void SuperDelete_deletes_a_file_after_handle_is_released()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var delay = TimeSpan.FromSeconds(1);

            var stopwatch = Stopwatch.StartNew();
            var thread = CreateDisappearingLockedFile(fileName, delay);
            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            SuperDeleter.SuperDelete(fileName);

            // assert
            stopwatch.Stop();
            Assert.False(File.Exists(fileName), "Assert file no longer exists");

            // https://github.com/dotnet/runtime/issues/24432 says "no Unix or Linux file locking mechanism protects against deletion"
            //Assert.True(stopwatch.Elapsed >= delay, $"Assert elapsed time is greater than delay ({stopwatch.Elapsed.TotalMilliseconds:n1}ms < {delay.TotalMilliseconds:n1}ms)");

            // cleanup
            thread.Join();
        }

        [Fact]
        public void SuperDelete_deletes_a_read_only_file()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            CreateReadOnlyFile(fileName);
            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            SuperDeleter.SuperDelete(fileName);

            // assert
            Assert.False(File.Exists(fileName), "Assert file no longer exists");
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_file()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            using (var fileStream = File.Create(fileName))
            {
            }

            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            await SuperDeleter.SuperDeleteAsync(fileName);

            // assert
            Assert.False(File.Exists(fileName), "Assert file no longer exists");
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_file_after_handle_is_released()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var delay = TimeSpan.FromSeconds(1);

            var stopwatch = Stopwatch.StartNew();
            var task = CreateDisappearingLockedFileAsync(fileName, delay);
            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            await SuperDeleter.SuperDeleteAsync(fileName);

            // assert
            stopwatch.Stop();
            Assert.False(File.Exists(fileName), "Assert file no longer exists");

            // https://github.com/dotnet/runtime/issues/24432 says "no Unix or Linux file locking mechanism protects against deletion"
            //Assert.True(stopwatch.Elapsed >= delay, $"Assert elapsed time is greater than delay ({stopwatch.Elapsed.TotalMilliseconds:n1}ms < {delay.TotalMilliseconds:n1}ms)");

            // cleanup
            await task;
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_read_only_file()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            CreateReadOnlyFile(fileName);
            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            await SuperDeleter.SuperDeleteAsync(fileName);

            // assert
            Assert.False(File.Exists(fileName), "Assert file no longer exists");
        }

        [Fact]
        public void SuperDelete_deletes_a_directory()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryName);
            Assert.True(Directory.Exists(directoryName), "Assert directory exists before starting");

            // act
            SuperDeleter.SuperDelete(directoryName);

            // assert
            Assert.False(Directory.Exists(directoryName), "Assert directory does not exist");
        }

        [Fact]
        public void SuperDelete_deletes_a_directory_after_handle_is_released()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var fileName = Path.Combine(directoryName, Guid.NewGuid().ToString());
            var delay = TimeSpan.FromSeconds(1);

            Directory.CreateDirectory(directoryName);
            var stopwatch = Stopwatch.StartNew();
            var thread = CreateDisappearingLockedFile(fileName, delay);
            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            SuperDeleter.SuperDelete(directoryName);

            // assert
            stopwatch.Stop();
            Assert.False(File.Exists(fileName), "Assert file no longer exists");
            Assert.False(Directory.Exists(directoryName), "Assert directory does not exist");

            // https://github.com/dotnet/runtime/issues/24432 says "no Unix or Linux file locking mechanism protects against deletion"
            //Assert.True(stopwatch.Elapsed >= delay, $"Assert elapsed time is greater than delay ({stopwatch.Elapsed.TotalMilliseconds:n1}ms < {delay.TotalMilliseconds:n1}ms)");

            // cleanup
            thread.Join();
        }

        [Fact]
        public void SuperDelete_deletes_a_directory_with_read_only_file()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var fileName = Path.Combine(directoryName, Guid.NewGuid().ToString());

            Directory.CreateDirectory(directoryName);
            CreateReadOnlyFile(fileName);
            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            SuperDeleter.SuperDelete(directoryName);

            // assert
            Assert.False(File.Exists(fileName), "Assert file no longer exists");
            Assert.False(Directory.Exists(directoryName), "Assert directory does not exist");
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_directory()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryName);
            Assert.True(Directory.Exists(directoryName), "Assert directory exists before starting");

            // act
            await SuperDeleter.SuperDeleteAsync(directoryName);

            // assert
            Assert.False(Directory.Exists(directoryName), "Assert directory does not exist");
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_directory_after_handle_is_released()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var fileName = Path.Combine(directoryName, Guid.NewGuid().ToString());
            var delay = TimeSpan.FromSeconds(1);

            Directory.CreateDirectory(directoryName);
            var stopwatch = Stopwatch.StartNew();
            var task = CreateDisappearingLockedFileAsync(fileName, delay);
            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            await SuperDeleter.SuperDeleteAsync(directoryName);

            // assert
            stopwatch.Stop();
            Assert.False(File.Exists(fileName), "Assert file no longer exists");
            Assert.False(Directory.Exists(directoryName), "Assert directory does not exist");

            // https://github.com/dotnet/runtime/issues/24432 says "no Unix or Linux file locking mechanism protects against deletion"
            //Assert.True(stopwatch.Elapsed >= delay, $"Assert elapsed time is greater than delay ({stopwatch.Elapsed.TotalMilliseconds:n1}ms < {delay.TotalMilliseconds:n1}ms)");

            // cleanup
            await task;
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_directory_with_read_only_file()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var fileName = Path.Combine(directoryName, Guid.NewGuid().ToString());

            Directory.CreateDirectory(directoryName);
            CreateReadOnlyFile(fileName);
            Assert.True(File.Exists(fileName), "Assert file exists before starting");

            // act
            await SuperDeleter.SuperDeleteAsync(directoryName);

            // assert
            Assert.False(File.Exists(fileName), "Assert file no longer exists");
            Assert.False(Directory.Exists(directoryName), "Assert directory does not exist");
        }

        private void CreateReadOnlyFile(string path)
        {
            using (var fileStream = File.Create(path))
            {
            }

            File.SetAttributes(path, FileAttributes.ReadOnly);
        }

        private Thread CreateDisappearingLockedFile(string path, TimeSpan delay)
        {
            var thread = new Thread(() =>
            {
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    fileStream.Lock(0, 0);
                    Thread.Sleep(delay);
                }
            });
            thread.Start();
            while (!File.Exists(path))
            {
                Thread.Sleep(1);
            }

            return thread;
        }

        private Task CreateDisappearingLockedFileAsync(string path, TimeSpan delay)
        {
            var task = Task.Run(async () =>
            {
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    fileStream.Lock(0, 0);
                    await Task.Delay(delay);
                }
            });

            while (!File.Exists(path))
            {
                Thread.Sleep(1);
            }

            return task;
        }
    }
}

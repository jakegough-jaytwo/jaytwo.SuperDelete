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

            Assert.True(File.Exists(fileName));

            // act
            SuperDeleter.SuperDelete(fileName);

            // assert
            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public void SuperDelete_deletes_a_file_after_handle_is_released()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var delay = TimeSpan.FromSeconds(1);

            var thread = CreateDisappearingFile(fileName, delay);
            Assert.True(File.Exists(fileName));
            var stopwatch = Stopwatch.StartNew();

            // act
            SuperDeleter.SuperDelete(fileName);

            // assert
            stopwatch.Stop();
            Assert.False(File.Exists(fileName));
            Assert.True(stopwatch.Elapsed >= delay);

            // cleanup
            thread.Join();
        }

        [Fact]
        public void SuperDelete_deletes_a_read_only_file()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            CreateReadOnlyFile(fileName);
            Assert.True(File.Exists(fileName));

            // act
            SuperDeleter.SuperDelete(fileName);

            // assert
            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_file()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            using (var fileStream = File.Create(fileName))
            {
            }

            Assert.True(File.Exists(fileName));

            // act
            await SuperDeleter.SuperDeleteAsync(fileName);

            // assert
            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_file_after_handle_is_released()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var delay = TimeSpan.FromSeconds(1);

            var task = CreateDisappearingFileAsync(fileName, delay);
            Assert.True(File.Exists(fileName));
            var stopwatch = Stopwatch.StartNew();

            // act
            await SuperDeleter.SuperDeleteAsync(fileName);

            // assert
            stopwatch.Stop();
            Assert.False(File.Exists(fileName));
            Assert.True(stopwatch.Elapsed >= delay, $"{stopwatch.Elapsed.TotalMilliseconds}ms elapsed");

            // cleanup
            await task;
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_read_only_file()
        {
            // arrange
            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            CreateReadOnlyFile(fileName);
            Assert.True(File.Exists(fileName));

            // act
            await SuperDeleter.SuperDeleteAsync(fileName);

            // assert
            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public void SuperDelete_deletes_a_directory()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryName);
            Assert.True(Directory.Exists(directoryName));

            // act
            SuperDeleter.SuperDelete(directoryName);

            // assert
            Assert.False(Directory.Exists(directoryName));
        }

        [Fact]
        public void SuperDelete_deletes_a_directory_after_handle_is_released()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var fileName = Path.Combine(directoryName, Guid.NewGuid().ToString());
            var delay = TimeSpan.FromSeconds(1);

            Directory.CreateDirectory(directoryName);
            var thread = CreateDisappearingFile(fileName, delay);
            Assert.True(File.Exists(fileName));
            var stopwatch = Stopwatch.StartNew();

            // act
            SuperDeleter.SuperDelete(directoryName);

            // assert
            stopwatch.Stop();
            Assert.False(File.Exists(fileName));
            Assert.True(stopwatch.Elapsed >= delay);

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

            // act
            SuperDeleter.SuperDelete(directoryName);

            // assert
            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_directory()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryName);
            Assert.True(Directory.Exists(directoryName));

            // act
            await SuperDeleter.SuperDeleteAsync(directoryName);

            // assert
            Assert.False(Directory.Exists(directoryName));
        }

        [Fact]
        public async Task SuperDeleteAsync_deletes_a_directory_after_handle_is_released()
        {
            // arrange
            var directoryName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var fileName = Path.Combine(directoryName, Guid.NewGuid().ToString());
            var delay = TimeSpan.FromSeconds(1);

            Directory.CreateDirectory(directoryName);
            var task = CreateDisappearingFileAsync(fileName, delay);
            Assert.True(File.Exists(fileName));
            var stopwatch = Stopwatch.StartNew();

            // act
            await SuperDeleter.SuperDeleteAsync(directoryName);

            // assert
            stopwatch.Stop();
            Assert.False(File.Exists(fileName));
            Assert.True(stopwatch.Elapsed >= delay);

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

            // act
            await SuperDeleter.SuperDeleteAsync(directoryName);

            // assert
            Assert.False(File.Exists(fileName));
        }

        private void CreateReadOnlyFile(string path)
        {
            using (var fileStream = File.Create(path))
            {
            }

            File.SetAttributes(path, FileAttributes.ReadOnly);
        }

        private Thread CreateDisappearingFile(string path, TimeSpan delay)
        {
            var thread = new Thread(() =>
            {
                using (var fileStream = File.Create(path))
                {
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

        private Task CreateDisappearingFileAsync(string path, TimeSpan delay)
        {
            var task = Task.Run(async () =>
            {
                using (var fileStream = File.Create(path))
                {
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

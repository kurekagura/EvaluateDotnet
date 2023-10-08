using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WpfApp.Services;

public interface IFileSystemService
{
    Task<List<FileSystemInfo>> GetFileSystemAsync(string baseDirectory);
}

public class FileSystemService : IFileSystemService
{
    public async Task<List<FileSystemInfo>> GetFileSystemAsync(string baseDirectory)
    {
        var items = new List<FileSystemInfo>();

        await Task.Run(() =>
        {
            if (Directory.Exists(baseDirectory))
            {
                // ディレクトリ内のフォルダを取得
                foreach (var directory in Directory.GetDirectories(baseDirectory))
                {
                    items.Add(new DirectoryInfo(directory));
                }

                // ディレクトリ内のファイルを取得
                foreach (var file in Directory.GetFiles(baseDirectory))
                {
                    items.Add(new FileInfo(file));
                }
            }
            else
            {
                Console.WriteLine("指定されたディレクトリは存在しません。");
            }
        });

        return items;
    }
}

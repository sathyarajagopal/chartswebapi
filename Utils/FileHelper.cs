using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ChartsWebAPI.Utils
{
    public static class FileHelper
    {
        public static bool FileExistsOnPath(string fileName)
        {
            return GetFilePath(fileName) != null;
        }

        public static bool FolderExistsOnPath(string folderName)
        {
            return GetFolderPath(folderName) != null;
        }

        public static string GetFilePath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

        public static string GetFolderPath(string folderName)
        {
            if (Directory.Exists(folderName))
                return Path.GetFullPath(folderName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, folderName);
                if (Directory.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }
    }
}

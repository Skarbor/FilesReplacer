using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FilesReplacer.BusinessLayer
{
    public class FilesReplacer
    {
        private string sourceDirectoryTreePath;
        private string destinationDirectoryTreePath;
        private string[] extensionOfTheFilesToReplace;
        private string[] fileNamesToReplace;
        private string[] fileNamesNotToReplace;

        public FilesReplacer()
        {
            Init();
        }
        private void Init()
        {
            sourceDirectoryTreePath = @"C:\repo\SAM\bin\Debug";
            destinationDirectoryTreePath = @"C:\Program Files (x86)\SolarWinds\Orion";
            extensionOfTheFilesToReplace = new string[] { "dll", "pdb" };
            //fileNamesNotToReplace = new string[] { "a.ps1" };
        }

        public void Replace()
        {
            var filesFromSource = GetFilesMatchingCriteria(sourceDirectoryTreePath).Where(file => GetFileName(file).StartsWith("SolarWinds.APM."));
            var filesFromDest = GetFilesMatchingCriteria(destinationDirectoryTreePath).Where(file => GetFileName(file).StartsWith("SolarWinds.APM."));

            Console.WriteLine($"Found {filesFromSource.Count()} in source catalog that match criteria.");
            Console.WriteLine($"Found {filesFromDest.Count()} in source catalog that match criteria.");

            foreach (var destFileFullName in filesFromDest)
            {
                var dstFileName = GetFileName(destFileFullName);
                var srcFile = filesFromSource.Where(file => GetFileName(file) == dstFileName).FirstOrDefault();

                if (srcFile == null)
                {
                    Console.WriteLine($"Have not found {dstFileName} in source catalog");
                    continue;
                }

                //File.Replace(srcFile, destFileFullName, GetFileName(dstFileName));
                File.Copy(srcFile, destFileFullName, true);
                Console.WriteLine($"{GetFileName(srcFile)}   --->   {destFileFullName}");

            }



            //foreach (var fileFromSource in filesFromSource) 
            //{
            //    var srcFileName = GetFileName(fileFromSource);
            //    var destFiles = GetFilesByName(srcFileName, destinationDirectoryTreePath);

            //    if (destFiles == null) continue;

            //    foreach (var destFile in destFiles)
            //    {
            //        File.Replace(fileFromSource, destFile, GetFileName(destFile));
            //        Console.WriteLine($"{fileFromSource}   --->   {destFile}");
            //    }
            //}
        }

        public IEnumerable<string> GetFilesByName(string fileName, string dir)
        {
            var files = new List<string>();
            var filesInDirectory = Directory.GetFiles(dir);

            files.AddRange(filesInDirectory.Where(fileInDirectoryName => fileName == GetFileName(fileInDirectoryName)));

            foreach (var directory in Directory.GetDirectories(dir))
            {
                files.AddRange(GetFilesByName(fileName, directory));
            }

            return files;
        }

        private string[] GetFilesMatchingCriteria(string dir)
        {
            var files = new List<string>();
            files.AddRange(Directory.GetFiles(dir).Where(file => ShouldBeTakenToReplace(file)));
            files.AddRange(Directory.GetDirectories(dir).SelectMany(dir => GetFilesMatchingCriteria(dir)));

            return files.ToArray();
        }

        private bool ShouldBeTakenToReplace(string fileNameWithPath)
        {
            return ((ShouldReplaceBasedOnExtension(fileNameWithPath) || ShouldReplaceBasedOnName(fileNameWithPath)) && !ShouldNotReplaceBasedOnName(fileNameWithPath));
        }

        private bool ShouldReplaceBasedOnExtension(string fileNameWithPath)
        {
            return extensionOfTheFilesToReplace.Contains(GetFileExtension(fileNameWithPath));
        }

        private bool ShouldReplaceBasedOnName(string fileNameWithPath)
        {
            if (fileNamesToReplace is null) return false;
            return fileNamesToReplace.Contains(GetFileName(fileNameWithPath));
        }

        private bool ShouldNotReplaceBasedOnName(string fileNameWithPath)
        {
            if (fileNamesNotToReplace is null) return false;
            return fileNamesNotToReplace.Contains(GetFileName(fileNameWithPath));
        }

        private string GetFileExtension(string fileNameWithPath)
        {
            return fileNameWithPath.Substring(fileNameWithPath.LastIndexOf(".") + 1);
        }

        private string GetFileName(string fileNameWithPath)
        {
            return fileNameWithPath.Substring(fileNameWithPath.LastIndexOf("\\") + 1);
        }
    }
}

#if false
namespace JcCommon;

using System;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Reflection;
using System.Text;
using Global;

public class Installer
{
    public static string InstallZipFromURL(string url, string targetDir, string baseName)
    {
        string guid = Api.GuidString();
        string zipPath = Path.Combine(targetDir, @$"{baseName}.zip");
        SafeDownload(url, zipPath);
        string installDir = Path.Combine(targetDir, baseName);
        SafeZipExtract(zipPath, installDir);
        return installDir;
    }
    public static string InstallResourceDll(Assembly assembly, string targetDir, string name)
    {
        string guid = Api.GuidString();
        var dllBytes = Api.ResourceAsBytes(assembly, name);
        SHA256 crypto = new SHA256CryptoServiceProvider();
        byte[] hashValue = crypto.ComputeHash(dllBytes);
        string sha256 = String.Join("", hashValue.Select(x => x.ToString("x2")).ToArray());
        string dllName = $"{Path.GetFileNameWithoutExtension(name.Replace(":", "-"))}-{sha256}.dll";
        var dllPath = Path.Combine(targetDir, dllName);
        SafeFileWrite(dllPath, dllBytes);
        return dllPath;
    }
    public static string InstallResourceZip(Assembly assembly, string targetDir, string name)
    {
        string guid = Api.GuidString();
        var zipBytes = Api.ResourceAsBytes(assembly, name);
        SHA256 crypto = new SHA256CryptoServiceProvider();
        byte[] hashValue = crypto.ComputeHash(zipBytes);
        string sha256 = String.Join("", hashValue.Select(x => x.ToString("x2")).ToArray());
        string zipName = $"{Path.GetFileNameWithoutExtension(name.Replace(":", "-"))}-{sha256}";
        var extractPath = Path.Combine(targetDir, zipName);
        string zipPath = Path.Combine(targetDir, $"{zipName}.zip");
        SafeFileWrite(zipPath, zipBytes);
        SafeZipExtract(zipPath, extractPath);
        return extractPath;
    }
    public static void SafeDownload(string url, string filePath)
    {
        if (File.Exists(filePath)) return;
        string guid = Api.GuidString();
        EasyObject.Log($"Donloading to {filePath}...");
        Api.DownloadBinaryFromUrl(url, $"{filePath}.{guid}");
        try
        {
            File.Move($"{filePath}.{guid}", filePath);
        }
        catch (Exception)
        {
            ;
        }
    }
    public static void SafeFileWrite(string filePath, byte[] contents)
    {
        if (File.Exists(filePath)) return;
        Console.Error.WriteLine($"[Log] Writing to {filePath}...");
        string guid = Api.GuidString();
        Api.PrepareForFile(filePath);
        File.WriteAllBytes($"{filePath}.{guid}", contents);
        try
        {
            File.Move($"{filePath}.{guid}", filePath);
        }
        catch (Exception)
        {
            ;
        }
    }
    public static void SafeFileWrite(string filePath, string text)
    {
        SafeFileWrite(filePath, Encoding.UTF8.GetBytes(text));
    }
    public static void SafeZipExtract(string zipPath, string dirPath)
    {
        if (Directory.Exists(dirPath)) return;
        EasyObject.Log($"Extracting to {dirPath}...");
        string guid = Api.GuidString();
        ZipFile.ExtractToDirectory(zipPath, $"{dirPath}.{guid}");
        try
        {
            Directory.Move($"{dirPath}.{guid}", dirPath);
        }
        catch (Exception)
        {
            ;
        }
    }
}
#endif

//css_nuget Global.Sys
namespace JcCommon;

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Global;
using static Global.EasyObject;

public static class CppUtil
{
    public static List<string> SrcList = new List<string> { };
    public static List<string> HdrList = new List<string> { };
    //public static List<string> PkgList = new List<string> { };
    //public static List<string> AsmList = new List<string> { };
    public static List<string> ResList = new List<string> { };
    //public static List<string> DllList = new List<string> { };
    public static void DebugDump()
    {
        Log(SrcList, "SrcList");
        Log(HdrList, "HdrList");
        //Log(PkgList, "PkgList");
        //Log(AsmList, "AsmList");
        Log(ResList, "ResList");
        //Log(DllList, "DllList");
    }
    public static void ParseProject(string projFileName)
    {
        string cwd = Directory.GetCurrentDirectory();
        projFileName = Path.GetFullPath(projFileName);
        ParseProjectHelper(projFileName, false);
        Directory.SetCurrentDirectory(cwd);
        for (int i = 0; i < SrcList.Count; i++)
        {
            string src = SrcList[i];
            ParseSource(src);
        }
        for (int i = 0; i < HdrList.Count; i++)
        {
            string src = HdrList[i];
            ParseSource(src);
        }
    }
    private static void ParseProjectHelper(string projFileName, bool header)
    {
        if (!projFileName.StartsWith("$"))
        {
            projFileName = Path.GetFullPath(projFileName);
        }
        if (header)
        {
            if (!HdrList.Contains(projFileName))
                HdrList.Add(projFileName);

        }
        else
        {
            if (!SrcList.Contains(projFileName))
                SrcList.Add(projFileName);

        }
        if (projFileName.StartsWith("$")) return;
        string projDir = Path.GetDirectoryName(projFileName);
        Directory.SetCurrentDirectory(projDir);
        string source = File.ReadAllText(projFileName);
        string[] lines = Sys.TextToLines(source).ToArray();
        for (int i = 0; i < lines.Length; i++)
        {
            {
                string pat = @"^//[+]source[ ]+([^ ;]+)[ ]*;?[ ]*";
                Regex r = new Regex(pat);
                Match m = r.Match(lines[i]);
                if (m.Success)
                {
                    string srcName = m.Groups[1].Value;
                    //srcName = Path.GetFullPath(srcName);
                    ParseProjectHelper(srcName, false);
                }
            }
            {
                string pat = @"^//[+]header[ ]+([^ ;]+)[ ]*;?[ ]*";
                Regex r = new Regex(pat);
                Match m = r.Match(lines[i]);
                if (m.Success)
                {
                    string srcName = m.Groups[1].Value;
                    //srcName = Path.GetFullPath(srcName);
                    ParseProjectHelper(srcName, true);
                }
            }
        }
    }
    private static void ParseSource(string srcPath)
    {
        if (srcPath.StartsWith("$")) return;
        string source = File.ReadAllText(srcPath);
        Directory.SetCurrentDirectory(Path.GetDirectoryName(srcPath));
        string[] lines = Sys.TextToLines(source).ToArray();
        for (int i = 0; i < lines.Length; i++)
        {
            {
                string pat = @"^//[+]embed[ ]+([^ ;]+)[ ]*;?[ ]*";
                Regex r = new Regex(pat);
                Match m = r.Match(lines[i]);
                if (m.Success)
                {
                    string resName = m.Groups[1].Value;
                    //resName = Path.GetFullPath(resName);
                    if (!ResList.Contains(resName))
                    {
                        ResList.Add(resName);
                    }
                }

            }
        }
    }
}

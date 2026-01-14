namespace JcCommon;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Global;

public class JavaScript
{
    protected Jint.Engine engine = JintScript.CreateEngine();
    protected List<Assembly> asmList = new List<Assembly>();
    public JavaScript()
    {
    }
    public void Init(string[] asmSpecList = null, Assembly[] memAsmList = null)
    {
        Console.Error.WriteLine($"[myjs] Initializing {typeof(JavaScript).Assembly.Location}...");
        asmList.Clear();
        if (memAsmList != null )
        {
            foreach (var asm in memAsmList)
            {
                asmList.Add(asm);
            }
        }
        if (asmSpecList != null )
        {
            foreach (var asmSpec in asmSpecList)
            {
                var asm = LoadAssemblyForSpec(asmSpec, Assembly.GetExecutingAssembly());
                asmList.Add(asm);
            }
        }
        engine = JintScript.CreateEngine(asmList.ToArray());
    }
    public void InitForScript(string script, Assembly[] memAsmList = null)
    {
        var lines = TextToLines(script);
        //Echo(lines, "lines");
        List<string> specs = new List<string>();
        foreach (var line in lines)
        {
            if (line.StartsWith("//+"))
            {
                string spec = line.Substring(3);
                spec = spec.Trim();
                specs.Add(spec);
            }
        }
        Init(specs.ToArray(), memAsmList);
    }
    protected string[] TextToLines(string text)
    {
        List<string> lines = new List<string>();
        using (StringReader sr = new StringReader(text))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                lines.Add(line);
            }
        }
        return lines.ToArray();
    }
    protected Assembly LoadAssemblyForSpec(string asmSpec, Assembly assembly)
    {
        string cwd = Path.GetDirectoryName(assembly.Location);
        return LoadAssemblyForSpec(asmSpec, cwd);
    }

    protected Assembly LoadAssemblyForSpec(string asmSpec, string cwd = null)
    {
        Console.Error.WriteLine($"[myjs] //+ {asmSpec}");
        string realPath = null;
        if (cwd != null)
        {
            realPath = FindExePath(asmSpec, cwd);
        }
        else
        {
            realPath = FindExePath(asmSpec);
        }
        if (realPath is null)
        {
            string error = $"{asmSpec} not found";
            Console.Error.WriteLine(error);
            throw new Exception(error);
        }
        Console.Error.WriteLine($"[myjs] Loading {realPath}...");
        var asm = Assembly.LoadFrom(realPath);
        return asm;
    }
    protected string FindExePath(string exe)
    {
        string cwd = "";
        return FindExePath(exe, cwd);
    }
    protected string FindExePath(string exe, string cwd)
    {
        exe = Environment.ExpandEnvironmentVariables(exe);
        if (Path.IsPathRooted(exe))
        {
            if (!File.Exists(exe)) return null;
            return Path.GetFullPath(exe);
        }
        var PATH = Environment.GetEnvironmentVariable("PATH") ?? "";
        PATH = $"{cwd};{PATH}";
        foreach (string test in PATH.Split(';'))
        {
            string path = test.Trim();
            if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                return Path.GetFullPath(path);
        }
        return null;
    }
    protected string FindExePath(string exe, Assembly assembly)
    {
        int bit = IntPtr.Size * 8;
        string cwd = AssemblyDirectory(assembly);
        string result = FindExePath(exe, cwd);
        if (result == null)
        {
            result = FindExePath(exe, $"{cwd}\\{bit}bit");
            if (result == null)
            {
                cwd = Path.Combine(cwd, "assets");
                result = FindExePath(exe, $"{cwd}\\{bit}bit");
            }
        }
        return result;
    }
    protected string AssemblyDirectory(Assembly assembly)
    {
        string codeBase = assembly.CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);
        return Path.GetDirectoryName(path);
    }
    public void SetValue(string name, dynamic value)
    {
        engine.Execute($"globalThis.{name}=({EasyObject.FromObject(value).ToJson()})");
    }
    public dynamic GetValue(string name)
    {
        return engine.GetValue(name).ToObject();
    }
    public EasyObject GetValueAsEasyObject(string name)
    {
        return EasyObject.FromObject(GetValue(name));
    }
    public void Execute(string script, params object[] vars)
    {
        if (vars is null) vars = new object[] { };
        for (int i = 0; i < vars.Length; i++)
        {
            SetValue($"${i + 1}", vars[i]);
        }
        engine.Execute(script);
        for (int i = 0; i < vars.Length; i++)
        {
            engine.Execute($"delete globalThis.${i + 1};");
        }
    }
    public dynamic Evaluate(string script, params object[] vars)
    {
        if (vars is null) vars = new object[] { };
        for (int i = 0; i < vars.Length; i++)
        {
            SetValue($"${i + 1}", vars[i]);
        }
        var result = engine.Evaluate(script).ToObject();
        for (int i = 0; i < vars.Length; i++)
        {
            engine.Execute($"delete globalThis.${i + 1};");
        }
        return result;
    }
    public EasyObject EvaluateAsEasyObject(string script, params object[] vars)
    {
        return EasyObject.FromObject(Evaluate(script, vars));
    }
    public dynamic Call(string name, params object[] vars)
    {
        if (vars is null) vars = new object[] { };
        string script = name + "(";
        for (int i = 0; i < vars.Length; i++)
        {
            if (i > 0) script += ", ";
            script += $"${i + 1}";
        }
        script += ")";
        var result = Evaluate(script, vars);
        for (int i = 0; i < vars.Length; i++)
        {
            engine.Execute($"delete globalThis.${i + 1};");
        }
        return result;
    }
    public EasyObject CallAsEasyObject(string name, params object[] vars)
    {
        return EasyObject.FromObject(Call(name, vars));
    }
}

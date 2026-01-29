using Global;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        //JcCommon.CscsUtil.ParseProject(@"D:/home12/cs/kill-child-auto/main.cs");
        //JcCommon.CscsUtil.ParseProject(@"D:\home12\cs\babel-parser\@main.cs");
        JcCommon.CscsUtil.ParseProject(@"C:\ProgramData\home13\cs\cs-gen-test\demo.main.cs");
        JcCommon.CscsUtil.DebugDump();
   }
}
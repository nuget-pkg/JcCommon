using Global;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;

class Program
{
    static void Main()
    {
        JcCommon.CscsUtil.ParseProject(@"D:/home12/cs/kill-child-auto/main.cs");
        JcCommon.CscsUtil.DebugDump();
   }
}
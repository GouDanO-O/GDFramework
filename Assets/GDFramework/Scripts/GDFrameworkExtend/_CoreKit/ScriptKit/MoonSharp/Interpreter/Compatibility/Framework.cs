using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MoonSharp.Interpreter.Compatibility.Frameworks;

namespace MoonSharp.Interpreter.Compatibility
{
    public static class Framework
    {
        private static FrameworkCurrent s_FrameworkCurrent = new();

        public static FrameworkBase Do => s_FrameworkCurrent;
    }
}
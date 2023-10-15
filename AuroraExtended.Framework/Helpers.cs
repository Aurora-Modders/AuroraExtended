using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuroraExtended.Framework
{
    public static class Helpers
    {
        public static IEnumerable<Control> EnumerateChildren(this Control control)
        {
            foreach (var c in control.Controls.Cast<Control>())
            {
                yield return c;

                if (c.Controls != null)
                {
                    foreach (var d in c.EnumerateChildren())
                    {
                        yield return d;
                    }
                }
            }
        }

        public static bool IsAuroraCode()
        {
            var depth = 0;

            while (true)
            {
                var sf = new StackFrame(depth, false);
                var method = sf.GetMethod();

                if (method == null || depth > 32)
                {
                    return false;
                }

                if (method.DeclaringType != null && method.DeclaringType.Assembly.FullName.Contains("Aurora,"))
                {
                    return true;
                }

                depth++;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuroraExtended
{
    public static class Helpers
    {
        public static bool TryParseAuroraDate(string date_string, out DateTime date)
        {
            date_string = date_string.Trim();

            if (DateTime.TryParseExact(date_string, "F", null, System.Globalization.DateTimeStyles.None, out date))
            {
                return true;
            }
            else if (DateTime.TryParseExact(date_string, "D", null, System.Globalization.DateTimeStyles.None, out date))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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

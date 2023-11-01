using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraExtended.Framework
{
    public static class AuroraInfo
    {
        public static DateTime GetGameDate(this AuroraExtended aurora)
        {
            var pieces = aurora.TacticalMap.Text.Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var sb = new StringBuilder();
            DateTime? date = null;

            for (int start = 0; start < pieces.Count; start++)
            {
                for (int end = start + 1; end <= pieces.Count; end++)
                {
                    sb.Clear();

                    for (int i = start; i < end; i++)
                    {
                        sb.Append(pieces[i]);
                        sb.Append(" ");
                    }

                    var text = sb.ToString().Trim();

                    if (Helpers.TryParseAuroraDate(text, out var d))
                    {
                        date = d;
                    }
                }
            }

            if (date.HasValue)
            {
                return date.Value;
            }

            throw new Exception("Can not find date.");
        }
    }
}

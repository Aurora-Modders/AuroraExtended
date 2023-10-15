using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuroraExtended
{
    internal static class AuroraTests
    {
        private static AuroraExtended Aurora { get; set; }

        internal static void Initialize(Harmony harmony, AuroraExtended aurora)
        {
            Aurora = aurora;
            Aurora.PostIncrement += OnPostIncrement;

            Aurora.AddButton("T1", ButtonClicked);
        }

        private static void OnPostIncrement(object sender, EventArgs e)
        {
            var date = Aurora.GetGameDate();
            Aurora.LogInfo($"game date {date}");

            foreach (var control in Aurora.TacticalMap.EnumerateChildren())
            {
                Aurora.LogInfo($"control {control.GetType().Name} {control.Name}");
            }
        }

        private static void ButtonClicked(Button button)
        {
            MessageBox.Show($"you clicked {button.Name}");
        }
    }
}

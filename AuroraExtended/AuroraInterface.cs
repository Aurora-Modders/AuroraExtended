using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace AuroraExtended
{
    public static class AuroraInterface
    {
        private static List<KeyValuePair<Button, Action<Button>>> Buttons { get; } = new List<KeyValuePair<Button, Action<Button>>>();
        private static Size ButtonSize { get; set; }
        private static Point ButtonOffset { get; set; }
        private static bool AddedButtons { get; set; } = false;

        internal static void Initialize(Harmony harmony, AuroraExtended aurora)
        {
            var prefix = new HarmonyMethod(typeof(AuroraInterface).GetMethod("ResumeLayoutPrefix", AccessTools.all));

            foreach (var method in typeof(Control).GetMethods(AccessTools.all).Where(x => x.Name.Contains("ResumeLayout")))
            {
                harmony.Patch(method, prefix);
            }
        }

        public static void AddButton(this AuroraExtended aurora, string text, Action<Button> click)
        {
            var button = new Button() { Text = text };
            AddButton(button, click);
            aurora.LogInfo($"Added button {text}");
        }

        private static void AddButton(Button button, Action<Button> click)
        {
            if (Buttons.Count == 0)
            {
                ButtonSize = new Size(48, 48);
                ButtonOffset = new Point(1152, 0);
            }

            var offsetx = ButtonSize.Width * (Buttons.Count / 2);
            button.Size = ButtonSize;
            button.Location = new Point(ButtonOffset.X + offsetx, 0);
            button.Margin = new Padding(0);
            button.TabStop = false;
            button.Name = $"cmd{Guid.NewGuid()}";
            button.UseVisualStyleBackColor = true;
            button.Click += OnButtonClick;

            Buttons.Add(new KeyValuePair<Button, Action<Button>>(button, click));
        }

        private static void OnButtonClick(object sender, EventArgs e)
        {
            var kvp = Buttons.Single(x => x.Key == sender as Button);
            kvp.Value(kvp.Key);
        }

        private static void ResumeLayoutPrefix(Control __instance)
        {
            if (AddedButtons)
            {
                return;
            }

            if (Buttons.Count == 0)
            {
                return;
            }

            if (__instance.Name != "tblIncrement" && __instance.Name != "tblSubPulse")
            {
                return;
            }

            AddedButtons = true;

            var index = __instance.Name == "tblIncrement" ? 0 : 1;
            var count = 0;

            for (int i = index; i < Buttons.Count; i += 2)
            {
                __instance.Controls.Add(Buttons[i].Key);
                count++;
            }

            var extra = count * ButtonSize.Width;
            __instance.Size = new Size(__instance.Size.Width + extra, __instance.Size.Height);
        }

        internal static void Initialize(AuroraExtended aurora)
        {
            aurora.FormShown += OnFormShown;
        }

        private static void OnFormShown(object sender, EventArgs e)
        {
            var form = sender as Form;

            foreach (var control in form.EnumerateChildren())
            {
                if (control is ListView listview)
                {
                    ListViewUpdate(listview, null);
                    listview.SelectedIndexChanged += ListViewUpdate;
                }
            }
        }

        private static void ListViewUpdate(object sender, EventArgs e)
        {
            var listview = sender as ListView;

            listview.BeginUpdate();

            foreach (var item in listview.Items.OfType<ListViewItem>())
            {
                foreach (var subitem in item.SubItems.OfType<ListViewSubItem>())
                {
                    var text = ReformatDate(subitem.Text);

                    if (text != subitem.Text)
                    {
                        subitem.Text = text;
                    }
                }
            }

            listview.EndUpdate();
        }

        private static string ReformatDate(string date)
        {
            if (Helpers.TryParseAuroraDate(date, out var dt))
            {
                return dt.ToShortDateString();
            }
            else
            {
                return date;
            }
        }
    }
}

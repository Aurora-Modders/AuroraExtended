using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuroraExtended
{
    public static class ShadowForms
    {
        private static Dictionary<string, Form> MyShadowForms { get; } = new Dictionary<string, Form>();

        public static Form GetShadowForm(this AuroraExtended aurora, string name)
        {
            var form = MyShadowForms[name];

            return form;
        }

        public static bool IsAuroraForm(this AuroraExtended aurora, Form form)
        {
            if (MyShadowForms.Values.Contains(form))
            {
                return false;
            }

            return true;
        }

        internal static void Initialize(AuroraExtended aurora)
        {
            if (MyShadowForms.Count > 0)
            {
                return;
            }

            var existing_forms = new HashSet<string>();

            foreach (var form in Application.OpenForms.Cast<Form>())
            {
                existing_forms.Add(form.Name);
            }

            foreach (var button in aurora.TacticalMap.EnumerateChildren().OfType<Button>())
            {
                aurora.LogInfo($"Button {button.Name}");

                if (!button.Name.StartsWith("cmdToolbar"))
                {
                    continue;
                }

                if (button.Name == "cmdToolbarGame" || button.Name == "cmdToolbarAuto" || button.Name == "cmdToolbarMedals")
                {
                    continue;
                }

                aurora.LogInfo("is toolbar");

                button.PerformClick();

                foreach (var form in Application.OpenForms.OfType<Form>().ToList())
                {
                    if (existing_forms.Contains(form.Name))
                    {
                        continue;
                    }

                    if (MyShadowForms.ContainsKey(form.Name))
                    {
                        form.Close();

                        continue;
                    }

                    MyShadowForms.Add(form.Name, form);
                    form.Hide();
                    RemoveFromOpen(form);
                }
            }

            foreach (var form in MyShadowForms.Values)
            {
                aurora.LogInfo($"my form {form.Name}");
            }

            aurora.PreIncrement += OnPreIncrement;
            aurora.PostIncrement += OnPostIncrement;
        }

        private static void OnPreIncrement(object sender, EventArgs e)
        {
            foreach (var form in MyShadowForms.Values)
            {
                AddToOpen(form);
            }
        }

        private static void OnPostIncrement(object sender, EventArgs e)
        {
            foreach (var form in MyShadowForms.Values)
            {
                RemoveFromOpen(form);
            }
        }

        private static void AddToOpen(Form form)
        {
            var open_forms = (ArrayList)typeof(FormCollection)
                    .GetProperty("InnerList", AccessTools.all)
                    .GetValue(Application.OpenForms);

            if (open_forms.Contains(form))
            {
                return;
            }

            open_forms.Add(form);
        }

        private static void RemoveFromOpen(Form form)
        {
            var open_forms = (ArrayList)typeof(FormCollection)
                    .GetProperty("InnerList", AccessTools.all)
                    .GetValue(Application.OpenForms);

            if (!open_forms.Contains(form))
            {
                return;
            }

            open_forms.Remove(form);
        }
    }
}

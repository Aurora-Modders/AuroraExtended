using AuroraExtended.Framework;
using AuroraPatch;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace AuroraExtended.Framework
{
    public class AuroraExtended : AuroraPatch.Patch
    {
        public override string Description => "Extended";

        public event EventHandler PreIncrement;
        public event EventHandler PostIncrement;
        public event PaintEventHandler FormPaint;
        public event EventHandler FormShown;

        private static AuroraExtended Instance { get; set; }

        protected override void Loaded(Harmony harmony)
        {
            Instance = this;
            SetupFormEvents(harmony);
            AuroraInterface.Initialize(harmony, this);
        }

        protected override void Started()
        {
            TacticalMap.FormClosing += OnTacticalClosing;
            SetupIncrements();
            ShadowForms.Initialize(this);
            AuroraInterface.Initialize(this);
        }

        protected override void ChangeSettings()
        {
            MessageBox.Show("Extended options");
        }

        private void SetupFormEvents(Harmony harmony)
        {
            var postfix = new HarmonyMethod(GetType().GetMethod("FormConstructorPostfix", AccessTools.all));

            foreach (var type in AuroraAssembly.GetTypes().Where(x => typeof(Form).IsAssignableFrom(x)))
            {
                foreach (var ctr in type.GetConstructors(AccessTools.all))
                {
                    harmony.Patch(ctr, null, postfix);
                }
            }
        }

        private void SetupIncrements()
        {
            foreach (var button in TacticalMap.EnumerateChildren().OfType<Button>().Where(x => x.Name.StartsWith("cmdIncrement")))
            {
                var events = typeof(Button)
                    .GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(button) as EventHandlerList;

                var key = typeof(Control)
                    .GetField("EventClick", BindingFlags.NonPublic | BindingFlags.Static)
                    .GetValue(button);

                var handler = events[key] as EventHandler;

                if (handler == null)
                {
                    LogWarning($"Failed to get handler for {button.Name}");

                    continue;
                }
                else
                {
                    LogInfo($"Found handler for {button.Name}");
                }

                var list = handler.GetInvocationList().OfType<EventHandler>().ToList();

                foreach (var h in list)
                {
                    events.RemoveHandler(key, h);
                }

                button.Click += OnPreIncrement;

                foreach (var h in list)
                {
                    events.AddHandler(key, h);
                }

                button.Click += OnPostIncrement;
            }
        }

        private void OnPreIncrement(object sender, EventArgs e)
        {
            PreIncrement?.Invoke(this, null);
        }

        private void OnPostIncrement(object sender, EventArgs e)
        {
            PostIncrement?.Invoke(this, null);
        }

        private void OnFormPaint(object sender, PaintEventArgs e)
        {
            var form = (Form)sender;

            if (!this.IsAuroraForm(form))
            {
                return;
            }

            FormPaint?.Invoke(sender, e);
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            var form = (Form)sender;

            if (!this.IsAuroraForm(form))
            {
                return;
            }

            FormShown?.Invoke(sender, e);
        }

        private void OnTacticalClosing(object sender, EventArgs e)
        {
            var button = TacticalMap.EnumerateChildren().OfType<Button>().Single(x => x.Name == "cmdToolbarSave");
            button.PerformClick();
        }

        private static void FormConstructorPostfix(Form __instance)
        {
            __instance.Paint += Instance.OnFormPaint;
            __instance.Shown += Instance.OnFormShown;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace AuroraExtended.Framework
{
    public static class AuroraInterface
    {
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
            if (DateTime.TryParseExact(date, "D", null, System.Globalization.DateTimeStyles.None, out var dt))
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

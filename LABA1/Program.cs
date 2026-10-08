using System;
using System.Windows.Forms;
using laba1.Forms;

namespace laba1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DeliveryForm());
        }
    }
}

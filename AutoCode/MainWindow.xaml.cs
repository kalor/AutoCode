using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput;
using WindowsInput.Native;
using static System.Collections.Specialized.NameObjectCollectionBase;

namespace AutoCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IList<HotKey> hotKeys = new List<HotKey>();
        private InputSimulator input = new InputSimulator();
        private System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
            notifyIcon.Visible = true;

            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);
            notifyIcon.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    Application.Current.Shutdown();
                };
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string item in ConfigurationManager.AppSettings.Keys)
            {
                try
                {
                    Key foo = (Key)Enum.Parse(typeof(Key), item);
                    HotKey hotKey = new HotKey(foo, KeyModifier.None, KeyPress);
                    hotKeys.Add(hotKey);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Błąd");
                }
            }
            Application.Current.MainWindow.Hide();
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (HotKey h in hotKeys)
                    h.Dispose();
            }
            catch
            { }
        }

        private void KeyPress(HotKey hotKey)
        {
            try
            {
                string code = ConfigurationManager.AppSettings[hotKey.Key.ToString()];
                Debug.WriteLine("Key = {0}, Value = {1}", hotKey.Key.ToString(), code);
                Debug.Write("Int ");
                foreach (int numb in code.Select(s => (int)s))
                {
                    Debug.Write(numb.ToString() + " ");
                    if (numb >= 65 && numb <= 90)
                        input.Keyboard.KeyDown(VirtualKeyCode.SHIFT);
                    VirtualKeyCode foo = (VirtualKeyCode)Enum.ToObject(typeof(VirtualKeyCode), numb >= 97 && numb <= 122 ? numb - 32 : numb);
                    input.Keyboard.KeyPress(foo);
                    if (numb >= 65 && numb <= 90)
                        input.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
                }
                Debug.WriteLine("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Błąd");
            }
        }

    }
}

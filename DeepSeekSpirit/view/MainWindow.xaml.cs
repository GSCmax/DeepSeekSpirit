using CefSharp;
using CefSharp.Wpf.Experimental;
using System.Windows;
using System.Windows.Input;

namespace DeepSeekSpirit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            wv1.WpfKeyboardHandler = new WpfImeKeyboardHandler(wv1);
            wv1.LifeSpanHandler = new CustomLifeSpanHandler();
        }

        private void Link1_Click(object sender, RoutedEventArgs e)
        {
            if (wv1.Visibility == Visibility.Visible)
            {
                wv1.Visibility = Visibility.Collapsed;
                ConfigBD.Visibility = Visibility.Collapsed;
            }
            else
            {
                wv1.Visibility = Visibility.Visible;
            }
        }

        public void ShowWV()
        {
            wv1.Visibility = Visibility.Visible;
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
        }

        private void Info_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowAttach.GetIsDragElement(this))
                WindowAttach.SetIsDragElement(this, false);
            else
                WindowAttach.SetIsDragElement(this, true);
        }

        private void Info_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var o = Math.Round(this.Opacity, 2);
            if (e.Delta > 0)
            {
                if (o < 1.0)
                    this.Opacity += 0.05;
                else
                    this.Opacity = 1.0;
            }
            else
            {
                if (o > 0.5)
                    this.Opacity -= 0.05;
                else
                    this.Opacity = 0.5;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ReloadDS_Button_Click(object sender, RoutedEventArgs e)
        {
            wv1.Address = GlobalDataHelper.appConfig!.DeepSeekAddr;
        }

        private void Config_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ConfigBD.Visibility == Visibility.Visible)
            {
                ConfigBD.Visibility = Visibility.Collapsed;
            }
            else
            {
                ConfigBD.Visibility = Visibility.Visible;
            }
        }

        private void Config_Close_Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigBD.Visibility = Visibility.Collapsed;
        }

        private void Config_Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            wv1.Address = ConfigTB1.Text;
            GlobalDataHelper.appConfig!.DeepSeekAddr = ConfigTB1.Text;

            double w, h;
            double.TryParse(ConfigTB2.Text, out w);
            double.TryParse(ConfigTB3.Text, out h);
            if (w != 0 && h != 0)
            {
                wv1.Width = w;
                wv1.Height = h;
                GlobalDataHelper.appConfig!.Width = w;
                GlobalDataHelper.appConfig!.Height = h;
            }

            ConfigBD.Visibility = Visibility.Collapsed;
        }
    }

    public class CustomLifeSpanHandler : LifeSpanHandler
    {
        protected override bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false;
        }

        protected override bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            chromiumWebBrowser.Load(targetUrl); // 在当前浏览器实例加载目标链接
            newBrowser = null;
            return true; // 返回 true 阻止默认的打开新窗口行为
        }
    }
}
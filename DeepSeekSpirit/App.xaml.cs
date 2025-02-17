using CefSharp;
using CefSharp.Wpf;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Application = System.Windows.Application;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace DeepSeekSpirit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow? mw;
        CefSettings? settings;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GlobalDataHelper.Init();

            #region cefsharp相关
            settings = new()
            {
                CachePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DeepSeekSpirit\\CefSharpCache"),
                UserDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DeepSeekSpirit\\CefSharpUserData"),
                PersistSessionCookies = true,
                Locale = "zh-CN"
            };
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            #endregion

            #region 窗口初始化
            mw = new();
            mw.Show();
            #endregion

            #region 热键注册
            HotKeyHelper.InitHotKey(GlobalDataHelper.appConfig!.HotKey, mw, (hotkey) => { mw.Topmost = true; mw.Activate(); mw.ShowWV(); });
            #endregion
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            GlobalDataHelper.appConfig!.X = mw!.Left;
            GlobalDataHelper.appConfig!.Y = mw!.Top;
            GlobalDataHelper.appConfig!.Opacity = Math.Round(mw!.Opacity, 2);
            GlobalDataHelper.appConfig!.Topmost = mw!.Topmost;
            GlobalDataHelper.appConfig!.CanDrag = WindowAttach.GetIsDragElement(mw!);

            GlobalDataHelper.Save();

            HotKeyHelper.DisposeHotKey();

            Cef.Shutdown();
        }
    }

    #region Converters
    public class BorderClipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 && values[0] is double width && values[1] is double height && values[2] is CornerRadius radius)
            {
                if (width < double.Epsilon || height < double.Epsilon)
                {
                    return Geometry.Empty;
                }

                var clip = new PathGeometry
                {
                    Figures =
                    [
                        new(new Point(radius.TopLeft, 0),
                        [
                            new LineSegment(new Point(width - radius.TopRight, 0), false),
                            new ArcSegment(new Point(width, radius.TopRight), new Size(radius.TopRight, radius.TopRight), 90, false, SweepDirection.Clockwise, false),
                            new LineSegment(new Point(width, height - radius.BottomRight), false),
                            new ArcSegment(new Point(width - radius.BottomRight, height), new Size(radius.BottomRight, radius.BottomRight), 90, false, SweepDirection.Clockwise, false),
                            new LineSegment(new Point(radius.BottomLeft, height), false),
                            new ArcSegment(new Point(0, height - radius.BottomLeft), new Size(radius.BottomLeft, radius.BottomLeft), 90, false, SweepDirection.Clockwise, false),
                            new LineSegment(new Point(0, radius.TopLeft), false),
                            new ArcSegment(new Point(radius.TopLeft, 0), new Size(radius.TopLeft, radius.TopLeft), 90, false, SweepDirection.Clockwise, false),
                        ], false)
                    ]
                };
                clip.Freeze();

                return clip;
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class Bool2ResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Application.Current.FindResource(((string)parameter).Split(',')[0]);
            else
                return Application.Current.FindResource(((string)parameter).Split(',')[1]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ExitIconShowConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)values[0] && (Visibility)values[1] != Visibility.Visible)
                return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CornerRadiusSelConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)values != Visibility.Visible)
                return new CornerRadius(0, 24, 24, 24);
            else return new CornerRadius(0, 0, 0, 24);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class Value2HalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    #endregion
}

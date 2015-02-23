using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirstFloor.ModernUI.App.Pages
{
    public class DpiAwarenessViewModel
            : NotifyPropertyChanged
    {
        private DpiAwareWindow wnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="DpiAwarenessViewModel" /> class.
        /// </summary>
        public DpiAwarenessViewModel()
        {
            this.wnd = (DpiAwareWindow)App.Current.MainWindow;
            this.wnd.DpiChanged += OnWndDpiChanged;
            this.wnd.SizeChanged += OnWndSizeChanged;
        }

        private void OnWndDpiChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(null);        // refresh all properties
        }

        private void OnWndSizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnPropertyChanged(null);        // refresh all properties
        }

        public string DpiAwareMessage
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "The DPI awareness of this process is [b]{0}[/b]", ModernUIHelper.GetDpiAwareness());
            }
        }

        public string WpfDpi
        {
            get
            {
                var info = this.wnd.DpiInformation;
                return string.Format(CultureInfo.InvariantCulture, "{0} x {1}", info.WpfDpiX, info.WpfDpiY);
            }
        }

        public string MonitorDpi
        {
            get
            {
                var info = this.wnd.DpiInformation;
                if (info.MonitorDpiX.HasValue) {
                    return string.Format(CultureInfo.InvariantCulture, "{0} x {1}", info.MonitorDpiX, info.MonitorDpiY);
                }
                return "n/a";
            }
        }

        public string LayoutScale
        {
            get
            {
                var info = this.wnd.DpiInformation;
                return string.Format(CultureInfo.InvariantCulture, "{0} x {1}", info.ScaleX, info.ScaleY);
            }
        }

        public string WindowSize
        {
            get
            {
                var info = this.wnd.DpiInformation;
                var width = this.wnd.ActualWidth * info.WpfDpiX / 96D;
                var height = this.wnd.ActualHeight * info.WpfDpiY / 96D;

                return string.Format(CultureInfo.InvariantCulture, "{0} x {1}", width, height);
            }
        }
    }
}

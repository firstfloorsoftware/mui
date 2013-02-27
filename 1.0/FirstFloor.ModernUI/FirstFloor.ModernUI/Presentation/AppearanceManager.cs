using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FirstFloor.ModernUI.Presentation
{
    /// <summary>
    /// Manages the theme, font size and accent colors for a Modern UI application.
    /// </summary>
    public static class AppearanceManager
    {
        struct Hsl
        {
            private double h;
            private double s;
            private double l;

            public Hsl(double h, double s, double l)
                : this()
            {
                this.H = h;
                this.S = s;
                this.L = l;
            }

            public double H
            {
                get { return this.h; }
                set { this.h = Math.Max(0, Math.Min(1, value)); }
            }
            public double S
            {
                get { return this.s; }
                set { this.s = Math.Max(0, Math.Min(1, value)); }
            }
            public double L
            {
                get { return this.l; }
                set { this.l = Math.Max(0, Math.Min(1, value)); }
            }
        }

        private static Hsl RgbToHsl(Color value)
        {
            var c = System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B);
            return new Hsl {
                H = c.GetHue() / 360,
                S = c.GetSaturation(),
                L = c.GetBrightness()
            };
        }

        private static Color HslToRgb(Hsl hsl)
        {
            double r = 0, g = 0, b = 0;
            double temp1, temp2;

            if (hsl.L == 0) {
                r = g = b = 0;
            }
            else {
                if (hsl.S == 0) {
                    r = g = b = hsl.L;
                }
                else {
                    temp2 = ((hsl.L <= 0.5) ? hsl.L * (1.0 + hsl.S) : hsl.L + hsl.S - (hsl.L * hsl.S));
                    temp1 = 2.0 * hsl.L - temp2;

                    var t3 = new double[] { hsl.H + 1.0 / 3.0, hsl.H, hsl.H - 1.0 / 3.0 };
                    var clr = new double[] { 0, 0, 0 };
                    for (int i = 0; i < 3; i++) {
                        if (t3[i] < 0)
                            t3[i] += 1.0;
                        if (t3[i] > 1)
                            t3[i] -= 1.0;

                        if (6.0 * t3[i] < 1.0)
                            clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
                        else if (2.0 * t3[i] < 1.0)
                            clr[i] = temp2;
                        else if (3.0 * t3[i] < 2.0)
                            clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
                        else
                            clr[i] = temp1;
                    }
                    r = clr[0];
                    g = clr[1];
                    b = clr[2];
                }
            }

            return Color.FromArgb(0xff, (byte)(0xff * r), (byte)(0xff * g), (byte)(0xff * b));
        }

        /// <summary>
        /// Occurs when the theme has changed.
        /// </summary>
        public static event EventHandler ThemeChanged;

        private static readonly Uri DarkThemeSource = new Uri("/FirstFloor.ModernUI;component/Assets/ModernUI.Dark.xaml", UriKind.Relative);
        private static readonly Uri LightThemeSource = new Uri("/FirstFloor.ModernUI;component/Assets/ModernUI.Light.xaml", UriKind.Relative);

        static AppearanceManager()
        {
            DarkThemeCommand = new RelayCommand(o => Theme = Theme.Dark, o => Theme == Theme.Light);
            LightThemeCommand = new RelayCommand(o => Theme = Theme.Light, o => Theme == Theme.Dark);
            SwitchThemeCommand = new RelayCommand(o => Theme = Theme == Theme.Dark ? Theme.Light : Theme.Dark);
            LargeFontSizeCommand = new RelayCommand(o => FontSize = FontSize.Large);
            SmallFontSizeCommand = new RelayCommand(o => FontSize = FontSize.Small);
            AccentColorCommand = new RelayCommand(o => {
                if (o is Color) {
                    AccentColor = (Color)o;
                }
                else {
                    // parse color from string
                    var str = o as string;
                    if (str != null) {
                        AccentColor = (Color)ColorConverter.ConvertFromString(str);
                    }
                }
            }, o => o is Color || o is string);
        }

        private static Theme GetTheme()
        {
            // determine the current theme by looking at the app resources
            var dictionaries = Application.Current.Resources.MergedDictionaries;

            if (dictionaries.Any(d => d.Source == DarkThemeSource)) {
                return Theme.Dark;
            }

            // otherwise just assume light theme
            return Theme.Light;
        }

        private static void SetTheme(Theme theme)
        {
            var source = theme == Presentation.Theme.Dark ? DarkThemeSource : LightThemeSource;
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var themeDict = new ResourceDictionary { Source = source };

            // add new before removing old theme to avoid dynamicresource not found warnings
            dictionaries.Add(themeDict);

            // remove all theme dictionaries (except for the one just added)
            var dictsToRemove = (from d in dictionaries
                                 where d != themeDict && (d.Source == DarkThemeSource || d.Source == LightThemeSource)
                                 select d).ToArray();
            foreach (var dict in dictsToRemove) {
                dictionaries.Remove(dict);
            }
        }

        private static void OnThemeChanged()
        {
            if (ThemeChanged != null) {
                ThemeChanged(null, EventArgs.Empty);
            }
        }

        private static FontSize GetFontSize()
        {
            var defaultFontSize = Application.Current.Resources["DefaultFontSize"] as double?;

            if (defaultFontSize.HasValue) {
                return defaultFontSize.Value == 12D ? FontSize.Small : FontSize.Large;
            }

            // default large
            return FontSize.Large;
        }

        private static void SetFontSize(FontSize fontSize)
        {
            Application.Current.Resources["DefaultFontSize"] = fontSize == FontSize.Small ? 12D : 13D;
            Application.Current.Resources["FixedFontSize"] = fontSize == FontSize.Small ? 10.667D : 13.333D;
        }

        private static Color GetAccentColor()
        {
            var accentColor = Application.Current.Resources["AccentColor"] as Color?;

            if (accentColor.HasValue) {
                return accentColor.Value;
            }

            // default blue
            return Color.FromArgb(0xff, 0x33, 0x99, 0xff);
        }

        private static void SetAccentColor(Color value)
        {
            Application.Current.Resources["AccentColor"] = value;

            // calculate AccentLightColor
            var hsl = RgbToHsl(value);
            hsl.L /= .8;
            var lightValue = HslToRgb(hsl);

            Application.Current.Resources["AccentLightColor"] = lightValue;

            // and update accent brushes
            Application.Current.Resources["Accent"] = new SolidColorBrush(value);
            Application.Current.Resources["AccentLight"] = new SolidColorBrush(lightValue);

            // and re-apply theme to ensure brushes referencing AccentColor and AccentLightColor are updated
            SetTheme(GetTheme());
        }

        /// <summary>
        /// The command that sets the dark theme.
        /// </summary>
        public static ICommand DarkThemeCommand { get; private set; }
        /// <summary>
        /// The command that sets the light color theme.
        /// </summary>
        public static ICommand LightThemeCommand { get; private set; }
        /// <summary>
        /// The command that switches between the dark and the light color theme.
        /// </summary>
        public static ICommand SwitchThemeCommand { get; private set; }
        /// <summary>
        /// The command that sets the large font size.
        /// </summary>
        public static ICommand LargeFontSizeCommand { get; private set; }
        /// <summary>
        /// The command that sets the small font size.
        /// </summary>
        public static ICommand SmallFontSizeCommand { get; private set; }
        /// <summary>
        /// The command that sets the accent color.
        /// </summary>
        public static ICommand AccentColorCommand { get; private set; }

        /// <summary>
        /// Gets or sets the current theme.
        /// </summary>
        public static Theme Theme
        {
            get { return GetTheme(); }
            set
            {
                if (GetTheme() != value) {
                    
                    SetTheme(value);

                    // raise theme changed event
                    OnThemeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public static FontSize FontSize
        {
            get { return GetFontSize(); }
            set
            {
                if (GetFontSize() != value) {
                    SetFontSize(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the accent color.
        /// </summary>
        public static Color AccentColor
        {
            get { return GetAccentColor(); }
            set
            {
                if (GetAccentColor() != value) {
                    SetAccentColor(value);
                }
            }
        }
    }
}

using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace $safeprojectname$.Content
{
/// <summary>
    /// A simple view model for configuring theme, font and accent colors.
    /// </summary>
    public class SettingsAppearanceViewModel
        : NotifyPropertyChanged
    {
        private const string ThemeDark = "dark";
        private const string ThemeLight = "light";

        private const string FontSmall = "small";
        private const string FontLarge = "large";

        // use colors from metro design principles
        private Color[] accentColors = new Color[]{
            Color.FromRgb(0x33, 0x99, 0xff),   // blue
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x33, 0x99, 0x33),   // green
            Color.FromRgb(0x8c, 0xbf, 0x26),   // lime
            Color.FromRgb(0xf0, 0x96, 0x09),   // orange
            Color.FromRgb(0xff, 0x45, 0x00),   // orange red
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xff, 0x00, 0x97),   // magenta
            Color.FromRgb(0xa2, 0x00, 0xff),   // purple            
        };

        private Color selectedAccentColor;

        private string selectedTheme;
        private string selectedFontSize;

        public SettingsAppearanceViewModel()
        {
            this.SelectedTheme = AppearanceManager.Theme == Theme.Dark ? ThemeDark : ThemeLight;
            this.SelectedFontSize = AppearanceManager.FontSize == FontSize.Large ? FontLarge : FontSmall;
            this.SelectedAccentColor = AppearanceManager.AccentColor;

            AppearanceManager.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            this.SelectedTheme = AppearanceManager.Theme == Theme.Dark ? ThemeDark : ThemeLight;
        }

        public string[] Themes
        {
            get { return new string[] { ThemeDark, ThemeLight }; }
        }

        public string[] FontSizes
        {
            get { return new string[] { FontSmall, FontLarge }; }
        }

        public Color[] AccentColors
        {
            get { return this.accentColors; }
        }

        public string SelectedTheme
        {
            get { return this.selectedTheme; }
            set
            {
                if (this.selectedTheme != value) {
                    this.selectedTheme = value;
                    OnPropertyChanged("SelectedTheme");

                    AppearanceManager.Theme = value == ThemeDark ? Theme.Dark : Theme.Light;
                }
            }
        }

        public string SelectedFontSize
        {
            get { return this.selectedFontSize; }
            set
            {
                if (this.selectedFontSize != value) {
                    this.selectedFontSize = value;
                    OnPropertyChanged("SelectedFontSize");

                    AppearanceManager.FontSize = value == FontLarge ? FontSize.Large : FontSize.Small;
                }
            }
        }

        public Color SelectedAccentColor
        {
            get { return this.selectedAccentColor; }
            set
            {
                if (this.selectedAccentColor != value) {
                    this.selectedAccentColor = value;
                    OnPropertyChanged("SelectedAccentColor");

                    AppearanceManager.AccentColor = value;
                }
            }
        }
    }
}

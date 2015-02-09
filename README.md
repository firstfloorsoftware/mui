# Modern UI for WPF (MUI)
A set of controls and styles converting your WPF application into a great looking Modern UI app. This open source project is a spin-off of [XAML Spy](http://xamlspy.com), the visual runtime inspector for Silverlight, Windows Phone, Windows Store and WPF. Read the [official announcement](http://xamlspy.com/news/open-sourcing-the-xaml-spy-ui)

## Demo
Check out the **MUI demo app** included in the [MUI release](https://github.com/firstfloorsoftware/mui/releases). The app demonstrates most features of the MUI framework. The [full source](https://github.com/firstfloorsoftware/mui/tree/master/1.0/FirstFloor.ModernUI/FirstFloor.ModernUI.App) of the demo app is included in the source code of this project.

## Documentation
See some [screenshots](https://github.com/firstfloorsoftware/mui/wiki/Screenshots) to get an idea of what Modern UI for WPF is all about. And visit the [wiki](https://github.com/firstfloorsoftware/mui/wiki) to learn how to incorporate Modern UI for WPF into your application.

![](http://firstfloorsoftware.com/media/github/mui/mui.intro.png)

## Features
* Appearance, configurable at runtime
  * Dark, light and custom themes
  * Accent color
  * Large and small fonts
* New modern controls
  * BBCodeBlock
  * ModernButton
  * ModernDialog
  * ModernFrame
  * ModernMenu
  * ModernProgressRing (with 8 built-in styles)
  * ModernTab
  * ModernWindow
  * RelativeAnimatingContentControl
  * TransitioningContentControl
* Layout
  * A set of predefined page layouts for a consistent look & feel
* Control styles
  * Styles for common WPF controls, such as Button, TextBlock, etc.
  * All styles automatically adapt the dark and light theme and use accent colors where appropiate
* Customizable navigation framework
  * ILinkNavigator and IContentLoader interfaces for maximum flexibility
  * Content loader exception templates in ModernFrame
* Project and item templates
  * Visual Studio 2012 and 2013 project and item templates for creating ModernUI apps as fast and smooth as possible
  * Read more and download the extension containing the templates from the [Visual Studio Gallery](http://visualstudiogallery.msdn.microsoft.com/7a4362a7-fe5d-4f9d-bc7b-0c0dc272fe31)

## Acknowledgements
* [WPF Shell Integration Library](http://archive.msdn.microsoft.com/WPFShell)
* Adapted the TransitioningContentControl from [WPF Toolkit](http://wpf.codeplex.com/)
* Adapted the Windows Phone [RelativeAnimatingContentControl](http://msdn.microsoft.com/en-us/library/gg442303(v=vs.92).aspx) for custom indeterminate ProgressBar styling
* ModernProgressRing styles taken from https://github.com/nigel-sampson/spinkit-xaml
* Modern button icons in sample app taken from http://modernuiicons.com/

## MahApp.Metro
[MahApps.Metro](http://mahapps.com/) is an existing open source library for creating Modern (or Metro) UI styled WPF apps. XAML Spy was initially inspired by MahApp.Metro, but the controls and styles used in XAML Spy (and Modern UI for WPF) are entirely custom-build. If you compare the two projects you'll notice that Modern UI for WPF takes a different approach on building Modern UI styled apps. 

If you haven't done so already, take a look at [MahApps.Metro](http://mahapps.com/), it's a great library.

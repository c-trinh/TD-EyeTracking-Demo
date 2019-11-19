using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GodsOfCalamityBeta
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenu : Page
    {
        public Windows.Graphics.Display.DisplayInformation display = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();

        public MainMenu()
        {
            this.InitializeComponent();

            mainMenu.Height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            mainMenu.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;


            /*Title Definition*/
            titleText.FontSize /= display.RawPixelsPerViewPixel;
            titleText.Margin = new Thickness(mainMenu.Width * .15, mainMenu.Height * .25, 0, 0);

            /*Logo Definition*/
            logo.Height /= display.RawPixelsPerViewPixel;
            logo.Width /= display.RawPixelsPerViewPixel;
            logo.Margin = new Thickness(mainMenu.Width / 2 - (logo.Width /2), mainMenu.Height / 2 - (logo.Height /2), 0, 0);

            /*Start Game Button Definition*/
            startGame.Height /= display.RawPixelsPerViewPixel;
            startGame.Width /= display.RawPixelsPerViewPixel;
            startGame.FontSize /= display.RawPixelsPerViewPixel;
            startGame.Margin = new Thickness(mainMenu.Width * .33 - (startGame.Width * .5), mainMenu.Height - startGame.Height * 1.25, 0, 0);
            startGame.Click += StartGame_Click;

            /*Exit Game Button Definition*/
            exitGame.Height /= display.RawPixelsPerViewPixel;
            exitGame.Width /= display.RawPixelsPerViewPixel;
            exitGame.FontSize /= display.RawPixelsPerViewPixel;
            exitGame.Margin = new Thickness(mainMenu.Width * .66 - (exitGame.Width * .5), mainMenu.Height - exitGame.Height * 1.25, 0, 0);
            exitGame.Click += ExitGame_Click;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GamePage));
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        /// Used to scale to high DPI displays
        private float ScaleToHighDPI(float f)
        {
            Windows.Graphics.Display.DisplayInformation d = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            f *= (float)d.LogicalDpi;
            return f;
        }
    }
}

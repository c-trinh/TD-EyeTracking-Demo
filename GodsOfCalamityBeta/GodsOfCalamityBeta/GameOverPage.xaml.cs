using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
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
    public sealed partial class GameOverPage : Page
    {
        public Windows.Graphics.Display.DisplayInformation display = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
        public Game1 game1;

        public GameOverPage()
        {
            this.InitializeComponent();
            double buttonWidthFactor = 0.2;
            double buttonHeightFactor = 0.2;

            GameOverSpace.Height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            GameOverSpace.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            restartButton.Content = "Restart Game";
            exitButton.Content = "Exit Game";

            restartButton.Height /= display.RawPixelsPerViewPixel;
            restartButton.Width /= display.RawPixelsPerViewPixel;

            exitButton.Height /= display.RawPixelsPerViewPixel;
            exitButton.Width /= display.RawPixelsPerViewPixel;

            restartButton.FontFamily = new FontFamily("Castellar");
            exitButton.FontFamily = new FontFamily("Castellar");
            restartButton.Foreground = new SolidColorBrush(Colors.White);
            exitButton.Foreground = new SolidColorBrush(Colors.White);
            restartButton.BorderBrush = new SolidColorBrush(Colors.White);
            exitButton.BorderBrush = new SolidColorBrush(Colors.White);

            restartButton.FontSize = 40 / display.RawPixelsPerViewPixel;
            exitButton.FontSize = 40 / display.RawPixelsPerViewPixel;

            restartButton.Height = (int)(buttonHeightFactor * GameOverSpace.Height);
            restartButton.Width = (int)(buttonWidthFactor * GameOverSpace.Width);
            exitButton.Height = (int)(buttonHeightFactor * GameOverSpace.Height);
            exitButton.Width = (int)(buttonWidthFactor * GameOverSpace.Width);

            gameOverText.Margin = new Thickness(GameOverSpace.Width * .15, GameOverSpace.Height * .20, 0, 0);
            gameOverText.FontSize /= display.RawPixelsPerViewPixel;
            Thickness restartMargin = new Thickness(GameOverSpace.Width * .33 - (restartButton.Width * .5), GameOverSpace.Height - restartButton.Height * 1.25, 0, 0);
            Thickness exitMargin = new Thickness(GameOverSpace.Width * .66 - (exitButton.Width * .5), GameOverSpace.Height - exitButton.Height * 1.25, 0, 0);

            restartButton.Margin = restartMargin;
            exitButton.Margin = exitMargin;

            restartButton.Click += RestartButton_Click;
            exitButton.Click += ExitButton_Click;
        }

        async private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await CoreApplication.RequestRestartAsync("Application Restart Programmatically ");

            if (result == AppRestartFailureReason.NotInForeground ||
                result == AppRestartFailureReason.RestartPending ||
                result == AppRestartFailureReason.Other)
            {
                var msgBox = new MessageDialog("Restart Failed", result.ToString());
                await msgBox.ShowAsync();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            game1 = e.Parameter as Game1;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            game1.Exit();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using GodsOfCalamityBeta.ECSComponents;
using Windows.UI;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GodsOfCalamityBeta
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
		readonly Game1 _game;
        public Dictionary<int, UserControl> DisasterUCDictionary = new Dictionary<int, UserControl>(); // List of usercontrols in dictionary by their associated entityID
        public Windows.Graphics.Display.DisplayInformation display = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();

        public GamePage()
        {
            this.InitializeComponent();

            // Set up Gaze interaction
            Windows.UI.Core.CoreWindow.GetForCurrentThread().KeyDown += new Windows.Foundation.TypedEventHandler<Windows.UI.Core.CoreWindow, Windows.UI.Core.KeyEventArgs>(delegate (Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args) {
                GazeInput.GetGazePointer(this).Click();
            });

            var sharedSettings = new ValueSet();
            GazeSettingsHelper.RetrieveSharedSettings(sharedSettings).Completed = new AsyncActionCompletedHandler((asyncInfo, asyncStatus) =>
            {
                GazeInput.LoadSettings(sharedSettings);
            });

            Game1.myGamePage = this;

            // Create the game.
            var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
            swapChainPanel.Height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            GameSpace.Height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            swapChainPanel.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;
            GameSpace.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;
            swapChainPanel.Margin = new Thickness(0, 0, 0, 0);
            GameSpace.Margin = new Thickness(0, 0, 0, 0);
            Canvas.SetZIndex(GameSpace, 7);

            // Create the pause button
            
            pauseButton.Content = "Pause Game";
            pauseButton.FontFamily = new FontFamily("Castellar");
            pauseButton.FontSize = 48 / display.RawPixelsPerViewPixel;
            double buttonWidthFactor = 0.2;
            double buttonHeightFactor = 0.15;
            pauseButton.Height = (int)(buttonHeightFactor * GameSpace.Height);
            pauseButton.Width = (int)(buttonWidthFactor * GameSpace.Width);
            Thickness buttonMargin = new Thickness();
            buttonMargin.Top = GameSpace.Height - pauseButton.Height * 1.25;
            buttonMargin.Left = (GameSpace.Width * .5) - (pauseButton.Width * .5);
            pauseButton.Margin = buttonMargin;
            pauseButton.Click += PauseButton_Click;

            // Define unpause and exit buttons
            resumeButton.Content = "Resume Game";
            exitButton.Content = "Exit Game";

            resumeButton.FontFamily = new FontFamily("Castellar");
            exitButton.FontFamily = new FontFamily("Castellar");

            resumeButton.FontSize = 45 / display.RawPixelsPerViewPixel;
            exitButton.FontSize = 45 / display.RawPixelsPerViewPixel;

            resumeButton.Height = (int)(buttonHeightFactor * GameSpace.Height);
            resumeButton.Width = (int)(buttonWidthFactor * GameSpace.Width);
            exitButton.Height = (int)(buttonHeightFactor * GameSpace.Height);
            exitButton.Width = (int)(buttonWidthFactor * GameSpace.Width);

            Thickness restartMargin = new Thickness(GameSpace.Width * .33 - (resumeButton.Width * .5), GameSpace.Height - resumeButton.Height * 1.25, 0, 0);
            Thickness exitMargin = new Thickness(GameSpace.Width * .66 - (exitButton.Width * .5), GameSpace.Height - exitButton.Height * 1.25, 0, 0);

            resumeButton.Margin = restartMargin;
            exitButton.Margin = exitMargin;

            resumeButton.Click += ResumeButton_Click;
            exitButton.Click += ExitButton_Click;

            resumeButton.Visibility = Visibility.Collapsed;
            exitButton.Visibility = Visibility.Collapsed;

            //Define the village button
            Village.Height = 256 / display.RawPixelsPerViewPixel;
            Village.Width = 256 / display.RawPixelsPerViewPixel;
            Thickness villageMargin = new Thickness();
            villageMargin.Top = GameSpace.Height / 2 - Village.Height / 2;
            villageMargin.Left = GameSpace.Width / 2 - Village.Width / 2;
            Village.Margin = villageMargin;

            //Create the health box
            healthblock.Height = 50 / display.RawPixelsPerViewPixel;
            healthblock.Width = Village.Width;
            healthblock.Opacity = 80;
            healthblock.Visibility = Visibility.Collapsed;

            //get the village entity
            var villageEntity = Game1._world.GetEntity(Game1.VillageID);
            var villageHealth = villageEntity.Get<HealthComponent>();
            var villageLocation = villageEntity.Get<PositionComponent>();

            //set the text of healthblock to villageHealth
            healthblock.Value = villageHealth.Health;

            //set the position of the health block to above the village
            Thickness healthMargin = new Thickness();
            healthMargin.Top = GameSpace.Height / 2 - Village.Height + (healthblock.Height * 2);
            healthMargin.Left = GameSpace.Width / 2 - Village.Width/2;
            healthblock.Margin = healthMargin;
            healthblock.HorizontalAlignment = HorizontalAlignment.Center;
            healthblock.VerticalAlignment = VerticalAlignment.Center;

            // Calculate the portion of fireGrid occupied by the pause button here
            int buttonXPos = (int)buttonMargin.Left;
            int buttonYPos = (int)buttonMargin.Top;
            int buttonMaxXPos = buttonXPos + (int)pauseButton.Width;
            int buttonMaxYPos = buttonYPos + (int)pauseButton.Height;
            int fireGridX;
            int fireGridY;
            int spriteWidth = Game1.SpriteDict["fire_1"].texture.Width;
            int spriteHeight = Game1.SpriteDict["fire_1"].texture.Height;

            while (buttonXPos <= buttonMaxXPos)
            {
                while (buttonYPos <= buttonMaxYPos)
                {
                    fireGridX = (buttonXPos - (buttonXPos % spriteWidth)) / spriteWidth;
                    fireGridY = (buttonYPos - (buttonYPos % spriteHeight)) / spriteHeight;

                    fireGridX -= 1;
                    fireGridY -= 1;

                    Game1.fireGrid[fireGridX, fireGridY] = 1;

                    buttonYPos += Game1.SpriteDict["fire_1"].texture.Height;
                }
                buttonYPos = (int)buttonMargin.Top;
                buttonXPos += Game1.SpriteDict["fire_1"].texture.Width;
            }

            ApplicationView view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)    // PauseButton_Click now needs to invoke the correct system.
        {
            Button pause = (Button)sender;
            sender = (Button)sender;
            if (!Game1.isPaused)
            {
                //Add the resume and exit button to game page
                resumeButton.Visibility = Visibility.Visible;
                exitButton.Visibility = Visibility.Visible;
                //Remove the pause button
                pause.Visibility = Visibility.Collapsed;
            }

            //sender = pause;
            Game1.isPaused = !Game1.isPaused;
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            Button resume = (Button)sender;
            sender = (Button)sender;

            if (Game1.isPaused)
            {
                //Add Pause button back
                pauseButton.Visibility = Visibility.Visible;

                //remove resume and exit
                resume.Visibility = Visibility.Collapsed;
                exitButton.Visibility = Visibility.Collapsed;
            }

            Game1.isPaused = !Game1.isPaused;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            GameOver();
        }

        private void Village_Click(object sender, StateChangedEventArgs e)
        {
            if (healthblock.Value < 50)
            {
                healthblock.Foreground = new SolidColorBrush(Colors.Red);
            }

            if (e.PointerState == PointerState.Enter)
            {
                healthblock.Visibility = Visibility.Visible;
                return;
            }
            else if (e.PointerState == PointerState.Exit)
            {
                healthblock.Visibility = Visibility.Collapsed;
                return;
            }
        }

        private void OnInvokeProgress(object sender, DwellProgressEventArgs e)      // This comes from the original sln
        {
            // This is needed for dwelling xaml elements on the front end
            e.Handled = true;
        }

        public void RemoveControlFromGameSpace(UserControl disaster)
        {
            GameSpace.Children.Remove(disaster);
            int entityId = -1;
            
            switch (disaster.Tag.ToString())
            {
                case "lightning":
                    LightningControl lightning = (LightningControl)disaster;
                    entityId = lightning.entityId;
                    break;

                case "meteor":
                    MeteorControl meteor = (MeteorControl)disaster;
                    entityId = meteor.entityId;
                    break;

                case "fire":
                    FireControl fire = (FireControl)disaster;
                    entityId = fire.entityId;
                    break;
            }

            DisasterUCDictionary.Remove(entityId);
        }

        /// <summary>
        /// Creates a new Meteor user control that will coordinate with a Meteor entity
        /// </summary>
        /// <param name="entityId">The ID that will associate the user control with a specific entity.</param>
        public void CreateNewMeteor(int entityId)
        {
            MeteorControl newMeteor = new MeteorControl(entityId);
            DisasterUCDictionary.Add(entityId, newMeteor);
            GameSpace.Children.Add(newMeteor);
        }

        /// <summary>
        /// Creates a new Lightning user control that will coordinate with a Lightning entity
        /// </summary>
        /// <param name="entityId">The ID that will associate the user control with a specific entity.</param>
        public void CreateNewLightning(int entityId)
        {
            LightningControl newLightning = new LightningControl(entityId);
            DisasterUCDictionary.Add(entityId, newLightning);
            GameSpace.Children.Add(newLightning);
        }

        /// <summary>
        /// Creates a new Fire user contorl that will coordinate with a Fire entity
        /// </summary>
        /// <param name="entityId">The ID that will associate the user control with a specific entity.</param>
        public void CreateNewFire(int entityId)
        {
            FireControl newFire = new FireControl(entityId);
            DisasterUCDictionary.Add(entityId, newFire);
            GameSpace.Children.Add(newFire);
        }

        public void DestroyDisaster(int entityId)
        {   
            var entity = Game1._world.GetEntity(entityId);
            if (entity != null && DisasterUCDictionary.ContainsKey(entityId))
            {
                var status = entity.Get<StatusComponent>();
                status.IsDestroyed = true;
                RemoveControlFromGameSpace(DisasterUCDictionary[entityId]);
            }
        }

        public void UpdateBackendGazeState(int entityID, bool newState)
        {
            var entity = Game1._world.GetEntity(entityID);
            if (entity == null)
            {
                return;
            }
            var status = entity.Get<StatusComponent>();
            status.IsGazed = newState;
        }

        public void UpdateDisasterPosition(int entityId)
        {
            var entity = Game1._world.GetEntity(entityId);
            var disaster = DisasterUCDictionary[entityId];
            var position = entity.Get<PositionComponent>();
            float spriteWidth = (float)disaster.Width;
            float spriteHeight = (float)disaster.Height;
            disaster.Margin = new Thickness((position.XCoor / display.RawPixelsPerViewPixel)- (disaster.Width/2), (position.YCoor / display.RawPixelsPerViewPixel) - (disaster.Height/2), 0, 0);
        }

        public void UpdateFrontEnd(int entityId)
        {
            //update village health
            try
            {
                var villageEntity = Game1._world.GetEntity(Game1.VillageID);
                var villageHealth = villageEntity.Get<HealthComponent>().Health;
                healthblock.Value = villageHealth;
            }
            catch { };

            if (DisasterUCDictionary.ContainsKey(entityId))
            {
                try
                {
                    // Get the entity
                    var entity = Game1._world.GetEntity(entityId);
                    var disaster = DisasterUCDictionary[entityId];

                    if (entity == null)
                    {
                        //Remove user control - entity no longer exists
                        RemoveControlFromGameSpace(disaster);
                    }
                    else if (disaster == null)
                    {
                        //var status = entity.Get<StatusComponent>();
                        //status.IsDestroyed = true;
                        
                    }
                    else
                    {
                        var position = entity.Get<PositionComponent>();
                        disaster.Margin = new Thickness((position.XCoor / display.RawPixelsPerViewPixel) - (disaster.Width / 2), (position.YCoor / display.RawPixelsPerViewPixel) - (disaster.Height / 2), 0, 0);
                    }
                }
                catch { };
            }
        }

        public void GameOver()
        {
            this.Frame.Navigate(typeof(GameOverPage), _game);
        }

        private void GazeElement_StateChanged(object sender, StateChangedEventArgs e)
        {

        }
    }
}

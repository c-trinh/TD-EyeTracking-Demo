using GodsOfCalamityBeta.ECSComponents;
using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GodsOfCalamityBeta
{
    public sealed partial class FireControl : UserControl
    {
        public int entityId;
        public bool toDestroy;
        public Windows.Graphics.Display.DisplayInformation display = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();

        public FireControl()
        {
            this.InitializeComponent();
        }

        public FireControl(int id)
        {
            this.InitializeComponent();
            entityId = id;
            var parentEntity = Game1._world.GetEntity(this.entityId);
            var sprite = parentEntity.Get<SpriteComponent>();
            var position = parentEntity.Get<PositionComponent>();

            toDestroy = false;

            this.Height = sprite.Sprite.texture.Height / display.RawPixelsPerViewPixel;
            this.Width = sprite.Sprite.texture.Width / display.RawPixelsPerViewPixel;

            this.Margin = new Thickness(position.XCoor - (Width / 2), position.YCoor - (Height / 2), 0, 0);

            //Setting Tag to identify type of disaster
            this.Tag = "fire";
        }

        private void GazeElement_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.PointerState == PointerState.Enter)
            {
                Game1.myGamePage.UpdateBackendGazeState(this.entityId, true);
            }
            if (e.PointerState == PointerState.Exit)
            {
                Game1.myGamePage.UpdateBackendGazeState(this.entityId, false);
            }
        }

        private void OnInvokeProgress(object sender, DwellProgressEventArgs e)
        {
            e.Handled = true;
        }

        private void FireButton_Invoked(object sender, DwellInvokedRoutedEventArgs e)
        {
            if (!Game1.isPaused)
            {
                Game1.myGamePage.DestroyDisaster(this.entityId);
            }
        }
    }
}

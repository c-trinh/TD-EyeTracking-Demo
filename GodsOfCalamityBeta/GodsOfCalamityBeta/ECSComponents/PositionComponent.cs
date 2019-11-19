using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GodsOfCalamityBeta.ECSComponents
{
    class PositionComponent : INotifyPropertyChanged
    {
        //public Vector2 position;
        public float XCoor { get; set; }
        public float YCoor { get; set; }
        public float Angle { get; set; }
        // The x and y variables represent the sprite’s current position on the plane
        // It’s important to note that, for this class, x and y represent the coordinates of the center of the sprite, (the default origin is the top-left corner).
        // This makes rotating sprites easier, as they will rotate around whatever origin they are given, and rotating around the center gives us a uniform spinning motion.

        PositionComponent()
        { }

        public PositionComponent(float initialX, float initialY, float initialAngle)
        {
            XCoor = initialX;
            YCoor = initialY;
            Angle = initialAngle;
        }

        // XCoor, YCoor and Angle are public facing so that movement systems can handle their manipulation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}

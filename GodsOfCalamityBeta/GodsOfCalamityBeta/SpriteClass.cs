// This class is necessary to create a valid sprite.
// It will compose SpriteCOmponent

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace GodsOfCalamityBeta
{
    public class SpriteClass
    {
        private float scale { get; }
        public Texture2D texture { get; }
        public SpriteClass(GraphicsDevice graphicsDevice, Texture2D newTexture, float scale)
        {
            this.scale = scale;
            texture = newTexture;
            /*
            if (texture == null)
            {
                using (var stream = Microsoft.Xna.Framework.TitleContainer.OpenStream(textureName))
                {
                    texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(graphicsDevice, stream);
                }
            }
            */
        }
    }
}

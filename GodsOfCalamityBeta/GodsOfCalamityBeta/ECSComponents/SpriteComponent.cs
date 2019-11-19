using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace GodsOfCalamityBeta.ECSComponents
{
    class SpriteComponent
    {
        public SpriteClass Sprite { get; set; }
        public float Scale { get; }

        SpriteComponent()
        { }

        public SpriteComponent(SpriteClass newSprite, float newScale)
        {
            Sprite = newSprite;
            Scale = newScale;
        }
    }
}

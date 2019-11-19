using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GodsOfCalamityBeta.ECSComponents
{
    class CollisionBoxComponet
    {
        public Rectangle HitBox { get; set; }

        public CollisionBoxComponet(SpriteClass Entitysprite, PositionComponent Entityposition)
        {
            SpriteClass EntitySprite = Entitysprite;
            Texture2D texture = EntitySprite.texture;
            // Look into how rectangle is generated, make sure that the position corresponds to the centor of the sprite**************************************
            HitBox = new Rectangle((int)Entityposition.XCoor, (int)Entityposition.YCoor, texture.Width / 2, texture.Height / 2);
        }
        public void UpdateHitboxPosition(SpriteClass ESprite, PositionComponent EPosition)
        {
            SpriteClass eSprite = ESprite;
            Texture2D Etexture = eSprite.texture;
            HitBox = new Rectangle((int)EPosition.XCoor, (int)EPosition.YCoor, Etexture.Width, Etexture.Height);
        }
    }
}

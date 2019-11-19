using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace GodsOfCalamityBeta.Systems
{
    
    class RenderSystem : MonoGame.Extended.Entities.Systems.EntityDrawSystem
    {

        public RenderSystem(AspectBuilder aspect) : base(aspect)       // EntityDrawSystem inherits from EntitySystem which takes an AspectBuilder upon construction
        {

        }

        public override void Draw(GameTime gameTime)    // overridden draw function that will render all entities with a sprite component every Darw() call
        {
            Game1.spriteBatch.Begin();
            foreach (var entityID in ActiveEntities)
            {
                // Get the entity that the entityID references
                Entity entity = Game1._world.GetEntity(entityID);

                //      **Get Position**
                // Pull the PositionComponent from the entity
                ECSComponents.PositionComponent entityPositionComponent = entity.Get<ECSComponents.PositionComponent>();
                // Get the coordinates of the entity from the entity's PositionComponent
                var entityX = entityPositionComponent.XCoor;
                var entityY = entityPositionComponent.YCoor;
                var angle = entityPositionComponent.Angle;

                //      **Get Sprite**
                // Pull the SpriteComponent from the entity
                ECSComponents.SpriteComponent entitySpriteComponent = entity.Get<ECSComponents.SpriteComponent>();
                // Get the Texture2D of the entity from the entity's SpriteComponent
                SpriteClass sprite = entitySpriteComponent.Sprite;
                Microsoft.Xna.Framework.Graphics.Texture2D texture = sprite.texture;
                float scale = entitySpriteComponent.Scale;

                // draw the entity
                Vector2 spritePosition = new Vector2(entityX, entityY);
                Game1.spriteBatch.Draw(texture, 
                    spritePosition, 
                    null,
                    Color.White,
                    angle,
                    new Vector2(texture.Width / 2, texture.Height / 2),
                    new Vector2(scale, scale),
                    Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                    0f);
            }
            Game1.spriteBatch.End();
        }

        public override void Initialize(IComponentMapperService mapperService)      // not yet implemented
        {
            
        }
    }
    
}

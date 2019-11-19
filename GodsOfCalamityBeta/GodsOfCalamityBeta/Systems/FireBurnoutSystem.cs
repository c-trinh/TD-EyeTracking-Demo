using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using GodsOfCalamityBeta.ECSComponents;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace GodsOfCalamityBeta.Systems
{
    class FireBurnoutSystem : EntityUpdateSystem
    {
        // This system updates elapsed fire burn frames and destroys fires that have reached the burn interval
        public FireBurnoutSystem(AspectBuilder aspect) : base(aspect)
        {
            
        }

        public override void Initialize(IComponentMapperService mapperService)      // not implemented
        {

        }

        public override void Update(GameTime gameTime)
        {
            foreach (int entityID in ActiveEntities)
            {
                Entity placeholder = Game1._world.GetEntity(entityID);
                StatusComponent capturedStatus = placeholder.Get<StatusComponent>();    // get the status component of the entity
                if (capturedStatus.Type == Game1.EntityType.Fire)    // if the entity is a Lightining Disaster
                {
                    if (capturedStatus.ElapsedBurnFrames >= capturedStatus.FireBurnInterval)
                    {
                        capturedStatus.IsDestroyed = true;
                    }
                    else
                    {
                        capturedStatus.ElapsedBurnFrames++;
                    }
                }
            }
        }
    }
}

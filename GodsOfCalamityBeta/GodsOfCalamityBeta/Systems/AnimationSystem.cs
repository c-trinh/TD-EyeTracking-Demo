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
    class AnimationSystem : EntityUpdateSystem
    {
        public AnimationSystem(AspectBuilder aspect) : base(aspect)
        {

        }

        public override void Initialize(IComponentMapperService mapperService)      // not implemented
        {

        }

        public override void Update(GameTime gameTime)
        {
            foreach(int eID in ActiveEntities)
            {
                Entity placeholder = Game1._world.GetEntity(eID);
                if (placeholder == null)
                {
                    return;
                }
                StatusComponent eStatus = placeholder.Get<StatusComponent>();

                if (eStatus.IsDestroyed && eStatus.Type != Game1.EntityType.Fire)
                {
                    // call the destruction animation
                    SpriteComponent newSprite = new SpriteComponent(Game1.SpriteDict["explosion"], 1);
                    placeholder.Detach<SpriteComponent>();
                    placeholder.Attach<SpriteComponent>(newSprite);
                    UpdateDestructFrame(eID);
                }
                else if (eStatus.IsDestroyed && eStatus.Type == Game1.EntityType.Fire)
                {
                    // call the destruction animation
                    SpriteComponent newSprite = new SpriteComponent(Game1.SpriteDict["ashes"], 1);
                    placeholder.Detach<SpriteComponent>();
                    placeholder.Attach<SpriteComponent>(newSprite);
                    UpdateDestructFrame(eID);
                }
                else if (eStatus.IsGazed && eStatus.ElapsedAnimFrames == 5)    // different animations when gazed
                {
                    eStatus.ElapsedAnimFrames = 0;
                    DwellUpdateSprite(eID);
                }
                else if (eStatus.ElapsedAnimFrames < 5)    // do not change anim frame
                {
                    eStatus.ElapsedAnimFrames++;
                }
                else if (!eStatus.IsGazed && eStatus.ElapsedAnimFrames == 5)
                {
                    eStatus.ElapsedAnimFrames = 0;
                    UpdateSprite(eID);
                    // set new sprite
                }
            }
        }

        private void UpdateDestructFrame(int entityID)
        {
            Entity placeholder = Game1._world.GetEntity(entityID);
            StatusComponent eStatus = placeholder.Get<StatusComponent>();

            if (eStatus.ElapsedDestructAnimFrames <= 5)
            {
                eStatus.ElapsedDestructAnimFrames++;
            }
            else
            {
                eStatus.IsReadyForCleanup = true;
            }
        }

        private void UpdateSprite(int entityID)
        {
            Entity placeholder = Game1._world.GetEntity(entityID);
            StatusComponent eStatus = placeholder.Get<StatusComponent>();

            switch (eStatus.Type)
            {
                case Game1.EntityType.Meteor:
                    PositionComponent pos = placeholder.Get<PositionComponent>();
                    if (eStatus.CurrentAnimSprite == 1)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        if (pos.XCoor <= (Game1.screenWidth * 0.5) && pos.YCoor <= (Game1.screenHeight * 0.5))  // SE meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorSE_2"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                        else if (pos.XCoor >= (Game1.screenWidth * 0.5) && pos.YCoor <= (Game1.screenHeight * 0.5))  // SW meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorSW_2"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                        else if (pos.XCoor <= (Game1.screenWidth * 0.5) && pos.YCoor >= (Game1.screenHeight * 0.5))  // NE meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorNE_2"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                        else if (pos.XCoor >= (Game1.screenWidth * 0.5) && pos.YCoor >= (Game1.screenHeight * 0.5))  // NW meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorNW_2"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                    }
                    else if (eStatus.CurrentAnimSprite == 2)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        if (pos.XCoor <= (Game1.screenWidth * 0.5) && pos.YCoor <= (Game1.screenHeight * 0.5))  // SE meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorSE_3"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                        else if (pos.XCoor >= (Game1.screenWidth * 0.5) && pos.YCoor <= (Game1.screenHeight * 0.5))  // SW meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorSW_3"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                        else if (pos.XCoor <= (Game1.screenWidth * 0.5) && pos.YCoor >= (Game1.screenHeight * 0.5))  // NE meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorNE_3"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                        else if (pos.XCoor >= (Game1.screenWidth * 0.5) && pos.YCoor >= (Game1.screenHeight * 0.5))  // NW meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorNW_3"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                    }
                    else
                    {
                        eStatus.CurrentAnimSprite = 1;
                        SpriteComponent newSprite;
                        if (pos.XCoor <= (Game1.screenWidth * 0.5) && pos.YCoor <= (Game1.screenHeight * 0.5))  // SE meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorSE_1"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                        else if (pos.XCoor >= (Game1.screenWidth * 0.5) && pos.YCoor <= (Game1.screenHeight * 0.5))  // SW meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorSW_1"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                        else if (pos.XCoor <= (Game1.screenWidth * 0.5) && pos.YCoor >= (Game1.screenHeight * 0.5))  // NE meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorNE_1"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                        else if (pos.XCoor >= (Game1.screenWidth * 0.5) && pos.YCoor >= (Game1.screenHeight * 0.5))  // NW meteor
                        {
                            newSprite = new SpriteComponent(Game1.SpriteDict["meteorNW_1"], 1);
                            placeholder.Detach<SpriteComponent>();
                            placeholder.Attach<SpriteComponent>(newSprite);
                        }
                    }
                    break;

                case Game1.EntityType.Lightning:
                    if (eStatus.CurrentAnimSprite == 1)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["lightning_2"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);

                    }
                    else if (eStatus.CurrentAnimSprite == 2)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["lightning_3"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);
                    }
                    else
                    {
                        eStatus.CurrentAnimSprite = 1;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["lightning_1"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);
                    }
                    break;

                case Game1.EntityType.Fire:
                    if (eStatus.CurrentAnimSprite == 1)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["fire_2"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);

                    }
                    else if (eStatus.CurrentAnimSprite == 2)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["fire_3"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);
                    }
                    else
                    {
                        eStatus.CurrentAnimSprite = 1;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["fire_1"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);
                    }
                    break;

                default:
                    break;
            }
            return;
        }

        private void DwellUpdateSprite(int entityID)
        {
            Entity placeholder = Game1._world.GetEntity(entityID);
            StatusComponent eStatus = placeholder.Get<StatusComponent>();

            switch (eStatus.Type)
            {
                case Game1.EntityType.Meteor:
                    break;

                case Game1.EntityType.Lightning:
                    if (eStatus.CurrentAnimSprite == 1)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["lightning_dwell_2"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);

                    }
                    else if (eStatus.CurrentAnimSprite == 2)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["lightning_dwell_3"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);
                    }
                    else
                    {
                        eStatus.CurrentAnimSprite = 1;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["lightning_dwell_1"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);
                    }
                    break;

                case Game1.EntityType.Fire:
                    if (eStatus.CurrentAnimSprite == 1)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["fire_dwell_2"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);

                    }
                    else if (eStatus.CurrentAnimSprite == 2)
                    {
                        eStatus.CurrentAnimSprite++;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["fire_dwell_3"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);
                    }
                    else
                    {
                        eStatus.CurrentAnimSprite = 1;
                        SpriteComponent newSprite;
                        newSprite = new SpriteComponent(Game1.SpriteDict["fire_dwell_1"], 1);
                        placeholder.Detach<SpriteComponent>();
                        placeholder.Attach<SpriteComponent>(newSprite);
                    }
                    break;

                default:
                    break;
            }
            return;
        }
    }
}

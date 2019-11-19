/* Author:  Cong Trinh
 * File:    EntityDestructSystem.cs
 * Desc:    EntityDestructSystem will delete entity from the entity manager in world based on the entity status.
 */

using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using GodsOfCalamityBeta.ECSComponents;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace GodsOfCalamityBeta.Systems
{
    class EntityDestructSystem : EntityUpdateSystem
    {
        ComponentMapper _transformMapper;
        ComponentMapper _spriteMapper;

        public EntityDestructSystem() : base(Aspect.All(typeof(StatusComponent)))
        { }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
        }

        public override void Update(GameTime game_time)
        {
            CheckEntityStatus();
        }

        public bool CheckEntityStatus() {

            bool entity_destroyed = false;
            
            /// Iterate through all entities w/ Status Component
            foreach (var entityId in ActiveEntities)
            {
                var entity = GetEntity(entityId);
                var status = entity.Get<StatusComponent>();

                /// Check if Entity is destroyed
                if (status.IsReadyForCleanup)
                {
                    if (status.Type == Game1.EntityType.Fire)       // if the entity being destroyed is fired, it's grid position must be freed up
                    {
                        // to do this we must get the entity's indicies in the fire grid, then we set that position in the array == 0
                        ECSComponents.PositionComponent capturedPos = entity.Get<ECSComponents.PositionComponent>();
                        int fireSpriteWidth = Game1.SpriteDict["fire_1"].texture.Width;
                        int fireSpriteHeight = Game1.SpriteDict["fire_1"].texture.Height;

                        int fireGridXIndex = ((int)capturedPos.XCoor - ((int)capturedPos.XCoor % fireSpriteWidth)) / fireSpriteWidth;
                        int fireGridYIndex = ((int)capturedPos.YCoor - ((int)capturedPos.YCoor % fireSpriteHeight)) / fireSpriteHeight;

                        fireGridXIndex -= 1;
                        fireGridYIndex -= 1;

                        Game1.fireGrid[fireGridXIndex, fireGridYIndex] = 0;
                        Game1.fireNum--;
                    }
                    if (status.Type == Game1.EntityType.Lightning)
                    {
                        Game1.cloudNum--;
                    }

                    DestroyEntity(entity);
                    Game1._world.DestroyEntity(entity);   // Removes entity from the world
                    entity_destroyed = true;
                }
            }

            return entity_destroyed;
        }

        public async void DestroyEntity(Entity entity)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    Game1.myGamePage.DestroyDisaster(entity.Id);
                }
                );
        }
    }
}

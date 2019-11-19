using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace GodsOfCalamityBeta.Systems
{
    class DamageSystem : MonoGame.Extended.Entities.Systems.EntityUpdateSystem
    {
        private int damagevalue = 10;
        private int fireDamageValue = 1;
        private int ElapsedTime = 0;
        private int firedamagetick { get; set; }
        Entity currentEntity;
        private Entity VillageEntity;
        public DamageSystem(int newdamagetick) : base(Aspect.All(typeof(ECSComponents.CollisionBoxComponet)))
        {
            firedamagetick = newdamagetick;
        }

        public override void Update(GameTime gameTime)
        {
            VillageEntity = GetEntity(Game1.VillageID);
            if (VillageEntity != null && !VillageEntity.Get<ECSComponents.StatusComponent>().IsDestroyed)
            {
                var Villagebox = VillageEntity.Get<ECSComponents.CollisionBoxComponet>();
                foreach (var entity in ActiveEntities)
                {
                    if (Villagebox != null)
                    {
                        currentEntity = GetEntity(entity);
                        if (currentEntity.Id == VillageEntity.Id)
                        {
                            continue;
                        }
                        var entitybox = currentEntity.Get<ECSComponents.CollisionBoxComponet>();
                        if (entitybox.HitBox.Intersects(Villagebox.HitBox) && currentEntity.Get<ECSComponents.StatusComponent>().IsDestroyed == false)
                        {
                            if (currentEntity.Get<ECSComponents.StatusComponent>().Type == Game1.EntityType.Fire && !VillageEntity.Get<ECSComponents.StatusComponent>().IsDestroyed)
                            {
                                if (ElapsedTime == firedamagetick)
                                {
                                    ElapsedTime = 0;
                                    VillageEntity.Get<ECSComponents.HealthComponent>().Health -= fireDamageValue;
                                    if (VillageEntity.Get<ECSComponents.HealthComponent>().Health <= 0)
                                    {
                                        VillageEntity.Get<ECSComponents.StatusComponent>().IsDestroyed = true;
                                        CallGameOverAsync();
                                        Game1.isPaused = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    ElapsedTime++;
                                }
                            }
                            else if (currentEntity.Get<ECSComponents.StatusComponent>().Type == Game1.EntityType.Meteor && !VillageEntity.Get<ECSComponents.StatusComponent>().IsDestroyed)
                            {
                                currentEntity.Get<ECSComponents.StatusComponent>().IsDestroyed = true;
                                VillageEntity.Get<ECSComponents.HealthComponent>().Health -= damagevalue;
                                if (VillageEntity.Get<ECSComponents.HealthComponent>().Health <= 0)
                                {
                                    VillageEntity.Get<ECSComponents.StatusComponent>().IsDestroyed = true;
                                    CallGameOverAsync();
                                    Game1.isPaused = true;
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                }
            }
        }

        private async void CallGameOverAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Game1.myGamePage.GameOver();
            }
            );
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            
        }
    }
}

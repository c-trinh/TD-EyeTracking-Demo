using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace GodsOfCalamityBeta.Systems
{
    class UpdateFrontEndSystem : MonoGame.Extended.Entities.Systems.EntityUpdateSystem
    {
        public UpdateFrontEndSystem(AspectBuilder aspect) : base(aspect)
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            
        }

        public async override void Update(GameTime gameTime)
        {
           foreach (var entityID in ActiveEntities)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            Game1.myGamePage.UpdateFrontEnd(entityID);
                        }
                        );
            }
        }
    }
}

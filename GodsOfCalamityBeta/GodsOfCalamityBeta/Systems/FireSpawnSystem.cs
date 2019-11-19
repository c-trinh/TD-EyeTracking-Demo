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
using GodsOfCalamityBeta.ECSComponents;

namespace GodsOfCalamityBeta.Systems
{
    class FireSpawnSystem : MonoGame.Extended.Entities.Systems.EntityUpdateSystem      // FireSpawnSystem needs to be separate from spawn so it can have access to ActiveEntities
    {
        private int ElapsedTime { get; set; }
        private int fireSpawnInterval = 480;
        private int counter = 0;
        private int phaseLength = 1800;
        private int maxFires = 30;

        public FireSpawnSystem(AspectBuilder aspect) : base(aspect)
        {
            ElapsedTime = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (counter == phaseLength)
            {
                // change spawn rate
                counter = 0;
                if (fireSpawnInterval > 200)
                {
                    fireSpawnInterval -= 80;
                }
                else
                {
                    fireSpawnInterval = 480;
                    if (phaseLength > 200)
                    {
                        phaseLength -= 200;
                    }
                }
            }
            else
            {
                counter++;
            }

            if (ElapsedTime >= fireSpawnInterval)
            {
                foreach (int entityID in ActiveEntities)
                {
                    MonoGame.Extended.Entities.Entity placeholder = Game1._world.GetEntity(entityID);
                    ECSComponents.StatusComponent capturedStatus = placeholder.Get<ECSComponents.StatusComponent>();    // get the status component of the entity
                    if (capturedStatus.Type == Game1.EntityType.Lightning)    // if the entity is a Lightining Disaster
                    {
                        // call SpawnFire Here
                        ECSComponents.PositionComponent capturedPosition = placeholder.Get<ECSComponents.PositionComponent>();
                        int spawnX = (int)capturedPosition.XCoor;
                        int spawnY = (int)capturedPosition.YCoor;
                        SpawnFire(spawnX, spawnY);
                    }

                    else if (capturedStatus.Type == Game1.EntityType.Fire)      // this is where Propagate will be called
                    {
                        Propagate(entityID);
                    }

                }

                ElapsedTime = 0;
                return;
            }

            else
            {
                ElapsedTime++;
                return;
            }
        }

        public async void SpawnFire(int initialX, int InitialY)
        {
            if (Game1.fireNum <= maxFires)
            {
                // PositionComponent
                ECSComponents.PositionComponent newPos = CalculateFireGridPos(initialX, InitialY);
                // SpriteComponent
                ECSComponents.SpriteComponent newSprite = new ECSComponents.SpriteComponent(Game1.SpriteDict["fire_1"], 1);
                // StatusComponent
                ECSComponents.StatusComponent newStatus = new ECSComponents.StatusComponent(false, Game1.EntityType.Fire);
                //hitboxComponet
                ECSComponents.CollisionBoxComponet newHitBox = new ECSComponents.CollisionBoxComponet(newSprite.Sprite, newPos);

                if (DetermineSpawnLegality(newPos))     // if this position isn't already occupied by a fireSprite
                {
                    Entity newDisaster = Game1._world.CreateEntity();   // create the entity
                    int newID = newDisaster.Id;     // track the new entity's id

                    newDisaster = Game1._world.GetEntity(newID);    // spawn the new fire entity
                                                                    // attach components
                    newDisaster.Attach(newPos);
                    newDisaster.Attach(newSprite);
                    newDisaster.Attach(newStatus);
                    newDisaster.Attach(newHitBox);

                    Game1.fireNum++;

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        Game1.myGamePage.CreateNewFire(newID);
                    });

                    return;
                }
                else
                {
                    return;     // fire already exists in this position so do not spawn it
                }
            }

            else
            {
                return;
            }
            
        }

        public override void Initialize(IComponentMapperService mapperService)      // not implemented
        {
            
        }

        public PositionComponent CalculateFireGridPos(int initialX, int initialY)   // determines the coordinates for a new fire entity based on the fireGrid. Then returns a position components with the appropriate coordinates
        {
            // The following code determines the coordinates for the new position component
            int fireSpriteWidth = Game1.SpriteDict["fire_1"].texture.Width;   // these values can change based on scaling
            int fireSpriteHeight = Game1.SpriteDict["fire_1"].texture.Height;

            // dividing these following numbers by the dimensions of fireSprite will yield the fireGrid indicies of the position
            // to determine the position for the position component, half of the fireSprite's height and width must be added so that the fire is spawned at the center of the "grid space"
            int potentialX = (initialX - (initialX % fireSpriteWidth)) + (fireSpriteWidth / 2);     
            int potentialY = (initialY - (initialY % fireSpriteHeight)) + (fireSpriteHeight / 2);

            PositionComponent newPos = new PositionComponent(potentialX, potentialY, 0);
            return newPos;
        }

        public bool DetermineSpawnLegality(PositionComponent potentialPosition)
        {
            // The following code determines the coordinates for the new position component
            int fireSpriteWidth = Game1.SpriteDict["fire_1"].texture.Width;   // these values can change based on scaling
            int fireSpriteHeight = Game1.SpriteDict["fire_1"].texture.Height;

            int fireGridXIndex = ((int)potentialPosition.XCoor - ((int)potentialPosition.XCoor % fireSpriteWidth)) / fireSpriteWidth;
            int fireGridYIndex = ((int)potentialPosition.YCoor - ((int)potentialPosition.YCoor % fireSpriteHeight)) / fireSpriteHeight;

            fireGridXIndex -= 1;    // necessary to keep within bounds of fireGrid because indexing starts at 0
            fireGridYIndex -= 1;    // necessary to keep within bounds of fireGrid because indexing starts at 0

            if (fireGridXIndex > Game1.fireGridMaxX || fireGridYIndex > Game1.fireGridMaxY || fireGridXIndex < 0 || fireGridYIndex < 0 )   // prevents indexing errors
            {
                return false;
            }

            if (Game1.fireGrid[fireGridXIndex, fireGridYIndex] == 0)        // The fire grid will have to be updated when a fire entity is destroyed
            {
                Game1.fireGrid[fireGridXIndex, fireGridYIndex] = 1;  // set this position to be occupied by the new fire entity, this is represented by a 1
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Propagate(int fireID)
        {
            // Get the entity positions
            Entity currentFire = Game1._world.GetEntity(fireID);
            PositionComponent currentPosition = currentFire.Get<PositionComponent>();

            // determine a random position to propagte in
            Random rand = new Random();
            switch (rand.Next(1, 5))
            {
                case 1:     // propagate north
                    SpawnFire((int)currentPosition.XCoor, (int)currentPosition.YCoor - 128);
                    return;
                case 2:     // propagate east
                    SpawnFire((int)currentPosition.XCoor + 128, (int)currentPosition.YCoor);
                    return;
                case 3:     // propagate south
                    SpawnFire((int)currentPosition.XCoor, (int)currentPosition.YCoor + 128);
                    return;
                case 4:     // propagate west
                    SpawnFire((int)currentPosition.XCoor - 128, (int)currentPosition.YCoor);
                    return;
                default:
                    return;
            }
        }
    }
}

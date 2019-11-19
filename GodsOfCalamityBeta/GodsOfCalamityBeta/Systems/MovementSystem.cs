/* Author:  Cong Trinh
 * File:    MovementSystem.cs
 * Desc:    MovementSystem will handle entity movements as well as calculating specific paths for certain entities.
 */

using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using GodsOfCalamityBeta.ECSComponents;
using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

namespace GodsOfCalamityBeta.Systems
{
    class MovementSystem : EntityUpdateSystem
    {
        public float W_WIDTH { get; set; }    // Window Width
        public float W_HEIGHT { get; set; }   // Window Height
        public float W_WIDTH_CENTER { get; set; }
        public float W_HEIGHT_CENTER { get; set; }
        double delta_time;

        enum Delta_Direction { Horizontal, Vertical };

        ComponentMapper _transformMapper;
        ComponentMapper _spriteMapper;

        public MovementSystem(float W_WIDTH, float W_HEIGHT) : base(Aspect.All(typeof(PositionComponent), typeof(DeltaComponent), typeof(StatusComponent)))
        {
            this.W_WIDTH = W_WIDTH;
            this.W_HEIGHT = W_HEIGHT;
            this.W_WIDTH_CENTER = W_WIDTH / 2;
            this.W_HEIGHT_CENTER = W_HEIGHT / 2;

            ///Print initial window sizes to debug console for position testing.
            Debug.WriteLine("Initialized MovementSystem()");
            Debug.WriteLine("W_HEIGHT:\t" + W_WIDTH.ToString());
            Debug.WriteLine("W_HEIGHT:\t" + W_HEIGHT.ToString());
            Debug.WriteLine("W_WIDTH_CENTER: \t" + W_WIDTH_CENTER.ToString());
            Debug.WriteLine("W_HEIGHT_CENTER:\t" + W_HEIGHT_CENTER.ToString());
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
        }

        public override void Update(GameTime currentTime)
        {
            delta_time = currentTime.ElapsedGameTime.TotalSeconds;

            foreach (var entity_id in ActiveEntities)
            {
                var entity = GetEntity(entity_id);
                MoveEntity(entity, CalculatePath(entity));
            }
        }

        private PositionComponent MoveEntity(Entity entity, float[] entity_direction)
        {
            var entity_position = entity.Get<PositionComponent>();

            /// Sets entity's deltas based on calculated path
            entity_position.XCoor += entity_direction[(int)Delta_Direction.Horizontal];     //Horizontal
            entity_position.YCoor += entity_direction[(int)Delta_Direction.Vertical];       //Vertical

            //here, collisionboxupdate must happen
            entity.Get<CollisionBoxComponet>().UpdateHitboxPosition(entity.Get<SpriteComponent>().Sprite, entity.Get<PositionComponent>());

            return entity_position;
        }

        private float[] CalculatePath(Entity entity)
        {   /// Will calculate how the entity will move
            float[] entity_direction = new float[] { 0, 0 };  //Will decide which directions the entity will move
            DeltaComponent entity_delta = entity.Get<DeltaComponent>();
            StatusComponent entity_status = entity.Get<StatusComponent>();

            /// Computes path for entity based on type of entity
            switch (entity_status.Type) {
                case (Game1.EntityType.Lightning):
                    ComputeLightningDelta(entity);
                    break;
                case (Game1.EntityType.Meteor):
                    ComputeMeteorDelta(entity);
                    break;
                case (Game1.EntityType.Fire):
                    break;
            }

            entity_direction[(int)Delta_Direction.Horizontal] = entity_delta.XDelta;
            entity_direction[(int)Delta_Direction.Vertical] = entity_delta.YDelta;

            return entity_direction;
        }

        private void ComputeLightningDelta(Entity entity)
        {   /// Calculate path for lighningh to move in circular formation
            DeltaComponent entity_delta = entity.Get<DeltaComponent>();
            PositionComponent entity_position = entity.Get<PositionComponent>();

            float new_delta_X = 2, new_delta_Y = 1;

            if (entity_position.YCoor > W_HEIGHT_CENTER)
                new_delta_X *= -1;    // Changes horizontal direction
            if (entity_position.XCoor < W_WIDTH_CENTER)
                new_delta_Y *= -1;    // Changes vertical direction
            
            entity_delta.XDelta = new_delta_X;
            entity_delta.YDelta = new_delta_Y;

            entity.Attach(entity_delta);    // Replaces current delta component of lightning entity
        }

        private void ComputeMeteorDelta(Entity entity)
        {   /// Calculate path for meteor to move to village
            DeltaComponent entity_delta = entity.Get<DeltaComponent>();
            PositionComponent entity_position = entity.Get<PositionComponent>();

            /// Compute position from Meteor point to Village point
            //PositionComponent village_position = new PositionComponent(W_WIDTH_CENTER, W_HEIGHT_CENTER, 0);
            //int compute_X = Math.Abs(entity_position.XCoor - village_position.XCoor);
            //int compute_Y = Math.Abs(entity_position.YCoor - village_position.YCoor);
            //int gcd = GCD(compute_X, compute_Y);

            float new_delta_x = 4, new_delta_y = 2;   //Temporarily hard-coded

            if (entity_position.XCoor > W_WIDTH_CENTER)
                new_delta_x *= -1;    // Changes vertical direction
            if (entity_position.YCoor > W_HEIGHT_CENTER)
                new_delta_y *= -1;    // Changes horizontal direction

            entity_delta.XDelta = new_delta_x;
            entity_delta.YDelta = new_delta_y;

            entity.Attach(entity_delta);
        }
    }
}

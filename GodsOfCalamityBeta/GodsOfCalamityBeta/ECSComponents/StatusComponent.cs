using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsOfCalamityBeta.ECSComponents
{
    class StatusComponent
    {
        public Game1.EntityType Type { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsGazed { get; set; }
        public int CurrentAnimSprite { get; set; }
        public int ElapsedAnimFrames { get; set; }
        public int ElapsedDestructAnimFrames { get; set; }
        public bool IsReadyForCleanup { get; set; }
        public int FireBurnInterval { get; private set; }
        public int ElapsedBurnFrames { get; set; }

        /// Add any game-statuses you want for entities in here

        StatusComponent()
        {
            IsDestroyed = false;
        }

        public StatusComponent(bool isDestroyed, Game1.EntityType Type )
        {
            this.IsDestroyed = isDestroyed;
            this.IsGazed = false;
            this.CurrentAnimSprite = 0;
            this.ElapsedDestructAnimFrames = 0;
            this.Type = Type;
            this.FireBurnInterval = 480;
            this.ElapsedBurnFrames = 0;
        }
    }
}

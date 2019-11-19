// HealthComponent will likely only be used by the village entity

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsOfCalamityBeta.ECSComponents
{
    class HealthComponent
    {
        public int Health { get; set; }

        public HealthComponent(int initialHealth)
        {
            Health = initialHealth;
        }

        // No need for member methods because "Health" is public
        // This way the health component is only manipulated by the relevant system
    }
}

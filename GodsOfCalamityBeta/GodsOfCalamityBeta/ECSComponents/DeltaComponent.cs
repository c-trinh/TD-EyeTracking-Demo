using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsOfCalamityBeta.ECSComponents
{
    class DeltaComponent
    {
        public float XDelta { get; set; }
        public float YDelta { get; set; }
        public float AngleDelta { get; set; }

        DeltaComponent()
        { }

        public DeltaComponent(float initialXDelta, float initialYDelta, float initialAngleDelta)
        {
            XDelta = initialXDelta;
            YDelta = initialYDelta;
            AngleDelta = initialAngleDelta;
        }

        // members are public facing so that they may be manipulated by systems
    }
}

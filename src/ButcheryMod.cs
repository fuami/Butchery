using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace butchery.src
{
    class ButcheryMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.RegisterItemClass("butchery:ItemCorpse", typeof(ItemCorpse));
            api.RegisterBlockClass("butchery:BlockHook", typeof(BlockHook));
            api.RegisterEntityBehaviorClass("butchery:corpseable", typeof(BehaviorCorpseable));
        }

    }
}

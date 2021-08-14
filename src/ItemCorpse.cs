using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace butchery.src
{
    class ItemCorpse : Item
    {
        public override void OnHeldDropped(IWorldAccessor world, IPlayer byPlayer, ItemSlot slot, int quantity, ref EnumHandling handling)
        {
            if (world.Side == EnumAppSide.Server)
            {
                ItemStack stack = slot.Itemstack;

                EntityProperties bodyType = world.GetEntityType(new AssetLocation(stack.Attributes["Code"].ToString()));
                Entity childEntity = world.ClassRegistry.CreateEntity(bodyType);

                Entity player = byPlayer.Entity;
                childEntity.ServerPos.SetFrom(player.ServerPos);
                childEntity.ServerPos.Motion.X += (world.Rand.NextDouble() - 0.5f) / 20f;
                childEntity.ServerPos.Motion.Z += (world.Rand.NextDouble() - 0.5f) / 20f;

                childEntity.Pos.SetFrom(childEntity.ServerPos);
                world.SpawnEntity(childEntity);
                childEntity.Attributes = stack.Attributes["Attributes"];
                childEntity.WatchedAttributes = stack.Attributes["WatchedAttributes"];
            }

            slot.Itemstack = null;
            slot.MarkDirty();
            handling = EnumHandling.PreventDefault;
        }

    }
}

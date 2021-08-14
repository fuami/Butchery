using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace butchery.src
{
    class BehaviorCorpseable : EntityBehavior
    {
        protected JsonItemStack drop;

        public BehaviorCorpseable(Entity entity) : base(entity)
        {
        }

        public override void Initialize(EntityProperties properties, JsonObject typeAttributes)
        {
            base.Initialize(properties, typeAttributes);

            if (entity.World.Side == EnumAppSide.Server)
            {
                drop = typeAttributes["drop"].AsObject<JsonItemStack>();
            }
        }

        public override void OnInteract(EntityAgent byEntity, ItemSlot itemslot, Vec3d hitPosition, EnumInteractMode mode, ref EnumHandling handled)
        {
            bool inRange = (byEntity.World.Side == EnumAppSide.Client && byEntity.Pos.SquareDistanceTo(entity.Pos) <= 5) || (byEntity.World.Side == EnumAppSide.Server && byEntity.Pos.SquareDistanceTo(entity.Pos) <= 14);

            if (!Corpseable || !inRange)
            {
                return;
            }

            if (entity.World.Side == EnumAppSide.Server)
            {
                if (drop.Resolve(byEntity.World, "BehaviorCorpseable"))
                {
                    ItemStack bodyStack = drop.ResolvedItemstack.Clone();

                    bodyStack.Attributes.SetString("Code",entity.Code.ToString());
                    bodyStack.Attributes["Attributes"] = entity.Attributes;
                    bodyStack.Attributes["WatchedAttributes"] = entity.WatchedAttributes;

                    EntityPlayer entityplr = byEntity as EntityPlayer;
                    IPlayer player = entity.World.PlayerByUid(entityplr.PlayerUID);
                    if (player.InventoryManager.TryGiveItemstack(bodyStack))
                    {
                        entity.Die(EnumDespawnReason.PickedUp);
                    }
                }
            }

        }

        WorldInteraction[] interactions = null;

        public override WorldInteraction[] GetInteractionHelp(IClientWorldAccessor world, EntitySelection es, IClientPlayer player, ref EnumHandling handled)
        {
            interactions = ObjectCacheUtil.GetOrCreate(world.Api, "corpseableEntityInteractions", () =>
            {
                return new WorldInteraction[] {
                    new WorldInteraction()
                    {
                        ActionLangCode = "blockhelp-creature-corpseable",
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "sneak"
                    }
                };
            });

            return !entity.Alive && Corpseable ? interactions : null;
        }

        public override string PropertyName()
        {
            return "butchery:corpseable";
        }

        public bool IsHarvested
        {
            get
            {
                return entity.WatchedAttributes.GetBool("harvested", false);
            }
        }

        public bool Corpseable
        {
            get
            {
                return !entity.Alive && !IsHarvested;
            }
        }


    }
}

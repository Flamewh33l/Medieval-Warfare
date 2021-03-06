﻿using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.MW.Traits
{
    public class DirectSpawnModifierInfo : ConditionalTraitInfo
    {
        [Desc("Number in ticks wich reduce spawntime.")]
        public readonly int Ticks = 25;

        public override object Create(ActorInitializer init) { return new DirectSpawnModifier(init.Self, this); }
    }

    public class DirectSpawnModifier : ConditionalTrait<DirectSpawnModifierInfo>, INotifyCreated, INotifyRemovedFromWorld
    {
        PlayerCivilization playerCiv;
        bool enabled = false;

        public DirectSpawnModifier(Actor self, DirectSpawnModifierInfo info) : base(info)
        {
            playerCiv = self.Owner.PlayerActor.Trait<PlayerCivilization>();
        }

        protected override void Created(Actor self)
        {
            if (!self.Owner.NonCombatant && self.Owner.WinState != WinState.Lost && self.Owner.PlayerActor.Info.HasTraitInfo<PlayerCivilizationInfo>())
            {
                if (!IsTraitDisabled && !enabled)
                {
                    playerCiv.DirectModifier += Info.Ticks;
                    enabled = true;
                }
            }
        }

        void INotifyRemovedFromWorld.RemovedFromWorld(Actor self)
        {
            if (!self.Owner.NonCombatant && self.Owner.WinState != WinState.Lost && self.Owner.PlayerActor.Info.HasTraitInfo<PlayerCivilizationInfo>())
            {
                if (!IsTraitDisabled && enabled)
                {
                    playerCiv.DirectModifier -= Info.Ticks;
                    enabled = false;
                }
            }
        }

        protected override void TraitEnabled(Actor self)
        {
            if (!IsTraitDisabled && !enabled)
            {
                playerCiv.DirectModifier += Info.Ticks;
                enabled = true;
            }
        }

        protected override void TraitDisabled(Actor self)
        {
            if (!IsTraitDisabled && enabled)
            {
                playerCiv.DirectModifier -= Info.Ticks;
                enabled = false;
            }
        }
    }
}

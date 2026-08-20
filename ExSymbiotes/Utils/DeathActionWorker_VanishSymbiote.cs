using ExSymbiotes.Utils;
using RimWorld;
using Verse.AI.Group;
using Verse;

namespace ExSymbiotes
{
    public class DeathActionWorker_VanishSymbiote : DeathActionWorker
    {
        public DeathActionProperties_Vanish Props => (DeathActionProperties_Vanish)this.props;

        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            if (this.Props.fleck != null)
                FleckMaker.Static(corpse.PositionHeld, corpse.MapHeld, this.Props.fleck);
            if (this.Props.filth != null)
            {
                int randomInRange = this.Props.filthCountRange.RandomInRange;
                for (int index = 0; index < randomInRange; ++index)
                    FilthMaker.TryMakeFilth(corpse.PositionHeld, corpse.MapHeld, this.Props.filth);
            }

            if (this.Props.meatExplosionSize.HasValue && ModsConfig.AnomalyActive)
                SymbioteUtility.MeatSplatter(0, corpse.PositionHeld, corpse.MapHeld,
                    this.Props.meatExplosionSize.Value);
            corpse.Destroy(DestroyMode.Vanish);
        }
    }
}
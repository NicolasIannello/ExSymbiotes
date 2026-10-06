using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class HediffComp_SymbioteThrall: HediffComp_SymbioteControl
    {
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            this.Pawn.SetFaction(Faction.OfPlayer);
            this.reproduce = false;
            this.thrall = true;
            color = red;
            ConditionalWeakTableRemove(this.Pawn);
            ConditionalWeakTableAdd(this.Pawn);
        }
        
        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if(this.parent.Severity>0.50)
            {
                if(!this.parent.pawn.Dead) this.parent.pawn.Kill(null, this.parent);
                if (this.parent.Severity > 0.75)
                {
                    CompRottable rotComp = this.parent.pawn.Corpse.GetComp<CompRottable>();
                    rotComp.RotImmediately();
                    if (this.parent.Severity >= 1)
                        rotComp.RotProgress = rotComp.PropsRot.TicksToDessicated;
                }
            }
            FilthMaker.TryMakeFilth(this.parent.pawn.PositionHeld, this.parent.pawn.MapHeld, ThingDefOf.Filth_RevenantBloodPool);
        }
    }
}
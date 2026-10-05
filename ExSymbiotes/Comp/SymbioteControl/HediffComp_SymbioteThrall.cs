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
            color = blue;
            ConditionalWeakTableRemove(this.Pawn);
            ConditionalWeakTableAdd(this.Pawn);
        }
    }
}
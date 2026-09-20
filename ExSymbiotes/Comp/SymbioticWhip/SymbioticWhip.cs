using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class SymbioticWhip : Bullet
    {

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Pawn pawn = intendedTarget.Pawn;
            if (hitThing == null && (pawn.Downed || pawn.Faction == Faction.OfPlayer)) hitThing = pawn;
            base.Impact(hitThing, blockedByShield);
        }
        
    }
}
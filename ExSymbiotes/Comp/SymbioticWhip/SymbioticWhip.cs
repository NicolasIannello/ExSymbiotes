using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class SymbioticWhip : Bullet
    {

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = this.Map;
            Pawn pawn = intendedTarget.Pawn;
            if (hitThing == null && (pawn.Downed || pawn.Faction == Faction.OfPlayer)) hitThing = pawn;
            base.Impact(hitThing, blockedByShield);
            FleckMaker.ConnectingLine(this.Launcher.DrawPos, pawn.DrawPos, ExSymbiotesDefOf.ExSymbiotes_PsycastPsychicLineBack, map);
        }
        
    }
}
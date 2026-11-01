using RimWorld;

namespace ExSymbiotes
{
    public class Thought_Hivemind : Thought_Situational
    {
        public override float MoodOffset()
        {
            HediffWithComps_Hivemind hivemind = (HediffWithComps_Hivemind)pawn.health.hediffSet.GetFirstHediffOfDef(this.def.hediff);
            if (hivemind == null) return 0f;
            
            int controlledPawns = hivemind.controlledPawns.Count + 1;
            return controlledPawns*BaseMoodOffset;
        }
    }
}
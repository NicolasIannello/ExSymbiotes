using RimWorld;

namespace ExSymbiotes
{
    public class Thought_Hivemind : Thought_Situational
    {
        public override float MoodOffset()
        {
            int controlledPawns = ((HediffWithComps_Hivemind)pawn.health.hediffSet.GetFirstHediffOfDef(this.def.hediff)).controlledPawns.Count + 1;
            return controlledPawns*BaseMoodOffset;
        }
    }
}
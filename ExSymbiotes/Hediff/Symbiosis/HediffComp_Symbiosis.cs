using Verse;

namespace ExSymbiotes
{
    public class HediffComp_Symbiosis: HediffComp
    {
        private HediffCompProperties_Symbiosis Props => (HediffCompProperties_Symbiosis) this.props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            Pawn.story.skinColorOverride = Pawn.story.SkinColor;
        }

        public override void CompPostPostRemoved()
        {
            Pawn.story.skinColorOverride = null;
        }
    }
}
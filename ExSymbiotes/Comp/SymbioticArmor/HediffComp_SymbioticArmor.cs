using Verse;

namespace ExSymbiotes
{
    public class HediffComp_SymbioticArmor: HediffComp_SymbioteBase
    {
        private HediffCompProperties_SymbioticArmor Props => (HediffCompProperties_SymbioticArmor) this.props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            color = blue;
            ConditionalWeakTableRemove(this.Pawn);
            ConditionalWeakTableAdd(this.Pawn);
        }

        public override void CompPostPostRemoved()
        {
            ConditionalWeakTableRemove(this.Pawn);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit && this.Pawn != null) ConditionalWeakTableAdd(this.Pawn);
        }
    }
}
using System.Runtime.CompilerServices;
using Verse;

namespace ExSymbiotes
{
    public class HediffComp_SymbioticArmor: HediffComp
    {
        private HediffCompProperties_SymbioticArmor Props => (HediffCompProperties_SymbioticArmor) this.props;
        public static readonly ConditionalWeakTable<Pawn, HediffComp_SymbioticArmor> SymbioticArmorWeakTable = new ConditionalWeakTable<Pawn, HediffComp_SymbioticArmor>();

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            SymbioticArmorWeakTable.Remove(this.Pawn);
            SymbioticArmorWeakTable.Add(this.Pawn, this);
        }

        public override void CompPostPostRemoved()
        {
            SymbioticArmorWeakTable.Remove(this.Pawn);
        }

        public override void CompExposeData()
        {
            if (Scribe.mode == LoadSaveMode.PostLoadInit && this.Pawn != null) SymbioticArmorWeakTable.Add(this.Pawn, this);
        }
    }
}
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompProperties_AbilitySymbiosisCost : CompProperties_AbilityEffect
    {
        public int symbiosisCost;

        public CompProperties_AbilitySymbiosisCost()
        {
            this.compClass = typeof (CompAbilityEffect_SymbiosisCost);
        }

        public override IEnumerable<string> ExtraStatSummary()
        {
            yield return (string) ("ExSymbiotes.AbilitySymbiosisCost".Translate() + ": ") + this.symbiosisCost;
        }
    }
}
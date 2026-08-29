using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompUseEffect_AddSymbiosis : CompUseEffect
    {
        public CompProperties_UseEffectAddSymbiosis Props => (CompProperties_UseEffectAddSymbiosis) this.props;

        public override void DoEffect(Pawn user) => user.health.AddHediff(this.Props.hediffDef);

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            return !this.Props.allowRepeatedUse && SymbioteUtility.HasSymbiosis(p)!=null ? (AcceptanceReport) "AlreadyHasHediff".Translate((NamedArgument) "ExSymbiotes.Symbiosis".Translate()) : (AcceptanceReport) true;
        }
    }
}
using System;
using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompUseEffect_GrendelGainAbility : CompUseEffect
    {
        public CompProperties_UseEffect_GrendelGainAbility Props
        {
            get => (CompProperties_UseEffect_GrendelGainAbility) this.props;
        }
        
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            user.abilities.GainAbility(this.Props.ability);
            SymbioteUtility.HasSymbiosis(user, false).TryGetComp<HediffComp_Symbiosis>().EnergyMax += 10;
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (SymbioteUtility.HasSymbiosis(p, false)==null)
                return (AcceptanceReport) "ExSymbiotes.AbilityDisabledNoSymbiosisHediff".Translate(p);
            return p.abilities != null && p.abilities.abilities.Any<Ability>((Predicate<Ability>) (a => a.def == this.Props.ability)) ? (AcceptanceReport) "ExSymbiotes.AbilityAlreadyLearned".Translate(p.Named("USER"), (NamedArgument) this.Props.ability.LabelCap) : base.CanBeUsedBy(p);
        }
    }
}
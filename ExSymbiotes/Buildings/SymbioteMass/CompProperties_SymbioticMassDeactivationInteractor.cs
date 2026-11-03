using RimWorld;

namespace ExSymbiotes
{
    public class CompProperties_SymbioticMassDeactivationInteractor : CompProperties_Interactable
    {
        public int shardsRequired = 2;

        public CompProperties_SymbioticMassDeactivationInteractor()
        {
            this.compClass = typeof (CompSymbioticMassDeactivationInteractor);
        }
    }
}
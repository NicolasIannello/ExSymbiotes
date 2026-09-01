using Verse;

namespace ExSymbiotes
{
    public class HediffCompProperties_SymbioticTendrils : HediffCompProperties
    {
        public ThingDef turretGunDef;
        public float cd;
        
        public HediffCompProperties_SymbioticTendrils() => this.compClass = typeof (HediffComp_SymbioticTendrils);
    }
}
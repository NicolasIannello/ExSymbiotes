using Verse;

namespace ExSymbiotes
{
    public class HediffCompProperties_SymbioticCannon : HediffCompProperties
    {
        public ThingDef turretGunDef;
        
        public HediffCompProperties_SymbioticCannon() => this.compClass = typeof (HediffComp_SymbioticCannon);
    }
}
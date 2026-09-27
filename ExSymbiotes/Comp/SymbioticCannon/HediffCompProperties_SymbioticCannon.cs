using Verse;

namespace ExSymbiotes
{
    public class HediffCompProperties_SymbioticCannon : HediffCompProperties
    {
        public ThingDef turretGunDef;
        public float cd;
        
        public HediffCompProperties_SymbioticCannon() => this.compClass = typeof (HediffComp_SymbioticCannon);
    }
}
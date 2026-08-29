using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class CompProperties_UseEffectAddSymbiosis : CompProperties_UseEffect
    {
        public HediffDef hediffDef;
        public bool allowRepeatedUse;

        public CompProperties_UseEffectAddSymbiosis() => this.compClass = typeof (CompUseEffect_AddSymbiosis);
    }
}
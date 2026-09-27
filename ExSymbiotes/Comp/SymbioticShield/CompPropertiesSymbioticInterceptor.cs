using RimWorld;

namespace ExSymbiotes
{
    public class CompPropertiesSymbioticInterceptor : CompProperties_ProjectileInterceptor
    {
        public CompPropertiesSymbioticInterceptor()
        {
            this.compClass = typeof (CompSymbioticInterceptor);
        }
    }
}
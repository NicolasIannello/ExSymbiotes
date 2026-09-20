using System.Collections.Generic;
using Verse;

namespace ExSymbiotes
{
    public class CompProperties_ProjectileFleck : CompProperties
    {
        public FleckDef fleckDef;

        public CompProperties_ProjectileFleck() => this.compClass = typeof (Comp_ProjectileFleck);

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            if (!typeof (Projectile).IsAssignableFrom(parentDef.thingClass))
                yield return this.GetType().Name + " is only meant to be used on Projectile derived Things";
        }
    }
}
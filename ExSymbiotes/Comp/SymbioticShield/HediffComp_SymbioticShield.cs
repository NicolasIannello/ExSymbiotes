using System.Collections.Generic;
using Verse;

namespace ExSymbiotes
{
    public class HediffComp_SymbioticShield: HediffComp
    {
        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (!Find.Selector.IsSelected((object) this.Pawn))
                yield return (Gizmo) null;
            else if (this.Pawn.Spawned)
            {
                List<Thing> thingList = this.Pawn.Position.GetThingList(this.Pawn.Map);
                for (int index = 0; index < thingList.Count; ++index)
                {
                    if (thingList[index] is MechShield activeMechShield && activeMechShield.IsTargeting((Thing)this.Pawn))
                    {
                        yield return (Gizmo) new Gizmo_Symbiotic_Interceptor()
                        {
                            interceptor = activeMechShield.TryGetComp<CompSymbioticInterceptor>()
                        };
                    }
                }
            }
        }
    }
}
using Verse;

namespace ExSymbiotes
{
    public class CompSymbioticWeapon : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.parent.Destroy();
        }
    }
}
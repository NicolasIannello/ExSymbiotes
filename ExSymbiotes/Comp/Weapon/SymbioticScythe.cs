using Verse;

namespace ExSymbiotes
{
    public class SymbioticScythe : ThingWithComps
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            if (!this.Destroyed || this.Spawned)
                this.Destroy();
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            if (!this.Destroyed && !this.Spawned)
                this.Destroy();
        }
    }
}
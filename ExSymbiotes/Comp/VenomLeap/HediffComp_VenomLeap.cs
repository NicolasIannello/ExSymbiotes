using Verse;

namespace ExSymbiotes
{
    public class HediffComp_VenomLeap: HediffComp
    {
        protected bool venomLeap = false;

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<bool>(ref this.venomLeap, "venomLeap");
        }
        
        public void SetStage(bool stage) => this.venomLeap = stage;
        
        public bool GetStage() => this.venomLeap;
    }
}
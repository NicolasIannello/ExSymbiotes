using Verse;

namespace ExSymbiotes
{
    public class HediffComp_VenomLeap: HediffComp
    {
        protected bool venomLeap = false;
        protected bool venomLanding = false;
        public bool Landing => this.venomLanding;
        public bool Leap => this.venomLeap;

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<bool>(ref this.venomLeap, "venomLeap");
            Scribe_Values.Look<bool>(ref this.venomLanding, "venomLanding");
        }
        
        public void SetStage(bool stage)
        {
            if (!stage) venomLanding = false;
            this.venomLeap = stage;
        }
        
        public void SetLanding(bool stage) => this.venomLanding = stage;
    }
}
using Verse;

namespace ExSymbiotes
{
    public class HediffComp_Symbiosis: HediffComp
    {
        public HediffCompProperties_Symbiosis Props => (HediffCompProperties_Symbiosis) this.props;
        // protected int energy;
        // public int Energy => this.energy;
        // public int EnergyMax = 100;
        
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            Pawn.story.skinColorOverride = this.Props.color;
        }

        public override void CompPostPostRemoved()
        {
            Pawn.story.skinColorOverride = null;
        }
        
        // public override IEnumerable<Gizmo> CompGetGizmos()
        // {
        //     IEnumerable<Gizmo> compGetGizmos = base.CompGetGizmos();
        //     if (compGetGizmos != null) foreach (Gizmo gizmo in compGetGizmos) yield return gizmo;
        //     
        //     IEnumerable<Gizmo> getGizmos = this.GetGizmos();
        //     if (getGizmos != null) foreach (Gizmo gizmo in getGizmos) yield return gizmo;
        //
        //     if (DebugSettings.ShowDevGizmos)
        //     {
        //         Command_Action commandAction1 = new Command_Action();
        //         commandAction1.defaultLabel = "DEV: Fill";
        //         commandAction1.action = new Action(this.Fill);
        //         yield return (Gizmo) commandAction1;
        //     }
        // }
        //
        // private IEnumerable<Gizmo> GetGizmos()
        // {
        //     if ((this.Pawn.Faction == Faction.OfPlayer || this.Pawn is Pawn pawn && pawn.RaceProps.IsMechanoid) && Find.Selector.SingleSelectedThing == this.Pawn)
        //         yield return (Gizmo) new Gizmo_Symbiosis(Props.color)
        //         {
        //             shield = this
        //         };
        // }
        //
        // private void Fill()
        // {
        //     this.energy = 100;
        // }
        //
        // public override void CompExposeData()
        // {
        //     base.CompExposeData();
        //     Scribe_Values.Look<int>(ref this.energy, "energy");
        // }
    }
}
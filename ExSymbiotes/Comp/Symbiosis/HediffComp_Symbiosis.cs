using System;
using System.Collections.Generic;
using ExSymbiotes.Utils;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class HediffComp_Symbiosis: HediffComp
    {
        public HediffCompProperties_Symbiosis Props => (HediffCompProperties_Symbiosis) this.props;
        protected int energy;
        public int Energy => this.energy;
        public int EnergyMax = 20;
        private bool deathlessGene = false;
        private bool vacuumGene = false;
        private bool immunityGene = false;
        
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            Pawn.story.skinColorOverride = this.Props.color;
            if (ModsConfig.BiotechActive)
            {
                deathlessGene = SymbioteUtility.CheckAddGene(this.Pawn, GeneDefOf.Deathless);
                immunityGene = SymbioteUtility.CheckAddGene(this.Pawn, ExSymbiotesDefOf.PerfectImmunity);
                if(ModsConfig.OdysseyActive) vacuumGene = SymbioteUtility.CheckAddGene(this.Pawn, ExSymbiotesDefOf.VacuumResistance_Total);
            }
        }

        public override void CompPostPostRemoved()
        {
            Pawn.story.skinColorOverride = null;
            if (ModsConfig.BiotechActive)
            {
                SymbioteUtility.CheckRemoveGene(this.Pawn, GeneDefOf.Deathless, this.deathlessGene);
                SymbioteUtility.CheckRemoveGene(this.Pawn, ExSymbiotesDefOf.PerfectImmunity, this.immunityGene);
                if (ModsConfig.OdysseyActive) SymbioteUtility.CheckRemoveGene(this.Pawn, ExSymbiotesDefOf.VacuumResistance_Total, this.vacuumGene);
            }
        }
        
        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            IEnumerable<Gizmo> compGetGizmos = base.CompGetGizmos();
            if (compGetGizmos != null) foreach (Gizmo gizmo in compGetGizmos) yield return gizmo;
            
            IEnumerable<Gizmo> getGizmos = this.GetGizmos();
            if (getGizmos != null) foreach (Gizmo gizmo in getGizmos) yield return gizmo;
        
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action commandAction1 = new Command_Action();
                commandAction1.defaultLabel = "DEV: Fill Symbiosis";
                commandAction1.action = new Action(this.Fill);
                yield return (Gizmo) commandAction1;
            }
        }
        
        private IEnumerable<Gizmo> GetGizmos()
        {
            if ((this.Pawn.Faction == Faction.OfPlayer || this.Pawn is Pawn pawn && pawn.RaceProps.IsMechanoid) && Find.Selector.SingleSelectedThing == this.Pawn)
                yield return (Gizmo) new Gizmo_Symbiosis(Props.color)
                {
                    shield = this
                };
        }
        
        private void Fill() => this.energy = EnergyMax;
        
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.energy, "energy");
            Scribe_Values.Look<bool>(ref this.deathlessGene, "deathlessGene");
            Scribe_Values.Look<bool>(ref this.vacuumGene, "vacuumGene");
            Scribe_Values.Look<bool>(ref this.immunityGene, "immunityGene");
        }
        
        public void AddSymbiosis(int amount) => this.energy = (energy + amount)>10 ? 10 : energy + amount;
        
        public void RemoveSymbiosis(int amount) => this.energy = (energy - amount)<0 ? 0 : energy - amount;
    }
}
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class HediffWithComps_SymbioteStats : HediffWithComps
    {
        private HediffStage curStage;
        public override HediffStage CurStage
        {
            get
            {
                if (this.curStage == null)
                {
                    this.curStage = new HediffStage();
                    this.curStage.damageFactors = this.def.stages[this.CurStageIndex].damageFactors;
                    this.curStage.statOffsets = this.def.stages[this.CurStageIndex].statOffsets;
                    this.curStage.capMods = this.def.stages[this.CurStageIndex].capMods;
                    this.curStage.regeneration = this.def.stages[this.CurStageIndex].regeneration;
                    HediffComp_Stats comp = this.TryGetComp<HediffComp_Stats>();
                    if (comp != null)
                    {
                        if(comp.Props.NeedDef != null)
                            this.curStage.disablesNeeds = comp.Props.NeedDef;
                        if(comp.Props.regenValue > 0f)
                            this.curStage.regeneration = comp.Props.regenValue;

                        if (comp.Props.Stats != null)
                        {
                            for (int i = 0; i < comp.Props.Stats.Count; i++)
                            {
                                StatModifier statModifier = new StatModifier();
                                statModifier.stat = comp.Props.Stats[i].stat;
                                statModifier.value = comp.Props.Stats[i].value;

                                if (comp.Props.Stats[i].type == "statFactors" && this.curStage.statFactors == null)
                                    this.curStage.statFactors = new List<StatModifier>();
                                else if (comp.Props.Stats[i].type == "statOffsets" && this.curStage.statOffsets == null)
                                    this.curStage.statOffsets = new List<StatModifier>();

                                if (comp.Props.Stats[i].type == "statFactors")
                                    this.AddStat(this.curStage.statFactors, statModifier);
                                else if (comp.Props.Stats[i].type == "statOffsets")
                                    this.AddStat(this.curStage.statOffsets, statModifier);
                            }
                        }

                        if(comp.Props.StatDefRemoveGroup!=null)
                            for (int i = 0; i < comp.Props.StatDefRemoveGroup.Count; i++)
                                if (comp.Props.StatDefRemoveGroup[i] == "damageFactors")
                                    this.curStage.damageFactors = new List<DamageFactor>();
                    }
                }
                return this.curStage;
            }
        }

        private void AddStat(List<StatModifier> list, StatModifier stat)
        {
            bool added = false;
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].stat == stat.stat)
                {
                    list[j].value = stat.value;
                    added = true;
                }
            }
            if (!added) list.Add(stat);
        }
    }
}
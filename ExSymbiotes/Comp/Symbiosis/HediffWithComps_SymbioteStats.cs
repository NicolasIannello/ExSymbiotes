using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    this.curStage = this.def.stages[this.CurStageIndex];
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
        public override string TipStringExtra
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                HediffStage stage = this.CurStage;
                if (stage != null)
                {
                    if (!stage.capMods.NullOrEmpty())
                    {
                        stringBuilder.Append($"  - {"TabStats".Translate()}: {stage.capMods[0].offset * 100f:+#;-#}%, x{stage.capMods[0].postFactor.ToStringPercent()}");
                        stringBuilder.AppendLine();
                    }
                    if (stage.regeneration>0f && stage.showRegenerationStat)
                    {
                        stringBuilder.Append($"  - {"Stat_Hediff_Regeneration_Name".Translate()}: {"Stat_Hediff_Regeneration_Stat".Translate((NamedArgument)$"{stage.regeneration:0}")}");
                        stringBuilder.AppendLine();
                    }
                    if (stage.statOffsets != null)
                    {
                        for(int i = 0; i<stage.statOffsets.Count; i++)
                        {
                            if (stage.statOffsets[i].stat.CanShowWithLoadedMods())
                            {
                                stringBuilder.Append($"  - {stage.statOffsets[i].stat.LabelCap}: {stage.statOffsets[i].stat.Worker.ValueToString(stage.statOffsets[i].value, false, ToStringNumberSense.Offset)}");
                                stringBuilder.AppendLine();
                            }
                        }
                    }
                    if (stage.statFactors != null)
                    {
                        for (int i = 0; i < stage.statFactors.Count; i++)
                        {
                            if (stage.statFactors[i].stat.CanShowWithLoadedMods())
                            {
                                stringBuilder.Append($"  - {stage.statFactors[i].stat.LabelCap}: {stage.statFactors[i].stat.Worker.ValueToString(stage.statFactors[i].value, false, ToStringNumberSense.Factor)}");
                                stringBuilder.AppendLine();
                            }
                        }
                    }
                    if (!stage.damageFactors.NullOrEmpty<DamageFactor>())
                    {
                        for (int i = 0; i < stage.damageFactors.Count; i++)
                        {
                            stringBuilder.Append($"  - {"DamageFactorWeakness".Translate(stage.damageFactors[i].damageDef.Named("DAMAGE")).CapitalizeFirst()}: x{stage.damageFactors[i].factor.ToStringPercent()}");
                            stringBuilder.AppendLine();
                        }
                    }
                    if (stage.foodPoisoningChanceFactor != 1)
                    {
                        stringBuilder.Append($"  - {"Stat_Hediff_FoodPoisoningChanceFactor_Name".Translate()} : {stage.foodPoisoningChanceFactor.ToStringByStyle(ToStringStyle.FloatTwo, ToStringNumberSense.Factor)}");
                        stringBuilder.AppendLine();
                    }
                }
                if (ModsConfig.AnomalyActive && !this.def.aptitudes.NullOrEmpty<Aptitude>())
                    stringBuilder.AppendLine(this.def.aptitudes.Select<Aptitude, string>((Func<Aptitude, string>) (x => $"{x.skill.LabelCap.ToString()} {x.level.ToStringWithSign()}")).ToLineList("  - ", true));
                if (stage != null)
                {
                    if (!stage.enablesNeeds.NullOrEmpty<NeedDef>())
                    {
                        if (stringBuilder.Length > 0)
                            stringBuilder.AppendLine();
                        stringBuilder.AppendLine(("AddsNeeds".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
                        stringBuilder.AppendLine(stage.enablesNeeds.Select<NeedDef, string>((Func<NeedDef, string>) (x => x.LabelCap.ToString())).ToLineList("  - "));
                    }
                    if (!stage.disablesNeeds.NullOrEmpty<NeedDef>())
                    {
                        if (stringBuilder.Length > 0)
                            stringBuilder.AppendLine();
                        stringBuilder.AppendLine(("DisabledNeedsLabel".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
                        stringBuilder.AppendLine(stage.disablesNeeds.Select<NeedDef, string>((Func<NeedDef, string>) (x => x.LabelCap.ToString())).ToLineList("  - "));
                    }
                    if (stage.disabledWorkTags != WorkTags.None)
                    {
                        if (stringBuilder.Length > 0)
                            stringBuilder.AppendLine();
                        stringBuilder.AppendLine(("DisabledWorkLabel".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
                        IEnumerable<string> items = DefDatabase<WorkTypeDef>.AllDefsListForReading.Where<WorkTypeDef>((Func<WorkTypeDef, bool>) (x => (stage.disabledWorkTags & x.workTags) != 0)).Select<WorkTypeDef, string>((Func<WorkTypeDef, string>) (x => x.labelShort));
                        stringBuilder.Append("  - " + items.ToCommaList().CapitalizeFirst());
                    }
                }
                if (this.def.CompProps<HediffCompProperties_GiveLovinMTBFactor>() != null)
                    stringBuilder.AppendLine((string) ("  - " + "IncreasesChanceOfLovin".Translate()));
                return stringBuilder.ToString();
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
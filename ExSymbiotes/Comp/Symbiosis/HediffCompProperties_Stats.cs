using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class HediffCompProperties_Stats : HediffCompProperties
    {
        public List<Stat> Stats;
        public List<string> StatDefRemoveGroup;
        public List<NeedDef> NeedDef;
        public float regenValue;

        public HediffCompProperties_Stats() => this.compClass = typeof (HediffComp_Stats);
    }
    
    public class Stat
    {
        public StatDef stat;
        public float value;
        public string type;
    }
}
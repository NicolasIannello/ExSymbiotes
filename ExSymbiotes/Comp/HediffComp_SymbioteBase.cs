using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class HediffComp_SymbioteBase : HediffComp
    {
        public static readonly ConditionalWeakTable<Pawn, HediffComp_SymbioteBase> SymbioteWeakTable = new ConditionalWeakTable<Pawn, HediffComp_SymbioteBase>();
        public Color color;
        public Color skin;
        protected static readonly Color blue= new Color(0.118f, 0, 0.812f);
        protected static readonly Color red= new Color(0.812f, 0, 0.118f);
        protected static readonly Color white= new Color(0.118f, 0.001f, 0.812f);

        public void ConditionalWeakTableAdd(Pawn pawn)
        {
            HediffComp_SymbioteBase.SymbioteWeakTable.Add(pawn, this);
        }
        
        public void ConditionalWeakTableRemove(Pawn pawn)
        {
            HediffComp_SymbioteBase.SymbioteWeakTable.Remove(pawn);
        }
        
        public bool ConditionalWeakTableTryGet(Pawn pawn)
        {
            return HediffComp_SymbioteBase.SymbioteWeakTable.TryGetValue(pawn, out HediffComp_SymbioteBase _);
        }
        
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<Color>(ref this.color, "color");
            Scribe_Values.Look<Color>(ref this.skin, "skin");
        }
    }
}
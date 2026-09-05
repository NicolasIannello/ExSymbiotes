using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class HediffCompProperties_SymbioteControl : HediffCompProperties
    {
        public Color color;
        public HediffCompProperties_SymbioteControl() => this.compClass = typeof (HediffComp_SymbioteControl);
    }
}
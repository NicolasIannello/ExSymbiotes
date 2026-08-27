using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class HediffCompProperties_Symbiosis : HediffCompProperties
    {
        public Color color;
        
        public HediffCompProperties_Symbiosis() => this.compClass = typeof (HediffComp_Symbiosis);
    }
}
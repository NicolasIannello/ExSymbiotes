using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class HediffCompProperties_Symbiosis : HediffCompProperties
    {
        public Color color;
        public Color hair;
        public float minDrawSize = 1.2f;
        public float maxDrawSize = 1.55f;
        
        public HediffCompProperties_Symbiosis() => this.compClass = typeof (HediffComp_Symbiosis);
    }
}
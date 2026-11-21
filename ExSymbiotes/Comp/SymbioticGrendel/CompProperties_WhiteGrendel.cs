using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ExSymbiotes
{
    public class CompProperties_WhiteGrendel : CompProperties
    {
        public float size;
        public Vector3 offsetSouth;
        public Vector3 offsetNorth;
        public Vector3 offsetEast;
        public Vector3 offsetWest;

        public CompProperties_WhiteGrendel()
        {
            this.compClass = typeof (CompWhiteGrendel);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string configError in base.ConfigErrors(parentDef))
                yield return configError;
        }
    }
}
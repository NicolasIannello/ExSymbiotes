using RimWorld;
using Verse;

namespace ExSymbiotes
{
    public class Comp_ProjectileFleck : ThingComp
    {

        public CompProperties_ProjectileFleck Props => (CompProperties_ProjectileFleck) this.props;

        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.Spawned)
            {
                Projectile parent = this.parent as Projectile;
                if (parent == null) return;
                FleckMaker.ConnectingLine(parent.Launcher.DrawPos, parent.DrawPos, this.Props.fleckDef, parent.Map);
                if(!GenSight.LineOfSight(parent.Launcher.Position, parent.intendedTarget.Cell, parent.Map)) parent.Destroy();
            }
        }
    }
}
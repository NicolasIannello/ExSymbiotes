using Verse;

namespace ExSymbiotes
{
    public class TendrilTop
    {
      private HediffComp_SymbioticTendrils parentTurret;
      private float curRotationInt;
      private int ticksUntilIdleTurn;
      private int idleTurnTicksLeft;
      private bool idleTurnClockwise;
      private const float IdleTurnDegreesPerTick = 0.26f;
      private const int IdleTurnDuration = 140;
      private const int IdleTurnIntervalMin = 150;
      private const int IdleTurnIntervalMax = 350;
      public static readonly int ArtworkRotation = -90;

      public float CurRotation
      {
        get => this.curRotationInt;
        set
        {
          this.curRotationInt = value;
          if ((double) this.curRotationInt > 360.0)
            this.curRotationInt -= 360f;
          if ((double) this.curRotationInt >= 0.0)
            return;
          this.curRotationInt += 360f;
        }
      }

      public void SetRotationFromOrientation() => this.CurRotation = this.parentTurret.Pawn.Rotation.AsAngle;

      public TendrilTop(HediffComp_SymbioticTendrils ParentTurret) => this.parentTurret = ParentTurret;

      public void ForceFaceTarget(LocalTargetInfo targ)
      {
        if (!targ.IsValid)
          return;
        this.CurRotation = (targ.Cell.ToVector3Shifted() - this.parentTurret.Pawn.DrawPos).AngleFlat();
      }

      public void TurretTopTick()
      {
        LocalTargetInfo currentTarget = this.parentTurret.CurrentTarget;
        if (currentTarget.IsValid)
        {
          this.CurRotation = (currentTarget.Cell.ToVector3Shifted() - this.parentTurret.Pawn.DrawPos).AngleFlat();
          this.ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
        }
        else if (this.ticksUntilIdleTurn > 0)
        {
          --this.ticksUntilIdleTurn;
          if (this.ticksUntilIdleTurn != 0)
            return;
          this.idleTurnClockwise = (double) Rand.Value < 0.5;
          this.idleTurnTicksLeft = 140;
        }
        else
        {
          if (this.idleTurnClockwise)
            this.CurRotation += 0.26f;
          else
            this.CurRotation -= 0.26f;
          --this.idleTurnTicksLeft;
          if (this.idleTurnTicksLeft > 0)
            return;
          this.ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
        }
      }
    }
}
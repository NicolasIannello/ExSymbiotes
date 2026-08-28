using ExSymbiotes.Utils;
using Verse;
using RimWorld;

namespace ExSymbiotes
{
  public class CompSymbioteMass_Black : CompObelisk_ExplodingSpawner
  {
    public Building_SymbioteMass Heart => this.parent as Building_SymbioteMass;
    public int nextTick = -99999;

    public override void TriggerInteractionEffect(Pawn interactor, bool triggeredByPlayer = false)
    {
      this.Heart?.StartTachycardiacOverload(StudyFinished);
    }

    public override void OnActivityActivated()
    {
      base.OnActivityActivated();
      SymbioteUtility.SymbioteHorde(Heart.Map, Heart, true);
      this.nextTick = Find.TickManager.TicksGame + 60000 + 15000;
    }

    public override void CompTick()
    {
      base.CompTick();
      if(Heart.IsHashIntervalTick(475) && nextTick>0 && nextTick<=Find.TickManager.TicksGame)
      {
        SymbioteUtility.SymbioteHorde(Heart.Map, Heart);
        this.nextTick = Find.TickManager.TicksGame + 60000 + 15000;
      }
    }
    
    public override void PostExposeData()
    {
      base.PostExposeData();
      Scribe_Values.Look<int>(ref this.nextTick, "nextTick");
    }

    public void SpawnRedMass()
    {
      this.Heart?.SpawnRedMass();
    }
  }
}

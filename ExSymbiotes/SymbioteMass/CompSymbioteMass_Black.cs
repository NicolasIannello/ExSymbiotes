using ExSymbiotes.Utils;
using Verse;
using RimWorld;

namespace ExSymbiotes
{
  public class CompSymbioteMass_Black : CompObelisk_ExplodingSpawner
  {
    public Building_SymbioteMass Heart => this.parent as Building_SymbioteMass;
    public int nextTick = -99999;
    private float scale = 1.25f;
    
    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        Heart.texture = ((CompProperties_SymbioteMass)this.Props).texture;
    }
    
    public override void TriggerInteractionEffect(Pawn interactor, bool triggeredByPlayer = false)
    {
      this.Heart?.StartTachycardiacOverload(StudyLevel>=2);
    }

    public override void OnActivityActivated()
    {
      base.OnActivityActivated();
      SymbioteUtility.SymbioteHorde(Heart.Map, Heart, StorytellerUtility.DefaultThreatPointsNow(Heart.Map), "ExSymbiotes.LabelSymbioteHorde", "ExSymbiotes.TextSymbioteHorde");
      this.nextTick = Find.TickManager.TicksGame + 60000 + 15000;
    }

    public override void CompTick()
    {
      base.CompTick();
      if(Heart.IsHashIntervalTick(475) && nextTick>0 && nextTick<=Find.TickManager.TicksGame)
      {
        SymbioteUtility.SymbioteHorde(Heart.Map, Heart, StorytellerUtility.DefaultThreatPointsNow(Heart.Map)*scale, "ExSymbiotes.LabelSymbioteHorde2", "ExSymbiotes.TextSymbioteHorde2");
        this.nextTick = Find.TickManager.TicksGame + 60000 + 15000;
        this.scale += 0.25f;
      }
    }
    
    public override void PostExposeData()
    {
      base.PostExposeData();
      Scribe_Values.Look<int>(ref this.nextTick, "nextTick");
      Scribe_Values.Look<float>(ref this.scale, "scale");
    }

    public void SpawnRedMass()
    {
      this.Heart?.SpawnRedMass();
    }
  }
}

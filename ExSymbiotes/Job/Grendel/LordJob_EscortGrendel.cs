using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ExSymbiotes
{

  public class LordJob_EscortGrendel : LordJob
  {
    public Pawn escortee;
    private Faction escorteeFaction;

    public override bool AlwaysShowWeapon => true;

    public LordJob_EscortGrendel() { }

    public LordJob_EscortGrendel(Pawn escortee)
    {
      this.escortee = escortee;
      this.escorteeFaction = escortee.Faction;
    }

    public override StateGraph CreateGraph()
    {
      StateGraph graph = new StateGraph();
      LordToil_EscortPawn lordToilEscortPawn = new LordToil_EscortPawn(this.escortee, 12f);
      graph.AddToil((LordToil)lordToilEscortPawn);
      return graph;
    }

    public override void ExposeData()
    {
      Scribe_References.Look<Pawn>(ref this.escortee, "escortee");
      Scribe_References.Look<Faction>(ref this.escorteeFaction, "escorteeFaction");
    }
  }
}
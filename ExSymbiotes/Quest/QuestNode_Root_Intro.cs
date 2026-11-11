using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace ExSymbiotes
{
  public class QuestNode_Root_Intro : QuestNode
  {
      protected override void RunInt()
      {
        Quest quest = RimWorld.QuestGen.QuestGen.quest;
        Map map = QuestGen_Get.GetMap();
        IncidentDef incident = Rand.RangeInclusive(1, 10) == 1 ? ExSymbiotesDefOf.ExSymbiotes_IncidentStrangeAsteroidRed : ExSymbiotesDefOf.ExSymbiotes_IncidentStrangeAsteroid;
        Find.Storyteller.incidentQueue.Add(incident, Find.TickManager.TicksGame, new IncidentParms()
        {
          target = (IIncidentTarget) map,
          pointMultiplier = 0,
          forced = true
        });
        quest.End(QuestEndOutcome.Success, false, false);
      }
      
      protected override bool TestRunInt(Slate slate)
      {
        return QuestGen_Get.GetMap() != null;
      }

  }
}
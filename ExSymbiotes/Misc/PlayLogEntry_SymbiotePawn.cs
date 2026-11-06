using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExSymbiotes.Misc
{
  public class PlayLogEntry_SymbiotePawn : PlayLogEntry_Interaction
  {
    public string symbiote;
    public bool symbioteInitiator;
    public Color symbioteColor;
    private string RecipientName => this.recipient == null ? "null" : this.recipient.LabelShort;

    public PlayLogEntry_SymbiotePawn() : base()
    {
    }

    public PlayLogEntry_SymbiotePawn(
      InteractionDef intDef,
      Pawn initiator,
      Pawn recipient,
      List<RulePackDef> extraSentencePacks,
      string symbiote,
      bool symbioteInitiator,
      Color symbioteColor)
      : base()
    {
      this.intDef = intDef;
      this.initiator = initiator;
      this.recipient = recipient;
      this.extraSentencePacks = extraSentencePacks;
      this.initiatorFaction = initiator.Faction;
      this.initiatorIdeo = initiator.Ideo;
      this.symbiote = symbiote;
      this.symbioteInitiator = symbioteInitiator;
      this.symbioteColor = symbioteColor;
    }

    protected override string ToGameStringFromPOV_Worker(Thing pov, bool forceLog)
    {
      string stringFromPovWorker = base.ToGameStringFromPOV_Worker(pov, forceLog);
      string hexColor = ColorUtility.ToHtmlStringRGBA(this.symbioteColor);
      string symbiote = $"<color=#{hexColor}>{this.symbiote}</color>";
      if(this.symbioteInitiator)
        stringFromPovWorker = stringFromPovWorker.ReplaceFirst("<color=#D09B61FF>"+this.initiator.LabelShort+"</color>", symbiote);
      else
      {
        stringFromPovWorker = stringFromPovWorker.Replace("<color=#D09B61FF>"+this.initiator.LabelShort+"</color>", symbiote);
        stringFromPovWorker = stringFromPovWorker.ReplaceFirst(symbiote, "<color=#D09B61FF>"+this.initiator.LabelShort+"</color>");
      }
      return stringFromPovWorker;
    }

    public override string ToString()
    {
      if (symbioteInitiator) return $"{this.intDef.label}: {this.symbiote}->{this.RecipientName}";

      return $"{this.intDef.label}: {this.InitiatorName}->{this.symbiote}";
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.Look<string>(ref this.symbiote, "symbiote");
      Scribe_Values.Look<bool>(ref this.symbioteInitiator, "symbioteInitiator");
      Scribe_Values.Look<Color>(ref this.symbioteColor, "symbioteColor");
    }
  }
}
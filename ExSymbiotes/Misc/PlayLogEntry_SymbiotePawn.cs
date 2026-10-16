using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace ExSymbiotes.Misc;

public class PlayLogEntry_SymbiotePawn : PlayLogEntry_Interaction
{
    public string symbiote;
    public bool symbioteInitiator;
    public Color symbioteColor;
    private string RecipientName => this.recipient == null ? "null" : this.recipient.LabelShort;
    
    public PlayLogEntry_SymbiotePawn() : base() { }

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
      if (this.initiator == null || this.recipient == null)
      {
        Log.ErrorOnce("PlayLogEntry_Interaction has a null pawn reference.", 34422);
        return $"[{this.intDef.label} error: null pawn reference]";
      }
      Rand.PushState();
      Rand.Seed = this.logID;
      GrammarRequest grammarRequest = this.GenerateGrammarRequest();
      string stringFromPovWorker;
      string hexColor = ColorUtility.ToHtmlStringRGBA(this.symbioteColor);
      if (pov == this.initiator)
      {
        grammarRequest.IncludesBare.Add(this.intDef.logRulesInitiator);
        if (symbioteInitiator)
        {
          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiator, grammarRequest.Constants));
          grammarRequest.Rules.RemoveAll(r => r.keyword == "INITIATOR_nameDef");
          grammarRequest.Rules.Add(new Rule_String("INITIATOR_nameDef", $"<color=#{hexColor}>{symbiote}</color>"));
          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipient, grammarRequest.Constants));
        }
        else
        {
          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiator, grammarRequest.Constants));
          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipient, grammarRequest.Constants));
          grammarRequest.Rules.RemoveAll(r => r.keyword == "RECIPIENT_nameDef");
          grammarRequest.Rules.Add(new Rule_String("RECIPIENT_nameDef", $"<color=#{hexColor}>{symbiote}</color>"));
        }
        stringFromPovWorker = GrammarResolver.Resolve("r_logentry", grammarRequest, "interaction from initiator", forceLog);
      }
      else if (pov == this.recipient)
      {
        if (this.intDef.logRulesRecipient != null)
          grammarRequest.IncludesBare.Add(this.intDef.logRulesRecipient);
        else
          grammarRequest.IncludesBare.Add(this.intDef.logRulesInitiator);
        if (symbioteInitiator)
        {
          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiator, grammarRequest.Constants));
          grammarRequest.Rules.RemoveAll(r => r.keyword == "INITIATOR_nameDef");
          grammarRequest.Rules.Add(new Rule_String("INITIATOR_nameDef", $"<color=#{hexColor}>{symbiote}</color>"));
          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipient, grammarRequest.Constants));
        }
        else
        {
          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiator, grammarRequest.Constants));
          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipient, grammarRequest.Constants));
          grammarRequest.Rules.RemoveAll(r => r.keyword == "RECIPIENT_nameDef");
          grammarRequest.Rules.Add(new Rule_String("RECIPIENT_nameDef", $"<color=#{hexColor}>{symbiote}</color>"));
        }
        stringFromPovWorker = GrammarResolver.Resolve("r_logentry", grammarRequest, "interaction from recipient", forceLog);
      }
      else
      {
        Log.ErrorOnce("Cannot display PlayLogEntry_Interaction from POV who isn't initiator or recipient.", 51251);
        stringFromPovWorker = this.ToString();
      }
      if (this.extraSentencePacks != null)
      {
        for (int index = 0; index < this.extraSentencePacks.Count; ++index)
        {
          grammarRequest.Clear();
          grammarRequest.Includes.Add(this.extraSentencePacks[index]);
          if (symbioteInitiator)
          {
            grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiator, grammarRequest.Constants));
            grammarRequest.Rules.RemoveAll(r => r.keyword == "INITIATOR_nameDef");
            grammarRequest.Rules.Add(new Rule_String("INITIATOR_nameDef", $"<color=#{hexColor}>{symbiote}</color>"));
            grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipient, grammarRequest.Constants));
          }
          else
          {
            grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiator, grammarRequest.Constants));
            grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipient, grammarRequest.Constants));
            grammarRequest.Rules.RemoveAll(r => r.keyword == "RECIPIENT_nameDef");
            grammarRequest.Rules.Add(new Rule_String("RECIPIENT_nameDef", $"<color=#{hexColor}>{symbiote}</color>"));
          }
          stringFromPovWorker = $"{stringFromPovWorker} {GrammarResolver.Resolve(this.extraSentencePacks[index].FirstRuleKeyword, grammarRequest, "extraSentencePack", forceLog, this.extraSentencePacks[index].FirstUntranslatedRuleKeyword)}";
        }
      }
      Rand.PopState();
      return stringFromPovWorker;
    }
    
    public override string ToString()
    {
        if(symbioteInitiator) return $"{this.intDef.label}: {this.symbiote}->{this.RecipientName}";
        
        return $"{this.intDef.label}: {this.InitiatorName}->{this.symbiote}";
    }
}
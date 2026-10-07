using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ExSymbiotes
{
    public class HediffWithComps_Hivemind : HediffWithComps
    {
        public List<Pawn> controlledPawns = new List<Pawn>();

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            MoteMaker.MakeThoughtBubble(this.pawn, "Things/Pawn/Symbiote/Attachments/SymbioteSpiral/SymbioteSpiral_south");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            IEnumerable<Gizmo> compGetGizmos = base.GetGizmos();
            if (compGetGizmos != null) foreach (Gizmo gizmo in compGetGizmos) yield return gizmo;
            
            Command_Action gizmoTarget = new Command_Action
            {
                defaultLabel = (string) "ExSymbiotes.ThrallAttack".Translate(),
                icon = (Texture) ContentFinder<Texture2D>.Get("UI/ExSymbiotesThrallAttack"),
                action = delegate
                {
                    Find.Targeter.BeginTargeting(targetParams: TargetingParameters.ForAttackAny(), action: this.OrderAttack);
                }
            };
            yield return (Gizmo) gizmoTarget;
            
            Command_Action gizmoCancel = new Command_Action
            {
                defaultLabel = (string) "ExSymbiotes.ThrallCancel".Translate(),
                icon = (Texture) ContentFinder<Texture2D>.Get("UI/ExSymbiotesThrallCancel"),
                action = this.CancelJob
            };
            yield return (Gizmo) gizmoCancel;

            Command_Action gizmoMove = new Command_Action
            {
                defaultLabel = (string) "ExSymbiotes.ThrallMove".Translate(),
                icon = (Texture) ContentFinder<Texture2D>.Get("UI/ExSymbiotesThrallMove"),
                action = delegate
                {
                    Find.Targeter.BeginTargeting(targetParams: TargetingParameters.ForCell(), action: this.OrderMove);
                }
            };
            yield return (Gizmo) gizmoMove;
        }

        private void OrderAttack(LocalTargetInfo target)
        {
            for (int i = 0; i < controlledPawns.Count; i++)
            {
                JobDef jobDef = (controlledPawns[i].equipment?.Primary?.def?.IsRangedWeapon ?? false) ? 
                    JobDefOf.AttackStatic : JobDefOf.AttackMelee;
                Job job = JobMaker.MakeJob(jobDef, target);
                controlledPawns[i].jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
        }
        
        private void CancelJob()
        {
            for (int i = 0; i < controlledPawns.Count; i++)
            {
                controlledPawns[i].jobs.EndCurrentJob(JobCondition.InterruptForced);
            }
        }
        
        private void OrderMove(LocalTargetInfo target)
        {
            for (int i = 0; i < controlledPawns.Count; i++)
            {
                Job job = JobMaker.MakeJob(JobDefOf.Goto, target);
                controlledPawns[i].jobs.TryTakeOrderedJob(job);
            }
        }
        
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look<Pawn>(ref controlledPawns, "controlledPawns", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && this.controlledPawns == null)
                this.controlledPawns = new List<Pawn>();
        }
    }
}
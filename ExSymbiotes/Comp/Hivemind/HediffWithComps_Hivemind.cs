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
        
        public override IEnumerable<Gizmo> GetGizmos()
        {
            IEnumerable<Gizmo> compGetGizmos = base.GetGizmos();
            if (compGetGizmos != null) foreach (Gizmo gizmo in compGetGizmos) yield return gizmo;
            
            Command_Action gizmoTarget = new Command_Action
            {
                defaultLabel = (string) "Hive attack",
                icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/Attack"),
                action = delegate
                {
                    Find.Targeter.BeginTargeting(targetParams: TargetingParameters.ForAttackAny(), action: this.OrderAttack);
                }
            };
            yield return (Gizmo) gizmoTarget;
            
            Command_Action gizmoCancel = new Command_Action
            {
                defaultLabel = (string) "Hive cancel",
                icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/Attack"),
                action = this.CancelJob
            };
            yield return (Gizmo) gizmoCancel;

            Command_Action gizmoMove = new Command_Action
            {
                defaultLabel = (string) "Hive move",
                icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/Attack"),
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
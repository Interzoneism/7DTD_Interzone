using System;
using System.Collections.Generic;
using GameEvent.SequenceActions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceDecisions
{
	// Token: 0x0200163C RID: 5692
	[Preserve]
	public class BaseDecision : BaseAction
	{
		// Token: 0x17001389 RID: 5001
		// (get) Token: 0x0600AE8B RID: 44683 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public override bool UseRequirements
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600AE8C RID: 44684 RVA: 0x00442814 File Offset: 0x00440A14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			base.OnInit();
			List<int> list = new List<int>();
			for (int i = 0; i < this.Actions.Count; i++)
			{
				if (!list.Contains(this.Actions[i].Phase))
				{
					list.Add(this.Actions[i].Phase);
				}
				this.Actions[i].ActionIndex = i;
			}
			list.Sort();
			if (list.Count > 0)
			{
				this.phaseMax = list[list.Count - 1] + 1;
				return;
			}
			this.phaseMax = 0;
		}

		// Token: 0x0600AE8D RID: 44685 RVA: 0x004428B4 File Offset: 0x00440AB4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnReset()
		{
			base.OnReset();
			for (int i = 0; i < this.Actions.Count; i++)
			{
				this.Actions[i].Reset();
			}
			this.currentPhase = 0;
		}

		// Token: 0x0600AE8E RID: 44686 RVA: 0x004428F8 File Offset: 0x00440AF8
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseAction.ActionCompleteStates HandleActions()
		{
			bool flag = false;
			int phaseOnComplete = this.currentPhase;
			for (int i = 0; i < this.Actions.Count; i++)
			{
				if (this.Actions[i].Phase == this.currentPhase && !this.Actions[i].IsComplete)
				{
					flag = true;
					this.Actions[i].Owner = base.Owner;
					BaseAction.ActionCompleteStates actionCompleteStates = this.Actions[i].PerformAction();
					if (actionCompleteStates == BaseAction.ActionCompleteStates.Complete || (actionCompleteStates == BaseAction.ActionCompleteStates.InCompleteRefund && this.Actions[i].IgnoreRefund))
					{
						this.Actions[i].IsComplete = true;
						if (this.Actions[i].PhaseOnComplete != -1)
						{
							phaseOnComplete = this.Actions[i].PhaseOnComplete;
						}
					}
					else if (base.Owner.AllowRefunds && actionCompleteStates == BaseAction.ActionCompleteStates.InCompleteRefund)
					{
						return BaseAction.ActionCompleteStates.InCompleteRefund;
					}
				}
			}
			if (!flag)
			{
				this.currentPhase++;
			}
			else if (this.currentPhase != phaseOnComplete)
			{
				this.currentPhase = phaseOnComplete;
				for (int j = 0; j < this.Actions.Count; j++)
				{
					if (this.Actions[j].Phase >= this.currentPhase)
					{
						this.Actions[j].Reset();
					}
				}
			}
			if (this.currentPhase >= this.phaseMax)
			{
				this.IsComplete = true;
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600AE8F RID: 44687 RVA: 0x00442A74 File Offset: 0x00440C74
		public override void HandleTemplateInit(GameEventActionSequence seq)
		{
			base.HandleTemplateInit(seq);
			for (int i = 0; i < this.Actions.Count; i++)
			{
				seq.HandleVariablesForProperties(this.Actions[i].Properties);
				this.Actions[i].ParseProperties(this.Actions[i].Properties);
			}
		}

		// Token: 0x0600AE90 RID: 44688 RVA: 0x00442AD8 File Offset: 0x00440CD8
		public override BaseAction Clone()
		{
			BaseDecision baseDecision = (BaseDecision)base.Clone();
			for (int i = 0; i < this.Actions.Count; i++)
			{
				BaseAction baseAction = this.Actions[i].Clone();
				baseAction.Owner = base.Owner;
				baseDecision.Actions.Add(baseAction);
			}
			baseDecision.phaseMax = this.phaseMax;
			return baseDecision;
		}

		// Token: 0x04008773 RID: 34675
		public List<BaseAction> Actions = new List<BaseAction>();

		// Token: 0x04008774 RID: 34676
		[PublicizedFrom(EAccessModifier.Protected)]
		public int currentPhase;

		// Token: 0x04008775 RID: 34677
		[PublicizedFrom(EAccessModifier.Protected)]
		public int phaseMax;
	}
}

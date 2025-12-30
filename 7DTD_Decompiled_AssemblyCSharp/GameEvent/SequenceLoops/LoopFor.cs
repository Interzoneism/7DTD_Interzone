using System;
using GameEvent.SequenceActions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceLoops
{
	// Token: 0x02001634 RID: 5684
	[Preserve]
	public class LoopFor : BaseLoop
	{
		// Token: 0x0600AE6A RID: 44650 RVA: 0x004416C4 File Offset: 0x0043F8C4
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this.loopCount == -1)
			{
				this.loopCount = GameEventManager.GetIntValue(base.Owner.Target as EntityAlive, this.loopCountText, 1);
			}
			if (base.HandleActions() == BaseAction.ActionCompleteStates.Complete)
			{
				this.currentLoop++;
				this.CurrentPhase = 0;
				for (int i = 0; i < this.Actions.Count; i++)
				{
					this.Actions[i].Reset();
				}
				if (this.currentLoop >= this.loopCount)
				{
					this.IsComplete = true;
					return BaseAction.ActionCompleteStates.Complete;
				}
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600AE6B RID: 44651 RVA: 0x00441759 File Offset: 0x0043F959
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnReset()
		{
			base.OnReset();
			this.loopCount = -1;
			this.currentLoop = 0;
		}

		// Token: 0x0600AE6C RID: 44652 RVA: 0x0044176F File Offset: 0x0043F96F
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(LoopFor.PropLoopCount, ref this.loopCountText);
		}

		// Token: 0x0600AE6D RID: 44653 RVA: 0x00441789 File Offset: 0x0043F989
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new LoopFor
			{
				loopCountText = this.loopCountText
			};
		}

		// Token: 0x04008746 RID: 34630
		[PublicizedFrom(EAccessModifier.Private)]
		public int loopCount = -1;

		// Token: 0x04008747 RID: 34631
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentLoop;

		// Token: 0x04008748 RID: 34632
		public string loopCountText;

		// Token: 0x04008749 RID: 34633
		public static string PropLoopCount = "loop_count";
	}
}

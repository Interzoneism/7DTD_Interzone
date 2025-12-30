using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016D9 RID: 5849
	[Preserve]
	public class ActionPOIReset : BaseAction
	{
		// Token: 0x0600B15E RID: 45406 RVA: 0x00453086 File Offset: 0x00451286
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this._state == ActionPOIReset.State.Start)
			{
				this._state = ActionPOIReset.State.Wait;
				this._retVal = BaseAction.ActionCompleteStates.InComplete;
				GameManager.Instance.StartCoroutine(this.onPerformAction());
			}
			return this._retVal;
		}

		// Token: 0x0600B15F RID: 45407 RVA: 0x004530B5 File Offset: 0x004512B5
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator onPerformAction()
		{
			Vector3i poiposition = base.Owner.POIPosition;
			World world = GameManager.Instance.World;
			if (base.Owner.POIInstance == null)
			{
				this._retVal = BaseAction.ActionCompleteStates.InCompleteRefund;
				yield break;
			}
			List<PrefabInstance> prefabsIntersecting = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabsIntersecting(base.Owner.POIInstance);
			int entityID = -1;
			if (!GameManager.Instance.IsEditMode() && !GameUtils.IsPlaytesting())
			{
				entityID = ((base.Owner.Requester != null) ? base.Owner.Requester.entityId : -1);
			}
			yield return world.ResetPOIS(prefabsIntersecting, QuestEventManager.manualResetTag, entityID, null, null);
			this._retVal = BaseAction.ActionCompleteStates.Complete;
			yield break;
		}

		// Token: 0x0600B160 RID: 45408 RVA: 0x004530C4 File Offset: 0x004512C4
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator UpdateBlocks(List<BlockChangeInfo> blockChanges)
		{
			yield return new WaitForSeconds(1f);
			GameManager.Instance.World.SetBlocksRPC(blockChanges);
			yield break;
		}

		// Token: 0x0600B161 RID: 45409 RVA: 0x004530D3 File Offset: 0x004512D3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionPOIReset();
		}

		// Token: 0x0600B162 RID: 45410 RVA: 0x004530DA File Offset: 0x004512DA
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnReset()
		{
			this._state = ActionPOIReset.State.Start;
		}

		// Token: 0x04008AD8 RID: 35544
		[PublicizedFrom(EAccessModifier.Private)]
		public BaseAction.ActionCompleteStates _retVal;

		// Token: 0x04008AD9 RID: 35545
		[PublicizedFrom(EAccessModifier.Private)]
		public ActionPOIReset.State _state;

		// Token: 0x020016DA RID: 5850
		[PublicizedFrom(EAccessModifier.Private)]
		public enum State
		{
			// Token: 0x04008ADB RID: 35547
			Start,
			// Token: 0x04008ADC RID: 35548
			Wait
		}
	}
}

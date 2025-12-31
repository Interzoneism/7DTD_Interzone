using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200168A RID: 5770
	[Preserve]
	public class ActionRemoveEntities : BaseAction
	{
		// Token: 0x0600AFDC RID: 45020 RVA: 0x0044B8F0 File Offset: 0x00449AF0
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this.targetGroup != "")
			{
				List<Entity> entityGroup = base.Owner.GetEntityGroup(this.targetGroup);
				if (entityGroup != null)
				{
					GameEventManager gm = GameEventManager.Current;
					for (int i = 0; i < entityGroup.Count; i++)
					{
						this.HandleRemoveData(gm, entityGroup[i]);
						GameManager.Instance.StartCoroutine(this.removeLater(entityGroup[i]));
					}
				}
				return BaseAction.ActionCompleteStates.Complete;
			}
			if (base.Owner.Target != null)
			{
				this.HandleRemoveData(GameEventManager.Current, base.Owner.Target);
				GameManager.Instance.StartCoroutine(this.removeLater(base.Owner.Target));
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AFDD RID: 45021 RVA: 0x0044B9A9 File Offset: 0x00449BA9
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void HandleRemoveData(GameEventManager gm, Entity ent)
		{
			gm.RemoveSpawnedEntry(ent);
		}

		// Token: 0x0600AFDE RID: 45022 RVA: 0x0044B9B2 File Offset: 0x00449BB2
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator removeLater(Entity e)
		{
			yield return new WaitForSeconds(0.25f);
			EntityVehicle entityVehicle = e as EntityVehicle;
			if (entityVehicle != null)
			{
				entityVehicle.Kill();
			}
			if (e != null)
			{
				GameManager.Instance.World.RemoveEntity(e.entityId, EnumRemoveEntityReason.Killed);
			}
			yield break;
		}

		// Token: 0x0600AFDF RID: 45023 RVA: 0x0044B9C1 File Offset: 0x00449BC1
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionRemoveEntities.PropTargetGroup, ref this.targetGroup);
		}

		// Token: 0x0600AFE0 RID: 45024 RVA: 0x0044B9DB File Offset: 0x00449BDB
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRemoveEntities
			{
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x04008963 RID: 35171
		[PublicizedFrom(EAccessModifier.Protected)]
		public string targetGroup = "";

		// Token: 0x04008964 RID: 35172
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetGroup = "target_group";
	}
}

using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001659 RID: 5721
	[Preserve]
	public class ActionBaseTeleport : ActionBaseTargetAction
	{
		// Token: 0x0600AF14 RID: 44820 RVA: 0x0044610C File Offset: 0x0044430C
		[PublicizedFrom(EAccessModifier.Protected)]
		public void TeleportEntity(Entity entity, Vector3 position)
		{
			float floatValue = GameEventManager.GetFloatValue(entity as EntityAlive, this.teleportDelayText, 0.1f);
			GameManager.Instance.StartCoroutine(base.TeleportEntity(entity, position, floatValue));
		}

		// Token: 0x0600AF15 RID: 44821 RVA: 0x00446144 File Offset: 0x00444344
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionBaseTeleport.PropTeleportDelay, ref this.teleportDelayText);
		}

		// Token: 0x0600AF16 RID: 44822 RVA: 0x00019766 File Offset: 0x00017966
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return null;
		}

		// Token: 0x04008838 RID: 34872
		[PublicizedFrom(EAccessModifier.Protected)]
		public string teleportDelayText = "";

		// Token: 0x04008839 RID: 34873
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTeleportDelay = "teleport_delay";
	}
}

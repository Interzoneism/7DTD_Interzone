using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001695 RID: 5781
	[Preserve]
	public class ActionResetMap : ActionBaseClientAction
	{
		// Token: 0x0600B019 RID: 45081 RVA: 0x0044CAD8 File Offset: 0x0044ACD8
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				if (this.removeDiscovery)
				{
					entityPlayerLocal.ChunkObserver.mapDatabase.Clear();
				}
				if (this.removeWaypoints)
				{
					for (int i = 0; i < entityPlayerLocal.Waypoints.Collection.list.Count; i++)
					{
						Waypoint waypoint = entityPlayerLocal.Waypoints.Collection.list[i];
						if (waypoint.navObject != null)
						{
							NavObjectManager.Instance.UnRegisterNavObject(waypoint.navObject);
						}
					}
					entityPlayerLocal.WaypointInvites.Clear();
					entityPlayerLocal.Waypoints.Collection.Clear();
					entityPlayerLocal.markerPosition = Vector3i.zero;
				}
			}
		}

		// Token: 0x0600B01A RID: 45082 RVA: 0x0044CB87 File Offset: 0x0044AD87
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionResetMap.PropRemoveDiscovery, ref this.removeDiscovery);
			properties.ParseBool(ActionResetMap.PropRemoveWaypoints, ref this.removeWaypoints);
		}

		// Token: 0x0600B01B RID: 45083 RVA: 0x0044CBB2 File Offset: 0x0044ADB2
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionResetMap
			{
				removeDiscovery = this.removeDiscovery,
				removeWaypoints = this.removeWaypoints
			};
		}

		// Token: 0x04008988 RID: 35208
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool removeDiscovery;

		// Token: 0x04008989 RID: 35209
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool removeWaypoints;

		// Token: 0x0400898A RID: 35210
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRemoveDiscovery = "remove_discovery";

		// Token: 0x0400898B RID: 35211
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRemoveWaypoints = "remove_waypoints";
	}
}

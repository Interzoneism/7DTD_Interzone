using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016B6 RID: 5814
	[Preserve]
	public class ActionTeleportToSpecial : ActionBaseClientAction
	{
		// Token: 0x0600B0AF RID: 45231 RVA: 0x0044F8A4 File Offset: 0x0044DAA4
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				World world = GameManager.Instance.World;
				this.position = Vector3.zero;
				switch (this.pointType)
				{
				case ActionTeleportToSpecial.SpecialPointTypes.Bedroll:
					if (entityPlayer.SpawnPoints.Count == 0)
					{
						return;
					}
					this.position = entityPlayer.SpawnPoints[0];
					break;
				case ActionTeleportToSpecial.SpecialPointTypes.Landclaim:
				{
					PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(entityPlayer.entityId);
					if (playerDataFromEntityID.LPBlocks == null || playerDataFromEntityID.LPBlocks.Count == 0)
					{
						return;
					}
					this.position = playerDataFromEntityID.LPBlocks[0];
					break;
				}
				case ActionTeleportToSpecial.SpecialPointTypes.Backpack:
				{
					Vector3i lastDroppedBackpackPosition = entityPlayer.GetLastDroppedBackpackPosition();
					if (lastDroppedBackpackPosition == Vector3i.zero)
					{
						return;
					}
					this.position = lastDroppedBackpackPosition;
					break;
				}
				}
				GameManager.Instance.StartCoroutine(this.handleTeleport(entityPlayer));
			}
		}

		// Token: 0x0600B0B0 RID: 45232 RVA: 0x0044F991 File Offset: 0x0044DB91
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator handleTeleport(EntityPlayer player)
		{
			yield return new WaitForSeconds(this.teleportDelay);
			if (this.position.y > 0f)
			{
				this.position += Vector3.up * 2f;
				if (player.isEntityRemote)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(player.entityId).SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(this.position, null, false));
				}
				else
				{
					((EntityPlayerLocal)player).PlayerUI.windowManager.CloseAllOpenWindows(null, false);
					((EntityPlayerLocal)player).TeleportToPosition(this.position, false, null);
				}
			}
			yield break;
		}

		// Token: 0x0600B0B1 RID: 45233 RVA: 0x0044F9A7 File Offset: 0x0044DBA7
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<ActionTeleportToSpecial.SpecialPointTypes>(ActionTeleportToSpecial.PropSpecialType, ref this.pointType);
			properties.ParseFloat(ActionTeleportToSpecial.PropTeleportDelay, ref this.teleportDelay);
		}

		// Token: 0x0600B0B2 RID: 45234 RVA: 0x0044F9D2 File Offset: 0x0044DBD2
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTeleportToSpecial
			{
				targetGroup = this.targetGroup,
				pointType = this.pointType,
				teleportDelay = this.teleportDelay
			};
		}

		// Token: 0x04008A28 RID: 35368
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 position;

		// Token: 0x04008A29 RID: 35369
		[PublicizedFrom(EAccessModifier.Protected)]
		public float teleportDelay = 0.1f;

		// Token: 0x04008A2A RID: 35370
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionTeleportToSpecial.SpecialPointTypes pointType;

		// Token: 0x04008A2B RID: 35371
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSpecialType = "special_type";

		// Token: 0x04008A2C RID: 35372
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTeleportDelay = "teleport_delay";

		// Token: 0x020016B7 RID: 5815
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum SpecialPointTypes
		{
			// Token: 0x04008A2E RID: 35374
			Bedroll,
			// Token: 0x04008A2F RID: 35375
			Landclaim,
			// Token: 0x04008A30 RID: 35376
			Backpack
		}
	}
}

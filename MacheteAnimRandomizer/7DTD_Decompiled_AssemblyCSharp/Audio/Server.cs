using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
	// Token: 0x020017A8 RID: 6056
	public class Server : IDisposable
	{
		// Token: 0x0600B542 RID: 46402 RVA: 0x00461C08 File Offset: 0x0045FE08
		public void Play(Entity playOnEntity, string soundGroupName, float _occlusion, bool signalOnly = false)
		{
			if (GameManager.IsDedicatedServer && playOnEntity != null)
			{
				Manager.ConvertName(ref soundGroupName, playOnEntity);
				Manager.SignalAI(playOnEntity, playOnEntity.GetPosition(), soundGroupName, 1f);
			}
			if (!signalOnly)
			{
				foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
				{
					if (Manager.IgnoresDistanceCheck(soundGroupName) || Entity.CheckDistance(playOnEntity, keyValuePair.Value.entityId))
					{
						keyValuePair.Value.Play(playOnEntity.entityId, soundGroupName, _occlusion);
					}
				}
			}
		}

		// Token: 0x0600B543 RID: 46403 RVA: 0x00461CB4 File Offset: 0x0045FEB4
		public void Play(Vector3 position, string soundGroupName, float _occlusion, int entityId = -1)
		{
			if (GameManager.IsDedicatedServer)
			{
				Manager.ConvertName(ref soundGroupName, null);
				Manager.SignalAI(null, position, soundGroupName, 1f);
			}
			foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
			{
				if (Manager.IgnoresDistanceCheck(soundGroupName) || Entity.CheckDistance(position, keyValuePair.Value.entityId))
				{
					keyValuePair.Value.Play(position, soundGroupName, _occlusion, entityId);
				}
			}
		}

		// Token: 0x0600B544 RID: 46404 RVA: 0x00461D4C File Offset: 0x0045FF4C
		public void Stop(int playOnEntityId, string soundGroupName)
		{
			foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
			{
				keyValuePair.Value.Stop(playOnEntityId, soundGroupName);
			}
		}

		// Token: 0x0600B545 RID: 46405 RVA: 0x00461DA8 File Offset: 0x0045FFA8
		public void Stop(Vector3 position, string soundGroupName)
		{
			foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
			{
				keyValuePair.Value.Stop(position, soundGroupName);
			}
		}

		// Token: 0x0600B546 RID: 46406 RVA: 0x00461E04 File Offset: 0x00460004
		public void AttachLocalPlayer(EntityPlayerLocal localPlayer)
		{
			this.m_localPlayer = localPlayer;
		}

		// Token: 0x0600B547 RID: 46407 RVA: 0x00461E10 File Offset: 0x00460010
		public void EntityAddedToWorld(Entity entity, World world)
		{
			if (entity is EntityPlayer && (this.m_localPlayer == null || entity.entityId != this.m_localPlayer.entityId))
			{
				Client value;
				if (this.m_players.TryGetValue(entity.entityId, out value))
				{
					Log.Warning("[AudioLog] AudioManagerServer: consistency error, client id '" + entity.entityId.ToString() + "' already exists, but is being added again!");
					return;
				}
				value = new Client(entity.entityId);
				this.m_players[entity.entityId] = value;
			}
		}

		// Token: 0x0600B548 RID: 46408 RVA: 0x00461E9C File Offset: 0x0046009C
		public void EntityRemovedFromWorld(Entity entity, World world)
		{
			Client client;
			if (this.m_players.TryGetValue(entity.entityId, out client))
			{
				this.m_players.Remove(entity.entityId);
				client.Dispose();
			}
		}

		// Token: 0x0600B549 RID: 46409 RVA: 0x00461ED8 File Offset: 0x004600D8
		public void Dispose()
		{
			foreach (KeyValuePair<int, Client> keyValuePair in this.m_players)
			{
				keyValuePair.Value.Dispose();
			}
			this.m_players = null;
			this.m_localPlayer = null;
		}

		// Token: 0x04008E04 RID: 36356
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal m_localPlayer;

		// Token: 0x04008E05 RID: 36357
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, Client> m_players = new Dictionary<int, Client>();
	}
}

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000794 RID: 1940
[Preserve]
public class NetPackageSimpleChat : NetPackage
{
	// Token: 0x06003846 RID: 14406 RVA: 0x0016F260 File Offset: 0x0016D460
	public NetPackageSimpleChat Setup(string _msg)
	{
		this.msg = (string.IsNullOrEmpty(_msg) ? string.Empty : _msg);
		return this;
	}

	// Token: 0x06003847 RID: 14407 RVA: 0x0016F279 File Offset: 0x0016D479
	public NetPackageSimpleChat Setup(string _msg, List<int> _recipientEntityIds)
	{
		this.msg = (string.IsNullOrEmpty(_msg) ? string.Empty : _msg);
		this.recipientEntityIds = _recipientEntityIds;
		return this;
	}

	// Token: 0x06003848 RID: 14408 RVA: 0x0016F29C File Offset: 0x0016D49C
	public override void read(PooledBinaryReader _br)
	{
		this.msg = _br.ReadString();
		int num = _br.ReadInt32();
		if (num > 0)
		{
			this.recipientEntityIds = new List<int>();
			for (int i = 0; i < num; i++)
			{
				this.recipientEntityIds.Add(_br.ReadInt32());
			}
		}
	}

	// Token: 0x06003849 RID: 14409 RVA: 0x0016F2E8 File Offset: 0x0016D4E8
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.msg);
		_bw.Write((this.recipientEntityIds != null) ? this.recipientEntityIds.Count : 0);
		if (this.recipientEntityIds != null && this.recipientEntityIds.Count > 0)
		{
			for (int i = 0; i < this.recipientEntityIds.Count; i++)
			{
				_bw.Write(this.recipientEntityIds[i]);
			}
		}
	}

	// Token: 0x0600384A RID: 14410 RVA: 0x0016F364 File Offset: 0x0016D564
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			if (this.recipientEntityIds == null)
			{
				return;
			}
			using (List<int>.Enumerator enumerator = this.recipientEntityIds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int entityId = enumerator.Current;
					EntityPlayerLocal entityPlayerLocal = _world.GetEntity(entityId) as EntityPlayerLocal;
					if (entityPlayerLocal != null)
					{
						LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
						if (null != uiforPlayer && null != uiforPlayer.windowManager)
						{
							XUiC_ChatOutput.AddMessage(uiforPlayer.xui, EnumGameMessages.PlainTextLocal, this.msg, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.Server, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
						}
					}
					else
					{
						ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId);
						if (clientInfo != null)
						{
							clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageSimpleChat>().Setup(this.msg));
						}
					}
				}
				return;
			}
		}
		List<EntityPlayerLocal> localPlayers = _callbacks.World.GetLocalPlayers();
		for (int i = 0; i < localPlayers.Count; i++)
		{
			LocalPlayerUI uiforPlayer2 = LocalPlayerUI.GetUIForPlayer(localPlayers[i]);
			if (null != uiforPlayer2 && null != uiforPlayer2.windowManager)
			{
				XUiC_ChatOutput.AddMessage(uiforPlayer2.xui, EnumGameMessages.PlainTextLocal, this.msg, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.Server, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
			}
		}
	}

	// Token: 0x0600384B RID: 14411 RVA: 0x0016F4AC File Offset: 0x0016D6AC
	public override int GetLength()
	{
		return 4 + this.msg.Length;
	}

	// Token: 0x04002DA5 RID: 11685
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> recipientEntityIds;

	// Token: 0x04002DA6 RID: 11686
	[PublicizedFrom(EAccessModifier.Private)]
	public string msg;
}

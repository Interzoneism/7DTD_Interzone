using System;
using UnityEngine.Scripting;

// Token: 0x0200079F RID: 1951
[Preserve]
public class NetPackageTraderStatus : NetPackage
{
	// Token: 0x06003881 RID: 14465 RVA: 0x0016FF1F File Offset: 0x0016E11F
	public NetPackageTraderStatus Setup(int _traderId, bool _isOpen = false)
	{
		this.traderId = _traderId;
		this.isOpen = _isOpen;
		return this;
	}

	// Token: 0x06003882 RID: 14466 RVA: 0x0016FF30 File Offset: 0x0016E130
	public override void read(PooledBinaryReader _br)
	{
		this.traderId = _br.ReadInt32();
		this.isOpen = _br.ReadBoolean();
	}

	// Token: 0x06003883 RID: 14467 RVA: 0x0016FF4A File Offset: 0x0016E14A
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.traderId);
		_bw.Write(this.isOpen);
	}

	// Token: 0x06003884 RID: 14468 RVA: 0x0016FF6C File Offset: 0x0016E16C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		EntityTrader entityTrader = GameManager.Instance.World.GetEntity(this.traderId) as EntityTrader;
		if (entityTrader == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (QuestEventManager.Current.GetQuestList(_world, this.traderId, base.Sender.entityId) == null)
			{
				EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(base.Sender.entityId) as EntityPlayer;
				if (entityPlayer != null)
				{
					entityTrader.SetupActiveQuestsForPlayer(entityPlayer, -1);
				}
			}
			NetPackageTraderStatus package = NetPackageManager.GetPackage<NetPackageTraderStatus>();
			package.Setup(this.traderId, entityTrader.traderArea == null || !entityTrader.traderArea.IsClosed);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, base.Sender.entityId, -1, -1, null, 192, false);
			return;
		}
		entityTrader.ActivateTrader(this.isOpen);
	}

	// Token: 0x06003885 RID: 14469 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002DCC RID: 11724
	[PublicizedFrom(EAccessModifier.Private)]
	public int traderId;

	// Token: 0x04002DCD RID: 11725
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOpen;
}

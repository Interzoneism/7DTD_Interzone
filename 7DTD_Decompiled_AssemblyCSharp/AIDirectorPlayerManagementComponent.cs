using System;
using UnityEngine.Scripting;

// Token: 0x020003C4 RID: 964
[Preserve]
public class AIDirectorPlayerManagementComponent : AIDirectorComponent
{
	// Token: 0x06001D5F RID: 7519 RVA: 0x000B78DC File Offset: 0x000B5ADC
	public override void Tick(double _dt)
	{
		base.Tick(_dt);
		this.TickPlayerStates(_dt);
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x000B78EC File Offset: 0x000B5AEC
	public void AddPlayer(EntityPlayer _player)
	{
		if (!this.trackedPlayers.dict.ContainsKey(_player.entityId))
		{
			AIDirectorPlayerState aidirectorPlayerState = this.playerPool.Alloc(false);
			if (aidirectorPlayerState != null)
			{
				this.trackedPlayers.Add(_player.entityId, aidirectorPlayerState.Construct(_player));
			}
		}
	}

	// Token: 0x06001D61 RID: 7521 RVA: 0x000B793C File Offset: 0x000B5B3C
	public void RemovePlayer(EntityPlayer _player)
	{
		AIDirectorPlayerState aidirectorPlayerState;
		if (this.trackedPlayers.dict.TryGetValue(_player.entityId, out aidirectorPlayerState))
		{
			this.trackedPlayers.Remove(_player.entityId);
			aidirectorPlayerState.Reset();
			this.playerPool.Free(aidirectorPlayerState);
		}
	}

	// Token: 0x06001D62 RID: 7522 RVA: 0x000B7988 File Offset: 0x000B5B88
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickPlayerStates(double _dt)
	{
		for (int i = 0; i < this.trackedPlayers.list.Count; i++)
		{
			AIDirectorPlayerState ps = this.trackedPlayers.list[i];
			this.TickPlayerState(ps, _dt);
		}
	}

	// Token: 0x06001D63 RID: 7523 RVA: 0x000B79CC File Offset: 0x000B5BCC
	public void UpdatePlayerInventory(int entityId, AIDirectorPlayerInventory inventory)
	{
		AIDirectorPlayerState aidirectorPlayerState;
		if (this.trackedPlayers.dict.TryGetValue(entityId, out aidirectorPlayerState))
		{
			aidirectorPlayerState.Inventory = inventory;
		}
	}

	// Token: 0x06001D64 RID: 7524 RVA: 0x000B79F5 File Offset: 0x000B5BF5
	public void UpdatePlayerInventory(EntityPlayerLocal player)
	{
		this.UpdatePlayerInventory(player.entityId, AIDirectorPlayerInventory.FromEntity(player));
	}

	// Token: 0x06001D65 RID: 7525 RVA: 0x000B7A09 File Offset: 0x000B5C09
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickPlayerState(AIDirectorPlayerState _ps, double _dt)
	{
		_ps.Dead = _ps.Player.IsDead();
		if (_ps.Dead)
		{
			return;
		}
		_ps.EmitSmell(_dt);
	}

	// Token: 0x0400141D RID: 5149
	public DictionaryList<int, AIDirectorPlayerState> trackedPlayers = new DictionaryList<int, AIDirectorPlayerState>();

	// Token: 0x0400141E RID: 5150
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryPooledObject<AIDirectorPlayerState> playerPool = new MemoryPooledObject<AIDirectorPlayerState>(32);
}

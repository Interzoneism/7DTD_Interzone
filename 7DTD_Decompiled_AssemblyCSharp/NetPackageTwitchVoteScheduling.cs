using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x020007A3 RID: 1955
[Preserve]
public class NetPackageTwitchVoteScheduling : NetPackage
{
	// Token: 0x0600389B RID: 14491 RVA: 0x00112051 File Offset: 0x00110251
	public NetPackageTwitchVoteScheduling Setup()
	{
		return this;
	}

	// Token: 0x0600389C RID: 14492 RVA: 0x00002914 File Offset: 0x00000B14
	public override void read(PooledBinaryReader _br)
	{
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x00161C1F File Offset: 0x0015FE1F
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x0017058C File Offset: 0x0016E78C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			TwitchVoteScheduler.Current.AddParticipant(base.Sender.entityId);
			return;
		}
		TwitchManager.Current.VotingManager.RequestApprovedToStart();
	}

	// Token: 0x0600389F RID: 14495 RVA: 0x000F298B File Offset: 0x000F0B8B
	public override int GetLength()
	{
		return 30;
	}
}

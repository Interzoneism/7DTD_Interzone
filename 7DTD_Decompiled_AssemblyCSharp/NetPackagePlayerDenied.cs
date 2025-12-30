using System;
using UnityEngine.Scripting;

// Token: 0x0200076F RID: 1903
[Preserve]
public class NetPackagePlayerDenied : NetPackage
{
	// Token: 0x1700059D RID: 1437
	// (get) Token: 0x0600375A RID: 14170 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool FlushQueue
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700059E RID: 1438
	// (get) Token: 0x0600375B RID: 14171 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedBeforeAuth
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600375C RID: 14172 RVA: 0x0016AAA4 File Offset: 0x00168CA4
	public NetPackagePlayerDenied Setup(GameUtils.KickPlayerData _kickData)
	{
		this.kickData = _kickData;
		return this;
	}

	// Token: 0x0600375D RID: 14173 RVA: 0x0016AAB0 File Offset: 0x00168CB0
	public override void read(PooledBinaryReader _reader)
	{
		this.kickData.reason = (GameUtils.EKickReason)_reader.ReadInt32();
		this.kickData.apiResponseEnum = _reader.ReadInt32();
		this.kickData.banUntil = DateTime.FromBinary(_reader.ReadInt64());
		this.kickData.customReason = _reader.ReadString();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			ThreadManager.AddSingleTaskMainThread("PlayerDenied.ProcessPackage", delegate(object _taskInfo)
			{
				this.ProcessPackage(GameManager.Instance.World, GameManager.Instance);
			}, null);
		}
	}

	// Token: 0x0600375E RID: 14174 RVA: 0x0016AB2C File Offset: 0x00168D2C
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((int)this.kickData.reason);
		_writer.Write(this.kickData.apiResponseEnum);
		_writer.Write(this.kickData.banUntil.ToBinary());
		_writer.Write(this.kickData.customReason);
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x0016AB89 File Offset: 0x00168D89
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!this.processed)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
			_callbacks.ShowMessagePlayerDenied(this.kickData);
			this.processed = true;
		}
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x1700059F RID: 1439
	// (get) Token: 0x06003761 RID: 14177 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002CE9 RID: 11497
	public GameUtils.KickPlayerData kickData;

	// Token: 0x04002CEA RID: 11498
	[PublicizedFrom(EAccessModifier.Private)]
	public bool processed;
}

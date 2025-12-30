using System;
using System.Threading;
using UnityEngine.Scripting;

// Token: 0x020006F2 RID: 1778
[Preserve]
public abstract class NetPackage
{
	// Token: 0x1700053F RID: 1343
	// (get) Token: 0x06003462 RID: 13410 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual int Channel
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x17000540 RID: 1344
	// (get) Token: 0x06003463 RID: 13411 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool Compress
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000541 RID: 1345
	// (get) Token: 0x06003464 RID: 13412 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool FlushQueue
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000542 RID: 1346
	// (get) Token: 0x06003465 RID: 13413 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.Both;
		}
	}

	// Token: 0x17000543 RID: 1347
	// (get) Token: 0x06003466 RID: 13414 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool AllowedBeforeAuth
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000544 RID: 1348
	// (get) Token: 0x06003467 RID: 13415 RVA: 0x00160BD5 File Offset: 0x0015EDD5
	public int PackageId
	{
		get
		{
			return NetPackageManager.GetPackageId(base.GetType());
		}
	}

	// Token: 0x17000545 RID: 1349
	// (get) Token: 0x06003468 RID: 13416 RVA: 0x00160BE2 File Offset: 0x0015EDE2
	// (set) Token: 0x06003469 RID: 13417 RVA: 0x00160BEA File Offset: 0x0015EDEA
	public ClientInfo Sender { get; set; }

	// Token: 0x17000546 RID: 1350
	// (get) Token: 0x0600346A RID: 13418 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool ReliableDelivery
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600346B RID: 13419
	public abstract void read(PooledBinaryReader _reader);

	// Token: 0x0600346C RID: 13420 RVA: 0x00160BF3 File Offset: 0x0015EDF3
	public virtual void write(PooledBinaryWriter _writer)
	{
		_writer.Write((byte)this.PackageId);
	}

	// Token: 0x0600346D RID: 13421
	public abstract void ProcessPackage(World _world, GameManager _callbacks);

	// Token: 0x0600346E RID: 13422
	public abstract int GetLength();

	// Token: 0x0600346F RID: 13423 RVA: 0x00160C04 File Offset: 0x0015EE04
	public override string ToString()
	{
		string result;
		if ((result = this.classnameCached) == null)
		{
			result = (this.classnameCached = base.GetType().Name);
		}
		return result;
	}

	// Token: 0x06003470 RID: 13424 RVA: 0x00160C2F File Offset: 0x0015EE2F
	public void RegisterSendQueue()
	{
		Interlocked.Increment(ref this.inSendQueuesCount);
	}

	// Token: 0x06003471 RID: 13425 RVA: 0x00160C3D File Offset: 0x0015EE3D
	public void SendQueueHandled()
	{
		if (Interlocked.Decrement(ref this.inSendQueuesCount) == 0)
		{
			NetPackageManager.FreePackage(this);
		}
	}

	// Token: 0x06003472 RID: 13426 RVA: 0x00160C54 File Offset: 0x0015EE54
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool ValidEntityIdForSender(int _entityId, bool _allowAttachedToEntity = false)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return true;
		}
		if (_entityId == this.Sender.entityId)
		{
			return true;
		}
		if (_allowAttachedToEntity)
		{
			EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(this.Sender.entityId) as EntityPlayer;
			if (entityPlayer != null && entityPlayer.AttachedToEntity != null && entityPlayer.AttachedToEntity.entityId == _entityId)
			{
				return true;
			}
		}
		Log.Warning(string.Format("Received {0} with invalid entityId {1} from {2}", this.ToString(), _entityId, this.Sender));
		return false;
	}

	// Token: 0x06003473 RID: 13427 RVA: 0x00160CEC File Offset: 0x0015EEEC
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool ValidUserIdForSender(PlatformUserIdentifierAbs _userId)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return true;
		}
		if (object.Equals(_userId, this.Sender.PlatformId) || object.Equals(_userId, this.Sender.CrossplatformId))
		{
			return true;
		}
		Log.Warning(string.Format("Received {0} with invalid userId {1} from {2}", this.ToString(), _userId, this.Sender));
		return false;
	}

	// Token: 0x06003474 RID: 13428 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public NetPackage()
	{
	}

	// Token: 0x04002AF4 RID: 10996
	[PublicizedFrom(EAccessModifier.Private)]
	public int inSendQueuesCount;

	// Token: 0x04002AF5 RID: 10997
	[PublicizedFrom(EAccessModifier.Private)]
	public string classnameCached;
}

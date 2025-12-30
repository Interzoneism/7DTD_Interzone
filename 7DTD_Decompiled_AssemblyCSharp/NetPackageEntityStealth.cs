using System;
using UnityEngine.Scripting;

// Token: 0x02000744 RID: 1860
[Preserve]
public class NetPackageEntityStealth : NetPackage
{
	// Token: 0x17000581 RID: 1409
	// (get) Token: 0x0600365C RID: 13916 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.Both;
		}
	}

	// Token: 0x0600365D RID: 13917 RVA: 0x00166BE8 File Offset: 0x00164DE8
	public NetPackageEntityStealth Setup(EntityPlayer player, bool _isCrouching)
	{
		this.id = player.entityId;
		this.data = (_isCrouching ? 1 : 0);
		return this;
	}

	// Token: 0x0600365E RID: 13918 RVA: 0x00166C04 File Offset: 0x00164E04
	public NetPackageEntityStealth Setup(EntityPlayer player, int _lightLevel, int _noiseVolume, bool _isAlert)
	{
		this.id = player.entityId;
		this.data = (ushort)((int)((byte)_lightLevel) | (_noiseVolume & 127) << 8);
		if (_isAlert)
		{
			this.data |= 32768;
		}
		return this;
	}

	// Token: 0x0600365F RID: 13919 RVA: 0x00166C3A File Offset: 0x00164E3A
	public override void read(PooledBinaryReader _br)
	{
		this.id = _br.ReadInt32();
		this.data = _br.ReadUInt16();
	}

	// Token: 0x06003660 RID: 13920 RVA: 0x00166C54 File Offset: 0x00164E54
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.id);
		_bw.Write(this.data);
	}

	// Token: 0x06003661 RID: 13921 RVA: 0x00166C78 File Offset: 0x00164E78
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.id, false))
		{
			return;
		}
		EntityPlayer entityPlayer = _world.GetEntity(this.id) as EntityPlayer;
		if (entityPlayer == null)
		{
			Log.Out("Discarding " + base.GetType().Name);
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			entityPlayer.Crouching = ((this.data & 1) > 0);
			return;
		}
		float lightLevel = (float)((byte)this.data);
		float noiseVolume = (float)(this.data >> 8 & 127);
		entityPlayer.Stealth.SetClientLevels(lightLevel, noiseVolume, (this.data & 32768) > 0);
	}

	// Token: 0x06003662 RID: 13922 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002C1C RID: 11292
	[PublicizedFrom(EAccessModifier.Private)]
	public int id;

	// Token: 0x04002C1D RID: 11293
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFIsCrouching = 1;

	// Token: 0x04002C1E RID: 11294
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFIsAlert = 32768;

	// Token: 0x04002C1F RID: 11295
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort data;
}

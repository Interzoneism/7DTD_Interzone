using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000737 RID: 1847
[Preserve]
public class NetPackageEntityMapMarkerRemove : NetPackage
{
	// Token: 0x17000579 RID: 1401
	// (get) Token: 0x06003609 RID: 13833 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x0600360A RID: 13834 RVA: 0x001658F7 File Offset: 0x00163AF7
	public NetPackageEntityMapMarkerRemove Setup(EnumMapObjectType _mapObjectType, int _entityId)
	{
		this.mapObjectType = _mapObjectType;
		this.entityId = _entityId;
		this.RemoveByType = NetPackageEntityMapMarkerRemove.RemoveByTypes.EntityID;
		return this;
	}

	// Token: 0x0600360B RID: 13835 RVA: 0x0016590F File Offset: 0x00163B0F
	public NetPackageEntityMapMarkerRemove Setup(EnumMapObjectType _mapObjectType, Vector3 _position)
	{
		this.mapObjectType = _mapObjectType;
		this.position = _position;
		this.RemoveByType = NetPackageEntityMapMarkerRemove.RemoveByTypes.Position;
		return this;
	}

	// Token: 0x0600360C RID: 13836 RVA: 0x00165927 File Offset: 0x00163B27
	public override void read(PooledBinaryReader _reader)
	{
		this.RemoveByType = (NetPackageEntityMapMarkerRemove.RemoveByTypes)_reader.ReadInt32();
		if (this.RemoveByType == NetPackageEntityMapMarkerRemove.RemoveByTypes.EntityID)
		{
			this.entityId = _reader.ReadInt32();
		}
		else
		{
			this.position = StreamUtils.ReadVector3(_reader);
		}
		this.mapObjectType = (EnumMapObjectType)_reader.ReadInt32();
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x00165964 File Offset: 0x00163B64
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((int)this.RemoveByType);
		if (this.RemoveByType == NetPackageEntityMapMarkerRemove.RemoveByTypes.EntityID)
		{
			_writer.Write(this.entityId);
		}
		else
		{
			StreamUtils.Write(_writer, this.position);
		}
		_writer.Write((int)this.mapObjectType);
	}

	// Token: 0x0600360E RID: 13838 RVA: 0x001659B2 File Offset: 0x00163BB2
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (this.RemoveByType == NetPackageEntityMapMarkerRemove.RemoveByTypes.EntityID)
		{
			_world.ObjectOnMapRemove(this.mapObjectType, this.entityId);
			return;
		}
		_world.ObjectOnMapRemove(this.mapObjectType, this.position);
	}

	// Token: 0x0600360F RID: 13839 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override int GetLength()
	{
		return 4;
	}

	// Token: 0x04002BEB RID: 11243
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002BEC RID: 11244
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 position;

	// Token: 0x04002BED RID: 11245
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumMapObjectType mapObjectType;

	// Token: 0x04002BEE RID: 11246
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageEntityMapMarkerRemove.RemoveByTypes RemoveByType;

	// Token: 0x02000738 RID: 1848
	[PublicizedFrom(EAccessModifier.Private)]
	public enum RemoveByTypes
	{
		// Token: 0x04002BF0 RID: 11248
		EntityID,
		// Token: 0x04002BF1 RID: 11249
		Position
	}
}

using System;
using UnityEngine.Scripting;

// Token: 0x020007A1 RID: 1953
[Preserve]
public class NetPackageTurretSync : NetPackage
{
	// Token: 0x0600388D RID: 14477 RVA: 0x001703A6 File Offset: 0x0016E5A6
	public NetPackageTurretSync Setup(int _entityId, int _targetEntityId, bool _isOn, ItemValue _originalItemValue)
	{
		this.entityId = _entityId;
		this.targetEntityId = _targetEntityId;
		this.isOn = _isOn;
		this.itemValue = _originalItemValue;
		return this;
	}

	// Token: 0x0600388E RID: 14478 RVA: 0x001703C8 File Offset: 0x0016E5C8
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.targetEntityId = _reader.ReadInt32();
		this.isOn = _reader.ReadBoolean();
		this.itemValue = ItemValue.None.Clone();
		this.itemValue.Read(_reader);
	}

	// Token: 0x0600388F RID: 14479 RVA: 0x00170415 File Offset: 0x0016E615
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.targetEntityId);
		_writer.Write(this.isOn);
		this.itemValue.Write(_writer);
	}

	// Token: 0x06003890 RID: 14480 RVA: 0x00170450 File Offset: 0x0016E650
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityTurret entityTurret = GameManager.Instance.World.GetEntity(this.entityId) as EntityTurret;
		if (entityTurret != null)
		{
			entityTurret.TargetEntityId = this.targetEntityId;
			entityTurret.OriginalItemValue = this.itemValue;
			entityTurret.IsOn = this.isOn;
		}
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x170005B5 RID: 1461
	// (get) Token: 0x06003892 RID: 14482 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002DD3 RID: 11731
	public int entityId;

	// Token: 0x04002DD4 RID: 11732
	public int targetEntityId;

	// Token: 0x04002DD5 RID: 11733
	public bool isOn;

	// Token: 0x04002DD6 RID: 11734
	public ItemValue itemValue;
}

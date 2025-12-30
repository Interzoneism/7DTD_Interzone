using System;
using UnityEngine.Scripting;

// Token: 0x0200072A RID: 1834
[Preserve]
public class NetPackageEmitSmell : NetPackage
{
	// Token: 0x060035B8 RID: 13752 RVA: 0x00164E39 File Offset: 0x00163039
	public NetPackageEmitSmell Setup(int entityId, string smellName)
	{
		this.EntityId = entityId;
		this.SmellName = smellName;
		return this;
	}

	// Token: 0x060035B9 RID: 13753 RVA: 0x00164E4A File Offset: 0x0016304A
	public override void read(PooledBinaryReader _reader)
	{
		this.EntityId = _reader.ReadInt32();
		this.SmellName = _reader.ReadString();
		if (string.IsNullOrEmpty(this.SmellName))
		{
			this.SmellName = null;
		}
	}

	// Token: 0x060035BA RID: 13754 RVA: 0x00164E78 File Offset: 0x00163078
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.EntityId);
		_writer.Write((this.SmellName != null) ? this.SmellName : "");
	}

	// Token: 0x060035BB RID: 13755 RVA: 0x00002914 File Offset: 0x00000B14
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
	}

	// Token: 0x060035BC RID: 13756 RVA: 0x000768A9 File Offset: 0x00074AA9
	public override int GetLength()
	{
		return 10;
	}

	// Token: 0x04002BC7 RID: 11207
	public int EntityId;

	// Token: 0x04002BC8 RID: 11208
	public string SmellName;
}

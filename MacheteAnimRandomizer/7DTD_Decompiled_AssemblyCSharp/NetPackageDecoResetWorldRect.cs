using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000718 RID: 1816
[Preserve]
public class NetPackageDecoResetWorldRect : NetPackage
{
	// Token: 0x17000569 RID: 1385
	// (get) Token: 0x0600355F RID: 13663 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003560 RID: 13664 RVA: 0x001639A0 File Offset: 0x00161BA0
	public NetPackageDecoResetWorldRect Setup(Rect _worldRect)
	{
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(this.ms);
			pooledBinaryWriter.Write((int)_worldRect.x);
			pooledBinaryWriter.Write((int)_worldRect.y);
			pooledBinaryWriter.Write((int)_worldRect.width);
			pooledBinaryWriter.Write((int)_worldRect.height);
		}
		return this;
	}

	// Token: 0x06003561 RID: 13665 RVA: 0x00163A1C File Offset: 0x00161C1C
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~NetPackageDecoResetWorldRect()
	{
		MemoryPools.poolMemoryStream.FreeSync(this.ms);
	}

	// Token: 0x06003562 RID: 13666 RVA: 0x00163A54 File Offset: 0x00161C54
	public override void read(PooledBinaryReader _br)
	{
		int length = _br.ReadInt32();
		StreamUtils.StreamCopy(_br.BaseStream, this.ms, length, null, true);
	}

	// Token: 0x06003563 RID: 13667 RVA: 0x00163A7C File Offset: 0x00161C7C
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((int)this.ms.Length);
		this.ms.WriteTo(_bw.BaseStream);
	}

	// Token: 0x06003564 RID: 13668 RVA: 0x00163AA8 File Offset: 0x00161CA8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			PooledExpandableMemoryStream obj = this.ms;
			lock (obj)
			{
				pooledBinaryReader.SetBaseStream(this.ms);
				this.ms.Position = 0L;
				int num = pooledBinaryReader.ReadInt32();
				int num2 = pooledBinaryReader.ReadInt32();
				int num3 = pooledBinaryReader.ReadInt32();
				int num4 = pooledBinaryReader.ReadInt32();
				Rect worldRect = new Rect((float)num, (float)num2, (float)num3, (float)num4);
				DecoManager.Instance.ResetDecosInWorldRect(worldRect);
			}
		}
	}

	// Token: 0x06003565 RID: 13669 RVA: 0x00163B5C File Offset: 0x00161D5C
	public override int GetLength()
	{
		return (int)this.ms.Length;
	}

	// Token: 0x04002B85 RID: 11141
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledExpandableMemoryStream ms = MemoryPools.poolMemoryStream.AllocSync(true);
}

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000719 RID: 1817
[Preserve]
public class NetPackageDecoUpdate : NetPackage
{
	// Token: 0x1700056A RID: 1386
	// (get) Token: 0x06003567 RID: 13671 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003568 RID: 13672 RVA: 0x00163B84 File Offset: 0x00161D84
	public NetPackageDecoUpdate Setup(List<DecoObject> _decoList, ref int _currentIndex)
	{
		this.firstPackage = (_currentIndex == 0);
		int num = Math.Min(32768, _decoList.Count - _currentIndex);
		int num2 = _currentIndex + num;
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(this.ms);
			pooledBinaryWriter.Write(num);
			for (int i = _currentIndex; i < num2; i++)
			{
				_decoList[i].Write(pooledBinaryWriter, null);
			}
		}
		_currentIndex = num2;
		return this;
	}

	// Token: 0x06003569 RID: 13673 RVA: 0x00163C10 File Offset: 0x00161E10
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~NetPackageDecoUpdate()
	{
		MemoryPools.poolMemoryStream.FreeSync(this.ms);
	}

	// Token: 0x0600356A RID: 13674 RVA: 0x00163C48 File Offset: 0x00161E48
	public override void read(PooledBinaryReader _br)
	{
		this.firstPackage = _br.ReadBoolean();
		int length = _br.ReadInt32();
		StreamUtils.StreamCopy(_br.BaseStream, this.ms, length, null, true);
	}

	// Token: 0x0600356B RID: 13675 RVA: 0x00163C7C File Offset: 0x00161E7C
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.firstPackage);
		_bw.Write((int)this.ms.Length);
		this.ms.WriteTo(_bw.BaseStream);
	}

	// Token: 0x0600356C RID: 13676 RVA: 0x00163CB4 File Offset: 0x00161EB4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			PooledExpandableMemoryStream obj = this.ms;
			lock (obj)
			{
				pooledBinaryReader.SetBaseStream(this.ms);
				this.ms.Position = 0L;
				DecoManager.Instance.Read(pooledBinaryReader, int.MaxValue, this.firstPackage);
			}
		}
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x00163D40 File Offset: 0x00161F40
	public override int GetLength()
	{
		return (int)this.ms.Length;
	}

	// Token: 0x04002B86 RID: 11142
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledExpandableMemoryStream ms = MemoryPools.poolMemoryStream.AllocSync(true);

	// Token: 0x04002B87 RID: 11143
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstPackage = true;

	// Token: 0x04002B88 RID: 11144
	[PublicizedFrom(EAccessModifier.Private)]
	public const int decoSize = 16;

	// Token: 0x04002B89 RID: 11145
	[PublicizedFrom(EAccessModifier.Private)]
	public const int decosPerPackage = 32768;
}

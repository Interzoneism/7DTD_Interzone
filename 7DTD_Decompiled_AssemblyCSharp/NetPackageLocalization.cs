using System;
using System.IO;
using Noemax.GZip;
using UnityEngine.Scripting;

// Token: 0x0200075A RID: 1882
[Preserve]
public class NetPackageLocalization : NetPackage
{
	// Token: 0x1700058F RID: 1423
	// (get) Token: 0x060036E1 RID: 14049 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool Compress
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060036E2 RID: 14050 RVA: 0x00168A3D File Offset: 0x00166C3D
	public NetPackageLocalization Setup(byte[] _data)
	{
		if (_data.Length > 2097152)
		{
			Log.Error(string.Format("Modded Localization larger than allowed in compressed form: Allowed {0} B, got {1} B.", 2097152, _data.Length));
		}
		this.data = _data;
		return this;
	}

	// Token: 0x060036E3 RID: 14051 RVA: 0x00168A74 File Offset: 0x00166C74
	public override void read(PooledBinaryReader _reader)
	{
		int count = _reader.ReadInt32();
		this.data = _reader.ReadBytes(count);
		using (MemoryStream memoryStream = new MemoryStream(this.data))
		{
			using (DeflateInputStream deflateInputStream = new DeflateInputStream(memoryStream))
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					StreamUtils.StreamCopy(deflateInputStream, memoryStream2, null, true);
					this.data = memoryStream2.ToArray();
				}
			}
		}
	}

	// Token: 0x060036E4 RID: 14052 RVA: 0x00168B0C File Offset: 0x00166D0C
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.data.Length);
		_writer.Write(this.data);
	}

	// Token: 0x060036E5 RID: 14053 RVA: 0x00168B2F File Offset: 0x00166D2F
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		Localization.LoadServerPatchDictionary(this.data);
	}

	// Token: 0x060036E6 RID: 14054 RVA: 0x00168B3D File Offset: 0x00166D3D
	public override int GetLength()
	{
		return this.data.Length;
	}

	// Token: 0x17000590 RID: 1424
	// (get) Token: 0x060036E7 RID: 14055 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002C81 RID: 11393
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] data;
}

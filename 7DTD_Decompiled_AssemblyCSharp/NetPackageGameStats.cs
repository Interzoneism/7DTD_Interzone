using System;
using System.Collections;
using UnityEngine.Scripting;

// Token: 0x0200074F RID: 1871
[Preserve]
public class NetPackageGameStats : NetPackage
{
	// Token: 0x0600369A RID: 13978 RVA: 0x00167EEC File Offset: 0x001660EC
	public NetPackageGameStats Setup(GameStats _gs)
	{
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(this.ms);
			_gs.Write(pooledBinaryWriter);
		}
		return this;
	}

	// Token: 0x0600369B RID: 13979 RVA: 0x00167F38 File Offset: 0x00166138
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~NetPackageGameStats()
	{
		MemoryPools.poolMemoryStream.FreeSync(this.ms);
	}

	// Token: 0x0600369C RID: 13980 RVA: 0x00167F70 File Offset: 0x00166170
	public override void read(PooledBinaryReader _reader)
	{
		int length = (int)_reader.ReadInt16();
		StreamUtils.StreamCopy(_reader.BaseStream, this.ms, length, null, true);
	}

	// Token: 0x0600369D RID: 13981 RVA: 0x00167F98 File Offset: 0x00166198
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((short)this.ms.Length);
		this.ms.WriteTo(_writer.BaseStream);
	}

	// Token: 0x0600369E RID: 13982 RVA: 0x00167FC4 File Offset: 0x001661C4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		ThreadManager.StartCoroutine(this.readStatsCo());
	}

	// Token: 0x0600369F RID: 13983 RVA: 0x00167FD2 File Offset: 0x001661D2
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator readStatsCo()
	{
		while (GameManager.Instance.World == null && SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield return null;
		}
		if (GameManager.Instance.World == null)
		{
			yield break;
		}
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			PooledExpandableMemoryStream obj = this.ms;
			lock (obj)
			{
				pooledBinaryReader.SetBaseStream(this.ms);
				this.ms.Position = 0L;
				GameStats.Instance.Read(pooledBinaryReader);
			}
		}
		GameManager.Instance.GetGameStateManager().OnUpdateTick();
		yield break;
	}

	// Token: 0x060036A0 RID: 13984 RVA: 0x00167FE1 File Offset: 0x001661E1
	public override int GetLength()
	{
		return (int)this.ms.Length;
	}

	// Token: 0x17000587 RID: 1415
	// (get) Token: 0x060036A1 RID: 13985 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002C60 RID: 11360
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledExpandableMemoryStream ms = MemoryPools.poolMemoryStream.AllocSync(true);
}

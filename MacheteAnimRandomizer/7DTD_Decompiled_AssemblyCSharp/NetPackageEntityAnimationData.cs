using System;
using System.Collections.Generic;
using UnityEngine.Profiling;
using UnityEngine.Scripting;

// Token: 0x02000733 RID: 1843
[Preserve]
public class NetPackageEntityAnimationData : NetPackage, IMemoryPoolableObject
{
	// Token: 0x060035EC RID: 13804 RVA: 0x001654FE File Offset: 0x001636FE
	public NetPackageEntityAnimationData Setup(int _entityId, List<AnimParamData> _animationParameterData)
	{
		this.entityId = _entityId;
		_animationParameterData.CopyTo(this.animationParameterData);
		return this;
	}

	// Token: 0x060035ED RID: 13805 RVA: 0x00165514 File Offset: 0x00163714
	public NetPackageEntityAnimationData Setup(int _entityId, Dictionary<int, AnimParamData> _animationParameterData)
	{
		this.entityId = _entityId;
		_animationParameterData.CopyValuesTo(this.animationParameterData);
		return this;
	}

	// Token: 0x060035EE RID: 13806 RVA: 0x0016552C File Offset: 0x0016372C
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.animationParameterData.Count);
		for (int i = 0; i < this.animationParameterData.Count; i++)
		{
			this.animationParameterData[i].Write(_writer);
		}
	}

	// Token: 0x060035EF RID: 13807 RVA: 0x00165588 File Offset: 0x00163788
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		int num = _reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			this.animationParameterData.Add(AnimParamData.CreateFromBinary(_reader));
		}
	}

	// Token: 0x060035F0 RID: 13808 RVA: 0x001655C8 File Offset: 0x001637C8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.entityId) as EntityAlive;
		if (entityAlive == null || !entityAlive.isEntityRemote)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAnimationData>().Setup(this.entityId, this.animationParameterData), false, -1, this.entityId, this.entityId, null, 192, false);
		}
		if (entityAlive.emodel == null)
		{
			return;
		}
		AvatarController avatarController = entityAlive.emodel.avatarController;
		if (avatarController == null)
		{
			return;
		}
		List<AnimParamData> list = new List<AnimParamData>();
		this.animationParameterData.CopyTo(list);
		avatarController.SetAnimParameters(list);
	}

	// Token: 0x060035F1 RID: 13809 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x060035F2 RID: 13810 RVA: 0x00165684 File Offset: 0x00163884
	public void Reset()
	{
		this.entityId = 0;
		this.animationParameterData.Clear();
	}

	// Token: 0x060035F3 RID: 13811 RVA: 0x00165698 File Offset: 0x00163898
	public void Cleanup()
	{
		this.Reset();
	}

	// Token: 0x04002BE2 RID: 11234
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002BE3 RID: 11235
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<AnimParamData> animationParameterData = new List<AnimParamData>();

	// Token: 0x04002BE4 RID: 11236
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CustomSampler getSampler = CustomSampler.Create("NetPackageEntityAnimationData.read", false);
}

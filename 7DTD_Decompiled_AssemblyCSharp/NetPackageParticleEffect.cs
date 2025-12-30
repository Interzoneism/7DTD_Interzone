using System;
using UnityEngine.Scripting;

// Token: 0x02000767 RID: 1895
[Preserve]
public class NetPackageParticleEffect : NetPackage
{
	// Token: 0x06003733 RID: 14131 RVA: 0x00169E52 File Offset: 0x00168052
	public NetPackageParticleEffect Setup(ParticleEffect _pe, int _entityThatCausedIt, bool _forceCreation = false, bool _worldSpawn = false)
	{
		this.pe = _pe;
		this.entityThatCausedIt = _entityThatCausedIt;
		this.worldSpawn = _worldSpawn;
		this.forceCreation = _forceCreation;
		return this;
	}

	// Token: 0x06003734 RID: 14132 RVA: 0x00169E72 File Offset: 0x00168072
	public override void read(PooledBinaryReader _br)
	{
		this.pe = new ParticleEffect();
		this.pe.Read(_br);
		this.entityThatCausedIt = _br.ReadInt32();
		this.forceCreation = _br.ReadBoolean();
		this.worldSpawn = _br.ReadBoolean();
	}

	// Token: 0x06003735 RID: 14133 RVA: 0x00169EAF File Offset: 0x001680AF
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		this.pe.Write(_bw);
		_bw.Write(this.entityThatCausedIt);
		_bw.Write(this.forceCreation);
		_bw.Write(this.worldSpawn);
	}

	// Token: 0x06003736 RID: 14134 RVA: 0x00169EE8 File Offset: 0x001680E8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			_world.GetGameManager().SpawnParticleEffectServer(this.pe, this.entityThatCausedIt, this.forceCreation, this.worldSpawn);
			return;
		}
		_world.GetGameManager().SpawnParticleEffectClient(this.pe, this.entityThatCausedIt, this.forceCreation, this.worldSpawn);
	}

	// Token: 0x06003737 RID: 14135 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002CBD RID: 11453
	[PublicizedFrom(EAccessModifier.Private)]
	public ParticleEffect pe;

	// Token: 0x04002CBE RID: 11454
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityThatCausedIt;

	// Token: 0x04002CBF RID: 11455
	[PublicizedFrom(EAccessModifier.Private)]
	public bool forceCreation;

	// Token: 0x04002CC0 RID: 11456
	[PublicizedFrom(EAccessModifier.Private)]
	public bool worldSpawn;
}

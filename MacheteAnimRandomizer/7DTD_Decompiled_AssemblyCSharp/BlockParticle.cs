using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000117 RID: 279
[Preserve]
public class BlockParticle : Block
{
	// Token: 0x060007A3 RID: 1955 RVA: 0x00035BF7 File Offset: 0x00033DF7
	public BlockParticle()
	{
		base.IsNotifyOnLoadUnload = true;
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x00035C08 File Offset: 0x00033E08
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("ParticleName"))
		{
			this.particleName = base.Properties.Values["ParticleName"];
			ParticleEffect.LoadAsset(this.particleName);
		}
		if (base.Properties.Values.ContainsKey("ParticleOffset"))
		{
			this.offset = StringParsers.ParseVector3(base.Properties.Values["ParticleOffset"], 0, -1);
		}
		if (GameManager.IsDedicatedServer && this.particleName != null && this.particleName.Length > 0)
		{
			if (BlockParticle.particleLights == null)
			{
				BlockParticle.particleLights = new Dictionary<int, List<Light>>();
			}
			this.particleId = ParticleEffect.ToId(this.particleName);
			if (!BlockParticle.particleLights.ContainsKey(this.particleId))
			{
				BlockParticle.particleLights.Add(this.particleId, new List<Light>());
				Transform dynamicTransform = ParticleEffect.GetDynamicTransform(this.particleId);
				if (dynamicTransform != null)
				{
					Light[] componentsInChildren = dynamicTransform.GetComponentsInChildren<Light>();
					if (componentsInChildren != null)
					{
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							BlockParticle.particleLights[this.particleId].Add(componentsInChildren[i]);
						}
					}
				}
			}
		}
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x00035D45 File Offset: 0x00033F45
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		this.removeParticles(_world, _blockPos.x, _blockPos.y, _blockPos.z, _blockValue);
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x00035D6D File Offset: 0x00033F6D
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (!_chunk.NeedsDecoration)
		{
			this.checkParticles(_world, _chunk.ClrIdx, _blockPos, _blockValue);
		}
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x00035D98 File Offset: 0x00033F98
	public override void OnNeighborBlockChange(WorldBase world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		Transform blockParticleEffect;
		if (_myBlockPos == _blockPosThatChanged + Vector3i.up && _newNeighborBlockValue.Block.shape.IsTerrain() && _myBlockValue.Block.IsTerrainDecoration && this.particleName != null && (blockParticleEffect = world.GetGameManager().GetBlockParticleEffect(_myBlockPos)) != null)
		{
			float num = 0f;
			if (_myBlockPos.y > 0)
			{
				sbyte density = world.GetDensity(_clrIdx, _myBlockPos.x, _myBlockPos.y, _myBlockPos.z);
				sbyte density2 = world.GetDensity(_clrIdx, _myBlockPos.x, _myBlockPos.y - 1, _myBlockPos.z);
				num = MarchingCubes.GetDecorationOffsetY(density, density2);
			}
			blockParticleEffect.localPosition = new Vector3((float)_myBlockPos.x, (float)_myBlockPos.y + num, (float)_myBlockPos.z) + this.getParticleOffset(_myBlockValue);
		}
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x00035E80 File Offset: 0x00034080
	public override void OnBlockValueChanged(WorldBase world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		Transform blockParticleEffect;
		if (_oldBlockValue.rotation != _newBlockValue.rotation && this.particleName != null && (blockParticleEffect = world.GetGameManager().GetBlockParticleEffect(_blockPos)) != null)
		{
			Vector3 particleOffset = this.getParticleOffset(_oldBlockValue);
			Vector3 particleOffset2 = this.getParticleOffset(_newBlockValue);
			blockParticleEffect.localPosition -= particleOffset;
			blockParticleEffect.localPosition += particleOffset2;
			blockParticleEffect.localRotation = this.shape.GetRotation(_newBlockValue);
		}
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x00035F14 File Offset: 0x00034114
	public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);
		if (this.particleName != null)
		{
			if (GameManager.IsDedicatedServer && BlockParticle.particleLights.ContainsKey(this.particleId))
			{
				List<Light> list = BlockParticle.particleLights[this.particleId];
				if (list.Count > 0)
				{
					Vector3 a;
					a.x = (float)_blockPos.x;
					a.y = (float)_blockPos.y;
					a.z = (float)_blockPos.z;
					this.dediLights = new List<Light>();
					for (int i = 0; i < list.Count; i++)
					{
						Light light = UnityEngine.Object.Instantiate<Light>(list[i]);
						Transform transform = light.transform;
						transform.position = a + this.getParticleOffset(_blockValue) - Origin.position;
						transform.parent = BlockParticle.hierarchyParentT;
						this.dediLights.Add(light);
						LightManager.RegisterLight(light);
					}
				}
			}
			this.checkParticles(_world, _clrIdx, _blockPos, _blockValue);
		}
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x00036012 File Offset: 0x00034212
	public override void OnBlockUnloaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockUnloaded(_world, _clrIdx, _blockPos, _blockValue);
		this.removeParticles(_world, _blockPos.x, _blockPos.y, _blockPos.z, _blockValue);
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0003603C File Offset: 0x0003423C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual Vector3 getParticleOffset(BlockValue _blockValue)
	{
		return this.shape.GetRotation(_blockValue) * (this.offset - new Vector3(0.5f, 0.5f, 0.5f)) + new Vector3(0.5f, 0.5f, 0.5f);
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x00036092 File Offset: 0x00034292
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void checkParticles(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (this.particleName != null && !_world.GetGameManager().HasBlockParticleEffect(_blockPos))
		{
			this.addParticles(_world, _clrIdx, _blockPos.x, _blockPos.y, _blockPos.z, _blockValue);
		}
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x000360C8 File Offset: 0x000342C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void addParticles(WorldBase _world, int _clrIdx, int _x, int _y, int _z, BlockValue _blockValue)
	{
		if (this.particleName == null || this.particleName == "")
		{
			return;
		}
		float num = 0f;
		if (_y > 0 && _blockValue.Block.IsTerrainDecoration && _world.GetBlock(_x, _y - 1, _z).Block.shape.IsTerrain())
		{
			sbyte density = _world.GetDensity(_clrIdx, _x, _y, _z);
			sbyte density2 = _world.GetDensity(_clrIdx, _x, _y - 1, _z);
			num = MarchingCubes.GetDecorationOffsetY(density, density2);
		}
		_world.GetGameManager().SpawnBlockParticleEffect(new Vector3i(_x, _y, _z), new ParticleEffect(this.particleName, new Vector3((float)_x, (float)_y + num, (float)_z) + this.getParticleOffset(_blockValue), this.shape.GetRotation(_blockValue), 1f, Color.white));
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x000361A0 File Offset: 0x000343A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void removeParticles(WorldBase _world, int _x, int _y, int _z, BlockValue _blockValue)
	{
		if (GameManager.IsDedicatedServer && this.dediLights != null)
		{
			for (int i = 0; i < this.dediLights.Count; i++)
			{
				LightManager.UnRegisterLight(this.dediLights[i].transform.position + Origin.position, this.dediLights[i].range);
				UnityEngine.Object.Destroy(this.dediLights[i]);
			}
			this.dediLights.Clear();
		}
		_world.GetGameManager().RemoveBlockParticleEffect(new Vector3i(_x, _y, _z));
	}

	// Token: 0x04000825 RID: 2085
	[PublicizedFrom(EAccessModifier.Private)]
	public string particleName;

	// Token: 0x04000826 RID: 2086
	[PublicizedFrom(EAccessModifier.Private)]
	public int particleId;

	// Token: 0x04000827 RID: 2087
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Light> dediLights;

	// Token: 0x04000828 RID: 2088
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 offset;

	// Token: 0x04000829 RID: 2089
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform hierarchyParentT = new GameObject("BlockParticleLights").transform;

	// Token: 0x0400082A RID: 2090
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<int, List<Light>> particleLights;
}

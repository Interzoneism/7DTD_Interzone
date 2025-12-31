using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

// Token: 0x02000180 RID: 384
[Preserve]
public class BlockShapeModelEntity : BlockShapeInvisible
{
	// Token: 0x06000B4F RID: 2895 RVA: 0x00049E92 File Offset: 0x00048092
	public BlockShapeModelEntity()
	{
		this.IsRotatable = true;
		this.IsNotifyOnLoadUnload = true;
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x00049EB4 File Offset: 0x000480B4
	public override void Init(Block _block)
	{
		base.Init(_block);
		this.modelNameWithPath = _block.Properties.Values["Model"];
		if (this.modelNameWithPath == null)
		{
			throw new Exception("No model specified on block with name " + _block.GetBlockName());
		}
		if (this.modelNameWithPath.Length > 0)
		{
			_block.Properties.ParseInt(EntityClass.PropCensor, ref this.censorMode);
			if (this.censorMode != 0 && GameManager.Instance && GameManager.Instance.IsGoreCensored())
			{
				if (this.modelNameWithPath.Contains("@"))
				{
					this.modelNameWithPath = this.modelNameWithPath.Replace(".", "_CGore.");
				}
				else if (!this.modelNameWithPath.Contains("."))
				{
					this.modelNameWithPath += "_CGore";
				}
			}
		}
		this.modelName = GameIO.GetFilenameFromPathWithoutExtension(this.modelNameWithPath);
		this.modelOffset = new Vector3(0f, 0.5f, 0f);
		_block.Properties.ParseVec("ModelOffset", ref this.modelOffset);
		_block.Properties.ParseFloat("LODCullScale", ref this.LODCullScale);
		_block.Properties.ParseInt("SymType", ref this.SymmetryType);
		string text;
		if (_block.Properties.Values.TryGetValue(BlockShapeModelEntity.PropDamagedMesh, out text))
		{
			string[] array = text.Split(',', StringSplitOptions.None);
			if (array.Length >= 2)
			{
				this.damageStates = new List<BlockShapeModelEntity.DamageState>();
				for (int i = 0; i < array.Length - 1; i += 2)
				{
					BlockShapeModelEntity.DamageState item;
					item.objName = array[i].Trim();
					item.health = float.Parse(array[i + 1]);
					this.damageStates.Add(item);
				}
			}
		}
		GameObjectPool.Instance.AddPooledObject(this.modelName, new GameObjectPool.LoadCallback(this.PoolLoadCallback), new GameObjectPool.CreateCallback(this.PoolCreateOnceToAllCallBack), new GameObjectPool.CreateCallback(this.PoolCreateCallBack));
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x0004A0B0 File Offset: 0x000482B0
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform PoolLoadCallback()
	{
		Transform prefab = this.getPrefab();
		if (prefab == null)
		{
			throw new Exception("Model '" + this.modelNameWithPath + "' not found on block with name " + this.block.GetBlockName());
		}
		return prefab;
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0004A0E8 File Offset: 0x000482E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void PoolCreateOnceToAllCallBack(GameObject obj)
	{
		Collider component = obj.transform.GetComponent<Collider>();
		if (component != null)
		{
			if (component is BoxCollider)
			{
				Vector3 center = ((BoxCollider)component).center;
				Vector3 size = ((BoxCollider)component).size;
				this.bounds = BoundsUtils.BoundsForMinMax(center.x - size.x / 2f, center.y - size.y / 2f, center.z - size.z / 2f, center.x + size.x / 2f, center.y + size.y / 2f, center.z + size.z / 2f);
				this.boundsArr[0] = this.bounds;
				this.isCustomBounds = true;
				return;
			}
			if (component is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = component as CapsuleCollider;
				Vector3 center2 = capsuleCollider.center;
				Vector3 vector = new Vector3(capsuleCollider.radius * 2f, capsuleCollider.height, capsuleCollider.radius * 2f);
				this.bounds = BoundsUtils.BoundsForMinMax(center2.x - vector.x / 2f, center2.y - vector.y / 2f, center2.z - vector.z / 2f, center2.x + vector.x / 2f, center2.y + vector.y / 2f, center2.z + vector.z / 2f);
				this.boundsArr[0] = this.bounds;
				this.isCustomBounds = true;
			}
		}
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0004A2A8 File Offset: 0x000484A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void PoolCreateCallBack(GameObject obj)
	{
		Transform transform = obj.transform;
		LODGroup lodgroup = transform.GetComponent<LODGroup>();
		if (lodgroup)
		{
			LODFadeMode fadeMode = lodgroup.fadeMode;
			if (fadeMode == LODFadeMode.SpeedTree)
			{
				return;
			}
			if (fadeMode == LODFadeMode.None)
			{
				lodgroup.fadeMode = LODFadeMode.CrossFade;
				lodgroup.animateCrossFading = true;
			}
			if (fadeMode == LODFadeMode.CrossFade)
			{
				lodgroup.animateCrossFading = true;
			}
			LOD[] lods = lodgroup.GetLODs();
			int num = lods.Length - 1;
			float num2 = lodgroup.size;
			if (num2 < 0.4f)
			{
				num2 *= 3.8f;
				if (num2 < 1f)
				{
					num2 = 1f;
				}
			}
			else if (num2 < 0.65f)
			{
				num2 *= 2.5f;
			}
			else if (num2 < 0.95f)
			{
				num2 *= 1.5f;
			}
			else if (num2 >= 1.45f)
			{
				if (num2 < 2.5f)
				{
					num2 *= 0.83f;
				}
				else if (num2 < 6.2f)
				{
					num2 *= 0.64f;
				}
				else
				{
					num2 *= 0.45f;
				}
			}
			float num3 = num2 * 0.02f * this.LODCullScale;
			if (num3 > 0.1f)
			{
				num3 = 0.1f;
			}
			lods[num].screenRelativeTransitionHeight = num3;
			if (num > 0)
			{
				float num4 = num3;
				for (int i = num - 1; i >= 0; i--)
				{
					float num5 = lods[i].screenRelativeTransitionHeight;
					if (num5 - 0.025f <= num4)
					{
						num5 = num4 + 0.025f;
						lods[i].screenRelativeTransitionHeight = num5;
					}
					num4 = num5;
				}
			}
			lodgroup.SetLODs(lods);
			if (num >= 2 && GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowDistance) <= 2)
			{
				foreach (Renderer renderer in lods[num].renderers)
				{
					if (renderer)
					{
						renderer.shadowCastingMode = ShadowCastingMode.Off;
					}
				}
				return;
			}
		}
		else if (transform.childCount == 0)
		{
			MeshRenderer component = obj.GetComponent<MeshRenderer>();
			if (component)
			{
				LOD lod;
				lod.screenRelativeTransitionHeight = 0.025f;
				lod.renderers = new Renderer[]
				{
					component
				};
				lod.fadeTransitionWidth = 0f;
				lodgroup = obj.AddComponent<LODGroup>();
				lodgroup.fadeMode = LODFadeMode.CrossFade;
				lodgroup.animateCrossFading = true;
				lodgroup.SetLODs(new LOD[]
				{
					lod
				});
			}
		}
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x0004A4EC File Offset: 0x000486EC
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform getPrefab()
	{
		Transform transform = DataLoader.LoadAsset<Transform>(this.modelNameWithPath, false);
		if (transform == null)
		{
			Log.Error("Model '{0}' not found on block with name {1}", new object[]
			{
				this.modelNameWithPath,
				this.block.GetBlockName()
			});
			transform = DataLoader.LoadAsset<Transform>("@:Entities/Misc/block_missingPrefab.prefab", false);
			if (transform == null)
			{
				return null;
			}
		}
		else
		{
			MeshLodOptimization.Apply(ref transform);
		}
		string filenameFromPathWithoutExtension = GameIO.GetFilenameFromPathWithoutExtension(this.modelNameWithPath);
		if (transform.name != filenameFromPathWithoutExtension)
		{
			Log.Error("Model has a wrong name '{0}'. Maybe check upper/lower case mismatch on block with name {1}?", new object[]
			{
				filenameFromPathWithoutExtension,
				this.block.GetBlockName()
			});
		}
		return transform;
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x0004A594 File Offset: 0x00048794
	public Transform CloneModel(BlockValue _blockValue, Transform _parent)
	{
		Transform transform = UnityEngine.Object.Instantiate<Transform>(this.getPrefab());
		transform.parent = _parent;
		Block block = _blockValue.Block;
		if (block.tintColor.a > 0f)
		{
			UpdateLight.SetTintColor(transform, block.tintColor);
		}
		Quaternion rotation = this.GetRotation(_blockValue);
		Vector3 rotatedOffset = this.GetRotatedOffset(block, rotation);
		transform.localPosition = rotatedOffset + new Vector3(0f, -0.5f, 0f);
		transform.localRotation = rotation;
		return transform;
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x0004A614 File Offset: 0x00048814
	public Vector3 GetRotatedOffset(Block block, Quaternion rot)
	{
		Vector3 vector = rot * this.modelOffset;
		Vector3 vector2 = Vector3.zero;
		vector2.y = -0.5f;
		if (block.isMultiBlock)
		{
			if ((block.multiBlockPos.dim.x & 1) == 0)
			{
				vector2.x = -0.5f;
			}
			if ((block.multiBlockPos.dim.z & 1) == 0)
			{
				vector2.z = -0.5f;
			}
		}
		vector2 = rot * vector2;
		vector += vector2;
		vector.y += 0.5f;
		return vector;
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x000497B1 File Offset: 0x000479B1
	public override Quaternion GetRotation(BlockValue _blockValue)
	{
		return BlockShapeNew.GetRotationStatic((int)_blockValue.rotation);
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x0004A6AC File Offset: 0x000488AC
	public override Bounds[] GetBounds(BlockValue _blockValue)
	{
		if (!this.isCustomBounds)
		{
			return base.GetBounds(_blockValue);
		}
		Quaternion rotation = this.GetRotation(_blockValue);
		Vector3 vector = rotation * this.bounds.min + this.modelOffset;
		Vector3 vector2 = rotation * this.bounds.max + this.modelOffset;
		this.boundsArr[0].min = new Vector3((vector2.x > vector.x) ? vector.x : vector2.x, (vector2.y > vector.y) ? vector.y : vector2.y, (vector2.z > vector.z) ? vector.z : vector2.z) + new Vector3(0.5f, 0f, 0.5f);
		this.boundsArr[0].max = new Vector3((vector2.x < vector.x) ? vector.x : vector2.x, (vector2.y < vector.y) ? vector.y : vector2.y, (vector2.z < vector.z) ? vector.z : vector2.z) + new Vector3(0.5f, 0f, 0.5f);
		return this.boundsArr;
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x0004A818 File Offset: 0x00048A18
	public override BlockValue RotateY(bool _bLeft, BlockValue _blockValue, int _rotCount)
	{
		if (_bLeft)
		{
			_rotCount = -_rotCount;
		}
		int rotation = (int)_blockValue.rotation;
		if (rotation >= 24)
		{
			_blockValue.rotation = (byte)((rotation - 24 + _rotCount & 3) + 24);
		}
		else
		{
			int num = 90 * _rotCount;
			_blockValue.rotation = (byte)BlockShapeNew.ConvertRotationFree(rotation, Quaternion.AngleAxis((float)num, Vector3.up), false);
		}
		return _blockValue;
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x0004A86F File Offset: 0x00048A6F
	public override byte Rotate(bool _bLeft, int _rotation)
	{
		_rotation += (_bLeft ? -1 : 1);
		if (_rotation > 10)
		{
			_rotation = 0;
		}
		if (_rotation < 0)
		{
			_rotation = 10;
		}
		return (byte)_rotation;
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x0004A890 File Offset: 0x00048A90
	public override BlockValue MirrorY(bool _bAlongX, BlockValue _blockValue)
	{
		if (!_bAlongX)
		{
			switch (_blockValue.rotation)
			{
			case 0:
				_blockValue.rotation = 2;
				break;
			case 1:
				_blockValue.rotation = 1;
				break;
			case 2:
				_blockValue.rotation = 0;
				break;
			case 3:
				_blockValue.rotation = 3;
				break;
			case 4:
				_blockValue.rotation = 7;
				break;
			case 5:
				_blockValue.rotation = 6;
				break;
			case 6:
				_blockValue.rotation = 5;
				break;
			case 7:
				_blockValue.rotation = 4;
				break;
			case 8:
				_blockValue.rotation = 8;
				break;
			case 9:
				_blockValue.rotation = 9;
				break;
			case 10:
				_blockValue.rotation = 10;
				break;
			case 11:
				_blockValue.rotation = 11;
				break;
			}
		}
		else
		{
			switch (_blockValue.rotation)
			{
			case 0:
				_blockValue.rotation = 0;
				break;
			case 1:
				_blockValue.rotation = 3;
				break;
			case 2:
				_blockValue.rotation = 2;
				break;
			case 3:
				_blockValue.rotation = 1;
				break;
			case 4:
				_blockValue.rotation = 7;
				break;
			case 5:
				_blockValue.rotation = 6;
				break;
			case 6:
				_blockValue.rotation = 5;
				break;
			case 7:
				_blockValue.rotation = 4;
				break;
			case 8:
				_blockValue.rotation = 8;
				break;
			case 9:
				_blockValue.rotation = 11;
				break;
			case 10:
				_blockValue.rotation = 10;
				break;
			case 11:
				_blockValue.rotation = 9;
				break;
			}
		}
		return _blockValue;
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x0004AA40 File Offset: 0x00048C40
	public override void OnBlockValueChanged(WorldBase _world, Vector3i _blockPos, int _clrIdx, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _blockPos, _clrIdx, _oldBlockValue, _newBlockValue);
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return;
		}
		Chunk chunk = (Chunk)chunkCluster.GetChunkFromWorldPos(_blockPos.x, _blockPos.y, _blockPos.z);
		if (chunk == null)
		{
			return;
		}
		BlockEntityData blockEntity = chunk.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return;
		}
		Block block = _newBlockValue.Block;
		if (_newBlockValue.rotation != _oldBlockValue.rotation)
		{
			blockEntity.transform.localRotation = block.shape.GetRotation(_newBlockValue);
		}
		blockEntity.blockValue = _newBlockValue;
		if (this.damageStates != null)
		{
			if (this.GetDamageStateIndex(_oldBlockValue) != this.GetDamageStateIndex(_newBlockValue))
			{
				this.UpdateDamageState(_oldBlockValue, _newBlockValue, blockEntity, true);
				return;
			}
		}
		else
		{
			int num = Mathf.Min(_newBlockValue.damage, block.MaxDamage) - 1;
			blockEntity.SetMaterialValue("_Damage", (float)num);
		}
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x0004AB24 File Offset: 0x00048D24
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue);
		_chunk.AddEntityBlockStub(new BlockEntityData(_blockValue, _blockPos)
		{
			bNeedsTemperature = true
		});
		this.registerSleepers(_blockPos, _blockValue);
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0004AB5C File Offset: 0x00048D5C
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		_chunk.RemoveEntityBlockStub(_blockPos);
		if (GameManager.Instance.IsEditMode() && _blockValue.Block.IsSleeperBlock)
		{
			Prefab.TransientSleeperBlockIncrement(_blockPos, -1);
			SleeperVolumeToolManager.UnRegisterSleeperBlock(_blockPos);
		}
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0004AB98 File Offset: 0x00048D98
	public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return;
		}
		Chunk chunk = (Chunk)chunkCluster.GetChunkFromWorldPos(_blockPos);
		if (chunk == null)
		{
			return;
		}
		chunk.AddEntityBlockStub(new BlockEntityData(_blockValue, _blockPos)
		{
			bNeedsTemperature = true
		});
		this.registerSleepers(_blockPos, _blockValue);
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x0004ABF4 File Offset: 0x00048DF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void registerSleepers(Vector3i _blockPos, BlockValue _blockValue)
	{
		if (GameManager.Instance.IsEditMode() && _blockValue.Block.IsSleeperBlock)
		{
			Prefab.TransientSleeperBlockIncrement(_blockPos, 1);
			ThreadManager.AddSingleTaskMainThread("OnBlockAddedOrLoaded.RegisterSleeperBlock", delegate
			{
				SleeperVolumeToolManager.RegisterSleeperBlock(_blockValue, this.CloneModel(_blockValue, null), _blockPos);
			}, null);
		}
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0004AC60 File Offset: 0x00048E60
	public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _blockValue, _ebcd);
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (this.damageStates != null)
		{
			this.UpdateDamageState(_blockValue, _blockValue, _ebcd, true);
		}
		else
		{
			int num = (int)(10f * (float)_blockValue.damage) / _blockValue.Block.MaxDamage;
			_ebcd.SetMaterialValue("_Damage", (float)num);
		}
		if (this.block.tintColor.a > 0f)
		{
			_ebcd.SetMaterialColor("_Color", this.block.tintColor);
			return;
		}
		if (this.block.defaultTintColor.a > 0f)
		{
			_ebcd.SetMaterialColor("_Color", this.block.defaultTintColor);
		}
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x0004AD1C File Offset: 0x00048F1C
	public override bool UseRepairDamageState(BlockValue _blockValue)
	{
		return this.damageStates.Count > 1 && this.GetDamageStateIndex(_blockValue) == this.damageStates.Count - 1;
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x0004AD48 File Offset: 0x00048F48
	public void UpdateDamageState(BlockValue _oldBlockValue, BlockValue _newBlockValue, BlockEntityData _data, bool bPlayEffects = true)
	{
		int damageStateIndex = this.GetDamageStateIndex(_oldBlockValue);
		int damageStateIndex2 = this.GetDamageStateIndex(_newBlockValue);
		bool flag = damageStateIndex2 > damageStateIndex;
		if (flag)
		{
			Transform transform = _data.transform.Find("FX");
			if (transform)
			{
				AudioPlayer componentInChildren = transform.GetComponentInChildren<AudioPlayer>();
				if (componentInChildren)
				{
					componentInChildren.Play();
				}
				ParticleSystem componentInChildren2 = transform.GetComponentInChildren<ParticleSystem>();
				if (componentInChildren2)
				{
					componentInChildren2.Emit(10);
				}
			}
		}
		for (int i = 0; i < this.damageStates.Count; i++)
		{
			BlockShapeModelEntity.DamageState damageState = this.damageStates[i];
			if (!(damageState.objName == "-"))
			{
				GameObject gameObject = _data.transform.Find(damageState.objName).gameObject;
				gameObject.SetActive(i == damageStateIndex2);
				if (i == damageStateIndex2 && flag)
				{
					AudioSource component = gameObject.GetComponent<AudioSource>();
					if (component != null)
					{
						component.PlayDelayed(0.15f);
					}
					AudioPlayer component2 = gameObject.GetComponent<AudioPlayer>();
					if (component2 != null)
					{
						component2.Play();
					}
					ParticleSystem component3 = gameObject.GetComponent<ParticleSystem>();
					if (component3)
					{
						component3.Emit(10);
					}
				}
			}
		}
		UpdateLightOnAllMaterials component4 = _data.transform.GetComponent<UpdateLightOnAllMaterials>();
		if (component4 != null)
		{
			component4.Reset();
		}
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x0004AEA0 File Offset: 0x000490A0
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetDamageStateIndex(BlockValue _blockValue)
	{
		float num = (float)(_blockValue.Block.MaxDamage - _blockValue.damage);
		int num2 = this.damageStates.Count - 1;
		for (int i = 0; i < num2; i++)
		{
			if (num > this.damageStates[i + 1].health)
			{
				return i;
			}
		}
		return num2;
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x0004AEF5 File Offset: 0x000490F5
	public float GetNextDamageStateDownHealth(BlockValue _blockValue)
	{
		return this.damageStates[Utils.FastMin(this.GetDamageStateCount() - 1, this.GetDamageStateIndex(_blockValue) + 1)].health;
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0004AF1D File Offset: 0x0004911D
	public float GetNextDamageStateUpHealth(BlockValue _blockValue)
	{
		return this.damageStates[Utils.FastMax(0, this.GetDamageStateIndex(_blockValue) - 1)].health;
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x0004AF3E File Offset: 0x0004913E
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetDamageStateCount()
	{
		return this.damageStates.Count;
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x0004AF4B File Offset: 0x0004914B
	public override float GetStepHeight(BlockValue _blockValue, BlockFace crossingFace)
	{
		if (this.isCustomBounds && _blockValue.Block.IsCollideMovement)
		{
			return this.boundsArr[0].size.y;
		}
		return base.GetStepHeight(_blockValue, crossingFace);
	}

	// Token: 0x040009D2 RID: 2514
	[PublicizedFrom(EAccessModifier.Private)]
	public static string PropDamagedMesh = "MeshDamage";

	// Token: 0x040009D3 RID: 2515
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cMissingPrefabEntityPath = "@:Entities/Misc/block_missingPrefab.prefab";

	// Token: 0x040009D4 RID: 2516
	public string modelName;

	// Token: 0x040009D5 RID: 2517
	public Vector3 modelOffset;

	// Token: 0x040009D6 RID: 2518
	public int censorMode;

	// Token: 0x040009D7 RID: 2519
	[PublicizedFrom(EAccessModifier.Protected)]
	public string modelNameWithPath;

	// Token: 0x040009D8 RID: 2520
	[PublicizedFrom(EAccessModifier.Private)]
	public float LODCullScale = 1f;

	// Token: 0x040009D9 RID: 2521
	[PublicizedFrom(EAccessModifier.Private)]
	public new Bounds bounds;

	// Token: 0x040009DA RID: 2522
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isCustomBounds;

	// Token: 0x040009DB RID: 2523
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockShapeModelEntity.DamageState> damageStates;

	// Token: 0x02000181 RID: 385
	[PublicizedFrom(EAccessModifier.Private)]
	public struct DamageState
	{
		// Token: 0x040009DC RID: 2524
		public string objName;

		// Token: 0x040009DD RID: 2525
		public float health;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02001061 RID: 4193
public class RenderDisplacedCube
{
	// Token: 0x06008499 RID: 33945 RVA: 0x0035F634 File Offset: 0x0035D834
	public RenderDisplacedCube(Transform _focusCubePrefab)
	{
		this.objectShader = GlobalAssets.FindShader("Game/Glow");
		this.objectShader_TA = GlobalAssets.FindShader("Game/OverlayHighlight_TA");
		this.terrainShader = GlobalAssets.FindShader("Game/TerrainGlow");
		this.transformParent = new GameObject("FocusBox").transform;
		Origin.Add(this.transformParent, 1);
		this.transformWireframeCube = new GameObject("Wireframe").transform;
		this.transformWireframeCube.parent = this.transformParent;
		this.transformWireframeCube.localPosition = Vector3.zero;
		this.transformFocusCubePrefab = _focusCubePrefab;
		if (this.transformFocusCubePrefab != null)
		{
			this.transformFocusCubePrefab.parent = this.transformWireframeCube;
			this.transformFocusCubePrefab.localPosition = new Vector3(0.5f, 0.01f, 0.5f);
			this.bUseFocusCubePrefab = true;
			return;
		}
		this.transformWireframeCube.localScale = new Vector3(1.1f, 1.1f, 1.1f);
		this.vertices.Add(new Vector3[4]);
		this.vertices.Add(new Vector3[4]);
		this.vertices.Add(new Vector3[4]);
		this.vertices.Add(new Vector3[4]);
		this.vertices.Add(new Vector3[4]);
		this.vertices.Add(new Vector3[4]);
		Material material = new Material(Shader.Find("Transparent/Diffuse"));
		material.SetTexture("_MainTex", Resources.Load("Textures/focusbox") as Texture2D);
		for (int i = 0; i < 6; i++)
		{
			this.goSides[i] = new GameObject();
			this.goSides[i].name = "Side_" + i.ToString();
			this.goSides[i].transform.parent = this.transformWireframeCube;
			this.goSides[i].transform.localScale = Vector3.one;
			this.meshFilters[i] = this.goSides[i].AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = this.goSides[i].AddComponent<MeshRenderer>();
			meshRenderer.material = material;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = true;
			this.meshFilters[i].mesh.Clear();
			this.calcVerticesForSide(i, this.cube, null);
			this.meshFilters[i].mesh.uv = this.uvs;
			this.meshFilters[i].mesh.triangles = this.indices;
			this.meshFilters[i].mesh.RecalculateNormals();
		}
	}

	// Token: 0x0600849A RID: 33946 RVA: 0x0035FA8C File Offset: 0x0035DC8C
	public void Cleanup()
	{
		if (this.previewMaterial != null)
		{
			UnityEngine.Object.Destroy(this.previewMaterial);
			this.previewMaterial = null;
		}
		if (this.transformParent != null)
		{
			Origin.Remove(this.transformParent);
			UnityEngine.Object.Destroy(this.transformParent.gameObject);
			this.transformParent = null;
		}
	}

	// Token: 0x0600849B RID: 33947 RVA: 0x0035FAEC File Offset: 0x0035DCEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void calcVerticesForSide(int _idx, Vector3[] _cube, Vector3[] _offsets)
	{
		for (int i = 0; i < 4; i++)
		{
			this.vertices[_idx][i] = _cube[this.sides[_idx, i]] + ((_offsets != null) ? _offsets[this.sides[_idx, i]] : Vector3.zero);
		}
		this.meshFilters[_idx].mesh.vertices = this.vertices[_idx];
	}

	// Token: 0x0600849C RID: 33948 RVA: 0x0035FB6C File Offset: 0x0035DD6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void rebuildCubeOnBounds(BlockValue _blockValue)
	{
		BlockShape shape = _blockValue.Block.shape;
		if (!(shape is BlockShapeModelEntity) && !(shape is BlockShapeExt3dModel) && !(shape is BlockShapeNew))
		{
			this.multiDim = Vector3.one;
			this.localPos = Vector3.zero;
			return;
		}
		Bounds blockPlacementBounds = GameUtils.GetBlockPlacementBounds(_blockValue.Block);
		this.multiDim = blockPlacementBounds.size;
		this.localPos = blockPlacementBounds.center;
	}

	// Token: 0x0600849D RID: 33949 RVA: 0x0035FBDC File Offset: 0x0035DDDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void setFocusType(RenderCubeType _focusType)
	{
		if (this.bUseFocusCubePrefab)
		{
			return;
		}
		if (_focusType != this.focusType)
		{
			this.focusType = _focusType;
			GameObject[] array = this.goSides;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(_focusType == RenderCubeType.FullBlockBothSides);
			}
			if (_focusType < RenderCubeType.FullBlockBothSides)
			{
				this.goSides[(int)_focusType].SetActive(true);
			}
		}
	}

	// Token: 0x0600849E RID: 33950 RVA: 0x0035FC38 File Offset: 0x0035DE38
	[PublicizedFrom(EAccessModifier.Private)]
	public void GenerateLandClaimPreview(float scale)
	{
		this.cachedLandClaimBoundary = new GameObject("LandClaimBoundary");
		this.cachedLandClaimBoundary.transform.parent = this.transformFocusCubePrefab;
		this.cachedLandClaimBoundary.transform.localPosition = Vector3.zero;
		this.cachedLandClaimBoundary.transform.localRotation = Quaternion.identity;
		this.cachedLandClaimBoundary.transform.localScale = new Vector3(1f, 10000f, 1f) * scale / 1f;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
		gameObject.transform.parent = this.cachedLandClaimBoundary.transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		Renderer component = gameObject.GetComponent<Renderer>();
		Material material = Resources.Load("Materials/LandClaimBoundary", typeof(Material)) as Material;
		component.material = material;
	}

	// Token: 0x0600849F RID: 33951 RVA: 0x0035FD48 File Offset: 0x0035DF48
	[PublicizedFrom(EAccessModifier.Private)]
	public bool update0(World _world, WorldRayHitInfo _hitInfo, Vector3i _focusBlockPos, EntityAlive _player, PersistentPlayerData _ppLocal, bool _bAlternativeBlockPos, BlockValue _holdingBlockValue, TextureFullArray _texture)
	{
		int clrIdx = _hitInfo.hit.clrIdx;
		HitInfoDetails hit = _hitInfo.hit;
		BlockPlacement.EnumRotationMode mode = BlockPlacement.EnumRotationMode.Advanced;
		int localRot = 0;
		if (_player.inventory.holdingItemData is ItemClassBlock.ItemBlockInventoryData)
		{
			mode = ((ItemClassBlock.ItemBlockInventoryData)_player.inventory.holdingItemData).mode;
			localRot = ((ItemClassBlock.ItemBlockInventoryData)_player.inventory.holdingItemData).localRot;
		}
		HitInfoDetails hitInfo = _hitInfo.hit.Clone();
		hitInfo.blockPos = _hitInfo.lastBlockPos;
		BlockPlacement.Result result = _holdingBlockValue.Block.BlockPlacementHelper.OnPlaceBlock(mode, localRot, _world, _holdingBlockValue, hitInfo, _player.GetPosition());
		_holdingBlockValue.rotation = result.blockValue.rotation;
		Vector3 zero = Vector3.zero;
		_world.GetBlock(clrIdx, _focusBlockPos);
		this.rebuildCubeOnBounds(result.blockValue);
		if (this.focusTransformBlockValue.Block.IsTerrainDecoration && _world.GetBlock(_focusBlockPos - Vector3i.up).Block.shape.IsTerrain())
		{
			sbyte density = _world.GetDensity(0, _focusBlockPos);
			sbyte density2 = _world.GetDensity(0, _focusBlockPos - Vector3i.up);
			zero.y += MarchingCubes.GetDecorationOffsetY(density, density2);
		}
		if (!_bAlternativeBlockPos)
		{
			_focusBlockPos = _holdingBlockValue.Block.GetFreePlacementPosition(_world, clrIdx, _focusBlockPos, _holdingBlockValue, _player);
		}
		Vector3 vector = _focusBlockPos.ToVector3() + new Vector3(0.5f, 0.5f, 0.5f);
		Vector3 vector2 = vector + zero;
		if ((vector2 - this.transformLastPos).sqrMagnitude > 64f)
		{
			this.transformLastPos = vector2;
		}
		Vector3 vector3 = Vector3.LerpUnclamped(this.transformLastPos, vector2, 0.3f);
		vector3 = Vector3.MoveTowards(vector3, vector2, 5f * Time.deltaTime);
		this.transformLastPos = vector3;
		vector3 -= Origin.position;
		bool flag = false;
		if (!this.focusTransformPosition.Equals(_focusBlockPos))
		{
			this.focusTransformPosition = _focusBlockPos;
			this.lastTimeFocusTransformMoved = Time.time;
			flag = true;
		}
		bool result2 = false;
		if (!_bAlternativeBlockPos && ItemClass.GetForId(_holdingBlockValue.type) != null && ItemClass.GetForId(_holdingBlockValue.type).IsBlock())
		{
			bool flag2 = (false | this.transformBlockPreview == null | !this.focusTransformBlockValue.Equals(_holdingBlockValue) | this.focusTransformBlockValue.rotation != _holdingBlockValue.rotation | this.focusedBlockFace != _hitInfo.hit.blockFace | this.focusedTexture != _texture) || flag;
			result2 = true;
			if (flag2)
			{
				if (!this.focusTransformBlockValue.Equals(_holdingBlockValue) || this.focusedTexture != _texture)
				{
					this.transparencyFade = 0f;
				}
				this.DestroyPreview();
				this.focusedBlockFace = _hitInfo.hit.blockFace;
				for (int i = 0; i < this.arrFocusBoxDisplacedVertices.Length; i++)
				{
					this.arrFocusBoxDisplacedVertices[i] = Vector3.zero;
				}
				this.focusTransformBlockValue = _holdingBlockValue;
				this.focusedTexture = _texture;
				hitInfo.blockPos = _hitInfo.lastBlockPos;
				this.transformBlockPreview = ItemClassBlock.CreateMesh(null, _world, this.focusTransformBlockValue, this.arrFocusBoxDisplacedVertices, _focusBlockPos.ToVector3(), this.transformParent, BlockShape.MeshPurpose.Preview, _texture);
				if (this.transformBlockPreview != null)
				{
					this.disableAllComponents(this.transformBlockPreview);
					this.transformBlockPreview.gameObject.layer = LayerMask.NameToLayer("NotInReflections");
					Block block = this.focusTransformBlockValue.Block;
					if (block.shape.IsTerrain())
					{
						if (this.previewMaterial != null)
						{
							UnityEngine.Object.DestroyImmediate(this.previewMaterial);
						}
						Renderer component = this.transformBlockPreview.GetComponent<Renderer>();
						if (component)
						{
							component.receiveShadows = true;
							this.previewMaterial = component.material;
							if (this.previewMaterial)
							{
								this.previewMaterial.shader = this.terrainShader;
								this.previewMaterial.SetFloat("_TexI", (float)block.TerrainTAIndex);
							}
						}
					}
					else if (block.shape is BlockShapeModelEntity)
					{
						Color value = new Color(0.7f, 0.7f, 0.7f);
						int nameID = Shader.PropertyToID("_MainTex");
						Renderer[] componentsInChildren = this.transformBlockPreview.GetComponentsInChildren<Renderer>();
						for (int j = 0; j < componentsInChildren.Length; j++)
						{
							foreach (Material material in componentsInChildren[j].materials)
							{
								material.EnableKeyword("_EMISSION");
								if (material.HasProperty(nameID))
								{
									Texture texture = material.GetTexture(nameID);
									material.SetTexture("_EmissionMap", texture ? texture : Texture2D.whiteTexture);
									material.SetColor("_EmissionColor", value);
								}
							}
							Material[] materials;
							this.blockPreviewMats.AddRange(materials);
						}
						Utils.SetLayerRecursively(this.transformBlockPreview.gameObject, 2);
					}
					else
					{
						if (this.previewMaterial != null)
						{
							UnityEngine.Object.DestroyImmediate(this.previewMaterial);
						}
						Renderer component2 = this.transformBlockPreview.GetComponent<Renderer>();
						if (component2)
						{
							component2.receiveShadows = true;
							this.previewMaterial = component2.material;
							if (this.previewMaterial)
							{
								string name = this.previewMaterial.shader.name;
								if (!name.StartsWith("Game/Debug"))
								{
									if (name.EndsWith("_TA"))
									{
										this.previewMaterial.shader = this.objectShader_TA;
									}
									else
									{
										this.previewMaterial.shader = this.objectShader;
										if (!block.blockMaterial.SurfaceCategory.EqualsCaseInsensitive("glass"))
										{
											this.previewMaterial.SetFloat("_Cutoff", 0.038f);
										}
										else
										{
											this.previewMaterial.SetFloat("_Cutoff", 0f);
										}
									}
								}
								if (block.MeshIndex == 3)
								{
									Texture texture2 = this.previewMaterial.GetTexture("_Albedo");
									this.previewMaterial.SetTexture("_MainTex", texture2);
								}
							}
						}
					}
				}
			}
			if (Mathf.Abs(_player.speedForward) + Mathf.Abs(_player.speedStrafe) < 0.45f)
			{
				this.transparencyFade += Time.deltaTime * 5f;
				this.transparencyFade = Mathf.Clamp01(this.transparencyFade);
			}
			else
			{
				this.transparencyFade = 0f;
			}
			float num = 1f * this.transparencyFade;
			if (this.transformBlockPreview)
			{
				bool flag3 = this.focusTransformBlockValue.Block.CanPlaceBlockAt(_world, clrIdx, _focusBlockPos, _holdingBlockValue, false);
				bool flag4 = _holdingBlockValue.Block.IndexName == "lpblock";
				bool flag5 = _holdingBlockValue.Block.IndexName == "brBlock";
				if (flag3)
				{
					if (flag4)
					{
						flag3 = _world.CanPlaceLandProtectionBlockAt(_focusBlockPos, _ppLocal);
					}
					else
					{
						flag3 = _world.CanPlaceBlockAt(_focusBlockPos, _ppLocal, false);
					}
				}
				if (flag3)
				{
					flag3 &= (_player.IsGodMode.Value || !GameUtils.IsColliderWithinBlock(_focusBlockPos, _holdingBlockValue));
				}
				this.transformWireframeCube.localScale = Vector3.one;
				if (this.cachedLandClaimBoundary != null)
				{
					UnityEngine.Object.DestroyImmediate(this.cachedLandClaimBoundary);
				}
				if (this.transformFocusCubePrefab)
				{
					if (flag4)
					{
						float scale = (float)GameStats.GetInt(EnumGameStats.LandClaimSize);
						this.GenerateLandClaimPreview(scale);
						this.transformFocusCubePrefab.localScale = this.multiDim * 1f;
					}
					else if (flag5)
					{
						float num2 = (float)GamePrefs.GetInt(EnumGamePrefs.BedrollDeadZoneSize) * 1f;
						this.transformFocusCubePrefab.localScale = new Vector3(num2, num2, num2);
					}
					else
					{
						this.transformFocusCubePrefab.localScale = this.multiDim * 1f;
					}
					this.transformFocusCubePrefab.localPosition = this.localPos;
					this.transformFocusCubePrefab.gameObject.SetActive(true);
				}
				float num3 = num;
				float magnitude = (_player.getHeadPosition() - vector).magnitude;
				if (magnitude < 1f)
				{
					num3 *= magnitude / 1f;
				}
				Color color;
				if (flag3)
				{
					color = Color.white;
					EnumLandClaimOwner landClaimOwner = _world.GetLandClaimOwner(_focusBlockPos, _ppLocal);
					if (landClaimOwner == EnumLandClaimOwner.Self)
					{
						color = Color.green;
					}
					else if (landClaimOwner == EnumLandClaimOwner.Ally)
					{
						color = Color.yellow;
					}
					if (this.cachedFocusBlockPos != _focusBlockPos || !this.focusTransformBlockValue.Equals(this.cachedFocusBlockValue))
					{
						this.drawStability = StabilityCalculator.GetBlockStabilityIfPlaced(_focusBlockPos, this.focusTransformBlockValue);
						this.cachedFocusBlockPos = _focusBlockPos;
						this.cachedFocusBlockValue = this.focusTransformBlockValue;
					}
				}
				else
				{
					color = Color.red;
					num3 *= 0.5f;
				}
				color.r *= 0.5f;
				color.g *= 0.5f;
				color.b *= 0.5f;
				Renderer[] array;
				if (this.transformFocusCubePrefab)
				{
					Color value2 = color;
					if (flag3 && this.drawStability <= 0f)
					{
						value2 = new Color(1f, 0f, 0.55f);
					}
					array = this.transformFocusCubePrefab.GetComponentsInChildren<Renderer>();
					for (int l = 0; l < array.Length; l++)
					{
						array[l].material.SetColor("_Color", value2);
					}
				}
				if (this.cachedLandClaimBoundary != null)
				{
					Color color2 = flag3 ? Color.green : Color.red;
					Renderer componentInChildren = this.cachedLandClaimBoundary.GetComponentInChildren<Renderer>();
					Color color3 = componentInChildren.material.GetColor("_BaseColor");
					componentInChildren.material.GetColor("_BoundaryColor");
					componentInChildren.material.SetColor("_BaseColor", new Color(color2.r, color2.g, color2.b, color3.a * 10f));
					componentInChildren.material.SetColor("_BoundaryColor", new Color(color2.r, color2.g, color2.b, color3.a * 10f));
				}
				Block block2 = _holdingBlockValue.Block;
				if (block2.tintColor.a > 0f)
				{
					color = block2.tintColor;
					color.a = 0.5f;
				}
				Renderer[] componentsInChildren2 = this.transformBlockPreview.GetComponentsInChildren<Renderer>();
				bool flag6 = false;
				array = componentsInChildren2;
				for (int l = 0; l < array.Length; l++)
				{
					if (array[l].shadowCastingMode == ShadowCastingMode.ShadowsOnly)
					{
						flag6 = true;
						break;
					}
				}
				foreach (Renderer renderer in componentsInChildren2)
				{
					Material material2 = renderer.material;
					material2.SetInt("_BlendModeSrc", flag3 ? 1 : 5);
					material2.SetInt("_BlendModeDest", flag3 ? 0 : 1);
					material2.SetFloat("_Alpha", num3);
					material2.SetColor("_Color", color);
					material2.SetFloat("_Stability", this.drawStability);
					if (!flag6)
					{
						renderer.enabled = (num3 > 0f);
						renderer.shadowCastingMode = ((num3 > 0.1f) ? ShadowCastingMode.On : ShadowCastingMode.Off);
					}
					else
					{
						float num4 = 0f;
						if (renderer.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
						{
							num4 = 0.1f;
						}
						renderer.enabled = (num3 > num4);
					}
				}
				this.transformBlockPreview.position = vector3;
				this.transformBlockPreview.localScale = Vector3.one;
			}
		}
		else if (!_bAlternativeBlockPos && this.transformFocusCubePrefab)
		{
			this.transformFocusCubePrefab.localScale = new Vector3(1f, 1f, 1f);
			this.transformFocusCubePrefab.gameObject.SetActive(Time.time - this.lastTimeFocusTransformMoved > 1f);
		}
		this.setFocusType(this.focusType);
		this.transformWireframeCube.position = vector3;
		this.transformWireframeCube.rotation = this.focusTransformBlockValue.Block.shape.GetRotation(this.focusTransformBlockValue);
		return result2;
	}

	// Token: 0x060084A0 RID: 33952 RVA: 0x00360958 File Offset: 0x0035EB58
	[PublicizedFrom(EAccessModifier.Private)]
	public void disableAllComponents(Transform _transform)
	{
		MonoBehaviour[] componentsInChildren = _transform.GetComponentsInChildren<MonoBehaviour>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		Animation[] componentsInChildren2 = _transform.GetComponentsInChildren<Animation>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].enabled = false;
		}
		Animator[] componentsInChildren3 = _transform.GetComponentsInChildren<Animator>();
		for (int k = 0; k < componentsInChildren3.Length; k++)
		{
			componentsInChildren3[k].enabled = false;
		}
		SpriteRenderer[] componentsInChildren4 = _transform.GetComponentsInChildren<SpriteRenderer>();
		for (int l = 0; l < componentsInChildren4.Length; l++)
		{
			componentsInChildren4[l].enabled = false;
		}
		Collider[] componentsInChildren5 = _transform.GetComponentsInChildren<Collider>();
		for (int m = 0; m < componentsInChildren5.Length; m++)
		{
			UnityEngine.Object.Destroy(componentsInChildren5[m]);
		}
	}

	// Token: 0x060084A1 RID: 33953 RVA: 0x00360A18 File Offset: 0x0035EC18
	public void Update(bool _bMeshSelected, World _world, WorldRayHitInfo _hitInfo, Vector3i _focusBlockPos, EntityAlive _player, PersistentPlayerData _ppLocal, bool _bAlternativeBlockPos)
	{
		bool flag = false;
		BlockValue holdingBlockValue = _player.inventory.holdingItemItemValue.ToBlockValue();
		Block block = holdingBlockValue.Block;
		if (block != null)
		{
			TextureFullArray texture = _player.inventory.holdingItemItemValue.TextureFullArray;
			if (block.SelectAlternates)
			{
				holdingBlockValue = block.GetAltBlockValue(_player.inventory.holdingItemItemValue.Meta);
				block = holdingBlockValue.Block;
				texture = ((block.GetAutoShapeType() != EAutoShapeType.None) ? _player.inventory.holdingItemItemValue.TextureFullArray : TextureFullArray.Default);
			}
			if (_player.inventory.holdingItemData is ItemClassBlock.ItemBlockInventoryData)
			{
				holdingBlockValue.rotation = ((ItemClassBlock.ItemBlockInventoryData)_player.inventory.holdingItemData).rotation;
			}
			RenderCubeType renderCubeType = _player.inventory.holdingItem.GetFocusType(_player.inventory.holdingItemData);
			int placementDistanceSq = block.GetPlacementDistanceSq();
			if (_hitInfo.hit.distanceSq > (float)placementDistanceSq)
			{
				renderCubeType = RenderCubeType.None;
			}
			if (_bMeshSelected && renderCubeType != RenderCubeType.None)
			{
				flag = this.update0(_world, _hitInfo, _focusBlockPos, _player, _ppLocal, _bAlternativeBlockPos, holdingBlockValue, texture);
			}
			if (this.transformWireframeCube != null && this.transformWireframeCube.gameObject != null)
			{
				this.transformWireframeCube.gameObject.SetActive(_bMeshSelected && renderCubeType != RenderCubeType.None && block.bHasPlacementWireframe);
			}
		}
		if (!flag)
		{
			this.transparencyFade = 0f;
			this.transformLastPos.x = float.MaxValue;
			this.DestroyPreview();
		}
	}

	// Token: 0x060084A2 RID: 33954 RVA: 0x00360B8C File Offset: 0x0035ED8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyPreview()
	{
		if (this.transformBlockPreview)
		{
			if (!(this.focusTransformBlockValue.Block.shape is BlockShapeModelEntity))
			{
				MeshFilter[] componentsInChildren = this.transformBlockPreview.GetComponentsInChildren<MeshFilter>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					VoxelMesh.AddPooledMesh(componentsInChildren[i].sharedMesh);
				}
			}
			UnityEngine.Object.Destroy(this.transformBlockPreview.gameObject);
			this.transformBlockPreview = null;
		}
		for (int j = 0; j < this.blockPreviewMats.Count; j++)
		{
			UnityEngine.Object.Destroy(this.blockPreviewMats[j]);
		}
		this.blockPreviewMats.Clear();
	}

	// Token: 0x040066A7 RID: 26279
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cWireframeFadeMaxDistance = 1f;

	// Token: 0x040066A8 RID: 26280
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTransparencyMax = 1f;

	// Token: 0x040066A9 RID: 26281
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCubeScale = 1f;

	// Token: 0x040066AA RID: 26282
	[PublicizedFrom(EAccessModifier.Private)]
	public RenderCubeType focusType;

	// Token: 0x040066AB RID: 26283
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshFilter[] meshFilters = new MeshFilter[6];

	// Token: 0x040066AC RID: 26284
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject[] goSides = new GameObject[6];

	// Token: 0x040066AD RID: 26285
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3[] cube = new Vector3[]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(1f, 1f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 0f, 1f),
		new Vector3(0f, 1f, 1f),
		new Vector3(1f, 1f, 1f),
		new Vector3(1f, 0f, 1f)
	};

	// Token: 0x040066AE RID: 26286
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3[] cubeCopy = new Vector3[8];

	// Token: 0x040066AF RID: 26287
	[PublicizedFrom(EAccessModifier.Private)]
	public int[,] sides = new int[,]
	{
		{
			0,
			3,
			2,
			1
		},
		{
			7,
			4,
			5,
			6
		},
		{
			4,
			0,
			1,
			5
		},
		{
			3,
			7,
			6,
			2
		},
		{
			6,
			5,
			1,
			2
		},
		{
			4,
			7,
			3,
			0
		}
	};

	// Token: 0x040066B0 RID: 26288
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3[]> vertices = new List<Vector3[]>();

	// Token: 0x040066B1 RID: 26289
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2[] uvs = new Vector2[]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(1f, 1f),
		new Vector2(0f, 1f)
	};

	// Token: 0x040066B2 RID: 26290
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] indices = new int[]
	{
		0,
		2,
		1,
		3,
		2,
		0,
		0,
		1,
		2,
		3,
		0,
		2
	};

	// Token: 0x040066B3 RID: 26291
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform transformParent;

	// Token: 0x040066B4 RID: 26292
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform transformWireframeCube;

	// Token: 0x040066B5 RID: 26293
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform transformBlockPreview;

	// Token: 0x040066B6 RID: 26294
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform transformFocusCubePrefab;

	// Token: 0x040066B7 RID: 26295
	[PublicizedFrom(EAccessModifier.Private)]
	public Shader objectShader;

	// Token: 0x040066B8 RID: 26296
	[PublicizedFrom(EAccessModifier.Private)]
	public Shader objectShader_TA;

	// Token: 0x040066B9 RID: 26297
	[PublicizedFrom(EAccessModifier.Private)]
	public Shader terrainShader;

	// Token: 0x040066BA RID: 26298
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Material> blockPreviewMats = new List<Material>();

	// Token: 0x040066BB RID: 26299
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastTimeFocusTransformMoved;

	// Token: 0x040066BC RID: 26300
	[PublicizedFrom(EAccessModifier.Private)]
	public Material previewMaterial;

	// Token: 0x040066BD RID: 26301
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3[] arrFocusBoxDisplacedVertices = new Vector3[8];

	// Token: 0x040066BE RID: 26302
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue focusTransformBlockValue;

	// Token: 0x040066BF RID: 26303
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i focusTransformPosition;

	// Token: 0x040066C0 RID: 26304
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bUseFocusCubePrefab;

	// Token: 0x040066C1 RID: 26305
	[PublicizedFrom(EAccessModifier.Private)]
	public TextureFullArray focusedTexture;

	// Token: 0x040066C2 RID: 26306
	[PublicizedFrom(EAccessModifier.Private)]
	public float transparencyFade;

	// Token: 0x040066C3 RID: 26307
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 transformLastPos;

	// Token: 0x040066C4 RID: 26308
	[PublicizedFrom(EAccessModifier.Private)]
	public float drawStability;

	// Token: 0x040066C5 RID: 26309
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockFace focusedBlockFace;

	// Token: 0x040066C6 RID: 26310
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue cachedFocusBlockValue;

	// Token: 0x040066C7 RID: 26311
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i cachedFocusBlockPos;

	// Token: 0x040066C8 RID: 26312
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 multiDim;

	// Token: 0x040066C9 RID: 26313
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 localPos;

	// Token: 0x040066CA RID: 26314
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject cachedLandClaimBoundary;
}

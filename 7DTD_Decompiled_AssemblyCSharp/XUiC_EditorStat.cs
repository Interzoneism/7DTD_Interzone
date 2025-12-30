using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CAA RID: 3242
[Preserve]
public class XUiC_EditorStat : XUiController
{
	// Token: 0x17000A37 RID: 2615
	// (get) Token: 0x06006422 RID: 25634 RVA: 0x00288685 File Offset: 0x00286885
	public bool hasPrefabLoaded
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return PrefabEditModeManager.Instance.IsActive() && PrefabEditModeManager.Instance.VoxelPrefab != null && PrefabEditModeManager.Instance.VoxelPrefab.location.Type != PathAbstractions.EAbstractedLocationType.None;
		}
	}

	// Token: 0x17000A38 RID: 2616
	// (get) Token: 0x06006423 RID: 25635 RVA: 0x002886BB File Offset: 0x002868BB
	public Prefab selectedPrefab
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator == null)
			{
				return null;
			}
			PrefabInstance activePrefab = dynamicPrefabDecorator.ActivePrefab;
			if (activePrefab == null)
			{
				return null;
			}
			return activePrefab.prefab;
		}
	}

	// Token: 0x17000A39 RID: 2617
	// (get) Token: 0x06006424 RID: 25636 RVA: 0x002886DD File Offset: 0x002868DD
	public bool hasPrefabSelected
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.selectedPrefab != null;
		}
	}

	// Token: 0x17000A3A RID: 2618
	// (get) Token: 0x06006425 RID: 25637 RVA: 0x002886E8 File Offset: 0x002868E8
	// (set) Token: 0x06006426 RID: 25638 RVA: 0x002886EF File Offset: 0x002868EF
	public static WorldStats ManualStats
	{
		get
		{
			return XUiC_EditorStat.manualStats;
		}
		set
		{
			XUiC_EditorStat.ManualStatsUpdateTime = DateTime.Now;
			XUiC_EditorStat.manualStats = value;
		}
	}

	// Token: 0x06006427 RID: 25639 RVA: 0x00282536 File Offset: 0x00280736
	public override void OnOpen()
	{
		base.OnOpen();
		this.IsDirty = true;
	}

	// Token: 0x06006428 RID: 25640 RVA: 0x00288704 File Offset: 0x00286904
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty || Time.time - this.lastDirtyTime >= 1f)
		{
			this.lootContainers = 0;
			this.fetchLootContainers = 0;
			this.restorePowerNodes = 0;
			this.totalBlockEntities = 0;
			this.hasSelection = false;
			this.selectionSize = default(Vector3i);
			if (this.hasPrefabLoaded)
			{
				ValueTuple<SelectionCategory, SelectionBox>? valueTuple;
				SelectionBox selectionBox = (SelectionBoxManager.Instance.Selection != null) ? valueTuple.GetValueOrDefault().Item2 : null;
				if (selectionBox != null)
				{
					this.selectionSize = selectionBox.GetScale();
					this.hasSelection = true;
				}
				if (this.hasLootStat)
				{
					PrefabEditModeManager.Instance.GetLootAndFetchLootContainerCount(out this.lootContainers, out this.fetchLootContainers, out this.restorePowerNodes);
				}
				if (this.hasBlockEntitiesStat)
				{
					foreach (ChunkGameObject chunkGameObject in GameManager.Instance.World.m_ChunkManager.GetUsedChunkGameObjects())
					{
						this.totalBlockEntities += this.countBlockEntities(chunkGameObject.transform);
					}
				}
			}
			base.RefreshBindings(false);
			this.IsDirty = false;
			this.lastDirtyTime = Time.time;
		}
	}

	// Token: 0x06006429 RID: 25641 RVA: 0x0028885C File Offset: 0x00286A5C
	[PublicizedFrom(EAccessModifier.Private)]
	public int countBlockEntities(Transform _t)
	{
		int num = 0;
		for (int i = 0; i < _t.childCount; i++)
		{
			Transform child = _t.GetChild(i);
			if (child.name == "_BlockEntities")
			{
				num += child.childCount;
			}
			else
			{
				num += this.countBlockEntities(child);
			}
		}
		return num;
	}

	// Token: 0x0600642A RID: 25642 RVA: 0x002888AC File Offset: 0x00286AAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		Prefab prefab = this.hasPrefabLoaded ? PrefabEditModeManager.Instance.VoxelPrefab : this.selectedPrefab;
		bool flag = prefab != null;
		bool flag2 = ((prefab != null) ? prefab.RenderingCostStats : null) != null;
		bool flag3 = XUiC_EditorStat.ManualStats != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 2389907569U)
		{
			if (num <= 1501340122U)
			{
				if (num <= 1007605620U)
				{
					if (num != 122989552U)
					{
						if (num != 856865979U)
						{
							if (num == 1007605620U)
							{
								if (_bindingName == "has_selected_prefab")
								{
									_value = this.hasPrefabSelected.ToString();
									return true;
								}
							}
						}
						else if (_bindingName == "show_quest_clear_count")
						{
							_value = (((prefab != null) ? prefab.ShowQuestClearCount.ToString() : null) ?? "");
							return true;
						}
					}
					else if (_bindingName == "drawcalls")
					{
						_value = this.batchesFormatter.Format(this.drawcallsSum / 20);
						return true;
					}
				}
				else if (num != 1281920084U)
				{
					if (num != 1330804927U)
					{
						if (num == 1501340122U)
						{
							if (_bindingName == "has_selection")
							{
								_value = this.hasSelection.ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "selection_size")
					{
						_value = (this.hasSelection ? this.selectionSizeFormatter.Format(this.selectionSize) : "-");
						return true;
					}
				}
				else if (_bindingName == "loot_containers")
				{
					this.hasLootStat = true;
					_value = this.lootFormatter.Format(this.lootContainers);
					return true;
				}
			}
			else if (num <= 1871983191U)
			{
				if (num != 1506237936U)
				{
					if (num != 1724383115U)
					{
						if (num == 1871983191U)
						{
							if (_bindingName == "tris")
							{
								_value = "";
								return true;
							}
						}
					}
					else if (_bindingName == "statsVertices")
					{
						_value = (flag2 ? this.statsVertsFormatter.Format(prefab.RenderingCostStats.TotalVertices) : "-");
						return true;
					}
				}
				else if (_bindingName == "has_loaded_prefab")
				{
					_value = this.hasPrefabLoaded.ToString();
					return true;
				}
			}
			else if (num != 2084430828U)
			{
				if (num != 2297592010U)
				{
					if (num == 2389907569U)
					{
						if (_bindingName == "statsTriangles")
						{
							_value = (flag2 ? this.statsTrisFormatter.Format(prefab.RenderingCostStats.TotalTriangles) : "-");
							return true;
						}
					}
				}
				else if (_bindingName == "statsManualUpdateTime")
				{
					_value = (flag3 ? this.statsManualUpdateTimeFormatter.Format(XUiC_EditorStat.ManualStatsUpdateTime.ToLocalTime()) : "<not captured>");
					return true;
				}
			}
			else if (_bindingName == "restorepower_nodes")
			{
				this.hasLootStat = true;
				_value = this.restorepowerFormatter.Format(this.restorePowerNodes);
				return true;
			}
		}
		else if (num <= 3034812289U)
		{
			if (num <= 2659686822U)
			{
				if (num != 2502672311U)
				{
					if (num != 2524445748U)
					{
						if (num == 2659686822U)
						{
							if (_bindingName == "prefab_volume")
							{
								_value = (flag ? prefab.size.Volume().ToString() : "");
								return true;
							}
						}
					}
					else if (_bindingName == "block_entities")
					{
						this.hasBlockEntitiesStat = true;
						_value = this.blockentitiesFormatter.Format(this.totalBlockEntities);
						return true;
					}
				}
				else if (_bindingName == "loaded_prefab_name")
				{
					_value = (((prefab != null) ? prefab.PrefabName : null) ?? "");
					return true;
				}
			}
			else if (num != 2708140049U)
			{
				if (num != 2723945129U)
				{
					if (num == 3034812289U)
					{
						if (_bindingName == "statsManualLightsVolume")
						{
							if (!flag3)
							{
								_value = "-";
								return true;
							}
							float lightsVolume = XUiC_EditorStat.ManualStats.LightsVolume;
							int num2 = (prefab != null) ? prefab.size.Volume() : 0;
							_value = string.Format("{0:F0} ({1:P1})", lightsVolume, lightsVolume / (float)num2);
							return true;
						}
					}
				}
				else if (_bindingName == "statsManualVertices")
				{
					_value = (flag3 ? this.statsManualVertsFormatter.Format(XUiC_EditorStat.ManualStats.TotalVertices) : "-");
					return true;
				}
			}
			else if (_bindingName == "difficulty_tier")
			{
				_value = (((prefab != null) ? prefab.DifficultyTier.ToString() : null) ?? "");
				return true;
			}
		}
		else if (num <= 3347817698U)
		{
			if (num != 3088255484U)
			{
				if (num != 3167497763U)
				{
					if (num == 3347817698U)
					{
						if (_bindingName == "fetchloot_containers")
						{
							this.hasLootStat = true;
							_value = this.fetchlootFormatter.Format(this.fetchLootContainers);
							return true;
						}
					}
				}
				else if (_bindingName == "verts")
				{
					_value = "";
					return true;
				}
			}
			else if (_bindingName == "sleeper_info")
			{
				_value = (((prefab != null) ? prefab.CalcSleeperInfo() : null) ?? "");
				return true;
			}
		}
		else if (num <= 3964975885U)
		{
			if (num != 3656015704U)
			{
				if (num == 3964975885U)
				{
					if (_bindingName == "prefab_size")
					{
						_value = (flag ? this.prefabSizeFormatter.Format(prefab.size) : "");
						return true;
					}
				}
			}
			else if (_bindingName == "loaded_prefab_changed")
			{
				_value = ((this.hasPrefabLoaded && PrefabEditModeManager.Instance.NeedsSaving) ? "*" : "");
				return true;
			}
		}
		else if (num != 4048861779U)
		{
			if (num == 4061059603U)
			{
				if (_bindingName == "statsManualTriangles")
				{
					_value = (flag3 ? this.statsManualTrisFormatter.Format(XUiC_EditorStat.ManualStats.TotalTriangles) : "-");
					return true;
				}
			}
		}
		else if (_bindingName == "statsLightsVolume")
		{
			if (!flag2)
			{
				_value = "-";
				return true;
			}
			float lightsVolume2 = prefab.RenderingCostStats.LightsVolume;
			int num3 = prefab.size.Volume();
			_value = string.Format("{0:F0} ({1:P1})", lightsVolume2, lightsVolume2 / (float)num3);
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x04004B47 RID: 19271
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastDirtyTime;

	// Token: 0x04004B48 RID: 19272
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasLootStat;

	// Token: 0x04004B49 RID: 19273
	[PublicizedFrom(EAccessModifier.Private)]
	public int lootContainers;

	// Token: 0x04004B4A RID: 19274
	[PublicizedFrom(EAccessModifier.Private)]
	public int fetchLootContainers;

	// Token: 0x04004B4B RID: 19275
	[PublicizedFrom(EAccessModifier.Private)]
	public int restorePowerNodes;

	// Token: 0x04004B4C RID: 19276
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasBlockEntitiesStat;

	// Token: 0x04004B4D RID: 19277
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalBlockEntities;

	// Token: 0x04004B4E RID: 19278
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasSelection;

	// Token: 0x04004B4F RID: 19279
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i selectionSize;

	// Token: 0x04004B50 RID: 19280
	[PublicizedFrom(EAccessModifier.Private)]
	public const int DC_AVERAGE_FRAMES = 20;

	// Token: 0x04004B51 RID: 19281
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] drawcallsBuf = new int[20];

	// Token: 0x04004B52 RID: 19282
	[PublicizedFrom(EAccessModifier.Private)]
	public int drawcallsBufIndex;

	// Token: 0x04004B53 RID: 19283
	[PublicizedFrom(EAccessModifier.Private)]
	public int drawcallsSum;

	// Token: 0x04004B54 RID: 19284
	[PublicizedFrom(EAccessModifier.Private)]
	public static WorldStats manualStats;

	// Token: 0x04004B55 RID: 19285
	[PublicizedFrom(EAccessModifier.Private)]
	public static DateTime ManualStatsUpdateTime;

	// Token: 0x04004B56 RID: 19286
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<Vector3i> prefabSizeFormatter = new CachedStringFormatter<Vector3i>((Vector3i _i) => _i.ToString());

	// Token: 0x04004B57 RID: 19287
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<Vector3i> selectionSizeFormatter = new CachedStringFormatter<Vector3i>((Vector3i _i) => _i.ToString());

	// Token: 0x04004B58 RID: 19288
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt lootFormatter = new CachedStringFormatterInt();

	// Token: 0x04004B59 RID: 19289
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt fetchlootFormatter = new CachedStringFormatterInt();

	// Token: 0x04004B5A RID: 19290
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt restorepowerFormatter = new CachedStringFormatterInt();

	// Token: 0x04004B5B RID: 19291
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt blockentitiesFormatter = new CachedStringFormatterInt();

	// Token: 0x04004B5C RID: 19292
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> vertsFormatter = new CachedStringFormatter<int>((int _i) => ((double)_i).FormatNumberWithMetricPrefix(true, 3));

	// Token: 0x04004B5D RID: 19293
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> trisFormatter = new CachedStringFormatter<int>((int _i) => ((double)_i).FormatNumberWithMetricPrefix(true, 3));

	// Token: 0x04004B5E RID: 19294
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt batchesFormatter = new CachedStringFormatterInt();

	// Token: 0x04004B5F RID: 19295
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt statsVertsFormatter = new CachedStringFormatterInt();

	// Token: 0x04004B60 RID: 19296
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt statsTrisFormatter = new CachedStringFormatterInt();

	// Token: 0x04004B61 RID: 19297
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<DateTime> statsManualUpdateTimeFormatter = new CachedStringFormatter<DateTime>((DateTime _dt) => _dt.ToString(Utils.StandardCulture));

	// Token: 0x04004B62 RID: 19298
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt statsManualVertsFormatter = new CachedStringFormatterInt();

	// Token: 0x04004B63 RID: 19299
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt statsManualTrisFormatter = new CachedStringFormatterInt();
}

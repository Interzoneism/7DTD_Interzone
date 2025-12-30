using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000138 RID: 312
[Preserve]
public class BlockSleeper : Block
{
	// Token: 0x060008C2 RID: 2242 RVA: 0x0003D5D8 File Offset: 0x0003B7D8
	public BlockSleeper()
	{
		this.IsSleeperBlock = true;
		this.HasTileEntity = true;
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0003D620 File Offset: 0x0003B820
	public override void Init()
	{
		base.Init();
		base.Properties.ParseInt(BlockSleeper.PropPose, ref this.pose);
		this.look = Vector3.forward;
		base.Properties.ParseVec(BlockSleeper.PropLookIdentity, ref this.look);
		string @string = base.Properties.GetString(BlockSleeper.PropExcludeWalkType);
		if (@string.Length > 0)
		{
			string[] array = @string.Split(',', StringSplitOptions.None);
			this.excludedWalkTypes = new List<int>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == "Crawler")
				{
					this.excludedWalkTypes.Add(21);
				}
				else
				{
					Log.Warning("Block {0}, invalid ExcludeWalkType {1}", new object[]
					{
						base.GetBlockName(),
						array[i]
					});
				}
			}
		}
		base.Properties.ParseString(BlockSleeper.PropSpawnGroup, ref this.spawnGroup);
		base.Properties.ParseEnum<BlockSleeper.eMode>(BlockSleeper.PropSpawnMode, ref this.spawnMode);
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0003D710 File Offset: 0x0003B910
	public override bool CanPlaceBlockAt(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		return _world.IsEditor() || base.CanPlaceBlockAt(_world, _clrIdx, _blockPos, _blockValue, _bOmitCollideCheck);
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x0003D72C File Offset: 0x0003B92C
	public float GetSleeperRotation(BlockValue _blockValue)
	{
		byte rotation = _blockValue.rotation;
		switch (rotation)
		{
		case 1:
			return 90f;
		case 2:
			return 180f;
		case 3:
			return 270f;
		default:
			switch (rotation)
			{
			case 24:
				return 45f;
			case 25:
				return 135f;
			case 26:
				return 225f;
			case 27:
				return 315f;
			default:
				return 0f;
			}
			break;
		}
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x0003D79F File Offset: 0x0003B99F
	public bool ExcludesWalkType(int _walkType)
	{
		return this.excludedWalkTypes != null && this.excludedWalkTypes.Contains(_walkType);
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsTileEntitySavedInPrefab()
	{
		return false;
	}

	// Token: 0x0400089C RID: 2204
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string PropPose = "Pose";

	// Token: 0x0400089D RID: 2205
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string PropLookIdentity = "LookIdentity";

	// Token: 0x0400089E RID: 2206
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string PropExcludeWalkType = "ExcludeWalkType";

	// Token: 0x0400089F RID: 2207
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string PropSpawnGroup = "SpawnGroup";

	// Token: 0x040008A0 RID: 2208
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string PropSpawnMode = "SpawnMode";

	// Token: 0x040008A1 RID: 2209
	public int pose;

	// Token: 0x040008A2 RID: 2210
	public Vector3 look;

	// Token: 0x040008A3 RID: 2211
	public string spawnGroup;

	// Token: 0x040008A4 RID: 2212
	public BlockSleeper.eMode spawnMode;

	// Token: 0x040008A5 RID: 2213
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> excludedWalkTypes;

	// Token: 0x040008A6 RID: 2214
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("open", "dummy", true, false, null)
	};

	// Token: 0x02000139 RID: 313
	public enum eMode
	{
		// Token: 0x040008A8 RID: 2216
		Normal,
		// Token: 0x040008A9 RID: 2217
		Bandit,
		// Token: 0x040008AA RID: 2218
		Infested
	}
}

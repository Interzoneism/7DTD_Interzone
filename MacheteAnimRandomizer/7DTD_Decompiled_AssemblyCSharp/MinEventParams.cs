using System;
using Challenges;
using UnityEngine;

// Token: 0x02000661 RID: 1633
public class MinEventParams
{
	// Token: 0x06003160 RID: 12640 RVA: 0x0015062C File Offset: 0x0014E82C
	public static void CopyTo(MinEventParams _source, MinEventParams _destination)
	{
		_destination.TileEntity = _source.TileEntity;
		_destination.Self = _source.Self;
		_destination.Instigator = _source.Instigator;
		_destination.Other = _source.Other;
		_destination.Others = _source.Others;
		_destination.ItemValue = _source.ItemValue;
		_destination.ItemActionData = _source.ItemActionData;
		_destination.ItemInventoryData = _source.ItemInventoryData;
		_destination.StartPosition = _source.StartPosition;
		_destination.Position = _source.Position;
		_destination.Transform = _source.Transform;
		_destination.Buff = _source.Buff;
		_destination.BlockValue = _source.BlockValue;
		_destination.POI = _source.POI;
		_destination.Area = _source.Area;
		_destination.Biome = _source.Biome;
		_destination.Tags = _source.Tags;
		_destination.DamageResponse = _source.DamageResponse;
		_destination.ProgressionValue = _source.ProgressionValue;
		_destination.Seed = _source.Seed;
	}

	// Token: 0x040027AC RID: 10156
	public static MinEventParams CachedEventParam = new MinEventParams();

	// Token: 0x040027AD RID: 10157
	public MinEffectController.SourceParentType ParentType;

	// Token: 0x040027AE RID: 10158
	public ITileEntity TileEntity;

	// Token: 0x040027AF RID: 10159
	public EntityAlive Self;

	// Token: 0x040027B0 RID: 10160
	public EntityAlive Instigator;

	// Token: 0x040027B1 RID: 10161
	public EntityAlive Other;

	// Token: 0x040027B2 RID: 10162
	public EntityAlive[] Others;

	// Token: 0x040027B3 RID: 10163
	public ItemValue ItemValue;

	// Token: 0x040027B4 RID: 10164
	public ItemActionData ItemActionData;

	// Token: 0x040027B5 RID: 10165
	public ItemInventoryData ItemInventoryData;

	// Token: 0x040027B6 RID: 10166
	public Vector3 StartPosition;

	// Token: 0x040027B7 RID: 10167
	public Vector3 Position;

	// Token: 0x040027B8 RID: 10168
	public Transform Transform;

	// Token: 0x040027B9 RID: 10169
	public BuffValue Buff;

	// Token: 0x040027BA RID: 10170
	public BlockValue BlockValue;

	// Token: 0x040027BB RID: 10171
	public PrefabInstance POI;

	// Token: 0x040027BC RID: 10172
	public Bounds Area;

	// Token: 0x040027BD RID: 10173
	public BiomeDefinition Biome;

	// Token: 0x040027BE RID: 10174
	public FastTags<TagGroup.Global> Tags;

	// Token: 0x040027BF RID: 10175
	public DamageResponse DamageResponse;

	// Token: 0x040027C0 RID: 10176
	public ProgressionValue ProgressionValue;

	// Token: 0x040027C1 RID: 10177
	public Challenge Challenge;

	// Token: 0x040027C2 RID: 10178
	public int Seed;

	// Token: 0x040027C3 RID: 10179
	public bool IsLocal;
}

using System;
using System.Collections.Generic;

// Token: 0x02000164 RID: 356
public class TileEntityFeatureData
{
	// Token: 0x06000AB1 RID: 2737 RVA: 0x0004559F File Offset: 0x0004379F
	public TileEntityFeatureData(TileEntityCompositeData _parent, string _name, int _customOrder, Type _type, DynamicProperties _props)
	{
		this.Parent = _parent;
		this.Name = _name;
		this.NameHash = this.Name.GetStableHashCode();
		this.CustomOrder = _customOrder;
		this.Type = _type;
		this.Props = _props;
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x000455DD File Offset: 0x000437DD
	public ITileEntityFeature InstantiateModule()
	{
		return ReflectionHelpers.Instantiate<ITileEntityFeature>(this.Type);
	}

	// Token: 0x04000959 RID: 2393
	public readonly TileEntityCompositeData Parent;

	// Token: 0x0400095A RID: 2394
	public readonly string Name;

	// Token: 0x0400095B RID: 2395
	public readonly int NameHash;

	// Token: 0x0400095C RID: 2396
	public readonly int CustomOrder;

	// Token: 0x0400095D RID: 2397
	public readonly Type Type;

	// Token: 0x0400095E RID: 2398
	public readonly DynamicProperties Props;

	// Token: 0x02000165 RID: 357
	public class FeatureDataSorterByName : IComparer<TileEntityFeatureData>
	{
		// Token: 0x06000AB3 RID: 2739 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Private)]
		public FeatureDataSorterByName()
		{
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x000455EA File Offset: 0x000437EA
		public int Compare(TileEntityFeatureData _x, TileEntityFeatureData _y)
		{
			if (_x == _y)
			{
				return 0;
			}
			if (_y == null)
			{
				return 1;
			}
			if (_x == null)
			{
				return -1;
			}
			return string.Compare(_x.Name, _y.Name, StringComparison.Ordinal);
		}

		// Token: 0x0400095F RID: 2399
		public static readonly TileEntityFeatureData.FeatureDataSorterByName Instance = new TileEntityFeatureData.FeatureDataSorterByName();
	}
}

using System;
using System.Collections.Generic;

// Token: 0x02000161 RID: 353
public class TileEntityCompositeData
{
	// Token: 0x06000AA0 RID: 2720 RVA: 0x00045047 File Offset: 0x00043247
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Init()
	{
		if (TileEntityCompositeData.knownFeatures.Count > 0)
		{
			return;
		}
		ReflectionHelpers.FindTypesImplementingBase(typeof(ITileEntityFeature), delegate(Type _type)
		{
			string name = _type.Name;
			Type type;
			if (TileEntityCompositeData.knownFeatures.TryGetValue(name, out type))
			{
				Log.Warning(string.Concat(new string[]
				{
					"Redeclaration of CompositeTileEntity feature ",
					name,
					": ",
					type.FullName,
					" vs ",
					_type.FullName
				}));
				return;
			}
			if (_type.GetConstructor(Type.EmptyTypes) == null)
			{
				Log.Warning("CompositeTileEntity feature " + name + " has no parameterless constructor!");
				return;
			}
			TileEntityCompositeData.knownFeatures[name] = _type;
		}, false);
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00045086 File Offset: 0x00043286
	public static void Cleanup()
	{
		TileEntityCompositeData.FeaturesByBlock.Clear();
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x00045094 File Offset: 0x00043294
	public static TileEntityCompositeData ParseBlock(BlockCompositeTileEntity _block)
	{
		DynamicProperties compositeProps;
		if (!_block.Properties.Classes.TryGetValue("CompositeFeatures", out compositeProps))
		{
			throw new ArgumentException("Block " + _block.GetBlockName() + " uses class BlockCompositeTileEntity but has no CompositeFeatures property");
		}
		return new TileEntityCompositeData(_block, compositeProps);
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x000450DC File Offset: 0x000432DC
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityCompositeData(BlockCompositeTileEntity _block, DynamicProperties _compositeProps)
	{
		TileEntityCompositeData.Init();
		this.Block = _block;
		this.CompositeProps = _compositeProps;
		int num = 0;
		foreach (KeyValuePair<string, DynamicProperties> keyValuePair in _compositeProps.Classes.Dict)
		{
			string text;
			DynamicProperties dynamicProperties;
			keyValuePair.Deconstruct(out text, out dynamicProperties);
			string text2 = text;
			DynamicProperties props = dynamicProperties;
			Type type;
			if (!TileEntityCompositeData.knownFeatures.TryGetValue(text2, out type))
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Block \"",
					_block.GetBlockName(),
					"\": CompositeFeature class \"",
					text2,
					"\" not found!"
				}));
			}
			TileEntityFeatureData item = new TileEntityFeatureData(this, text2, num, type, props);
			this.Features.Add(item);
			num++;
		}
		if (this.Features.Count == 0)
		{
			throw new ArgumentException("Block \"" + _block.GetBlockName() + "\": No CompositeFeatures specified!");
		}
		this.Features.Sort(TileEntityFeatureData.FeatureDataSorterByName.Instance);
		for (int i = 0; i < this.Features.Count; i++)
		{
			this.featureIndexByName[this.Features[i].Name.AsMemory()] = i;
		}
		TileEntityCompositeData.FeaturesByBlock[_block] = this;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x00045268 File Offset: 0x00043468
	public int GetFeatureIndex(ReadOnlyMemory<char> _featureName)
	{
		return this.featureIndexByName.GetValueOrDefault(_featureName, -1);
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00045278 File Offset: 0x00043478
	public int GetFeatureIndex<T>() where T : class
	{
		Type typeFromHandle = typeof(T);
		int result;
		if (this.featureIndexByType.TryGetValue(typeFromHandle, out result))
		{
			return result;
		}
		for (int i = 0; i < this.Features.Count; i++)
		{
			TileEntityFeatureData tileEntityFeatureData = this.Features[i];
			if (typeFromHandle.IsAssignableFrom(tileEntityFeatureData.Type))
			{
				this.featureIndexByType[typeFromHandle] = i;
				return i;
			}
		}
		this.featureIndexByType[typeFromHandle] = -1;
		return -1;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x000452F0 File Offset: 0x000434F0
	public bool HasFeature(ReadOnlyMemory<char> _featureName)
	{
		return this.GetFeatureIndex(_featureName) >= 0;
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x000452FF File Offset: 0x000434FF
	public bool HasFeature<T>() where T : class
	{
		return this.GetFeatureIndex<T>() >= 0;
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00045310 File Offset: 0x00043510
	public void PrintConfig()
	{
		Log.Out("Composite block: " + this.Block.GetBlockName() + ":");
		foreach (KeyValuePair<string, string> keyValuePair in this.CompositeProps.Values.Dict)
		{
			string text;
			string text2;
			keyValuePair.Deconstruct(out text, out text2);
			string str = text;
			string str2 = text2;
			Log.Out("    " + str + "=" + str2);
		}
		foreach (TileEntityFeatureData tileEntityFeatureData in this.Features)
		{
			Log.Out(string.Format("  Feature: {0} (class {1} in assembly ({2})), manual order = {3}:", new object[]
			{
				tileEntityFeatureData.Name,
				tileEntityFeatureData.Type.FullName,
				tileEntityFeatureData.Type.Assembly.FullName,
				tileEntityFeatureData.CustomOrder
			}));
			foreach (KeyValuePair<string, string> keyValuePair in tileEntityFeatureData.Props.Values.Dict)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text2, out text);
				string str3 = text2;
				string str4 = text;
				Log.Out("    " + str3 + "=" + str4);
			}
		}
	}

	// Token: 0x0400094F RID: 2383
	public static readonly Dictionary<BlockCompositeTileEntity, TileEntityCompositeData> FeaturesByBlock = new Dictionary<BlockCompositeTileEntity, TileEntityCompositeData>();

	// Token: 0x04000950 RID: 2384
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, Type> knownFeatures = new Dictionary<string, Type>();

	// Token: 0x04000951 RID: 2385
	public readonly BlockCompositeTileEntity Block;

	// Token: 0x04000952 RID: 2386
	public readonly DynamicProperties CompositeProps;

	// Token: 0x04000953 RID: 2387
	public readonly List<TileEntityFeatureData> Features = new List<TileEntityFeatureData>();

	// Token: 0x04000954 RID: 2388
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<Type, int> featureIndexByType = new Dictionary<Type, int>();

	// Token: 0x04000955 RID: 2389
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<ReadOnlyMemory<char>, int> featureIndexByName = new Dictionary<ReadOnlyMemory<char>, int>(TileEntityCompositeData.MemStringEqualityComparer.Instance);

	// Token: 0x02000162 RID: 354
	public class MemStringEqualityComparer : IEqualityComparer<ReadOnlyMemory<char>>
	{
		// Token: 0x06000AAA RID: 2730 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Private)]
		public MemStringEqualityComparer()
		{
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x000454CA File Offset: 0x000436CA
		public int GetHashCode(ReadOnlyMemory<char> _obj)
		{
			return _obj.Span.GetStableHashCode();
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x000454D8 File Offset: 0x000436D8
		public bool Equals(ReadOnlyMemory<char> _x, ReadOnlyMemory<char> _y)
		{
			return _x.Span.Equals(_y.Span, StringComparison.Ordinal);
		}

		// Token: 0x04000956 RID: 2390
		public static readonly TileEntityCompositeData.MemStringEqualityComparer Instance = new TileEntityCompositeData.MemStringEqualityComparer();
	}
}

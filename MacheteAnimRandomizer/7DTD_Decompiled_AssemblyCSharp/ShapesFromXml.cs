using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000BC1 RID: 3009
public static class ShapesFromXml
{
	// Token: 0x1700098C RID: 2444
	// (get) Token: 0x06005CB4 RID: 23732 RVA: 0x00257A88 File Offset: 0x00255C88
	public static ShapesFromXml.EDebugLevel DebugLevel
	{
		get
		{
			return ShapesFromXml.debug;
		}
	}

	// Token: 0x06005CB5 RID: 23733 RVA: 0x00257A90 File Offset: 0x00255C90
	[PublicizedFrom(EAccessModifier.Private)]
	static ShapesFromXml()
	{
		string launchArgument = GameUtils.GetLaunchArgument("debugshapes");
		if (launchArgument != null)
		{
			if (launchArgument == "verbose")
			{
				ShapesFromXml.debug = ShapesFromXml.EDebugLevel.Verbose;
			}
			else
			{
				ShapesFromXml.debug = ShapesFromXml.EDebugLevel.Normal;
			}
		}
		ShapesFromXml.TextureLabelsByChannel[0] = new ShapesFromXml.TextureLabels(string.Empty);
		for (int i = 1; i < 1; i++)
		{
			ShapesFromXml.TextureLabelsByChannel[i] = new ShapesFromXml.TextureLabels((i + 1).ToString());
		}
	}

	// Token: 0x06005CB6 RID: 23734 RVA: 0x00257B27 File Offset: 0x00255D27
	public static IEnumerator LoadShapes(XmlFile _xmlFile)
	{
		ShapesFromXml.shapes = new CaseInsensitiveStringDictionary<XElement>();
		ShapesFromXml.shapeCategories.Clear();
		BlockShapeNew.Cleanup();
		XElement root = _xmlFile.XmlDoc.Root;
		if (root == null || !root.HasElements)
		{
			throw new Exception("No element <shapes> found!");
		}
		int num = 1;
		foreach (XElement xelement in root.Elements("shape"))
		{
			string attribute = xelement.GetAttribute(XNames.name);
			ShapesFromXml.SetProperty(xelement, Block.PropCreativeSort2, XNames.value, num++.ToString("0000"));
			ShapesFromXml.shapes.Add(attribute, xelement);
		}
		using (IEnumerator<XElement> enumerator = root.Elements("categories").GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XElement parentElement = enumerator.Current;
				ShapesFromXml.ParseCategories(parentElement);
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x06005CB7 RID: 23735 RVA: 0x00257B36 File Offset: 0x00255D36
	public static void Cleanup()
	{
		ShapesFromXml.shapes.Clear();
		ShapesFromXml.shapes = null;
	}

	// Token: 0x06005CB8 RID: 23736 RVA: 0x00257B48 File Offset: 0x00255D48
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseCategories(XElement _parentElement)
	{
		foreach (XElement element in _parentElement.Elements(XNames.category))
		{
			string attribute = element.GetAttribute(XNames.name);
			string attribute2 = element.GetAttribute(XNames.icon);
			int order = int.Parse(element.GetAttribute(XNames.order));
			ShapesFromXml.shapeCategories.Add(attribute, new ShapesFromXml.ShapeCategory(attribute, attribute2, order));
		}
	}

	// Token: 0x06005CB9 RID: 23737 RVA: 0x00257BD0 File Offset: 0x00255DD0
	public static IEnumerator CreateShapeVariants(bool _bEditMode, XElement _elementBlock)
	{
		string blockBaseName = _elementBlock.GetAttribute(XNames.name);
		string allowedShapes = _elementBlock.GetAttribute(XNames.shapes);
		XAttribute xattribute = _elementBlock.Attribute(XNames.hideHelperInCreative);
		bool hideHelperInCreative;
		StringParsers.TryParseBool(((xattribute != null) ? xattribute.Value : null) ?? "false", out hideHelperInCreative, 0, -1, true);
		if (ShapesFromXml.debug != ShapesFromXml.EDebugLevel.Off)
		{
			Log.Out("Creating block+shape combinations for base block " + blockBaseName);
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
		}
		List<string> shapeNames = new List<string>();
		bool isAll = allowedShapes.EqualsCaseInsensitive("All");
		MicroStopwatch msw = new MicroStopwatch(true);
		foreach (KeyValuePair<string, XElement> shapeKvp in ShapesFromXml.shapes)
		{
			if (isAll || shapeKvp.Value.GetAttribute(XNames.tag).EqualsCaseInsensitive(allowedShapes))
			{
				ShapesFromXml.CreateShapeMaterialCombination(_bEditMode, _elementBlock, blockBaseName, shapeKvp);
				shapeNames.Add(shapeKvp.Key);
				if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
				{
					yield return null;
					msw.ResetAndRestart();
				}
			}
		}
		Dictionary<string, XElement>.Enumerator enumerator = default(Dictionary<string, XElement>.Enumerator);
		if (shapeNames.Count > 0)
		{
			ShapesFromXml.CreateMaterialHelper(_bEditMode, blockBaseName, shapeNames, hideHelperInCreative);
		}
		if (ShapesFromXml.debug != ShapesFromXml.EDebugLevel.Off)
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
		}
		yield break;
		yield break;
	}

	// Token: 0x06005CBA RID: 23738 RVA: 0x00257BE8 File Offset: 0x00255DE8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CreateMaterialHelper(bool _bEditMode, string _blockBaseName, List<string> _shapeNames, bool _hideHelperInCreative)
	{
		string text = _blockBaseName + ":" + ShapesFromXml.VariantHelperName;
		string text2 = _blockBaseName + ":" + _shapeNames[0];
		DynamicProperties dynamicProperties = BlocksFromXml.CreateProperties(text2, null);
		dynamicProperties.SetValue("Extends", text2);
		dynamicProperties.SetValue(Block.PropCreativeMode, (_hideHelperInCreative ? EnumCreativeMode.None : EnumCreativeMode.All).ToStringCached<EnumCreativeMode>());
		dynamicProperties.SetValue(Block.PropCreativeSort2, "0000");
		dynamicProperties.SetValue(Block.PropDescriptionKey, "blockVariantHelperGroupDesc");
		dynamicProperties.SetValue(Block.PropItemTypeIcon, "all_blocks");
		dynamicProperties.SetValue("SelectAlternates", "true");
		string value = _blockBaseName + ":" + string.Join("," + _blockBaseName + ":", _shapeNames);
		dynamicProperties.SetValue(Block.PropPlaceAltBlockValue, value);
		dynamicProperties.SetValue(Block.PropAutoShape, EAutoShapeType.Helper.ToStringCached<EAutoShapeType>());
		if (ShapesFromXml.debug != ShapesFromXml.EDebugLevel.Off)
		{
			Console.WriteLine("{0}: {1}", text, dynamicProperties.PrettyPrint());
		}
		Block block = BlocksFromXml.CreateBlock(_bEditMode, text, dynamicProperties);
		BlocksFromXml.LoadExtendedItemDrops(block);
		BlocksFromXml.InitBlock(block);
	}

	// Token: 0x06005CBB RID: 23739 RVA: 0x00257CF0 File Offset: 0x00255EF0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CreateShapeMaterialCombination(bool _bEditMode, XElement _elementBlock, string _blockBaseName, KeyValuePair<string, XElement> _shapeKvp)
	{
		string key = _shapeKvp.Key;
		string text = _blockBaseName + ":" + key;
		string text2;
		string excludedPropertiesList;
		BlocksFromXml.ParseExtendedBlock(_elementBlock, out text2, out excludedPropertiesList);
		if (text2 == null)
		{
			BlocksFromXml.ParseExtendedBlock(_shapeKvp.Value, out text2, out excludedPropertiesList);
		}
		DynamicProperties dynamicProperties = BlocksFromXml.CreateProperties(text2, excludedPropertiesList);
		BlocksFromXml.LoadProperties(dynamicProperties, _elementBlock);
		BlocksFromXml.LoadProperties(dynamicProperties, _shapeKvp.Value);
		if (dynamicProperties.Contains(Block.PropDowngradeBlock))
		{
			string text3 = dynamicProperties.Values[Block.PropDowngradeBlock];
			text3 = ShapesFromXml.AppendShapeName(text3, _blockBaseName, key);
			dynamicProperties.SetValue(Block.PropDowngradeBlock, text3);
		}
		if (dynamicProperties.Contains(Block.PropUpgradeBlockClassToBlock))
		{
			string text4 = dynamicProperties.Values[Block.PropUpgradeBlockClassToBlock];
			text4 = ShapesFromXml.AppendShapeName(text4, _blockBaseName, key);
			dynamicProperties.SetValue("UpgradeBlock", "ToBlock", text4);
		}
		if (dynamicProperties.Contains(Block.PropSiblingBlock))
		{
			string text5 = dynamicProperties.Values[Block.PropSiblingBlock];
			text5 = ShapesFromXml.PrependBlockBaseName(text5, _blockBaseName);
			dynamicProperties.SetValue(Block.PropSiblingBlock, text5);
		}
		if (dynamicProperties.Contains("MirrorSibling"))
		{
			string text6 = dynamicProperties.Values["MirrorSibling"];
			text6 = ShapesFromXml.PrependBlockBaseName(text6, _blockBaseName);
			dynamicProperties.SetValue("MirrorSibling", text6);
		}
		dynamicProperties.SetValue(Block.PropCreativeMode, EnumCreativeMode.Dev.ToStringCached<EnumCreativeMode>());
		dynamicProperties.SetParam1(Block.PropCanPickup, _blockBaseName + ":" + ShapesFromXml.VariantHelperName);
		ShapesFromXml.FixCustomIcon(dynamicProperties, _blockBaseName, _shapeKvp.Key);
		ShapesFromXml.FixImposterExchangeId(dynamicProperties, _blockBaseName, _shapeKvp.Key, 0);
		for (int i = 0; i < 1; i++)
		{
			ShapesFromXml.FixTextureId(dynamicProperties, _blockBaseName, _shapeKvp.Key, i);
		}
		ShapesFromXml.SetMaxDamage(dynamicProperties, _blockBaseName, _shapeKvp.Key);
		dynamicProperties.SetValue("AutoShape", EAutoShapeType.Shape.ToStringCached<EAutoShapeType>());
		if (ShapesFromXml.debug != ShapesFromXml.EDebugLevel.Off)
		{
			Console.WriteLine("{0}: {1}", text, dynamicProperties.PrettyPrint());
		}
		Block block = BlocksFromXml.CreateBlock(_bEditMode, text, dynamicProperties);
		bool flag;
		BlocksFromXml.ParseItemDrops(block, _shapeKvp.Value, out flag);
		if (block.itemsToDrop.Count == 0)
		{
			bool flag2;
			BlocksFromXml.ParseItemDrops(block, _elementBlock, out flag2);
			if (!flag2 && !flag)
			{
				BlocksFromXml.LoadExtendedItemDrops(block);
			}
		}
		BlocksFromXml.InitBlock(block);
	}

	// Token: 0x06005CBC RID: 23740 RVA: 0x00257F2C File Offset: 0x0025612C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetMaxDamage(DynamicProperties _properties, string _blockBaseName, string _shapeName)
	{
		if (!_properties.Contains("MaterialHitpointMultiplier"))
		{
			return;
		}
		if (!_properties.Contains("Material"))
		{
			Log.Warning(string.Concat(new string[]
			{
				"Blocks: Shape ",
				_shapeName,
				" defines a 'MaterialHitpointMultiplier' but block template ",
				_blockBaseName,
				" does not define a 'Material'!"
			}));
			return;
		}
		float num = StringParsers.ParseFloat(_properties.GetString("MaterialHitpointMultiplier"), 0, -1, NumberStyles.Any);
		if (num == 1f)
		{
			return;
		}
		string @string = _properties.GetString("Material");
		MaterialBlock materialBlock = MaterialBlock.fromString(@string);
		if (materialBlock == null)
		{
			Log.Error(string.Concat(new string[]
			{
				"Blocks: Block template ",
				_blockBaseName,
				" refers to an unknown Material '",
				@string,
				"'!"
			}));
			return;
		}
		int v = Mathf.RoundToInt(num * (float)materialBlock.MaxDamage);
		v = Utils.FastClamp(v, 1, 65535);
		_properties.SetValue(Block.PropMaxDamage, v.ToString());
		ShapesFromXml.ScaleProperty(_properties, Block.PropUpgradeBlockClass, Block.PropUpgradeBlockItemCount, num);
		_properties.SetValue(Block.PropResourceScale, num.ToCultureInvariantString());
	}

	// Token: 0x06005CBD RID: 23741 RVA: 0x00258040 File Offset: 0x00256240
	[PublicizedFrom(EAccessModifier.Private)]
	public static void FixCustomIcon(DynamicProperties _properties, string _blockBaseName, string _shapeName)
	{
		if (_properties.Contains(Block.PropCustomIcon))
		{
			return;
		}
		if (!_properties.Contains("Model"))
		{
			Log.Warning(string.Concat(new string[]
			{
				"Blocks: Neither shape ",
				_shapeName,
				" nor the block template ",
				_blockBaseName,
				" define a 'CustomIcon' or 'Model'!"
			}));
			return;
		}
		string @string = _properties.GetString("Model");
		_properties.SetValue(Block.PropCustomIcon, "shape" + @string);
	}

	// Token: 0x06005CBE RID: 23742 RVA: 0x002580BC File Offset: 0x002562BC
	public static void SetProperty(XElement _element, string _propertyName, XName _attribName, string _value)
	{
		XElement xelement = (from e in _element.Elements(XNames.property)
		where e.GetAttribute(XNames.name) == _propertyName
		select e).FirstOrDefault<XElement>();
		if (xelement == null)
		{
			xelement = new XElement(XNames.property, new XAttribute(XNames.name, _propertyName));
			_element.Add(xelement);
		}
		xelement.SetAttributeValue(_attribName, _value);
	}

	// Token: 0x06005CBF RID: 23743 RVA: 0x00258128 File Offset: 0x00256328
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ScaleProperty(DynamicProperties _properties, string _className, string _propertyName, float _scale)
	{
		if (_properties.Contains(_className, _propertyName))
		{
			int num = int.Parse(_properties.GetString(_className, _propertyName));
			if (num > 0)
			{
				num = (int)((float)num * _scale);
				if (num < 1)
				{
					num = 1;
				}
				_properties.SetValue(_className, _propertyName, num.ToString());
			}
		}
	}

	// Token: 0x06005CC0 RID: 23744 RVA: 0x0025816C File Offset: 0x0025636C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void FixImposterExchangeId(DynamicProperties _properties, string _blockBaseName, string _shapeName, int channel)
	{
		string texture = ShapesFromXml.TextureLabelsByChannel[channel].Texture;
		string imposterExchange = ShapesFromXml.TextureLabelsByChannel[channel].ImposterExchange;
		if (!_properties.Contains(imposterExchange))
		{
			return;
		}
		if (!_properties.Contains(texture))
		{
			Log.Warning(string.Concat(new string[]
			{
				"Blocks: Shape ",
				_shapeName,
				" defines ",
				imposterExchange,
				" but block template ",
				_blockBaseName,
				" does not have a '",
				texture,
				"' property!"
			}));
			return;
		}
		int id = BlockTextureData.GetDataByTextureID(int.Parse(_properties.GetString(texture))).ID;
		_properties.SetParam1(imposterExchange, id.ToString());
	}

	// Token: 0x06005CC1 RID: 23745 RVA: 0x00258220 File Offset: 0x00256420
	[PublicizedFrom(EAccessModifier.Private)]
	public static void FixTextureId(DynamicProperties _properties, string _blockBaseName, string _shapeName, int channel)
	{
		string texture = ShapesFromXml.TextureLabelsByChannel[channel].Texture;
		string shapeAltTexture = ShapesFromXml.TextureLabelsByChannel[channel].ShapeAltTexture;
		if (!_properties.Contains(shapeAltTexture))
		{
			return;
		}
		if (!_properties.Contains(texture))
		{
			Log.Warning(string.Concat(new string[]
			{
				"Blocks: Shape ",
				_shapeName,
				" defines ",
				shapeAltTexture,
				" but block template ",
				_blockBaseName,
				" does not have a '",
				texture,
				"' property!"
			}));
			return;
		}
		string[] array = _properties.GetString(shapeAltTexture).Split(',', StringSplitOptions.None);
		string @string = _properties.GetString(texture);
		for (int i = 0; i < array.Length; i++)
		{
			int num;
			if (!int.TryParse(array[i], out num))
			{
				array[i] = @string;
			}
		}
		string value = string.Join(",", array);
		_properties.SetValue(texture, value);
	}

	// Token: 0x06005CC2 RID: 23746 RVA: 0x002582FE File Offset: 0x002564FE
	[PublicizedFrom(EAccessModifier.Private)]
	public static string AppendShapeName(string _innerText, string _blockBaseName, string _shapeName)
	{
		if (_innerText[0] == ':')
		{
			return _blockBaseName + _innerText;
		}
		if (!_innerText.Contains(":"))
		{
			return _innerText + ":" + _shapeName;
		}
		return _innerText;
	}

	// Token: 0x06005CC3 RID: 23747 RVA: 0x0025832E File Offset: 0x0025652E
	[PublicizedFrom(EAccessModifier.Private)]
	public static string PrependBlockBaseName(string _innerText, string _blockBaseName)
	{
		return _blockBaseName + ":" + _innerText;
	}

	// Token: 0x0400465C RID: 18012
	[PublicizedFrom(EAccessModifier.Private)]
	public static ShapesFromXml.EDebugLevel debug = ShapesFromXml.EDebugLevel.Off;

	// Token: 0x0400465D RID: 18013
	public static readonly ShapesFromXml.TextureLabels[] TextureLabelsByChannel = new ShapesFromXml.TextureLabels[1];

	// Token: 0x0400465E RID: 18014
	public static readonly string VariantHelperName = "VariantHelper";

	// Token: 0x0400465F RID: 18015
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, XElement> shapes;

	// Token: 0x04004660 RID: 18016
	public static readonly Dictionary<string, ShapesFromXml.ShapeCategory> shapeCategories = new CaseInsensitiveStringDictionary<ShapesFromXml.ShapeCategory>();

	// Token: 0x02000BC2 RID: 3010
	public class ShapeCategory : IComparable<ShapesFromXml.ShapeCategory>, IEquatable<ShapesFromXml.ShapeCategory>, IComparable
	{
		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x06005CC4 RID: 23748 RVA: 0x0025833C File Offset: 0x0025653C
		public string LocalizedName
		{
			get
			{
				return Localization.Get(this.localizationName, false);
			}
		}

		// Token: 0x06005CC5 RID: 23749 RVA: 0x0025834A File Offset: 0x0025654A
		public ShapeCategory(string _name, string _icon, int _order)
		{
			this.Name = _name;
			this.Icon = _icon;
			this.Order = _order;
			this.localizationName = "shapeCategory" + this.Name;
		}

		// Token: 0x06005CC6 RID: 23750 RVA: 0x00258380 File Offset: 0x00256580
		public bool Equals(ShapesFromXml.ShapeCategory _other)
		{
			return _other != null && (this == _other || (this.Name == _other.Name && this.Icon == _other.Icon && this.Order == _other.Order));
		}

		// Token: 0x06005CC7 RID: 23751 RVA: 0x002583CE File Offset: 0x002565CE
		public override bool Equals(object _obj)
		{
			return _obj != null && (this == _obj || (!(_obj.GetType() != base.GetType()) && this.Equals((ShapesFromXml.ShapeCategory)_obj)));
		}

		// Token: 0x06005CC8 RID: 23752 RVA: 0x002583FC File Offset: 0x002565FC
		public override int GetHashCode()
		{
			return (((this.Name != null) ? this.Name.GetHashCode() : 0) * 397 ^ ((this.Icon != null) ? this.Icon.GetHashCode() : 0)) * 397 ^ this.Order;
		}

		// Token: 0x06005CC9 RID: 23753 RVA: 0x0025844C File Offset: 0x0025664C
		public int CompareTo(ShapesFromXml.ShapeCategory _other)
		{
			if (this == _other)
			{
				return 0;
			}
			if (_other == null)
			{
				return 1;
			}
			return this.Order.CompareTo(_other.Order);
		}

		// Token: 0x06005CCA RID: 23754 RVA: 0x00258478 File Offset: 0x00256678
		public int CompareTo(object _obj)
		{
			if (_obj == null)
			{
				return 1;
			}
			if (this == _obj)
			{
				return 0;
			}
			ShapesFromXml.ShapeCategory shapeCategory = _obj as ShapesFromXml.ShapeCategory;
			if (shapeCategory == null)
			{
				throw new ArgumentException("Object must be of type ShapeCategory");
			}
			return this.CompareTo(shapeCategory);
		}

		// Token: 0x04004661 RID: 18017
		public readonly string Name;

		// Token: 0x04004662 RID: 18018
		public readonly string Icon;

		// Token: 0x04004663 RID: 18019
		public readonly int Order;

		// Token: 0x04004664 RID: 18020
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string localizationName;
	}

	// Token: 0x02000BC3 RID: 3011
	public enum EDebugLevel
	{
		// Token: 0x04004666 RID: 18022
		Off,
		// Token: 0x04004667 RID: 18023
		Normal,
		// Token: 0x04004668 RID: 18024
		Verbose
	}

	// Token: 0x02000BC4 RID: 3012
	public struct TextureLabels
	{
		// Token: 0x06005CCB RID: 23755 RVA: 0x002584AC File Offset: 0x002566AC
		public TextureLabels(string suffix)
		{
			this.Texture = "Texture" + suffix;
			this.ImposterExchange = "ImposterExchange" + suffix;
			this.ShapeAltTexture = "ShapeAltTexture" + suffix;
			this.UseGlobalUV = "UseGlobalUV" + suffix;
		}

		// Token: 0x04004669 RID: 18025
		public readonly string Texture;

		// Token: 0x0400466A RID: 18026
		public readonly string ImposterExchange;

		// Token: 0x0400466B RID: 18027
		public readonly string ShapeAltTexture;

		// Token: 0x0400466C RID: 18028
		public readonly string UseGlobalUV;
	}
}

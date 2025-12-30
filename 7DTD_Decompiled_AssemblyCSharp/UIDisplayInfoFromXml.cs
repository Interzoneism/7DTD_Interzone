using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

// Token: 0x02000BD0 RID: 3024
public class UIDisplayInfoFromXml
{
	// Token: 0x06005D25 RID: 23845 RVA: 0x0025CE50 File Offset: 0x0025B050
	public static IEnumerator Load(XmlFile xmlFile)
	{
		XElement root = xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <ui_display_info> found!");
		}
		UIDisplayInfoFromXml.ParseNode(root);
		yield break;
	}

	// Token: 0x06005D26 RID: 23846 RVA: 0x0025CE60 File Offset: 0x0025B060
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseNode(XElement root)
	{
		foreach (XElement xelement in root.Elements())
		{
			if (xelement.Name == "item_display")
			{
				using (IEnumerator<XElement> enumerator2 = xelement.Elements().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						XElement e = enumerator2.Current;
						UIDisplayInfoFromXml.ParseItemDisplayInfo(e);
					}
					continue;
				}
			}
			if (xelement.Name == "character_stat_display")
			{
				using (IEnumerator<XElement> enumerator2 = xelement.Elements().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						XElement node = enumerator2.Current;
						DisplayInfoEntry displayInfoEntry = UIDisplayInfoFromXml.ParseDisplayInfoEntry(node);
						if (displayInfoEntry != null)
						{
							UIDisplayInfoManager.Current.AddCharacterDisplayInfo(displayInfoEntry);
						}
					}
					continue;
				}
			}
			if (xelement.Name == "crafting_category_display")
			{
				using (IEnumerator<XElement> enumerator2 = xelement.Elements().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						XElement e2 = enumerator2.Current;
						UIDisplayInfoFromXml.ParseCraftingCategoryList(e2);
					}
					continue;
				}
			}
			if (xelement.Name == "trader_category_display")
			{
				UIDisplayInfoFromXml.ParseTraderCategoryList(xelement);
			}
		}
	}

	// Token: 0x06005D27 RID: 23847 RVA: 0x0025CFFC File Offset: 0x0025B1FC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseItemDisplayInfo(XElement e)
	{
		if (!e.HasAttribute("display_type"))
		{
			throw new Exception("item_display_info must have an display_type attribute");
		}
		string attribute = e.GetAttribute("display_type");
		if (UIDisplayInfoManager.Current.ContainsItemDisplayStats(attribute))
		{
			throw new Exception("Duplicate item_display_info entry with tag " + attribute);
		}
		string group = attribute;
		if (e.HasAttribute("display_group"))
		{
			group = e.GetAttribute("display_group");
		}
		UIDisplayInfoManager.Current.AddItemDisplayStats(attribute, group);
		foreach (XElement node in e.Elements("display_entry"))
		{
			DisplayInfoEntry displayInfoEntry = UIDisplayInfoFromXml.ParseDisplayInfoEntry(node);
			if (displayInfoEntry != null)
			{
				UIDisplayInfoManager.Current.AddItemDisplayInfo(attribute, displayInfoEntry);
			}
		}
		if (!e.HasAttribute("extends"))
		{
			return;
		}
		string attribute2 = e.GetAttribute("extends");
		if (UIDisplayInfoManager.Current.ContainsItemDisplayStats(attribute2))
		{
			ItemDisplayEntry displayStatsForTag = UIDisplayInfoManager.Current.GetDisplayStatsForTag(attribute2);
			for (int i = 0; i < displayStatsForTag.DisplayStats.Count; i++)
			{
				UIDisplayInfoManager.Current.AddItemDisplayInfo(attribute, displayStatsForTag.DisplayStats[i]);
			}
			return;
		}
		throw new Exception(string.Format("Extends item_display_info {0} is not specified.'", attribute2));
	}

	// Token: 0x06005D28 RID: 23848 RVA: 0x0025D164 File Offset: 0x0025B364
	[PublicizedFrom(EAccessModifier.Private)]
	public static DisplayInfoEntry ParseDisplayInfoEntry(XElement node)
	{
		DisplayInfoEntry displayInfoEntry = new DisplayInfoEntry();
		if (node.HasAttribute("name"))
		{
			string attribute = node.GetAttribute("name");
			try
			{
				displayInfoEntry.StatType = EnumUtils.Parse<PassiveEffects>(attribute, true);
			}
			catch
			{
				displayInfoEntry.CustomName = attribute;
			}
		}
		if (node.HasAttribute("display_type"))
		{
			displayInfoEntry.DisplayType = EnumUtils.Parse<DisplayInfoEntry.DisplayTypes>(node.GetAttribute("display_type"), true);
		}
		if (node.HasAttribute("show_inverted"))
		{
			displayInfoEntry.ShowInverted = Convert.ToBoolean(node.GetAttribute("show_inverted"));
		}
		if (node.HasAttribute("title_key"))
		{
			displayInfoEntry.TitleOverride = Localization.Get(node.GetAttribute("title_key"), false);
		}
		if (node.HasAttribute("negative_preferred"))
		{
			displayInfoEntry.NegativePreferred = Convert.ToBoolean(node.GetAttribute("negative_preferred"));
		}
		if (node.HasAttribute("display_leading_plus"))
		{
			displayInfoEntry.DisplayLeadingPlus = Convert.ToBoolean(node.GetAttribute("display_leading_plus"));
		}
		if (node.HasAttribute("tags"))
		{
			displayInfoEntry.Tags = FastTags<TagGroup.Global>.Parse(node.GetAttribute("tags"));
		}
		return displayInfoEntry;
	}

	// Token: 0x06005D29 RID: 23849 RVA: 0x0025D2D4 File Offset: 0x0025B4D4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseCraftingCategoryList(XElement e)
	{
		if (!e.HasAttribute("display_type"))
		{
			throw new Exception("crafting_category_list must have an display_type attribute");
		}
		string attribute = e.GetAttribute("display_type");
		if (UIDisplayInfoManager.Current.ContainsCraftingCategoryList(attribute))
		{
			throw new Exception("Duplicate crafting_category_list entry with tag " + attribute);
		}
		foreach (XElement node in e.Elements("crafting_category"))
		{
			CraftingCategoryDisplayEntry craftingCategoryDisplayEntry = UIDisplayInfoFromXml.ParseCraftingCategory(node);
			if (craftingCategoryDisplayEntry != null)
			{
				UIDisplayInfoManager.Current.AddCraftingCategoryDisplayItem(attribute, craftingCategoryDisplayEntry);
			}
		}
	}

	// Token: 0x06005D2A RID: 23850 RVA: 0x0025D384 File Offset: 0x0025B584
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseTraderCategoryList(XElement e)
	{
		foreach (XElement node in e.Elements("trader_category"))
		{
			CraftingCategoryDisplayEntry craftingCategoryDisplayEntry = UIDisplayInfoFromXml.ParseCraftingCategory(node);
			if (craftingCategoryDisplayEntry != null)
			{
				UIDisplayInfoManager.Current.AddTraderCategoryDIsplayItem(craftingCategoryDisplayEntry);
			}
		}
	}

	// Token: 0x06005D2B RID: 23851 RVA: 0x0025D3E8 File Offset: 0x0025B5E8
	[PublicizedFrom(EAccessModifier.Private)]
	public static CraftingCategoryDisplayEntry ParseCraftingCategory(XElement node)
	{
		CraftingCategoryDisplayEntry craftingCategoryDisplayEntry = new CraftingCategoryDisplayEntry();
		if (node.HasAttribute("name"))
		{
			craftingCategoryDisplayEntry.Name = node.GetAttribute("name");
		}
		if (node.HasAttribute("icon"))
		{
			craftingCategoryDisplayEntry.Icon = node.GetAttribute("icon");
		}
		if (node.HasAttribute("display_name"))
		{
			craftingCategoryDisplayEntry.DisplayName = Localization.Get(node.GetAttribute("display_name"), false);
		}
		return craftingCategoryDisplayEntry;
	}
}

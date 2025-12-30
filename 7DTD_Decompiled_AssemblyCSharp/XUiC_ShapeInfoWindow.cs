using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E23 RID: 3619
[Preserve]
public class XUiC_ShapeInfoWindow : XUiController
{
	// Token: 0x0600714D RID: 29005 RVA: 0x002A9093 File Offset: 0x002A7293
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty && base.ViewComponent.IsVisible)
		{
			this.IsDirty = false;
		}
	}

	// Token: 0x0600714E RID: 29006 RVA: 0x002E2834 File Offset: 0x002E0A34
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 1674238749U)
		{
			if (num <= 1573573035U)
			{
				if (num != 325231145U)
				{
					if (num != 1556795416U)
					{
						if (num == 1573573035U)
						{
							if (_bindingName == "itemstattitle3")
							{
								_value = this.GetStatTitle(2);
								return true;
							}
						}
					}
					else if (_bindingName == "itemstattitle2")
					{
						_value = this.GetStatTitle(1);
						return true;
					}
				}
				else if (_bindingName == "blockname")
				{
					_value = this.shapeName;
					return true;
				}
			}
			else if (num != 1607128273U)
			{
				if (num != 1657461130U)
				{
					if (num == 1674238749U)
					{
						if (_bindingName == "itemstattitle5")
						{
							_value = this.GetStatTitle(4);
							return true;
						}
					}
				}
				else if (_bindingName == "itemstattitle4")
				{
					_value = this.GetStatTitle(3);
					return true;
				}
			}
			else if (_bindingName == "itemstattitle1")
			{
				_value = this.GetStatTitle(0);
				return true;
			}
		}
		else if (num <= 4140647608U)
		{
			if (num != 2594821807U)
			{
				if (num != 4048624866U)
				{
					if (num == 4140647608U)
					{
						if (_bindingName == "itemstat4")
						{
							_value = this.GetStatValue(3);
							return true;
						}
					}
				}
				else if (_bindingName == "blockicontint")
				{
					Color32 v = Color.white;
					if (this.blockData != null)
					{
						v = this.blockData.CustomIconTint;
					}
					_value = this.itemicontintcolorFormatter.Format(v);
					return true;
				}
			}
			else if (_bindingName == "blockicon")
			{
				_value = ((this.blockData == null) ? "" : this.blockData.GetIconName());
				return true;
			}
		}
		else if (num <= 4224535703U)
		{
			if (num != 4157425227U)
			{
				if (num == 4224535703U)
				{
					if (_bindingName == "itemstat1")
					{
						_value = this.GetStatValue(0);
						return true;
					}
				}
			}
			else if (_bindingName == "itemstat5")
			{
				_value = this.GetStatValue(4);
				return true;
			}
		}
		else if (num != 4241313322U)
		{
			if (num == 4258090941U)
			{
				if (_bindingName == "itemstat3")
				{
					_value = this.GetStatValue(2);
					return true;
				}
			}
		}
		else if (_bindingName == "itemstat2")
		{
			_value = this.GetStatValue(1);
			return true;
		}
		return false;
	}

	// Token: 0x0600714F RID: 29007 RVA: 0x002E2AE4 File Offset: 0x002E0CE4
	public void SetShape(Block _newBlockData)
	{
		this.blockData = _newBlockData;
		if (_newBlockData != null)
		{
			if (_newBlockData.GetAutoShapeType() == EAutoShapeType.None)
			{
				this.shapeName = this.blockData.GetLocalizedBlockName();
			}
			else
			{
				this.shapeName = this.blockData.GetLocalizedAutoShapeShapeName();
			}
		}
		else
		{
			this.shapeName = "";
		}
		this.itemValue = _newBlockData.ToBlockValue().ToItemValue();
		this.itemDisplayEntry = UIDisplayInfoManager.Current.GetDisplayStatsForTag(_newBlockData.DisplayType);
		base.RefreshBindings(false);
	}

	// Token: 0x06007150 RID: 29008 RVA: 0x002E2B68 File Offset: 0x002E0D68
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetStatTitle(int index)
	{
		if (this.itemDisplayEntry == null || this.itemDisplayEntry.DisplayStats.Count <= index)
		{
			return "";
		}
		if (this.itemDisplayEntry.DisplayStats[index].TitleOverride != null)
		{
			return this.itemDisplayEntry.DisplayStats[index].TitleOverride;
		}
		return UIDisplayInfoManager.Current.GetLocalizedName(this.itemDisplayEntry.DisplayStats[index].StatType);
	}

	// Token: 0x06007151 RID: 29009 RVA: 0x002E2BE8 File Offset: 0x002E0DE8
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetStatValue(int index)
	{
		if (this.itemDisplayEntry == null || this.itemDisplayEntry.DisplayStats.Count <= index)
		{
			return "";
		}
		DisplayInfoEntry infoEntry = this.itemDisplayEntry.DisplayStats[index];
		return XUiM_ItemStack.GetStatItemValueTextWithCompareInfo(this.itemValue, ItemValue.None, base.xui.playerUI.entityPlayer, infoEntry, false, true);
	}

	// Token: 0x04005625 RID: 22053
	[PublicizedFrom(EAccessModifier.Private)]
	public Block blockData;

	// Token: 0x04005626 RID: 22054
	[PublicizedFrom(EAccessModifier.Private)]
	public string shapeName = "";

	// Token: 0x04005627 RID: 22055
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue itemValue;

	// Token: 0x04005628 RID: 22056
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemDisplayEntry itemDisplayEntry;

	// Token: 0x04005629 RID: 22057
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor itemicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000E3D RID: 3645
[Preserve]
public class XUiC_SkillPerkInfoWindow : XUiC_InfoWindow
{
	// Token: 0x17000B98 RID: 2968
	// (get) Token: 0x06007271 RID: 29297 RVA: 0x002EA8A3 File Offset: 0x002E8AA3
	public ProgressionValue CurrentSkill
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (base.xui.selectedSkill == null || !base.xui.selectedSkill.ProgressionClass.IsPerk)
			{
				return null;
			}
			return base.xui.selectedSkill;
		}
	}

	// Token: 0x17000B99 RID: 2969
	// (get) Token: 0x06007272 RID: 29298 RVA: 0x002EA8D6 File Offset: 0x002E8AD6
	// (set) Token: 0x06007273 RID: 29299 RVA: 0x002EA8DE File Offset: 0x002E8ADE
	public int HoveredLevel
	{
		get
		{
			return this.hoveredLevel;
		}
		set
		{
			if (this.hoveredLevel != value)
			{
				this.hoveredLevel = value;
				base.RefreshBindings(false);
			}
		}
	}

	// Token: 0x06007274 RID: 29300 RVA: 0x002EA8F8 File Offset: 0x002E8AF8
	public override void Init()
	{
		base.Init();
		base.GetChildrenByType<XUiC_SkillPerkLevel>(this.levelEntries);
		int num = 1;
		foreach (XUiC_SkillPerkLevel xuiC_SkillPerkLevel in this.levelEntries)
		{
			xuiC_SkillPerkLevel.ListIndex = num - 1;
			xuiC_SkillPerkLevel.Level = num++;
			xuiC_SkillPerkLevel.HiddenEntriesWithPaging = this.hiddenEntriesWithPaging;
			xuiC_SkillPerkLevel.MaxEntriesWithoutPaging = this.levelEntries.Count;
			xuiC_SkillPerkLevel.OnScroll += this.Entry_OnScroll;
			xuiC_SkillPerkLevel.OnHover += this.Entry_OnHover;
			xuiC_SkillPerkLevel.btnBuy.Controller.OnHover += this.Entry_OnHover;
		}
		this.actionItemList = base.GetChildByType<XUiC_ItemActionList>();
		this.skillsPerPage = this.levelEntries.Count - this.hiddenEntriesWithPaging;
		this.pager = base.GetChildByType<XUiC_Paging>();
		if (this.pager != null)
		{
			this.pager.OnPageChanged += this.Pager_OnPageChanged;
		}
	}

	// Token: 0x06007275 RID: 29301 RVA: 0x002EAA18 File Offset: 0x002E8C18
	[PublicizedFrom(EAccessModifier.Private)]
	public void Entry_OnHover(XUiController _sender, bool _isOver)
	{
		XUiC_SkillPerkLevel xuiC_SkillPerkLevel = _sender as XUiC_SkillPerkLevel;
		if (xuiC_SkillPerkLevel == null)
		{
			xuiC_SkillPerkLevel = (_sender.Parent as XUiC_SkillPerkLevel);
		}
		if (_isOver && xuiC_SkillPerkLevel != null)
		{
			this.HoveredLevel = xuiC_SkillPerkLevel.Level;
			return;
		}
		this.HoveredLevel = -1;
	}

	// Token: 0x06007276 RID: 29302 RVA: 0x002EAA58 File Offset: 0x002E8C58
	public void SkillChanged()
	{
		XUiC_Paging xuiC_Paging = this.pager;
		if (xuiC_Paging != null)
		{
			xuiC_Paging.SetLastPageByElementsAndPageLength((this.CurrentSkill != null && this.CurrentSkill.ProgressionClass.MaxLevel > this.levelEntries.Count) ? (this.CurrentSkill.ProgressionClass.MaxLevel - 1) : 0, this.skillsPerPage);
		}
		XUiC_Paging xuiC_Paging2 = this.pager;
		if (xuiC_Paging2 != null)
		{
			xuiC_Paging2.Reset();
		}
		this.IsDirty = true;
	}

	// Token: 0x06007277 RID: 29303 RVA: 0x002EAAD0 File Offset: 0x002E8CD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateSkill()
	{
		if (this.CurrentSkill != null && this.actionItemList != null)
		{
			this.actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Skill, this);
		}
		XUiC_SkillEntry entryForSkill = this.windowGroup.Controller.GetChildByType<XUiC_SkillList>().GetEntryForSkill(this.CurrentSkill);
		XUiC_Paging xuiC_Paging = this.pager;
		int num = ((xuiC_Paging != null) ? xuiC_Paging.GetPage() : 0) * this.skillsPerPage + 1;
		foreach (XUiC_SkillPerkLevel xuiC_SkillPerkLevel in this.levelEntries)
		{
			xuiC_SkillPerkLevel.Level = num++;
			xuiC_SkillPerkLevel.IsDirty = true;
			if (entryForSkill != null)
			{
				xuiC_SkillPerkLevel.btnBuy.NavLeftTarget = entryForSkill.ViewComponent;
			}
		}
	}

	// Token: 0x06007278 RID: 29304 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void Pager_OnPageChanged()
	{
		this.IsDirty = true;
	}

	// Token: 0x06007279 RID: 29305 RVA: 0x002EAB98 File Offset: 0x002E8D98
	[PublicizedFrom(EAccessModifier.Private)]
	public void Entry_OnScroll(XUiController _sender, float _delta)
	{
		if (_delta > 0f)
		{
			XUiC_Paging xuiC_Paging = this.pager;
			if (xuiC_Paging == null)
			{
				return;
			}
			xuiC_Paging.PageDown();
			return;
		}
		else
		{
			XUiC_Paging xuiC_Paging2 = this.pager;
			if (xuiC_Paging2 == null)
			{
				return;
			}
			xuiC_Paging2.PageUp();
			return;
		}
	}

	// Token: 0x0600727A RID: 29306 RVA: 0x002EABC5 File Offset: 0x002E8DC5
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.actionItemList != null)
		{
			this.actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Skill, this);
		}
		XUiEventManager.Instance.OnSkillExperienceAdded += this.Current_OnSkillExperienceAdded;
		this.IsDirty = true;
	}

	// Token: 0x0600727B RID: 29307 RVA: 0x002EABFF File Offset: 0x002E8DFF
	public override void OnClose()
	{
		base.OnClose();
		XUiEventManager.Instance.OnSkillExperienceAdded -= this.Current_OnSkillExperienceAdded;
	}

	// Token: 0x0600727C RID: 29308 RVA: 0x002EAC20 File Offset: 0x002E8E20
	public override void Update(float _dt)
	{
		if (this.IsDirty)
		{
			this.IsDirty = false;
			this.UpdateSkill();
			base.RefreshBindings(this.IsDirty);
		}
		if (base.ViewComponent.UiTransform.gameObject.activeInHierarchy && this.CurrentSkill != null && !base.xui.playerUI.windowManager.IsInputActive() && ((PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard && base.xui.playerUI.playerInput.GUIActions.Inspect.WasPressed) || (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard && base.xui.playerUI.playerInput.GUIActions.DPad_Up.WasPressed)))
		{
			foreach (XUiC_SkillPerkLevel xuiC_SkillPerkLevel in this.levelEntries)
			{
				if (xuiC_SkillPerkLevel.CurrentSkill != null && xuiC_SkillPerkLevel.Level == this.CurrentSkill.Level + 1)
				{
					xuiC_SkillPerkLevel.btnBuy.Controller.Pressed(-1);
					break;
				}
			}
		}
		base.Update(_dt);
	}

	// Token: 0x0600727D RID: 29309 RVA: 0x002EAD6C File Offset: 0x002E8F6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_OnSkillExperienceAdded(ProgressionValue _changedSkill, int _newXp)
	{
		if (this.CurrentSkill == _changedSkill)
		{
			this.IsDirty = true;
		}
	}

	// Token: 0x0600727E RID: 29310 RVA: 0x002EAD80 File Offset: 0x002E8F80
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "hidden_entries_with_paging")
		{
			this.hiddenEntriesWithPaging = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
			foreach (XUiC_SkillPerkLevel xuiC_SkillPerkLevel in this.levelEntries)
			{
				if (xuiC_SkillPerkLevel != null)
				{
					xuiC_SkillPerkLevel.HiddenEntriesWithPaging = this.hiddenEntriesWithPaging;
				}
			}
			this.skillsPerPage = this.levelEntries.Count - this.hiddenEntriesWithPaging;
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x0600727F RID: 29311 RVA: 0x002EAE1C File Offset: 0x002E901C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 1912580562U)
		{
			if (num <= 464048759U)
			{
				if (num != 443815844U)
				{
					if (num == 464048759U)
					{
						if (_bindingName == "alwaysfalse")
						{
							_value = "false";
							return true;
						}
					}
				}
				else if (_bindingName == "skillLevel")
				{
					_value = ((this.CurrentSkill != null) ? this.skillLevelFormatter.Format(this.CurrentSkill.GetCalculatedLevel(entityPlayer)) : "0");
					return true;
				}
			}
			else if (num != 1275709072U)
			{
				if (num != 1283949528U)
				{
					if (num == 1912580562U)
					{
						if (_bindingName == "buycost")
						{
							_value = "-- PTS";
							if (this.CurrentSkill != null && this.CurrentSkill.CalculatedLevel(entityPlayer) < this.CurrentSkill.ProgressionClass.MaxLevel)
							{
								if (this.CurrentSkill.ProgressionClass.CurrencyType == ProgressionCurrencyType.SP)
								{
									_value = this.buyCostFormatter.Format(this.CurrentSkill.ProgressionClass.CalculatedCostForLevel(this.CurrentSkill.CalculatedLevel(entityPlayer) + 1));
								}
								else
								{
									_value = this.expCostFormatter.Format((int)((1f - this.CurrentSkill.PercToNextLevel) * (float)this.CurrentSkill.ProgressionClass.CalculatedCostForLevel(this.CurrentSkill.CalculatedLevel(entityPlayer) + 1)));
								}
							}
							return true;
						}
					}
				}
				else if (_bindingName == "currentlevel")
				{
					_value = Localization.Get("xuiSkillLevel", false);
					return true;
				}
			}
			else if (_bindingName == "maxSkillLevel")
			{
				_value = ((this.CurrentSkill != null) ? this.maxSkillLevelFormatter.Format((float)ProgressionClass.GetCalculatedMaxLevel(entityPlayer, this.CurrentSkill)) : "0");
				return true;
			}
		}
		else if (num <= 3268933568U)
		{
			if (num != 2606420134U)
			{
				if (num == 3268933568U)
				{
					if (_bindingName == "showPaging")
					{
						_value = (this.CurrentSkill != null && this.CurrentSkill.ProgressionClass.MaxLevel > this.levelEntries.Count).ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "groupdescription")
			{
				_value = ((this.CurrentSkill != null) ? Localization.Get(this.CurrentSkill.ProgressionClass.DescKey, false) : "");
				return true;
			}
		}
		else if (num != 3504806855U)
		{
			if (num != 4010384093U)
			{
				if (num == 4294521801U)
				{
					if (_bindingName == "detailsdescription")
					{
						if (this.CurrentSkill != null && this.hoveredLevel != -1 && this.CurrentSkill.ProgressionClass.MaxLevel >= this.hoveredLevel)
						{
							using (List<MinEffectGroup>.Enumerator enumerator = this.CurrentSkill.ProgressionClass.Effects.EffectGroups.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									MinEffectGroup minEffectGroup = enumerator.Current;
									if (minEffectGroup.EffectDescriptions != null)
									{
										for (int i = 0; i < minEffectGroup.EffectDescriptions.Count; i++)
										{
											if (this.hoveredLevel >= minEffectGroup.EffectDescriptions[i].MinLevel && this.hoveredLevel <= minEffectGroup.EffectDescriptions[i].MaxLevel)
											{
												_value = ((!string.IsNullOrEmpty(minEffectGroup.EffectDescriptions[i].LongDescription)) ? minEffectGroup.EffectDescriptions[i].LongDescription : minEffectGroup.EffectDescriptions[i].Description);
												return true;
											}
										}
									}
								}
								return true;
							}
						}
						_value = "";
						return true;
					}
				}
			}
			else if (_bindingName == "groupicon")
			{
				_value = ((this.CurrentSkill != null) ? this.CurrentSkill.ProgressionClass.Icon : "ui_game_symbol_skills");
				return true;
			}
		}
		else if (_bindingName == "groupname")
		{
			_value = ((this.CurrentSkill != null) ? Localization.Get(this.CurrentSkill.ProgressionClass.NameKey, false) : "Skill Info");
			return true;
		}
		return false;
	}

	// Token: 0x0400571C RID: 22300
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemActionList actionItemList;

	// Token: 0x0400571D RID: 22301
	[PublicizedFrom(EAccessModifier.Private)]
	public int hiddenEntriesWithPaging = 1;

	// Token: 0x0400571E RID: 22302
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_SkillPerkLevel> levelEntries = new List<XUiC_SkillPerkLevel>();

	// Token: 0x0400571F RID: 22303
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pager;

	// Token: 0x04005720 RID: 22304
	[PublicizedFrom(EAccessModifier.Private)]
	public int skillsPerPage;

	// Token: 0x04005721 RID: 22305
	[PublicizedFrom(EAccessModifier.Private)]
	public int hoveredLevel = -1;

	// Token: 0x04005722 RID: 22306
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat skillLevelFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x04005723 RID: 22307
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat maxSkillLevelFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x04005724 RID: 22308
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> buyCostFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString() + " " + Localization.Get("xuiSkillPoints", false));

	// Token: 0x04005725 RID: 22309
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> expCostFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString() + " " + Localization.Get("RewardExp_keyword", false));

	// Token: 0x04005726 RID: 22310
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, float, bool> attributeSubtractionFormatter = new CachedStringFormatter<string, float, bool>((string _s, float _f, bool _b) => _s + ": " + _f.ToCultureInvariantString("0.#") + (_b ? "%" : ""));

	// Token: 0x04005727 RID: 22311
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, float> attributeSetValueFormatter = new CachedStringFormatter<string, float>((string _s, float _f) => _s + ": " + _f.ToCultureInvariantString("0.#"));

	// Token: 0x04005728 RID: 22312
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly StringBuilder effectsStringBuilder = new StringBuilder();
}

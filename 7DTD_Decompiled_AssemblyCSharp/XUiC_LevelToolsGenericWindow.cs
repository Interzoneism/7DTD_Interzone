using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CE9 RID: 3305
[Preserve]
public class XUiC_LevelToolsGenericWindow : XUiController
{
	// Token: 0x0600667D RID: 26237 RVA: 0x00299BA3 File Offset: 0x00297DA3
	public override void Init()
	{
		base.Init();
		XUiC_LevelToolsGenericWindow.ID = base.WindowGroup.ID;
		this.initGenericButtons();
		this.initSpecialFeatures();
	}

	// Token: 0x0600667E RID: 26238 RVA: 0x00299BC7 File Offset: 0x00297DC7
	public override void OnOpen()
	{
		base.OnOpen();
		this.onOpenSpecialFeatures();
	}

	// Token: 0x0600667F RID: 26239 RVA: 0x00299BD5 File Offset: 0x00297DD5
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.updateGenericButtons();
	}

	// Token: 0x06006680 RID: 26240 RVA: 0x00299BE4 File Offset: 0x00297DE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void initGenericButtons()
	{
		foreach (XUiC_ToggleButton xuiC_ToggleButton in base.GetChildrenByType<XUiC_ToggleButton>(null))
		{
			string id = xuiC_ToggleButton.ViewComponent.ID;
			string label = xuiC_ToggleButton.Label;
			NGuiAction nguiAction = XUiC_LevelToolsHelpers.BuildAction(id, label, true);
			if (nguiAction != null)
			{
				this.setToggle(xuiC_ToggleButton, nguiAction);
			}
		}
		foreach (XUiC_SimpleButton xuiC_SimpleButton in base.GetChildrenByType<XUiC_SimpleButton>(null))
		{
			string id2 = xuiC_SimpleButton.ViewComponent.ID;
			string text = xuiC_SimpleButton.Text;
			NGuiAction nguiAction2 = XUiC_LevelToolsHelpers.BuildAction(id2, text, false);
			if (nguiAction2 != null)
			{
				this.setButton(xuiC_SimpleButton, nguiAction2);
			}
		}
	}

	// Token: 0x06006681 RID: 26241 RVA: 0x00299C80 File Offset: 0x00297E80
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateGenericButtons()
	{
		foreach (ValueTuple<XUiC_ToggleButton, NGuiAction> valueTuple in this.toggleList)
		{
			XUiC_ToggleButton item = valueTuple.Item1;
			NGuiAction item2 = valueTuple.Item2;
			item.Value = item2.IsChecked();
			item.Enabled = item2.IsEnabled();
		}
		foreach (ValueTuple<XUiC_SimpleButton, NGuiAction> valueTuple2 in this.buttonList)
		{
			XUiC_SimpleButton item3 = valueTuple2.Item1;
			NGuiAction item4 = valueTuple2.Item2;
			item3.Enabled = item4.IsEnabled();
		}
	}

	// Token: 0x06006682 RID: 26242 RVA: 0x00299D48 File Offset: 0x00297F48
	[PublicizedFrom(EAccessModifier.Private)]
	public void setToggle(XUiC_ToggleButton _toggle, NGuiAction _action)
	{
		_toggle.Label = XUiC_LevelToolsGenericWindow.buildCaption(_action);
		_toggle.OnValueChanged += delegate(XUiC_ToggleButton _, bool _)
		{
			_action.OnClick();
		};
		_toggle.Tooltip = _action.GetTooltip();
		this.toggleList.Add(new ValueTuple<XUiC_ToggleButton, NGuiAction>(_toggle, _action));
	}

	// Token: 0x06006683 RID: 26243 RVA: 0x00299DB0 File Offset: 0x00297FB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void setButton(XUiC_SimpleButton _button, NGuiAction _action)
	{
		_button.Text = XUiC_LevelToolsGenericWindow.buildCaption(_action);
		_button.OnPressed += delegate(XUiController _, int _)
		{
			_action.OnClick();
		};
		_button.Tooltip = _action.GetTooltip();
		this.buttonList.Add(new ValueTuple<XUiC_SimpleButton, NGuiAction>(_button, _action));
	}

	// Token: 0x06006684 RID: 26244 RVA: 0x00299E15 File Offset: 0x00298015
	[PublicizedFrom(EAccessModifier.Private)]
	public static string buildCaption(NGuiAction _action)
	{
		return _action.GetText() + " " + _action.GetHotkey().GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.KeyboardWithParentheses, null);
	}

	// Token: 0x06006685 RID: 26245 RVA: 0x00299E35 File Offset: 0x00298035
	[PublicizedFrom(EAccessModifier.Private)]
	public void initSpecialFeatures()
	{
		this.initBlockHighlighter();
		this.initBlockReplacer();
		this.initShapeMaterialReplacer();
	}

	// Token: 0x06006686 RID: 26246 RVA: 0x00299E4C File Offset: 0x0029804C
	[PublicizedFrom(EAccessModifier.Private)]
	public void onOpenSpecialFeatures()
	{
		List<string> list = null;
		if (!this.blockListsInitDone)
		{
			list = new List<string>();
			foreach (Block block in Block.list)
			{
				if (block != null)
				{
					list.Add(block.GetBlockName());
				}
			}
			this.blockListsInitDone = true;
		}
		this.onOpenBlockHighlighter(list);
		this.onOpenBlockReplacer(list);
		this.onOpenShapeMaterialReplacer(list);
	}

	// Token: 0x06006687 RID: 26247 RVA: 0x00299EAC File Offset: 0x002980AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void initBlockHighlighter()
	{
		this.dropdownHighlightBlockName = (base.GetChildById("txtHighlightBlockName") as XUiC_DropDown);
		if (this.dropdownHighlightBlockName != null)
		{
			this.blockListsInitDone = false;
			this.dropdownHighlightBlockName.OnChangeHandler += this.HighlightBlock_OnChangeHandler;
			this.dropdownHighlightBlockName.OnSubmitHandler += this.HighlightBlock_OnSubmitHandler;
		}
	}

	// Token: 0x06006688 RID: 26248 RVA: 0x00299F0C File Offset: 0x0029810C
	[PublicizedFrom(EAccessModifier.Private)]
	public void onOpenBlockHighlighter(List<string> _allBlockNames)
	{
		if (_allBlockNames != null && this.dropdownHighlightBlockName != null)
		{
			this.dropdownHighlightBlockName.AllEntries.AddRange(_allBlockNames);
			this.dropdownHighlightBlockName.UpdateFilteredList();
		}
	}

	// Token: 0x06006689 RID: 26249 RVA: 0x00299F38 File Offset: 0x00298138
	[PublicizedFrom(EAccessModifier.Private)]
	public void HighlightBlock_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		bool flag = Block.GetBlockByName(this.dropdownHighlightBlockName.Text, true) != null;
		this.dropdownHighlightBlockName.TextInput.ActiveTextColor = (flag ? Color.white : Color.red);
	}

	// Token: 0x0600668A RID: 26250 RVA: 0x00299F7C File Offset: 0x0029817C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HighlightBlock_OnSubmitHandler(XUiController _sender, string _text)
	{
		Block blockByName = Block.GetBlockByName(this.dropdownHighlightBlockName.Text, true);
		if (blockByName != null)
		{
			PrefabEditModeManager.Instance.HighlightBlocks(blockByName);
		}
	}

	// Token: 0x0600668B RID: 26251 RVA: 0x00299FAC File Offset: 0x002981AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void initBlockReplacer()
	{
		this.txtOldBlockId = (base.GetChildById("txtOldBlockId") as XUiC_DropDown);
		this.txtNewBlockId = (base.GetChildById("txtNewBlockId") as XUiC_DropDown);
		if (this.txtOldBlockId != null)
		{
			this.blockListsInitDone = false;
			this.txtOldBlockId.OnChangeHandler += this.ReplaceBlockIds_OnChangeHandler;
			this.txtOldBlockId.OnSubmitHandler += this.ReplaceBlockIds_OnSubmitHandler;
			XUiC_TextInput textInput = this.txtOldBlockId.TextInput;
			XUiC_DropDown xuiC_DropDown = this.txtNewBlockId;
			textInput.SelectOnTab = ((xuiC_DropDown != null) ? xuiC_DropDown.TextInput : null);
		}
		if (this.txtNewBlockId != null)
		{
			this.blockListsInitDone = false;
			this.txtNewBlockId.OnChangeHandler += this.ReplaceBlockIds_OnChangeHandler;
			this.txtNewBlockId.OnSubmitHandler += this.ReplaceBlockIds_OnSubmitHandler;
			XUiC_TextInput textInput2 = this.txtNewBlockId.TextInput;
			XUiC_DropDown xuiC_DropDown2 = this.txtOldBlockId;
			textInput2.SelectOnTab = ((xuiC_DropDown2 != null) ? xuiC_DropDown2.TextInput : null);
		}
		this.btnReplaceBlockIds = (base.GetChildById("btnReplaceBlockIds") as XUiC_SimpleButton);
		if (this.btnReplaceBlockIds != null)
		{
			this.btnReplaceBlockIds.OnPressed += this.BtnReplaceBlockIds_OnPressed;
		}
	}

	// Token: 0x0600668C RID: 26252 RVA: 0x0029A0D8 File Offset: 0x002982D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void onOpenBlockReplacer(List<string> _allBlockNames)
	{
		this.ReplaceBlockIds_OnChangeHandler(this, null, true);
		if (_allBlockNames != null)
		{
			if (this.txtOldBlockId != null)
			{
				this.txtOldBlockId.AllEntries.AddRange(_allBlockNames);
				this.txtOldBlockId.UpdateFilteredList();
			}
			if (this.txtNewBlockId != null)
			{
				this.txtNewBlockId.AllEntries.AddRange(_allBlockNames);
				this.txtNewBlockId.UpdateFilteredList();
			}
		}
	}

	// Token: 0x0600668D RID: 26253 RVA: 0x0029A13C File Offset: 0x0029833C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReplaceBlockIds_OnChangeHandler(XUiController _sender, string _text, bool _changefromcode)
	{
		if (this.txtOldBlockId == null || this.txtNewBlockId == null || this.btnReplaceBlockIds == null)
		{
			return;
		}
		bool flag = Block.GetBlockByName(this.txtOldBlockId.Text, true) != null;
		bool flag2 = Block.GetBlockByName(this.txtNewBlockId.Text, true) != null;
		this.txtOldBlockId.TextInput.ActiveTextColor = (flag ? Color.white : Color.red);
		this.txtNewBlockId.TextInput.ActiveTextColor = (flag2 ? Color.white : Color.red);
		this.btnReplaceBlockIds.Enabled = (flag && flag2);
	}

	// Token: 0x0600668E RID: 26254 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReplaceBlockIds_OnSubmitHandler(XUiController _sender, string _text)
	{
	}

	// Token: 0x0600668F RID: 26255 RVA: 0x0029A1D8 File Offset: 0x002983D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnReplaceBlockIds_OnPressed(XUiController _sender, int _mouseButton)
	{
		Block blockByName = Block.GetBlockByName(this.txtOldBlockId.Text, true);
		Block blockByName2 = Block.GetBlockByName(this.txtNewBlockId.Text, true);
		if (blockByName == null)
		{
			return;
		}
		if (blockByName2 == null)
		{
			return;
		}
		XUiC_LevelToolsHelpers.ReplaceBlockId(blockByName, blockByName2);
	}

	// Token: 0x06006690 RID: 26256 RVA: 0x0029A218 File Offset: 0x00298418
	[PublicizedFrom(EAccessModifier.Private)]
	public void initShapeMaterialReplacer()
	{
		this.txtOldShapeMaterial = (base.GetChildById("txtOldShapeMaterial") as XUiC_DropDown);
		this.txtNewShapeMaterial = (base.GetChildById("txtNewShapeMaterial") as XUiC_DropDown);
		if (this.txtOldShapeMaterial != null)
		{
			this.blockListsInitDone = false;
			this.txtOldShapeMaterial.OnChangeHandler += this.ReplaceShapeMaterial_OnChangeHandler;
			this.txtOldShapeMaterial.OnSubmitHandler += this.ReplaceShapeMaterial_OnSubmitHandler;
			XUiC_TextInput textInput = this.txtOldShapeMaterial.TextInput;
			XUiC_DropDown xuiC_DropDown = this.txtNewShapeMaterial;
			textInput.SelectOnTab = ((xuiC_DropDown != null) ? xuiC_DropDown.TextInput : null);
		}
		if (this.txtNewShapeMaterial != null)
		{
			this.blockListsInitDone = false;
			this.txtNewShapeMaterial.OnChangeHandler += this.ReplaceShapeMaterial_OnChangeHandler;
			this.txtNewShapeMaterial.OnSubmitHandler += this.ReplaceShapeMaterial_OnSubmitHandler;
			XUiC_TextInput textInput2 = this.txtNewShapeMaterial.TextInput;
			XUiC_DropDown xuiC_DropDown2 = this.txtOldShapeMaterial;
			textInput2.SelectOnTab = ((xuiC_DropDown2 != null) ? xuiC_DropDown2.TextInput : null);
		}
		this.btnReplaceShapeMaterials = (base.GetChildById("btnReplaceShapeMaterials") as XUiC_SimpleButton);
		if (this.btnReplaceShapeMaterials != null)
		{
			this.btnReplaceShapeMaterials.OnPressed += this.BtnReplaceShapeMaterial_OnPressed;
		}
		XUiController childById = base.GetChildById("btnReplaceShapeMaterialSwitchInOut");
		XUiV_Button xuiV_Button = ((childById != null) ? childById.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button != null)
		{
			xuiV_Button.Controller.OnPress += delegate(XUiController _sender, int _button)
			{
				if (this.txtOldShapeMaterial == null || this.txtNewShapeMaterial == null)
				{
					return;
				}
				XUiC_DropDown xuiC_DropDown3 = this.txtOldShapeMaterial;
				XUiC_DropDown xuiC_DropDown4 = this.txtNewShapeMaterial;
				string text = this.txtNewShapeMaterial.Text;
				string text2 = this.txtOldShapeMaterial.Text;
				xuiC_DropDown3.Text = text;
				xuiC_DropDown4.Text = text2;
				this.ReplaceShapeMaterial_OnChangeHandler(this, null, true);
			};
		}
	}

	// Token: 0x06006691 RID: 26257 RVA: 0x0029A37C File Offset: 0x0029857C
	[PublicizedFrom(EAccessModifier.Private)]
	public void onOpenShapeMaterialReplacer(List<string> _allBlockNames)
	{
		this.ReplaceShapeMaterial_OnChangeHandler(this, null, true);
		if (_allBlockNames != null)
		{
			HashSet<string> autoShapeMaterials = Block.GetAutoShapeMaterials();
			if (this.txtOldShapeMaterial != null)
			{
				this.txtOldShapeMaterial.AllEntries.AddRange(autoShapeMaterials);
				this.txtOldShapeMaterial.UpdateFilteredList();
			}
			if (this.txtNewShapeMaterial != null)
			{
				this.txtNewShapeMaterial.AllEntries.AddRange(autoShapeMaterials);
				this.txtNewShapeMaterial.UpdateFilteredList();
			}
		}
	}

	// Token: 0x06006692 RID: 26258 RVA: 0x0029A3E4 File Offset: 0x002985E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReplaceShapeMaterial_OnChangeHandler(XUiController _sender, string _text, bool _changefromcode)
	{
		if (this.txtOldShapeMaterial == null || this.txtNewShapeMaterial == null || this.btnReplaceShapeMaterials == null)
		{
			return;
		}
		HashSet<string> autoShapeMaterials = Block.GetAutoShapeMaterials();
		bool flag = autoShapeMaterials.Contains(this.txtOldShapeMaterial.Text);
		bool flag2 = autoShapeMaterials.Contains(this.txtNewShapeMaterial.Text);
		this.txtOldShapeMaterial.TextInput.ActiveTextColor = (flag ? Color.white : Color.red);
		this.txtNewShapeMaterial.TextInput.ActiveTextColor = (flag2 ? Color.white : Color.red);
		this.btnReplaceShapeMaterials.Enabled = (flag && flag2);
	}

	// Token: 0x06006693 RID: 26259 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReplaceShapeMaterial_OnSubmitHandler(XUiController _sender, string _text)
	{
	}

	// Token: 0x06006694 RID: 26260 RVA: 0x0029A480 File Offset: 0x00298680
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnReplaceShapeMaterial_OnPressed(XUiController _sender, int _mouseButton)
	{
		HashSet<string> autoShapeMaterials = Block.GetAutoShapeMaterials();
		string text = this.txtOldShapeMaterial.Text;
		string text2 = this.txtNewShapeMaterial.Text;
		if (!autoShapeMaterials.TryGetValue(text, out text))
		{
			return;
		}
		if (!autoShapeMaterials.TryGetValue(text2, out text2))
		{
			return;
		}
		XUiC_LevelToolsHelpers.ReplaceBlockShapeMaterials(text, text2);
	}

	// Token: 0x04004D54 RID: 19796
	public static string ID = "";

	// Token: 0x04004D55 RID: 19797
	[TupleElementNames(new string[]
	{
		"toggle",
		"action"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ValueTuple<XUiC_ToggleButton, NGuiAction>> toggleList = new List<ValueTuple<XUiC_ToggleButton, NGuiAction>>();

	// Token: 0x04004D56 RID: 19798
	[TupleElementNames(new string[]
	{
		"button",
		"action"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ValueTuple<XUiC_SimpleButton, NGuiAction>> buttonList = new List<ValueTuple<XUiC_SimpleButton, NGuiAction>>();

	// Token: 0x04004D57 RID: 19799
	[PublicizedFrom(EAccessModifier.Private)]
	public bool blockListsInitDone;

	// Token: 0x04004D58 RID: 19800
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DropDown dropdownHighlightBlockName;

	// Token: 0x04004D59 RID: 19801
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DropDown txtOldBlockId;

	// Token: 0x04004D5A RID: 19802
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DropDown txtNewBlockId;

	// Token: 0x04004D5B RID: 19803
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnReplaceBlockIds;

	// Token: 0x04004D5C RID: 19804
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DropDown txtOldShapeMaterial;

	// Token: 0x04004D5D RID: 19805
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DropDown txtNewShapeMaterial;

	// Token: 0x04004D5E RID: 19806
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnReplaceShapeMaterials;
}

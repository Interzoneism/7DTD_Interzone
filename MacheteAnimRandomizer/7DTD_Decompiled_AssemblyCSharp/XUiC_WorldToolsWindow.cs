using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000ECA RID: 3786
[Preserve]
public class XUiC_WorldToolsWindow : XUiController
{
	// Token: 0x060077BB RID: 30651 RVA: 0x0030C120 File Offset: 0x0030A320
	public override void Init()
	{
		base.Init();
		XUiC_WorldToolsWindow.ID = base.WindowGroup.ID;
		this.btnLevelStartPoint = base.GetChildById("btnLevelStartPoint");
		this.btnLevelStartPoint.GetChildById("clickable").OnPress += this.BtnLevelStartPoint_Controller_OnPress;
		this.cbxBoxSideTransparency = base.GetChildById("cbxBoxSideTransparency").GetChildByType<XUiC_ComboBoxFloat>();
		this.cbxBoxSideTransparency.OnValueChanged += this.CbxBoxSideTransparency_OnValueChanged;
		this.cbxBoxSelectionCaptions = base.GetChildById("cbxBoxSelectionCaptions").GetChildByType<XUiC_ComboBoxBool>();
		this.cbxBoxSelectionCaptions.OnValueChanged += this.CbxBoxSelectionCaptions_OnValueChanged;
		this.cbxBoxSelectionCaptions.Value = true;
		this.cbxBoxPrefabPreviewLimit = base.GetChildById("cbxBoxPrefabPreviewLimit").GetChildByType<XUiC_ComboBoxInt>();
		this.cbxBoxPrefabPreviewLimit.OnValueChanged += this.CbxBoxPrefabPreviewLimit_OnValueChanged;
	}

	// Token: 0x060077BC RID: 30652 RVA: 0x0030C208 File Offset: 0x0030A408
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLevelStartPoint_Controller_OnPress(XUiController _sender, int _mouseButton)
	{
		Vector3 raycastHitPoint = XUiC_LevelTools3Window.getRaycastHitPoint(100f, 0f);
		if (!raycastHitPoint.Equals(Vector3.zero))
		{
			Vector3i vector3i = World.worldToBlockPos(raycastHitPoint);
			GameManager.Instance.GetSpawnPointList().Add(new SpawnPoint(vector3i));
			SelectionCategory category = SelectionBoxManager.Instance.GetCategory("StartPoint");
			Vector3i vector3i2 = vector3i;
			category.AddBox(vector3i2.ToString() ?? "", vector3i, Vector3i.one, true, false);
			SelectionBoxManager instance = SelectionBoxManager.Instance;
			string category2 = "StartPoint";
			vector3i2 = vector3i;
			instance.SetActive(category2, vector3i2.ToString() ?? "", true);
		}
	}

	// Token: 0x060077BD RID: 30653 RVA: 0x0030C2B1 File Offset: 0x0030A4B1
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxBoxSelectionCaptions_OnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		SelectionCategory category = SelectionBoxManager.Instance.GetCategory("DynamicPrefabs");
		if (category == null)
		{
			return;
		}
		category.SetCaptionVisibility(_newValue);
	}

	// Token: 0x060077BE RID: 30654 RVA: 0x0030C2CD File Offset: 0x0030A4CD
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxBoxSideTransparency_OnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		SelectionBoxManager.Instance.AlphaMultiplier = (float)_newValue;
	}

	// Token: 0x060077BF RID: 30655 RVA: 0x0030C2DB File Offset: 0x0030A4DB
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxBoxPrefabPreviewLimit_OnValueChanged(XUiController _sender, long _oldValue, long _newValue)
	{
		DynamicPrefabDecorator.PrefabPreviewLimit = (int)_newValue;
	}

	// Token: 0x060077C0 RID: 30656 RVA: 0x0030C2E4 File Offset: 0x0030A4E4
	public override void OnOpen()
	{
		base.OnOpen();
		this.cbxBoxSideTransparency.Value = (double)SelectionBoxManager.Instance.AlphaMultiplier;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.btnLevelStartPoint.ViewComponent.IsVisible = false;
		}
	}

	// Token: 0x04005B40 RID: 23360
	public static string ID = "";

	// Token: 0x04005B41 RID: 23361
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnLevelStartPoint;

	// Token: 0x04005B42 RID: 23362
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat cbxBoxSideTransparency;

	// Token: 0x04005B43 RID: 23363
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool cbxBoxSelectionCaptions;

	// Token: 0x04005B44 RID: 23364
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt cbxBoxPrefabPreviewLimit;
}

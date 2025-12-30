using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CE5 RID: 3301
[Preserve]
public class XUiC_LevelTools3Window : XUiController
{
	// Token: 0x0600666C RID: 26220 RVA: 0x00299564 File Offset: 0x00297764
	public override void Init()
	{
		base.Init();
		XUiC_LevelTools3Window.ID = base.WindowGroup.ID;
		XUiController childById = base.GetChildById("buttonCopySleeperVolume");
		this.buttonCopySleeperVolume = ((childById != null) ? childById.GetChildByType<XUiC_SimpleButton>() : null);
		if (this.buttonCopySleeperVolume != null)
		{
			BlockToolSelection blockToolSelection = GameManager.Instance.GetActiveBlockTool() as BlockToolSelection;
			if (blockToolSelection != null)
			{
				NGuiAction action;
				if (blockToolSelection.GetActions().TryGetValue("copySleeperVolume", out action))
				{
					string text = action.GetText() + " " + action.GetHotkey().GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.KeyboardWithParentheses, null);
					string tooltip = action.GetTooltip();
					this.buttonCopySleeperVolume.Text = text;
					this.buttonCopySleeperVolume.OnPressed += delegate(XUiController _, int _)
					{
						action.OnClick();
					};
					this.buttonCopySleeperVolume.Tooltip = tooltip;
				}
			}
		}
		XUiController childById2 = base.GetChildById("volumeTypeSelector");
		this.volumeTypeSelector = ((childById2 != null) ? childById2.GetChildByType<XUiC_CategoryList>() : null);
		if (this.volumeTypeSelector != null)
		{
			this.volumeTypeSelector.SetCategoryToFirst();
		}
		XUiController childById3 = base.GetChildById("btnVolumesCreate");
		this.btnVolumesCreate = ((childById3 != null) ? childById3.GetChildByType<XUiC_SimpleButton>() : null);
		if (this.btnVolumesCreate != null)
		{
			this.btnVolumesCreate.OnPressed += this.BtnVolumesCreateOnOnPressed;
		}
		XUiController childById4 = base.GetChildById("btnVolumesCreateFromSelection");
		this.btnVolumesCreateFromSelection = ((childById4 != null) ? childById4.GetChildByType<XUiC_SimpleButton>() : null);
		if (this.btnVolumesCreateFromSelection != null)
		{
			this.btnVolumesCreateFromSelection.OnPressed += this.BtnVolumesCreateFromSelectionOnOnPressed;
		}
		XUiController childById5 = base.GetChildById("toggleVolumesShow");
		this.toggleVolumesShow = ((childById5 != null) ? childById5.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.toggleVolumesShow != null)
		{
			this.toggleVolumesShow.OnValueChanged += this.ToggleVolumesShowOnOnValueChanged;
		}
		XUiController childById6 = base.GetChildById("btnVolumesDupe");
		this.btnVolumesDupe = ((childById6 != null) ? childById6.GetChildByType<XUiC_SimpleButton>() : null);
		if (this.btnVolumesDupe != null)
		{
			this.btnVolumesDupe.OnPressed += this.BtnVolumesDupeOnOnPressed;
		}
	}

	// Token: 0x0600666D RID: 26221 RVA: 0x00299768 File Offset: 0x00297968
	public override void Update(float _dt)
	{
		base.Update(_dt);
		XUiC_CategoryEntry currentCategory = this.volumeTypeSelector.CurrentCategory;
		string name = ((currentCategory != null) ? currentCategory.CategoryName : null) ?? "";
		SelectionCategory category = SelectionBoxManager.Instance.GetCategory(name);
		XUiC_SimpleButton xuiC_SimpleButton = this.btnVolumesCreateFromSelection;
		BlockToolSelection instance = BlockToolSelection.Instance;
		xuiC_SimpleButton.Enabled = (instance != null && instance.SelectionActive);
		this.toggleVolumesShow.Value = (category != null && category.IsVisible());
		this.btnVolumesDupe.Enabled = (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && false);
	}

	// Token: 0x0600666E RID: 26222 RVA: 0x002997F8 File Offset: 0x002979F8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool volumeTypeByName(string _name, out XUiC_LevelTools3Window.VolumeTypeDefinition _result)
	{
		_result = this.volumeTypeDefinitions.Find((XUiC_LevelTools3Window.VolumeTypeDefinition _vtd) => _vtd.SelectionCategory.name == _name);
		return _result != null;
	}

	// Token: 0x0600666F RID: 26223 RVA: 0x00299830 File Offset: 0x00297A30
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnVolumesCreateOnOnPressed(XUiController _sender, int _mouseButton)
	{
		XUiC_CategoryEntry currentCategory = this.volumeTypeSelector.CurrentCategory;
		string name = ((currentCategory != null) ? currentCategory.CategoryName : null) ?? "";
		XUiC_LevelTools3Window.VolumeTypeDefinition volumeTypeDefinition;
		if (!this.volumeTypeByName(name, out volumeTypeDefinition))
		{
			return;
		}
		XUiC_LevelTools3Window.addVolume(volumeTypeDefinition.AddVolumeHandler);
	}

	// Token: 0x06006670 RID: 26224 RVA: 0x00299878 File Offset: 0x00297A78
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnVolumesCreateFromSelectionOnOnPressed(XUiController _sender, int _mouseButton)
	{
		XUiC_CategoryEntry currentCategory = this.volumeTypeSelector.CurrentCategory;
		string name = ((currentCategory != null) ? currentCategory.CategoryName : null) ?? "";
		XUiC_LevelTools3Window.VolumeTypeDefinition volumeTypeDefinition;
		if (!this.volumeTypeByName(name, out volumeTypeDefinition))
		{
			return;
		}
		XUiC_LevelTools3Window.addVolumeFromSelection(volumeTypeDefinition.AddVolumeHandler);
	}

	// Token: 0x06006671 RID: 26225 RVA: 0x002998C0 File Offset: 0x00297AC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleVolumesShowOnOnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		XUiC_CategoryEntry currentCategory = this.volumeTypeSelector.CurrentCategory;
		string name = ((currentCategory != null) ? currentCategory.CategoryName : null) ?? "";
		XUiC_LevelTools3Window.VolumeTypeDefinition volumeTypeDefinition;
		if (!this.volumeTypeByName(name, out volumeTypeDefinition))
		{
			return;
		}
		SelectionCategory selectionCategory = volumeTypeDefinition.SelectionCategory;
		selectionCategory.SetVisible(!selectionCategory.IsVisible());
	}

	// Token: 0x06006672 RID: 26226 RVA: 0x000424BD File Offset: 0x000406BD
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnVolumesDupeOnOnPressed(XUiController _sender, int _mouseButton)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06006673 RID: 26227 RVA: 0x00299910 File Offset: 0x00297B10
	[PublicizedFrom(EAccessModifier.Private)]
	public static void addVolume(Action<Vector3i, Vector3i> _addVolumeCallback)
	{
		Vector3 raycastHitPoint = XUiC_LevelTools3Window.getRaycastHitPoint(100f, 0f);
		if (raycastHitPoint.Equals(Vector3.zero))
		{
			return;
		}
		Vector3i vector3i = new Vector3i(5, 4, 5);
		Vector3i arg = World.worldToBlockPos(raycastHitPoint) - new Vector3i(vector3i.x / 2, 0, vector3i.z / 2);
		_addVolumeCallback(arg, vector3i);
	}

	// Token: 0x06006674 RID: 26228 RVA: 0x00299970 File Offset: 0x00297B70
	[PublicizedFrom(EAccessModifier.Private)]
	public static void addVolumeFromSelection(Action<Vector3i, Vector3i> _addVolumeCallback)
	{
		BlockToolSelection instance = BlockToolSelection.Instance;
		if (instance == null || !instance.SelectionActive)
		{
			return;
		}
		_addVolumeCallback(instance.SelectionMin, instance.SelectionSize);
	}

	// Token: 0x06006675 RID: 26229 RVA: 0x002999A4 File Offset: 0x00297BA4
	public static Vector3 getRaycastHitPoint(float _maxDistance = 100f, float _offsetUp = 0f)
	{
		Camera finalCamera = GameManager.Instance.World.GetPrimaryPlayer().finalCamera;
		Ray ray = finalCamera.ScreenPointToRay(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
		ray.origin += Origin.position;
		Transform transform = finalCamera.transform;
		ray.origin += transform.forward * 0.1f;
		ray.origin += transform.up * _offsetUp;
		if (Voxel.Raycast(GameManager.Instance.World, ray, _maxDistance, 4095, 0f))
		{
			return Voxel.voxelRayHitInfo.hit.pos - ray.direction * 0.05f;
		}
		return Vector3.zero;
	}

	// Token: 0x04004D48 RID: 19784
	public static string ID = "";

	// Token: 0x04004D49 RID: 19785
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton buttonCopySleeperVolume;

	// Token: 0x04004D4A RID: 19786
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_LevelTools3Window.VolumeTypeDefinition> volumeTypeDefinitions = new List<XUiC_LevelTools3Window.VolumeTypeDefinition>
	{
		new XUiC_LevelTools3Window.VolumeTypeDefinition("SleeperVolume", new Action<Vector3i, Vector3i>(PrefabSleeperVolumeManager.Instance.AddSleeperVolumeServer)),
		new XUiC_LevelTools3Window.VolumeTypeDefinition("TriggerVolume", new Action<Vector3i, Vector3i>(PrefabTriggerVolumeManager.Instance.AddTriggerVolumeServer)),
		new XUiC_LevelTools3Window.VolumeTypeDefinition("InfoVolume", new Action<Vector3i, Vector3i>(PrefabVolumeManager.Instance.AddInfoVolumeServer)),
		new XUiC_LevelTools3Window.VolumeTypeDefinition("TraderTeleport", new Action<Vector3i, Vector3i>(PrefabVolumeManager.Instance.AddTeleportVolumeServer)),
		new XUiC_LevelTools3Window.VolumeTypeDefinition("WallVolume", new Action<Vector3i, Vector3i>(PrefabVolumeManager.Instance.AddWallVolumeServer))
	};

	// Token: 0x04004D4B RID: 19787
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList volumeTypeSelector;

	// Token: 0x04004D4C RID: 19788
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnVolumesCreate;

	// Token: 0x04004D4D RID: 19789
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnVolumesCreateFromSelection;

	// Token: 0x04004D4E RID: 19790
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleVolumesShow;

	// Token: 0x04004D4F RID: 19791
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnVolumesDupe;

	// Token: 0x02000CE6 RID: 3302
	[PublicizedFrom(EAccessModifier.Private)]
	public class VolumeTypeDefinition
	{
		// Token: 0x06006678 RID: 26232 RVA: 0x00299B5E File Offset: 0x00297D5E
		public VolumeTypeDefinition(string _selectionCategoryName, Action<Vector3i, Vector3i> _addVolumeHandler)
		{
			this.SelectionCategory = SelectionBoxManager.Instance.GetCategory(_selectionCategoryName);
			this.AddVolumeHandler = _addVolumeHandler;
		}

		// Token: 0x04004D50 RID: 19792
		public readonly SelectionCategory SelectionCategory;

		// Token: 0x04004D51 RID: 19793
		public readonly Action<Vector3i, Vector3i> AddVolumeHandler;
	}
}

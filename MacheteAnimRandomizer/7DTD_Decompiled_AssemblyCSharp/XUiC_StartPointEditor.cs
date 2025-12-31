using System;
using UnityEngine.Scripting;

// Token: 0x02000E58 RID: 3672
[Preserve]
public class XUiC_StartPointEditor : XUiController
{
	// Token: 0x0600734A RID: 29514 RVA: 0x002F0124 File Offset: 0x002EE324
	public override void Init()
	{
		base.Init();
		XUiC_StartPointEditor.ID = base.WindowGroup.ID;
		this.cbxHeading = base.GetChildById("cbxHeading").GetChildByType<XUiC_ComboBoxInt>();
		this.cbxHeading.OnValueChanged += this.CbxHeading_OnValueChanged;
		((XUiC_SimpleButton)base.GetChildById("btnCancel")).OnPressed += this.BtnCancel_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnOk")).OnPressed += this.BtnOk_OnPressed;
	}

	// Token: 0x0600734B RID: 29515 RVA: 0x002F01B6 File Offset: 0x002EE3B6
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup, false);
	}

	// Token: 0x0600734C RID: 29516 RVA: 0x002F01D4 File Offset: 0x002EE3D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnOk_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.headingOnOpen = this.cbxHeading.Value;
		base.xui.playerUI.windowManager.Close(base.WindowGroup, false);
	}

	// Token: 0x0600734D RID: 29517 RVA: 0x002F0204 File Offset: 0x002EE404
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxHeading_OnValueChanged(XUiController _sender, long _oldvalue, long _newvalue)
	{
		this.spawnPoint.spawnPosition.heading = (this.selectionBox.facingDirection = (float)_newvalue);
		base.RefreshBindings(false);
	}

	// Token: 0x0600734E RID: 29518 RVA: 0x002F0238 File Offset: 0x002EE438
	public override void OnOpen()
	{
		base.OnOpen();
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple;
		this.selectionBox = ((SelectionBoxManager.Instance.Selection != null) ? valueTuple.GetValueOrDefault().Item2 : null);
		if (this.selectionBox == null)
		{
			return;
		}
		this.spawnPoint = GameManager.Instance.GetSpawnPointList().Find(Vector3i.Parse(this.selectionBox.name));
		this.cbxHeading.Value = (this.headingOnOpen = (long)this.spawnPoint.spawnPosition.heading);
		base.RefreshBindings(false);
	}

	// Token: 0x0600734F RID: 29519 RVA: 0x002F02D4 File Offset: 0x002EE4D4
	public override void OnClose()
	{
		base.OnClose();
		this.spawnPoint.spawnPosition.heading = (this.selectionBox.facingDirection = (float)this.headingOnOpen);
		this.spawnPoint = null;
		this.selectionBox = null;
	}

	// Token: 0x06007350 RID: 29520 RVA: 0x002F031C File Offset: 0x002EE51C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "position")
		{
			SpawnPoint spawnPoint = this.spawnPoint;
			_value = (((spawnPoint != null) ? spawnPoint.spawnPosition.position.ToCultureInvariantString() : null) ?? "");
			return true;
		}
		if (!(_bindingName == "cardinal"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		SpawnPoint spawnPoint2 = this.spawnPoint;
		_value = GameUtils.GetClosestDirection((spawnPoint2 != null) ? spawnPoint2.spawnPosition.heading : 0f, false).ToStringCached<GameUtils.DirEightWay>();
		return true;
	}

	// Token: 0x040057C5 RID: 22469
	public static string ID = "";

	// Token: 0x040057C6 RID: 22470
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt cbxHeading;

	// Token: 0x040057C7 RID: 22471
	[PublicizedFrom(EAccessModifier.Private)]
	public SpawnPoint spawnPoint;

	// Token: 0x040057C8 RID: 22472
	[PublicizedFrom(EAccessModifier.Private)]
	public SelectionBox selectionBox;

	// Token: 0x040057C9 RID: 22473
	[PublicizedFrom(EAccessModifier.Private)]
	public long headingOnOpen;
}

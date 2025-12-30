using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000E26 RID: 3622
[Preserve]
public class XUiC_ShapeStackGrid : XUiController
{
	// Token: 0x17000B69 RID: 2921
	// (get) Token: 0x06007171 RID: 29041 RVA: 0x002E370E File Offset: 0x002E190E
	// (set) Token: 0x06007172 RID: 29042 RVA: 0x002E3716 File Offset: 0x002E1916
	public int Length { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000B6A RID: 2922
	// (get) Token: 0x06007173 RID: 29043 RVA: 0x002E371F File Offset: 0x002E191F
	// (set) Token: 0x06007174 RID: 29044 RVA: 0x002E3727 File Offset: 0x002E1927
	public int Page
	{
		get
		{
			return this.page;
		}
		set
		{
			this.page = value;
			this.isDirty = true;
		}
	}

	// Token: 0x06007175 RID: 29045 RVA: 0x002E3738 File Offset: 0x002E1938
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_ShapeStack>(null);
		this.shapeControllers = childrenByType;
		this.Length = this.shapeControllers.Length;
		Log.Out("ShapeControllers: " + this.shapeControllers.Length.ToString());
		this.bAwakeCalled = true;
		this.IsDirty = false;
		this.IsDormant = true;
	}

	// Token: 0x06007176 RID: 29046 RVA: 0x002E379C File Offset: 0x002E199C
	public void SetShapes(List<XUiC_ShapeStackGrid.ShapeData> shapeIndexList, int newSelectedBlock = -1)
	{
		if (GameStats.GetBool(EnumGameStats.IsCreativeMenuEnabled))
		{
			GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled);
		}
		XUiC_ShapeInfoWindow childByType = base.xui.GetChildByType<XUiC_ShapeInfoWindow>();
		XUiC_ShapeMaterialInfoWindow childByType2 = base.xui.GetChildByType<XUiC_ShapeMaterialInfoWindow>();
		int count = shapeIndexList.Count;
		this.currentList = shapeIndexList;
		for (int i = 0; i < this.Length; i++)
		{
			int num = i + this.Length * this.page;
			XUiC_ShapeStack xuiC_ShapeStack = (XUiC_ShapeStack)this.shapeControllers[i];
			xuiC_ShapeStack.Owner = this;
			xuiC_ShapeStack.InfoWindow = childByType;
			xuiC_ShapeStack.MaterialInfoWindow = childByType2;
			if (num < count)
			{
				xuiC_ShapeStack.BlockData = shapeIndexList[num].Block;
				xuiC_ShapeStack.ShapeIndex = shapeIndexList[num].Index;
				if (xuiC_ShapeStack.BlockData == this.selectedBlock)
				{
					xuiC_ShapeStack.Selected = true;
				}
				if (xuiC_ShapeStack.Selected && xuiC_ShapeStack.BlockData != this.selectedBlock)
				{
					xuiC_ShapeStack.Selected = false;
				}
			}
			else
			{
				xuiC_ShapeStack.BlockData = null;
				xuiC_ShapeStack.ShapeIndex = -1;
				if (xuiC_ShapeStack.Selected)
				{
					xuiC_ShapeStack.Selected = false;
				}
			}
		}
		if (this.selectedBlock == null && newSelectedBlock != -1)
		{
			for (int j = 0; j < this.shapeControllers.Length; j++)
			{
				XUiC_ShapeStack xuiC_ShapeStack2 = this.shapeControllers[j] as XUiC_ShapeStack;
				if (xuiC_ShapeStack2.BlockData != null && xuiC_ShapeStack2.ShapeIndex == newSelectedBlock)
				{
					xuiC_ShapeStack2.SetSelectedShapeForItem();
					xuiC_ShapeStack2.Selected = true;
					return;
				}
			}
		}
		this.IsDirty = false;
	}

	// Token: 0x06007177 RID: 29047 RVA: 0x002A9F85 File Offset: 0x002A8185
	public override void OnOpen()
	{
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = true;
		}
		this.IsDormant = false;
	}

	// Token: 0x06007178 RID: 29048 RVA: 0x002A9FAF File Offset: 0x002A81AF
	public override void OnClose()
	{
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = false;
		}
		this.IsDormant = true;
	}

	// Token: 0x06007179 RID: 29049 RVA: 0x002E3916 File Offset: 0x002E1B16
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.isDirty)
		{
			this.SetShapes(this.currentList, this.Owner.ItemValue.Meta);
			this.isDirty = false;
		}
	}

	// Token: 0x04005647 RID: 22087
	[PublicizedFrom(EAccessModifier.Protected)]
	public int curPageIdx;

	// Token: 0x04005648 RID: 22088
	[PublicizedFrom(EAccessModifier.Protected)]
	public int numPages;

	// Token: 0x04005649 RID: 22089
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x0400564A RID: 22090
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x0400564C RID: 22092
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController[] shapeControllers;

	// Token: 0x0400564D RID: 22093
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bAwakeCalled;

	// Token: 0x0400564E RID: 22094
	public XUiC_ShapesWindow Owner;

	// Token: 0x0400564F RID: 22095
	[PublicizedFrom(EAccessModifier.Private)]
	public Block selectedBlock;

	// Token: 0x04005650 RID: 22096
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_ShapeStackGrid.ShapeData> currentList;

	// Token: 0x02000E27 RID: 3623
	public class ShapeData
	{
		// Token: 0x04005651 RID: 22097
		public Block Block;

		// Token: 0x04005652 RID: 22098
		public int Index;
	}
}

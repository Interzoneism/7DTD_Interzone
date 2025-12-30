using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CA0 RID: 3232
[Preserve]
public class XUiC_DragAndDropWindow : XUiController
{
	// Token: 0x17000A28 RID: 2600
	// (get) Token: 0x060063C0 RID: 25536 RVA: 0x00287143 File Offset: 0x00285343
	public EntityPlayer entityPlayer
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			XUi xui = base.xui;
			if (xui == null)
			{
				return null;
			}
			LocalPlayerUI playerUI = xui.playerUI;
			if (playerUI == null)
			{
				return null;
			}
			return playerUI.entityPlayer;
		}
	}

	// Token: 0x060063C1 RID: 25537 RVA: 0x00287161 File Offset: 0x00285361
	[PublicizedFrom(EAccessModifier.Internal)]
	public void DropCurrentItem()
	{
		base.xui.PlayerInventory.DropItem(this.CurrentStack);
		this.CurrentStack = ItemStack.Empty.Clone();
	}

	// Token: 0x060063C2 RID: 25538 RVA: 0x0028718C File Offset: 0x0028538C
	[PublicizedFrom(EAccessModifier.Internal)]
	public void DropCurrentItem(int _count)
	{
		if (_count < this.CurrentStack.count)
		{
			ItemStack itemStack = this.CurrentStack.Clone();
			itemStack.count = _count;
			this.CurrentStack.count -= _count;
			base.xui.PlayerInventory.DropItem(itemStack);
			this.CurrentStack = this.itemStack;
			return;
		}
		this.DropCurrentItem();
	}

	// Token: 0x17000A29 RID: 2601
	// (get) Token: 0x060063C3 RID: 25539 RVA: 0x002871F1 File Offset: 0x002853F1
	// (set) Token: 0x060063C4 RID: 25540 RVA: 0x002871FC File Offset: 0x002853FC
	public ItemStack CurrentStack
	{
		get
		{
			return this.itemStack;
		}
		set
		{
			XUiC_ItemStack itemStackControl = this.ItemStackControl;
			bool flag = XUiC_ItemStack.IsStackLocationFromPlayer((itemStackControl != null) ? new XUiC_ItemStack.StackLocationTypes?(itemStackControl.StackLocation) : null);
			this.itemStack = value;
			this.ItemStackControl.ItemStack = value;
			if (flag)
			{
				EntityPlayerLocal entityPlayerLocal = this.entityPlayer as EntityPlayerLocal;
				if (entityPlayerLocal != null)
				{
					entityPlayerLocal.DragAndDropItem = value;
				}
			}
		}
	}

	// Token: 0x060063C5 RID: 25541 RVA: 0x00287258 File Offset: 0x00285458
	public override void Init()
	{
		base.Init();
		this.ItemStackControl = base.GetChildByType<XUiC_ItemStack>();
		this.ItemStackControl.IsDragAndDrop = true;
		this.ItemStackControl.ItemStack = ItemStack.Empty.Clone();
		base.ViewComponent.IsSnappable = false;
	}

	// Token: 0x060063C6 RID: 25542 RVA: 0x002872A4 File Offset: 0x002854A4
	public override void Update(float _dt)
	{
		if (!this.InMenu)
		{
			this.PlaceItemBackInInventory();
		}
		if (this.itemStack != null && !this.itemStack.IsEmpty())
		{
			((XUiV_Window)base.ViewComponent).Panel.alpha = 1f;
			Vector2 screenPosition = base.xui.playerUI.CursorController.GetScreenPosition();
			Vector3 position = base.xui.playerUI.camera.ScreenToWorldPoint(screenPosition);
			Transform transform = base.xui.transform;
			position.z = transform.position.z - 3f * transform.lossyScale.z;
			base.ViewComponent.UiTransform.position = position;
		}
		else
		{
			((XUiV_Window)base.ViewComponent).Panel.alpha = 0f;
		}
		base.Update(_dt);
	}

	// Token: 0x060063C7 RID: 25543 RVA: 0x0028738C File Offset: 0x0028558C
	public override void OnOpen()
	{
		base.OnOpen();
		EntityPlayerLocal entityPlayerLocal = this.entityPlayer as EntityPlayerLocal;
		if (entityPlayerLocal != null && entityPlayerLocal.DragAndDropItem != ItemStack.Empty)
		{
			this.CurrentStack = entityPlayerLocal.DragAndDropItem;
			this.PlaceItemBackInInventory();
		}
	}

	// Token: 0x060063C8 RID: 25544 RVA: 0x002873CD File Offset: 0x002855CD
	public override void OnClose()
	{
		base.OnClose();
		this.PlaceItemBackInInventory();
	}

	// Token: 0x060063C9 RID: 25545 RVA: 0x002873DC File Offset: 0x002855DC
	public void PlaceItemBackInInventory()
	{
		if (!this.CurrentStack.IsEmpty())
		{
			if (base.xui.PlayerInventory.AddItem(this.itemStack))
			{
				Manager.PlayXUiSound(this.placeSound, 0.75f);
				this.CurrentStack = ItemStack.Empty.Clone();
				return;
			}
			base.xui.PlayerInventory.DropItem(this.itemStack);
			this.CurrentStack = ItemStack.Empty.Clone();
		}
	}

	// Token: 0x060063CA RID: 25546 RVA: 0x00287455 File Offset: 0x00285655
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "place_sound")
		{
			base.xui.LoadData<AudioClip>(value, delegate(AudioClip o)
			{
				this.placeSound = o;
			});
			return true;
		}
		return base.ParseAttribute(name, value, _parent);
	}

	// Token: 0x04004B22 RID: 19234
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemStack ItemStackControl;

	// Token: 0x04004B23 RID: 19235
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip placeSound;

	// Token: 0x04004B24 RID: 19236
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack itemStack = new ItemStack(new ItemValue(0, false), 0);

	// Token: 0x04004B25 RID: 19237
	public bool InMenu;

	// Token: 0x04004B26 RID: 19238
	public XUiC_ItemStack.StackLocationTypes PickUpType;
}

using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E75 RID: 3701
[Preserve]
public class XUiC_ToolTip : XUiController
{
	// Token: 0x17000BD2 RID: 3026
	// (get) Token: 0x0600744B RID: 29771 RVA: 0x002F4082 File Offset: 0x002F2282
	// (set) Token: 0x0600744C RID: 29772 RVA: 0x002F408C File Offset: 0x002F228C
	public string ToolTip
	{
		get
		{
			return this.tooltip;
		}
		set
		{
			if (this.tooltip != value)
			{
				if (value == null)
				{
					this.tooltip = "";
				}
				else if (value.Length > 0 && value[value.Length - 1] == '\n')
				{
					this.tooltip = value.Substring(0, value.Length - 1);
				}
				else
				{
					this.tooltip = value;
				}
				if (this.label != null && this.label.Label != null)
				{
					this.label.Label.overflowMethod = UILabel.Overflow.ResizeFreely;
					this.label.Text = this.tooltip;
					this.label.SetTextImmediately(this.tooltip);
					if (this.tooltip != "")
					{
						base.ViewComponent.Position = base.xui.GetMouseXUIPosition() + new Vector2i(0, -36);
						this.showDelay = Time.unscaledTime + XUiC_ToolTip.SHOW_DELAY_SEC;
					}
				}
			}
		}
	}

	// Token: 0x17000BD3 RID: 3027
	// (get) Token: 0x0600744D RID: 29773 RVA: 0x002F418A File Offset: 0x002F238A
	// (set) Token: 0x0600744E RID: 29774 RVA: 0x002F4197 File Offset: 0x002F2397
	public Vector2i ToolTipPosition
	{
		get
		{
			return base.ViewComponent.Position;
		}
		set
		{
			base.ViewComponent.Position = value;
		}
	}

	// Token: 0x0600744F RID: 29775 RVA: 0x002F41A8 File Offset: 0x002F23A8
	public override void Init()
	{
		base.Init();
		this.ID = base.WindowGroup.ID;
		base.xui.currentToolTip = this;
		this.label = (XUiV_Label)base.GetChildById("lblText").ViewComponent;
		this.background = (XUiV_Sprite)base.GetChildById("sprBackground").ViewComponent;
		this.border = (XUiV_Sprite)base.GetChildById("sprBackgroundBorder").ViewComponent;
		this.tooltip = "";
	}

	// Token: 0x06007450 RID: 29776 RVA: 0x002F4234 File Offset: 0x002F2434
	public override void Update(float _dt)
	{
		if (!GameManager.Instance.isAnyCursorWindowOpen(null))
		{
			this.tooltip = "";
		}
		if (this.tooltip != "")
		{
			if (Time.unscaledTime > this.showDelay)
			{
				((XUiV_Window)base.ViewComponent).TargetAlpha = 1f;
			}
			this.border.Size = new Vector2i(this.label.Label.width + 18, this.label.Label.height + 12);
			this.background.Size = new Vector2i(this.border.Size.x - 6, this.border.Size.y - 6);
			Vector2i xuiScreenSize = base.xui.GetXUiScreenSize();
			if (this.label.Label.width > xuiScreenSize.x / 4)
			{
				this.label.Label.overflowMethod = UILabel.Overflow.ResizeHeight;
				this.label.Label.width = xuiScreenSize.x / 4 - 10;
			}
			else
			{
				Vector2i vector2i = xuiScreenSize / 2;
				if ((base.ViewComponent.Position + this.border.Size).x > vector2i.x)
				{
					base.ViewComponent.Position -= new Vector2i(this.border.Size.x, 0);
				}
				if ((base.ViewComponent.Position - this.border.Size).y < -vector2i.y)
				{
					base.ViewComponent.Position += new Vector2i(20, 20 + this.border.Size.y);
				}
			}
		}
		else
		{
			((XUiV_Window)base.ViewComponent).TargetAlpha = 0.0015f;
		}
		base.Update(_dt);
	}

	// Token: 0x04005868 RID: 22632
	public string ID = "";

	// Token: 0x04005869 RID: 22633
	public static float SHOW_DELAY_SEC = 0.3f;

	// Token: 0x0400586A RID: 22634
	public XUiV_Label label;

	// Token: 0x0400586B RID: 22635
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite background;

	// Token: 0x0400586C RID: 22636
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite border;

	// Token: 0x0400586D RID: 22637
	[PublicizedFrom(EAccessModifier.Private)]
	public string tooltip = "";

	// Token: 0x0400586E RID: 22638
	[PublicizedFrom(EAccessModifier.Private)]
	public int oldHeight;

	// Token: 0x0400586F RID: 22639
	[PublicizedFrom(EAccessModifier.Private)]
	public int oldWidth;

	// Token: 0x04005870 RID: 22640
	[PublicizedFrom(EAccessModifier.Private)]
	public bool nextFrame;

	// Token: 0x04005871 RID: 22641
	[PublicizedFrom(EAccessModifier.Private)]
	public float showDelay;
}

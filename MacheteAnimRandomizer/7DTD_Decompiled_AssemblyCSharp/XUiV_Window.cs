using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000F10 RID: 3856
public class XUiV_Window : XUiView
{
	// Token: 0x17000CC8 RID: 3272
	// (get) Token: 0x06007ACF RID: 31439 RVA: 0x0031C38A File Offset: 0x0031A58A
	// (set) Token: 0x06007AD0 RID: 31440 RVA: 0x0031C392 File Offset: 0x0031A592
	public string Anchor
	{
		get
		{
			return this.anchor;
		}
		set
		{
			this.anchor = value;
			this.isDirty = true;
		}
	}

	// Token: 0x06007AD1 RID: 31441 RVA: 0x0031C3A2 File Offset: 0x0031A5A2
	public XUiV_Window(string _id) : base(_id)
	{
	}

	// Token: 0x17000CC9 RID: 3273
	// (get) Token: 0x06007AD2 RID: 31442 RVA: 0x0031C3C8 File Offset: 0x0031A5C8
	public bool IsCursorArea
	{
		get
		{
			return this.cursorArea;
		}
	}

	// Token: 0x17000CCA RID: 3274
	// (get) Token: 0x06007AD3 RID: 31443 RVA: 0x0031C3D0 File Offset: 0x0031A5D0
	public bool IsOpen
	{
		get
		{
			return this.isOpen;
		}
	}

	// Token: 0x17000CCB RID: 3275
	// (get) Token: 0x06007AD4 RID: 31444 RVA: 0x0031C3D8 File Offset: 0x0031A5D8
	public bool IsInStackpanel
	{
		get
		{
			return this.isInStackpanel;
		}
	}

	// Token: 0x06007AD5 RID: 31445 RVA: 0x003194A0 File Offset: 0x003176A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UIPanel>();
	}

	// Token: 0x06007AD6 RID: 31446 RVA: 0x0031C3E0 File Offset: 0x0031A5E0
	public override void InitView()
	{
		base.InitView();
		base.Controller.OnVisiblity += this.UpdateVisibility;
		this.Panel = this.uiTransform.gameObject.GetComponent<UIPanel>();
		this.Panel.depth = base.Depth + 1;
		this.Panel.alpha = 0f;
	}

	// Token: 0x06007AD7 RID: 31447 RVA: 0x0031C444 File Offset: 0x0031A644
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setRootNode()
	{
		if (this.rootNode == null)
		{
			Transform transform;
			if (this.Anchor == null)
			{
				transform = base.xui.transform;
			}
			else
			{
				transform = base.xui.transform.Find(this.Anchor);
				if (transform == null)
				{
					Log.Error(string.Concat(new string[]
					{
						"Specified window anchor \"",
						this.Anchor,
						"\" not found for window \"",
						base.ID,
						"\""
					}));
					throw new Exception();
				}
			}
			this.rootNode = transform;
			base.setRootNode();
			return;
		}
		if (this.uiTransform != null)
		{
			this.uiTransform.parent = this.rootNode;
			UITable component = this.rootNode.GetComponent<UITable>();
			if (component != null)
			{
				component.repositionNow = true;
			}
			this.IsVisible = true;
			this.uiTransform.gameObject.layer = 12;
			this.uiTransform.localScale = Vector3.one;
			this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
		}
	}

	// Token: 0x17000CCC RID: 3276
	// (get) Token: 0x06007AD8 RID: 31448 RVA: 0x0031C57A File Offset: 0x0031A77A
	// (set) Token: 0x06007AD9 RID: 31449 RVA: 0x0031C582 File Offset: 0x0031A782
	public float TargetAlpha
	{
		get
		{
			return this.targetAlpha;
		}
		set
		{
			if (value != this.targetAlpha)
			{
				this.targetAlpha = value;
				this.fadeTimer = 0f;
			}
		}
	}

	// Token: 0x06007ADA RID: 31450 RVA: 0x0031C5A0 File Offset: 0x0031A7A0
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if ((double)Time.timeScale < 0.01 || !this.fade)
		{
			this.delayTimer = this.delayToFadeTime + 1f;
			this.fadeTimer = this.fadeInTime;
		}
		if (this.delayTimer < this.delayToFadeTime)
		{
			this.delayTimer += _dt;
		}
		if (this.delayTimer < this.delayToFadeTime)
		{
			return;
		}
		if (this.fadeTimer > this.fadeInTime)
		{
			this.fadeTimer = this.fadeInTime;
		}
		this.Panel.alpha = Mathf.Lerp(this.Panel.alpha, this.targetAlpha, this.fadeTimer / this.fadeInTime);
		this.fadeTimer += _dt;
		if (this.cursorArea && this.oldTransformPosition != base.UiTransform.position && this.IsVisible)
		{
			base.xui.UpdateWindowSoftCursorBounds(this);
			this.oldTransformPosition = base.UiTransform.position;
		}
	}

	// Token: 0x06007ADB RID: 31451 RVA: 0x0031C6B1 File Offset: 0x0031A8B1
	public void ForceVisible(float _alpha = -1f)
	{
		this.delayTimer = 100f;
		this.fadeTimer = 100f;
		if (_alpha >= 0f)
		{
			this.targetAlpha = _alpha;
		}
		this.Panel.alpha = this.targetAlpha;
	}

	// Token: 0x06007ADC RID: 31452 RVA: 0x0031C6E9 File Offset: 0x0031A8E9
	public override void UpdateData()
	{
		if (this.uiTransform != null)
		{
			this.setRootNode();
		}
		base.UpdateData();
		this.Panel.SetDirty();
		this.isDirty = false;
	}

	// Token: 0x06007ADD RID: 31453 RVA: 0x0031C718 File Offset: 0x0031A918
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(attribute, value, _parent);
		if (!flag)
		{
			if (!(attribute == "anchor"))
			{
				if (!(attribute == "panel"))
				{
					if (!(attribute == "cursor_area"))
					{
						if (!(attribute == "fade_delay"))
						{
							if (!(attribute == "fade_time"))
							{
								if (!(attribute == "fade_window"))
								{
									return false;
								}
								this.fade = StringParsers.ParseBool(value, 0, -1, true);
							}
							else
							{
								this.fadeInTime = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
							}
						}
						else
						{
							this.delayToFadeTime = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
						}
					}
					else
					{
						this.cursorArea = StringParsers.ParseBool(value, 0, -1, true);
					}
				}
				else
				{
					Transform transform = base.xui.transform.Find("StackPanels").transform;
					if (value != "")
					{
						this.rootNode = transform.FindInChilds(value, false);
						this.isInStackpanel = true;
					}
				}
			}
			else
			{
				this.Anchor = value;
			}
			return true;
		}
		return flag;
	}

	// Token: 0x06007ADE RID: 31454 RVA: 0x0031C830 File Offset: 0x0031AA30
	public override void OnOpen()
	{
		base.OnOpen();
		this.Panel.alpha = 0f;
		this.targetAlpha = 1f;
		this.fadeTimer = 0f;
		this.delayTimer = 0f;
		if (this.cursorArea)
		{
			this.oldTransformPosition = Vector3.zero;
		}
		this.isOpen = true;
	}

	// Token: 0x06007ADF RID: 31455 RVA: 0x0031C890 File Offset: 0x0031AA90
	public override void OnClose()
	{
		base.OnClose();
		this.Panel.alpha = 0f;
		this.targetAlpha = 0f;
		this.fadeTimer = this.fadeInTime;
		this.delayTimer = this.delayToFadeTime;
		this.isOpen = false;
	}

	// Token: 0x06007AE0 RID: 31456 RVA: 0x0031C8DD File Offset: 0x0031AADD
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateVisibility(XUiController _sender, bool _visible)
	{
		if (this.cursorArea)
		{
			if (_visible)
			{
				base.xui.UpdateWindowSoftCursorBounds(this);
				return;
			}
			base.xui.RemoveWindowFromSoftCursorBounds(this);
		}
	}

	// Token: 0x04005CE4 RID: 23780
	[PublicizedFrom(EAccessModifier.Private)]
	public new string anchor;

	// Token: 0x04005CE5 RID: 23781
	public UIPanel Panel;

	// Token: 0x04005CE6 RID: 23782
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isInStackpanel;

	// Token: 0x04005CE7 RID: 23783
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool cursorArea;

	// Token: 0x04005CE8 RID: 23784
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 oldTransformPosition;

	// Token: 0x04005CE9 RID: 23785
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOpen;

	// Token: 0x04005CEA RID: 23786
	[PublicizedFrom(EAccessModifier.Private)]
	public float targetAlpha;

	// Token: 0x04005CEB RID: 23787
	[PublicizedFrom(EAccessModifier.Private)]
	public bool fade = true;

	// Token: 0x04005CEC RID: 23788
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadeInTime = 0.05f;

	// Token: 0x04005CED RID: 23789
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadeTimer;

	// Token: 0x04005CEE RID: 23790
	[PublicizedFrom(EAccessModifier.Private)]
	public float delayToFadeTime = 0.1f;

	// Token: 0x04005CEF RID: 23791
	[PublicizedFrom(EAccessModifier.Private)]
	public float delayTimer;
}

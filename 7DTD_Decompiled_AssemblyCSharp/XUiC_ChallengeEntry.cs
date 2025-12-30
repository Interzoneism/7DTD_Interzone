using System;
using Challenges;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C22 RID: 3106
[Preserve]
public class XUiC_ChallengeEntry : XUiController
{
	// Token: 0x170009D9 RID: 2521
	// (get) Token: 0x06005F77 RID: 24439 RVA: 0x0026B517 File Offset: 0x00269717
	// (set) Token: 0x06005F78 RID: 24440 RVA: 0x0026B520 File Offset: 0x00269720
	public Challenge Entry
	{
		get
		{
			return this.entry;
		}
		set
		{
			base.ViewComponent.Enabled = (value != null);
			this.entry = value;
			this.challengeClass = ((this.entry != null) ? this.entry.ChallengeClass : null);
			if (this.challengeClass != null)
			{
				this.IsChallengeVisible = this.challengeClass.ChallengeGroup.IsVisible(this.entry.Owner.Player);
			}
			else
			{
				this.IsChallengeVisible = true;
			}
			this.IsDirty = true;
		}
	}

	// Token: 0x170009DA RID: 2522
	// (get) Token: 0x06005F79 RID: 24441 RVA: 0x0026B59D File Offset: 0x0026979D
	// (set) Token: 0x06005F7A RID: 24442 RVA: 0x0026B5A5 File Offset: 0x002697A5
	public XUiC_ChallengeWindowGroup JournalUIHandler { get; set; }

	// Token: 0x170009DB RID: 2523
	// (get) Token: 0x06005F7B RID: 24443 RVA: 0x0026B5AE File Offset: 0x002697AE
	// (set) Token: 0x06005F7C RID: 24444 RVA: 0x0026B5B6 File Offset: 0x002697B6
	public bool Tracked { get; set; }

	// Token: 0x06005F7D RID: 24445 RVA: 0x0026B5C0 File Offset: 0x002697C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.entry != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 765459171U)
		{
			if (num != 21245043U)
			{
				if (num != 480848564U)
				{
					if (num == 765459171U)
					{
						if (bindingName == "rowstatecolor")
						{
							value = this.rowColor;
							if (flag)
							{
								if (this.Selected)
								{
									value = this.selectedColor;
								}
								else if (this.IsHovered)
								{
									value = this.hoverColor;
								}
							}
							return true;
						}
					}
				}
				else if (bindingName == "fillamount")
				{
					value = ((flag && this.entry.IsActive && this.IsChallengeVisible) ? this.entry.FillAmount.ToString() : "0");
					return true;
				}
			}
			else if (bindingName == "iconname")
			{
				value = "";
				if (flag)
				{
					value = (this.IsChallengeVisible ? this.challengeClass.Icon : "ui_game_symbol_other");
				}
				return true;
			}
		}
		else if (num <= 1566407741U)
		{
			if (num != 1149663213U)
			{
				if (num == 1566407741U)
				{
					if (bindingName == "hasentry")
					{
						value = (flag ? "true" : "false");
						return true;
					}
				}
			}
			else if (bindingName == "tracked")
			{
				value = (flag ? this.entry.IsTracked.ToString() : "false");
				return true;
			}
		}
		else if (num != 2240895362U)
		{
			if (num == 3106195591U)
			{
				if (bindingName == "iconcolor")
				{
					value = "255,255,255,255";
					if (flag)
					{
						if (!this.IsChallengeVisible)
						{
							value = this.disabledColor;
						}
						else if (this.entry.ChallengeState == Challenge.ChallengeStates.Redeemed)
						{
							value = this.disabledColor;
						}
						else if (this.entry.ReadyToComplete)
						{
							value = this.redeemableColor;
						}
						else if (this.entry.IsTracked)
						{
							value = this.trackedColor;
						}
						else if (this.IsHovered)
						{
							value = this.hoverColor;
						}
						else
						{
							value = this.enabledColor;
						}
					}
					return true;
				}
			}
		}
		else if (bindingName == "fillactive")
		{
			if (flag)
			{
				if (!this.IsChallengeVisible)
				{
					value = "false";
				}
				else
				{
					value = this.entry.IsActive.ToString();
				}
			}
			else
			{
				value = "false";
			}
			return true;
		}
		return false;
	}

	// Token: 0x06005F7E RID: 24446 RVA: 0x0026B83C File Offset: 0x00269A3C
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("itemIcon");
		if (childById != null)
		{
			this.itemIconSprite = (childById.ViewComponent as XUiV_Sprite);
			this.iconSize = this.itemIconSprite.Size;
		}
		this.tweenScale = this.itemIconSprite.UiTransform.gameObject.AddComponent<TweenScale>();
		this.IsDirty = true;
	}

	// Token: 0x06005F7F RID: 24447 RVA: 0x0026B8A2 File Offset: 0x00269AA2
	public override void OnCursorSelected()
	{
		base.OnCursorSelected();
		this.Owner.SelectedEntry = this;
	}

	// Token: 0x06005F80 RID: 24448 RVA: 0x0026B8B6 File Offset: 0x00269AB6
	public override void OnCursorUnSelected()
	{
		base.OnCursorUnSelected();
		this.Selected = false;
		this.IsDirty = true;
	}

	// Token: 0x06005F81 RID: 24449 RVA: 0x0026B8CC File Offset: 0x00269ACC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		if (!this.IsChallengeVisible)
		{
			this.IsHovered = false;
			return;
		}
		base.OnHovered(_isOver);
		if (this.Entry == null)
		{
			this.IsHovered = false;
			return;
		}
		if (this.Entry != null && !this.IsRedeemBlinking)
		{
			if (_isOver)
			{
				this.tweenScale.from = Vector3.one;
				this.tweenScale.to = Vector3.one * 1.1f;
				this.tweenScale.enabled = true;
				this.tweenScale.duration = 0.5f;
			}
			else
			{
				this.tweenScale.from = Vector3.one * 1.1f;
				this.tweenScale.to = Vector3.one;
				this.tweenScale.enabled = true;
				this.tweenScale.duration = 0.5f;
			}
		}
		if (this.IsHovered != _isOver)
		{
			this.IsHovered = _isOver;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06005F82 RID: 24450 RVA: 0x0026B9C0 File Offset: 0x00269BC0
	public override void Update(float _dt)
	{
		if (this.IsDirty || (this.entry != null && this.entry.NeedsUIUpdate))
		{
			if (this.challengeClass != null)
			{
				this.IsChallengeVisible = this.challengeClass.ChallengeGroup.IsVisible(this.entry.Owner.Player);
			}
			else
			{
				this.IsChallengeVisible = true;
			}
			base.ViewComponent.SoundPlayOnHover = this.IsChallengeVisible;
			base.ViewComponent.SoundPlayOnClick = this.IsChallengeVisible;
		}
		base.RefreshBindings(this.IsDirty);
		this.IsDirty = false;
		base.Update(_dt);
		if (this.IsRedeemBlinking && !this.Selected)
		{
			this.tweenScale.enabled = false;
			float num = Mathf.PingPong(Time.time, 0.5f);
			float num2 = 1f;
			if (num > 0.25f)
			{
				num2 = 1f + num - 0.25f;
			}
			this.itemIconSprite.Sprite.SetDimensions((int)((float)this.iconSize.x * num2), (int)((float)this.iconSize.y * num2));
			return;
		}
		if (this.Selected)
		{
			this.itemIconSprite.Sprite.SetDimensions(this.iconSize.x, this.iconSize.y);
		}
	}

	// Token: 0x06005F83 RID: 24451 RVA: 0x0026BB04 File Offset: 0x00269D04
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "enabled_color")
		{
			this.enabledColor = value;
			return true;
		}
		if (name == "disabled_color")
		{
			this.disabledColor = value;
			return true;
		}
		if (name == "row_color")
		{
			this.rowColor = value;
			return true;
		}
		if (name == "hover_color")
		{
			this.hoverColor = value;
			return true;
		}
		if (!(name == "selected_color"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		this.selectedColor = value;
		return true;
	}

	// Token: 0x040047EB RID: 18411
	[PublicizedFrom(EAccessModifier.Private)]
	public string enabledColor;

	// Token: 0x040047EC RID: 18412
	[PublicizedFrom(EAccessModifier.Private)]
	public string disabledColor;

	// Token: 0x040047ED RID: 18413
	[PublicizedFrom(EAccessModifier.Private)]
	public string rowColor;

	// Token: 0x040047EE RID: 18414
	[PublicizedFrom(EAccessModifier.Private)]
	public string hoverColor;

	// Token: 0x040047EF RID: 18415
	[PublicizedFrom(EAccessModifier.Private)]
	public string selectedColor;

	// Token: 0x040047F0 RID: 18416
	[PublicizedFrom(EAccessModifier.Private)]
	public string trackedColor = "255, 180, 0, 255";

	// Token: 0x040047F1 RID: 18417
	[PublicizedFrom(EAccessModifier.Private)]
	public string redeemableColor = "0,255,0,255";

	// Token: 0x040047F2 RID: 18418
	public new bool Selected;

	// Token: 0x040047F3 RID: 18419
	public bool IsHovered;

	// Token: 0x040047F4 RID: 18420
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Sprite itemIconSprite;

	// Token: 0x040047F5 RID: 18421
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i iconSize;

	// Token: 0x040047F6 RID: 18422
	public bool IsRedeemBlinking;

	// Token: 0x040047F7 RID: 18423
	public bool IsChallengeVisible;

	// Token: 0x040047F8 RID: 18424
	[PublicizedFrom(EAccessModifier.Private)]
	public Challenge entry;

	// Token: 0x040047F9 RID: 18425
	[PublicizedFrom(EAccessModifier.Private)]
	public ChallengeClass challengeClass;

	// Token: 0x040047FA RID: 18426
	[PublicizedFrom(EAccessModifier.Protected)]
	public TweenScale tweenScale;

	// Token: 0x040047FC RID: 18428
	public XUiC_ChallengeEntryList Owner;
}

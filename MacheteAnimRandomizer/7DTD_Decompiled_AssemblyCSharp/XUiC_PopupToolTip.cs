using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D82 RID: 3458
[Preserve]
public class XUiC_PopupToolTip : XUiController
{
	// Token: 0x17000ADD RID: 2781
	// (get) Token: 0x06006C0B RID: 27659 RVA: 0x002C30A5 File Offset: 0x002C12A5
	// (set) Token: 0x06006C0A RID: 27658 RVA: 0x002C3095 File Offset: 0x002C1295
	public float TextAlphaCurrent
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.textAlphaCurrent;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.textAlphaCurrent = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000ADE RID: 2782
	// (set) Token: 0x06006C0C RID: 27660 RVA: 0x002C30AD File Offset: 0x002C12AD
	public string TooltipText
	{
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.tooltipText = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x06006C0D RID: 27661 RVA: 0x002C30BD File Offset: 0x002C12BD
	public override void Init()
	{
		base.Init();
		XUiC_PopupToolTip.ID = base.WindowGroup.ID;
		this.toolbelt = base.xui.GetChildByType<XUiC_Toolbelt>();
	}

	// Token: 0x06006C0E RID: 27662 RVA: 0x00282536 File Offset: 0x00280736
	public override void OnOpen()
	{
		base.OnOpen();
		this.IsDirty = true;
	}

	// Token: 0x06006C0F RID: 27663 RVA: 0x002C30E8 File Offset: 0x002C12E8
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (GameStats.GetInt(EnumGameStats.GameState) == 1)
		{
			if ((base.xui.playerUI != null && base.xui.playerUI.entityPlayer != null && base.xui.playerUI.entityPlayer.IsDead()) || !base.xui.playerUI.windowManager.IsHUDEnabled())
			{
				this.ClearTooltipsInternal();
			}
			if (!this.pauseToolTips)
			{
				this.TextAlphaCurrent = Mathf.Lerp(this.textAlphaCurrent, this.textAlphaTarget, _dt * 3f);
				if (this.countdownTooltip.HasPassed() && base.xui.isReady && !XUiC_SubtitlesDisplay.IsDisplaying)
				{
					this.DisplayTooltipText();
				}
			}
		}
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006C10 RID: 27664 RVA: 0x002C31CC File Offset: 0x002C13CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "text")
		{
			_value = this.tooltipText;
			return true;
		}
		if (_bindingName == "textalpha")
		{
			_value = this.textalphaFormatter.Format((int)(255f * this.textAlphaCurrent));
			return true;
		}
		if (!(_bindingName == "yoffset_secondrow"))
		{
			return false;
		}
		_value = this.yoffsetFormatter.Format((this.toolbelt != null && this.toolbelt.HasSecondRow) ? this.yOffsetSecondRow : 0);
		return true;
	}

	// Token: 0x06006C11 RID: 27665 RVA: 0x002C3256 File Offset: 0x002C1456
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "yoffset_second_row")
		{
			this.yOffsetSecondRow = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x06006C12 RID: 27666 RVA: 0x002C327F File Offset: 0x002C147F
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearTooltipsInternal()
	{
		this.tooltipQueue.Clear();
		this.TextAlphaCurrent = 0f;
		this.textAlphaTarget = 0f;
	}

	// Token: 0x06006C13 RID: 27667 RVA: 0x002C32A4 File Offset: 0x002C14A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void QueueTooltipInternal(string _text, string[] _args, string _alertSound, ToolTipEvent _eventHandler, bool _showImmediately, bool _pinTooltip, float _timeout)
	{
		if (string.IsNullOrEmpty(_text) && string.IsNullOrEmpty(_alertSound) && _eventHandler == null)
		{
			return;
		}
		string text = Localization.Get(_text, false);
		if (_args != null && _args.Length != 0)
		{
			text = string.Format(text, _args);
		}
		XUiC_PopupToolTip.Tooltip item = new XUiC_PopupToolTip.Tooltip(_text, text, _alertSound, _eventHandler, _timeout, _pinTooltip);
		if (_pinTooltip)
		{
			this.immediateTip = item;
			if (!this.tooltipQueue.Contains(item))
			{
				this.tooltipQueue.Enqueue(item);
			}
			this.DisplayTooltipText();
			return;
		}
		if (_showImmediately)
		{
			this.immediateTip = item;
			this.DisplayTooltipText();
			return;
		}
		if (!this.tooltipQueue.Contains(item))
		{
			this.tooltipQueue.Enqueue(item);
		}
	}

	// Token: 0x06006C14 RID: 27668 RVA: 0x002C3348 File Offset: 0x002C1548
	[PublicizedFrom(EAccessModifier.Private)]
	public void RemovePinnedTooltipInternal(string _key)
	{
		foreach (XUiC_PopupToolTip.Tooltip tooltip in this.tooltipQueue)
		{
			if (tooltip.Pinned && tooltip.Key == _key)
			{
				tooltip.RemoveOnDequeue = true;
				break;
			}
		}
	}

	// Token: 0x06006C15 RID: 27669 RVA: 0x002C33B4 File Offset: 0x002C15B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void DisplayTooltipText()
	{
		if (this.tooltipQueue.Count == 0 && this.immediateTip == null)
		{
			this.TextAlphaCurrent = 0f;
			this.TooltipText = string.Empty;
			return;
		}
		XUiC_PopupToolTip.Tooltip tooltip = null;
		bool flag = false;
		if (this.immediateTip != null)
		{
			tooltip = this.immediateTip;
			this.immediateTip = null;
		}
		else
		{
			while (this.tooltipQueue.Count > 0)
			{
				tooltip = this.tooltipQueue.Dequeue();
				if (tooltip.RemoveOnDequeue)
				{
					tooltip = null;
				}
				else
				{
					if (tooltip.Pinned)
					{
						if (this.tooltipQueue.Count == 0)
						{
							flag = true;
						}
						this.tooltipQueue.Enqueue(tooltip);
						break;
					}
					if (tooltip.Timeout <= 0f || Time.time - tooltip.CreationTime < tooltip.Timeout)
					{
						break;
					}
					tooltip = null;
				}
			}
		}
		if (tooltip == null)
		{
			this.TextAlphaCurrent = 0f;
			this.TooltipText = string.Empty;
			return;
		}
		if (!string.IsNullOrEmpty(tooltip.AlertSoundName))
		{
			Manager.PlayInsidePlayerHead(tooltip.AlertSoundName, -1, 0f, false, false);
		}
		ToolTipEvent @event = tooltip.Event;
		if (@event != null)
		{
			@event.HandleEvent();
		}
		this.TextAlphaCurrent = (flag ? this.TextAlphaCurrent : 0f);
		if (!string.IsNullOrEmpty(tooltip.Text))
		{
			this.textAlphaTarget = 1f;
			this.TooltipText = tooltip.Text;
		}
		else
		{
			this.textAlphaTarget = 0f;
			this.TooltipText = "";
		}
		this.countdownTooltip.ResetAndRestart();
	}

	// Token: 0x06006C16 RID: 27670 RVA: 0x002C351F File Offset: 0x002C171F
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetToolTipPauseInternal(bool _isPaused)
	{
		this.pauseToolTips = _isPaused;
		if (_isPaused)
		{
			this.TextAlphaCurrent = 0f;
			return;
		}
		this.TextAlphaCurrent = this.textAlphaTarget;
	}

	// Token: 0x06006C17 RID: 27671 RVA: 0x002C3543 File Offset: 0x002C1743
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearCurrentTooltipInternal()
	{
		this.TooltipText = "";
		this.TextAlphaCurrent = 0f;
		this.textAlphaTarget = 0f;
	}

	// Token: 0x06006C18 RID: 27672 RVA: 0x002C3566 File Offset: 0x002C1766
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiC_PopupToolTip GetInstance(XUi _xui)
	{
		XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)_xui.playerUI.windowManager.GetWindow(XUiC_PopupToolTip.ID);
		if (xuiWindowGroup == null)
		{
			return null;
		}
		XUiController controller = xuiWindowGroup.Controller;
		if (controller == null)
		{
			return null;
		}
		return controller.GetChildByType<XUiC_PopupToolTip>();
	}

	// Token: 0x06006C19 RID: 27673 RVA: 0x002C3598 File Offset: 0x002C1798
	public static void ClearTooltips(XUi _xui)
	{
		XUiC_PopupToolTip instance = XUiC_PopupToolTip.GetInstance(_xui);
		if (instance == null)
		{
			return;
		}
		instance.ClearTooltipsInternal();
	}

	// Token: 0x06006C1A RID: 27674 RVA: 0x002C35AA File Offset: 0x002C17AA
	public static void QueueTooltip(XUi _xui, string _text, string[] _args, string _alertSound, ToolTipEvent _eventHandler, bool _showImmediately, bool _pinTooltip, float _timeout = 0f)
	{
		XUiC_PopupToolTip instance = XUiC_PopupToolTip.GetInstance(_xui);
		if (instance == null)
		{
			return;
		}
		instance.QueueTooltipInternal(_text, _args, _alertSound, _eventHandler, _showImmediately, _pinTooltip, _timeout);
	}

	// Token: 0x06006C1B RID: 27675 RVA: 0x002C35C7 File Offset: 0x002C17C7
	public static void SetToolTipPause(XUi _xui, bool _isPaused)
	{
		XUiC_PopupToolTip instance = XUiC_PopupToolTip.GetInstance(_xui);
		if (instance == null)
		{
			return;
		}
		instance.SetToolTipPauseInternal(_isPaused);
	}

	// Token: 0x06006C1C RID: 27676 RVA: 0x002C35DA File Offset: 0x002C17DA
	public static void ClearCurrentTooltip(XUi _xui)
	{
		XUiC_PopupToolTip instance = XUiC_PopupToolTip.GetInstance(_xui);
		if (instance == null)
		{
			return;
		}
		instance.ClearCurrentTooltipInternal();
	}

	// Token: 0x06006C1D RID: 27677 RVA: 0x002C35EC File Offset: 0x002C17EC
	public static void RemovePinnedTooltip(XUi _xui, string _key)
	{
		XUiC_PopupToolTip instance = XUiC_PopupToolTip.GetInstance(_xui);
		if (instance == null)
		{
			return;
		}
		instance.RemovePinnedTooltipInternal(_key);
	}

	// Token: 0x04005235 RID: 21045
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ID = "";

	// Token: 0x04005236 RID: 21046
	[PublicizedFrom(EAccessModifier.Private)]
	public int yOffsetSecondRow = 75;

	// Token: 0x04005237 RID: 21047
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Toolbelt toolbelt;

	// Token: 0x04005238 RID: 21048
	[PublicizedFrom(EAccessModifier.Private)]
	public string tooltipText = string.Empty;

	// Token: 0x04005239 RID: 21049
	[PublicizedFrom(EAccessModifier.Private)]
	public float textAlphaTarget;

	// Token: 0x0400523A RID: 21050
	[PublicizedFrom(EAccessModifier.Private)]
	public float textAlphaCurrent;

	// Token: 0x0400523B RID: 21051
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CountdownTimer countdownTooltip = new CountdownTimer(5f, true);

	// Token: 0x0400523C RID: 21052
	[PublicizedFrom(EAccessModifier.Private)]
	public bool pauseToolTips;

	// Token: 0x0400523D RID: 21053
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Queue<XUiC_PopupToolTip.Tooltip> tooltipQueue = new Queue<XUiC_PopupToolTip.Tooltip>();

	// Token: 0x0400523E RID: 21054
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PopupToolTip.Tooltip immediateTip;

	// Token: 0x0400523F RID: 21055
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt textalphaFormatter = new CachedStringFormatterInt();

	// Token: 0x04005240 RID: 21056
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt yoffsetFormatter = new CachedStringFormatterInt();

	// Token: 0x02000D83 RID: 3459
	[PublicizedFrom(EAccessModifier.Private)]
	public class Tooltip : IEquatable<XUiC_PopupToolTip.Tooltip>
	{
		// Token: 0x06006C20 RID: 27680 RVA: 0x002C3664 File Offset: 0x002C1864
		public Tooltip(string _key, string _text, string _alertSoundName, ToolTipEvent _event, float _timeout = 0f, bool _pinned = false)
		{
			this.Key = _key;
			this.Text = _text;
			this.AlertSoundName = _alertSoundName;
			this.Event = _event;
			this.Timeout = _timeout;
			this.Pinned = _pinned;
			this.CreationTime = Time.time;
		}

		// Token: 0x06006C21 RID: 27681 RVA: 0x002C36BA File Offset: 0x002C18BA
		public bool Equals(XUiC_PopupToolTip.Tooltip _other)
		{
			return _other != null && (this == _other || this.Text == _other.Text);
		}

		// Token: 0x06006C22 RID: 27682 RVA: 0x002C36D8 File Offset: 0x002C18D8
		public override bool Equals(object _obj)
		{
			return _obj != null && (this == _obj || (!(_obj.GetType() != base.GetType()) && this.Equals((XUiC_PopupToolTip.Tooltip)_obj)));
		}

		// Token: 0x06006C23 RID: 27683 RVA: 0x002C3706 File Offset: 0x002C1906
		public override int GetHashCode()
		{
			if (this.Text == null)
			{
				return 0;
			}
			return this.Text.GetHashCode();
		}

		// Token: 0x04005241 RID: 21057
		public readonly string Key;

		// Token: 0x04005242 RID: 21058
		public readonly string Text;

		// Token: 0x04005243 RID: 21059
		public readonly string AlertSoundName;

		// Token: 0x04005244 RID: 21060
		public readonly ToolTipEvent Event;

		// Token: 0x04005245 RID: 21061
		public readonly float Timeout = -1f;

		// Token: 0x04005246 RID: 21062
		public readonly bool Pinned;

		// Token: 0x04005247 RID: 21063
		public bool RemoveOnDequeue;

		// Token: 0x04005248 RID: 21064
		public readonly float CreationTime;
	}
}

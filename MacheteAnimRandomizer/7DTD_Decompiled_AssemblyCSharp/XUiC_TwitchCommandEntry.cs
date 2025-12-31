using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E82 RID: 3714
[Preserve]
public class XUiC_TwitchCommandEntry : XUiController
{
	// Token: 0x17000BEB RID: 3051
	// (get) Token: 0x060074F9 RID: 29945 RVA: 0x002F8F8C File Offset: 0x002F718C
	// (set) Token: 0x060074FA RID: 29946 RVA: 0x002F8F94 File Offset: 0x002F7194
	public XUiC_TwitchWindow Owner { get; set; }

	// Token: 0x17000BEC RID: 3052
	// (get) Token: 0x060074FB RID: 29947 RVA: 0x002F8F9D File Offset: 0x002F719D
	// (set) Token: 0x060074FC RID: 29948 RVA: 0x002F8FA5 File Offset: 0x002F71A5
	public TwitchAction Action
	{
		get
		{
			return this.action;
		}
		set
		{
			this.action = value;
			this.isDirty = true;
		}
	}

	// Token: 0x060074FD RID: 29949 RVA: 0x002F8FB8 File Offset: 0x002F71B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.action != null;
		if (bindingName == "hascommand")
		{
			value = flag.ToString();
			return true;
		}
		if (bindingName == "commandname")
		{
			value = (flag ? this.action.Command : "");
			return true;
		}
		if (bindingName == "commandcost")
		{
			if (flag)
			{
				if (this.isReady)
				{
					if (this.Owner != null)
					{
						switch (this.action.PointType)
						{
						case TwitchAction.PointTypes.PP:
							value = string.Format("{0} {1}", this.action.CurrentCost, this.Owner.lblPointsPP);
							break;
						case TwitchAction.PointTypes.SP:
							value = string.Format("{0} {1}", this.action.CurrentCost, this.Owner.lblPointsSP);
							break;
						case TwitchAction.PointTypes.Bits:
							value = "* ";
							break;
						}
					}
					else
					{
						value = "";
					}
				}
				else
				{
					value = "--";
				}
			}
			else
			{
				value = "";
			}
			return true;
		}
		if (bindingName == "commandcolor")
		{
			if (flag)
			{
				if (this.isReady)
				{
					if (this.action.IsPositive)
					{
						value = this.positiveColor;
					}
					else
					{
						value = this.negativeColor;
					}
				}
				else
				{
					value = this.disabledColor;
				}
			}
			return true;
		}
		if (bindingName == "costcolor")
		{
			if (flag)
			{
				if (this.isReady)
				{
					switch (this.action.PointType)
					{
					case TwitchAction.PointTypes.PP:
						value = this.defaultCostColor;
						break;
					case TwitchAction.PointTypes.SP:
						value = this.specialCostColor;
						break;
					case TwitchAction.PointTypes.Bits:
						value = this.bitCostColor;
						break;
					}
				}
				else
				{
					value = this.disabledColor;
				}
			}
			return true;
		}
		if (!(bindingName == "commandtextwidth"))
		{
			return false;
		}
		if (this.action != null && this.isBool)
		{
			value = "150";
		}
		else
		{
			value = "150";
		}
		return true;
	}

	// Token: 0x060074FE RID: 29950 RVA: 0x002F91AC File Offset: 0x002F73AC
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "positive_color")
		{
			this.positiveColor = value;
			return true;
		}
		if (name == "negative_color")
		{
			this.negativeColor = value;
			return true;
		}
		if (name == "disabled_color")
		{
			this.disabledColor = value;
			return true;
		}
		if (name == "default_cost_color")
		{
			this.defaultCostColor = value;
			return true;
		}
		if (!(name == "special_cost_color"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		this.specialCostColor = value;
		return true;
	}

	// Token: 0x060074FF RID: 29951 RVA: 0x002BF216 File Offset: 0x002BD416
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnCountChanged(XUiController _sender, OnCountChangedEventArgs _e)
	{
		base.RefreshBindings(true);
	}

	// Token: 0x06007500 RID: 29952 RVA: 0x002F9234 File Offset: 0x002F7434
	public override void Update(float _dt)
	{
		if (this.Action != null)
		{
			this.isDirty = true;
			this.isReady = this.action.IsReady(this.Owner.twitchManager);
		}
		else
		{
			this.isReady = false;
		}
		if (this.isDirty)
		{
			base.RefreshBindings(this.isDirty);
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x040058F5 RID: 22773
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchAction action;

	// Token: 0x040058F6 RID: 22774
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x040058F7 RID: 22775
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBool;

	// Token: 0x040058F8 RID: 22776
	[PublicizedFrom(EAccessModifier.Private)]
	public string positiveColor = "0,0,255";

	// Token: 0x040058F9 RID: 22777
	[PublicizedFrom(EAccessModifier.Private)]
	public string negativeColor = "255,0,0";

	// Token: 0x040058FA RID: 22778
	[PublicizedFrom(EAccessModifier.Private)]
	public string disabledColor = "80,80,80";

	// Token: 0x040058FB RID: 22779
	[PublicizedFrom(EAccessModifier.Private)]
	public string defaultCostColor = "255,255,255,255";

	// Token: 0x040058FC RID: 22780
	[PublicizedFrom(EAccessModifier.Private)]
	public string specialCostColor = "0,125,125,255";

	// Token: 0x040058FD RID: 22781
	[PublicizedFrom(EAccessModifier.Private)]
	public string bitCostColor = "145,70,255,255";

	// Token: 0x040058FF RID: 22783
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isReady;

	// Token: 0x04005900 RID: 22784
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string> objectiveOptionalFormatter = new CachedStringFormatter<string>((string _s) => "(" + _s + ") ");
}

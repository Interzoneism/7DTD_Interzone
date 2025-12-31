using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000C86 RID: 3206
[Preserve]
public class XUiC_DeathBar : XUiController
{
	// Token: 0x17000A16 RID: 2582
	// (get) Token: 0x060062DD RID: 25309 RVA: 0x00282DAE File Offset: 0x00280FAE
	// (set) Token: 0x060062DE RID: 25310 RVA: 0x00282DB6 File Offset: 0x00280FB6
	public EntityAlive Target { get; set; }

	// Token: 0x060062DF RID: 25311 RVA: 0x00282DBF File Offset: 0x00280FBF
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
		this.viewComponent.IsVisible = false;
	}

	// Token: 0x060062E0 RID: 25312 RVA: 0x00282DDC File Offset: 0x00280FDC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.LocalPlayer == null && base.xui != null && base.xui.playerUI != null && base.xui.playerUI.entityPlayer != null)
		{
			this.LocalPlayer = base.xui.playerUI.entityPlayer;
		}
		if (this.LocalPlayer == null)
		{
			return;
		}
		if (this.LocalPlayer.IsAlive())
		{
			this.viewComponent.IsVisible = false;
			return;
		}
		if (this.deathText != TwitchManager.DeathText)
		{
			this.deathText = TwitchManager.DeathText;
			base.RefreshBindings(true);
		}
	}

	// Token: 0x060062E1 RID: 25313 RVA: 0x00282E9A File Offset: 0x0028109A
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(true);
	}

	// Token: 0x060062E2 RID: 25314 RVA: 0x00282EAC File Offset: 0x002810AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (!(_bindingName == "death_text"))
		{
			if (!(_bindingName == "visible"))
			{
				return false;
			}
			if (this.LocalPlayer == null)
			{
				_value = "false";
				return true;
			}
			if (this.LocalPlayer.IsAlive())
			{
				_value = "false";
				return true;
			}
			if (TwitchManager.DeathText == "")
			{
				_value = "false";
				return true;
			}
			_value = "true";
			return true;
		}
		else
		{
			if (this.LocalPlayer == null)
			{
				_value = "";
				return true;
			}
			_value = this.deathText;
			return true;
		}
	}

	// Token: 0x04004A8E RID: 19086
	[PublicizedFrom(EAccessModifier.Private)]
	public string deathText = "";

	// Token: 0x04004A8F RID: 19087
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal LocalPlayer;
}

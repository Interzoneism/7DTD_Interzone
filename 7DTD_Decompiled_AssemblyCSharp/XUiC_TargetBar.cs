using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E5E RID: 3678
[Preserve]
public class XUiC_TargetBar : XUiController
{
	// Token: 0x17000BB7 RID: 2999
	// (get) Token: 0x0600738D RID: 29581 RVA: 0x002F13E3 File Offset: 0x002EF5E3
	// (set) Token: 0x0600738E RID: 29582 RVA: 0x002F13EB File Offset: 0x002EF5EB
	public EntityAlive Target { get; set; }

	// Token: 0x0600738F RID: 29583 RVA: 0x00282DBF File Offset: 0x00280FBF
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
		this.viewComponent.IsVisible = false;
	}

	// Token: 0x06007390 RID: 29584 RVA: 0x002F13F4 File Offset: 0x002EF5F4
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.deltaTime = _dt;
		if (this.gameEventManager == null)
		{
			this.gameEventManager = GameEventManager.Current;
		}
		if (!base.xui.playerUI.entityPlayer.IsAlive())
		{
			this.viewComponent.IsVisible = false;
			return;
		}
		if (this.gameEventManager.CurrentBossGroup != null)
		{
			this.CurrentBossGroup = this.gameEventManager.CurrentBossGroup;
			this.Target = this.CurrentBossGroup.BossEntity;
			bool flag = false;
			if (this.Target != null && this.Target.IsAlive())
			{
				flag = true;
			}
			if (this.CurrentBossGroup.MinionCount != 0)
			{
				flag = true;
			}
			if (flag)
			{
				this.viewComponent.IsVisible = true;
				this.noTargetFadeTime = 0f;
			}
			else if (this.noTargetFadeTime >= this.noTargetFadeTimeMax)
			{
				this.Target = null;
				this.viewComponent.IsVisible = false;
				this.CurrentBossGroup.BossEntity = null;
			}
			else
			{
				this.noTargetFadeTime += Time.deltaTime;
			}
		}
		else
		{
			if (this.CurrentBossGroup != null)
			{
				this.Target = null;
				this.CurrentBossGroup = null;
			}
			if (this.visibility == XUiC_TargetBar.EVisibility.Never)
			{
				this.viewComponent.IsVisible = false;
				return;
			}
			if (this.visibility == XUiC_TargetBar.EVisibility.GodMode && !base.xui.playerUI.entityPlayer.IsGodMode.Value)
			{
				this.viewComponent.IsVisible = false;
				return;
			}
			bool flag2 = false;
			WorldRayHitInfo hitInfo = base.xui.playerUI.entityPlayer.HitInfo;
			if (hitInfo.bHitValid && hitInfo.transform && hitInfo.tag.StartsWith("E_", StringComparison.Ordinal))
			{
				Transform hitRootTransform = GameUtils.GetHitRootTransform(hitInfo.tag, hitInfo.transform);
				EntityAlive entityAlive = null;
				if (hitRootTransform != null)
				{
					entityAlive = hitRootTransform.GetComponent<EntityAlive>();
				}
				if (entityAlive != null && entityAlive.IsAlive())
				{
					flag2 = true;
					this.Target = entityAlive;
				}
			}
			if (this.Target == null)
			{
				this.viewComponent.IsVisible = false;
				this.noTargetFadeTime = this.noTargetFadeTimeMax;
				return;
			}
			if (flag2)
			{
				this.viewComponent.IsVisible = true;
				this.noTargetFadeTime = 0f;
			}
			else if (this.noTargetFadeTime >= this.noTargetFadeTimeMax)
			{
				this.Target = null;
				this.viewComponent.IsVisible = false;
			}
			else
			{
				this.noTargetFadeTime += Time.deltaTime;
			}
			if (this.Target != null && (this.Target.IsDead() || this.Target.Health == 0))
			{
				this.Target = null;
				this.viewComponent.IsVisible = false;
				this.noTargetFadeTime = this.noTargetFadeTimeMax;
			}
		}
		base.RefreshBindings(this.IsDirty);
		this.IsDirty = false;
	}

	// Token: 0x06007391 RID: 29585 RVA: 0x002F16C0 File Offset: 0x002EF8C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1324577848U)
		{
			if (num <= 973435292U)
			{
				if (num != 17711523U)
				{
					if (num == 973435292U)
					{
						if (bindingName == "isboss")
						{
							if (this.gameEventManager == null)
							{
								value = "false";
								return true;
							}
							value = (this.CurrentBossGroup != null).ToString();
							return true;
						}
					}
				}
				else if (bindingName == "isnotboss")
				{
					if (this.gameEventManager == null)
					{
						value = "false";
						return true;
					}
					value = (this.CurrentBossGroup == null).ToString();
					return true;
				}
			}
			else if (num != 1080943954U)
			{
				if (num == 1324577848U)
				{
					if (bindingName == "currentwithmax")
					{
						if (this.Target == null)
						{
							value = "";
							return true;
						}
						value = this.statcurrentWMaxFormatterAOfB.Format(this.Target.IsAlive() ? ((int)this.Target.Stats.Health.Value) : 0, (int)this.Target.Stats.Health.Max);
						return true;
					}
				}
			}
			else if (bindingName == "minioncount")
			{
				if (this.gameEventManager == null || this.CurrentBossGroup == null)
				{
					value = "";
					return true;
				}
				value = this.CurrentBossGroup.MinionCount.ToString();
				return true;
			}
		}
		else if (num <= 2984927816U)
		{
			if (num != 2369371622U)
			{
				if (num == 2984927816U)
				{
					if (bindingName == "fill")
					{
						if (this.Target == null)
						{
							value = "0";
							return true;
						}
						float b = (float)this.Target.Health / (float)this.Target.GetMaxHealth();
						float v = Math.Max(this.lastValue, 0f) * 1.01f;
						value = this.statfillFormatter.Format(v);
						this.lastValue = Mathf.Lerp(this.lastValue, b, this.deltaTime * 3f);
						return true;
					}
				}
			}
			else if (bindingName == "name")
			{
				if (this.CurrentBossGroup != null)
				{
					value = this.CurrentBossGroup.BossName;
				}
				else if (this.Target == null)
				{
					value = "";
				}
				else
				{
					EntityPlayer entityPlayer = this.Target as EntityPlayer;
					value = ((entityPlayer != null) ? entityPlayer.PlayerDisplayName : Localization.Get(EntityClass.list[this.Target.entityClass].entityClassName, false));
				}
				return true;
			}
		}
		else if (num != 3647323098U)
		{
			if (num == 3767448944U)
			{
				if (bindingName == "boss_sprite")
				{
					if (this.gameEventManager == null || this.CurrentBossGroup == null)
					{
						value = "";
						return true;
					}
					value = ((this.CurrentBossGroup.BossIcon == "") ? this.defaultBossIcon : this.CurrentBossGroup.BossIcon);
					return true;
				}
			}
		}
		else if (bindingName == "current")
		{
			if (this.Target == null)
			{
				value = "";
				return true;
			}
			value = this.statcurrentFormatterInt.Format(this.Target.Health);
			return true;
		}
		return false;
	}

	// Token: 0x06007392 RID: 29586 RVA: 0x002F1A33 File Offset: 0x002EFC33
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "visibility")
		{
			this.visibility = EnumUtils.Parse<XUiC_TargetBar.EVisibility>(_value, true);
			return true;
		}
		if (!(_name == "default_boss_icon"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.defaultBossIcon = _value;
		return true;
	}

	// Token: 0x040057F2 RID: 22514
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastValue;

	// Token: 0x040057F3 RID: 22515
	[PublicizedFrom(EAccessModifier.Private)]
	public float deltaTime;

	// Token: 0x040057F4 RID: 22516
	[PublicizedFrom(EAccessModifier.Private)]
	public float noTargetFadeTime;

	// Token: 0x040057F5 RID: 22517
	[PublicizedFrom(EAccessModifier.Private)]
	public float noTargetFadeTimeMax = 3f;

	// Token: 0x040057F6 RID: 22518
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TargetBar.EVisibility visibility;

	// Token: 0x040057F7 RID: 22519
	[PublicizedFrom(EAccessModifier.Private)]
	public string defaultBossIcon = "ui_game_symbol_twitch_boss_bar_default";

	// Token: 0x040057F8 RID: 22520
	[PublicizedFrom(EAccessModifier.Private)]
	public GameEventManager gameEventManager;

	// Token: 0x040057F9 RID: 22521
	[PublicizedFrom(EAccessModifier.Private)]
	public BossGroup CurrentBossGroup;

	// Token: 0x040057FA RID: 22522
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt statcurrentFormatterInt = new CachedStringFormatterInt();

	// Token: 0x040057FB RID: 22523
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int, int> statcurrentWMaxFormatterAOfB = new CachedStringFormatter<int, int>((int _i, int _i1) => string.Format("{0}/{1}", _i, _i1));

	// Token: 0x040057FC RID: 22524
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat statfillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x02000E5F RID: 3679
	public enum EVisibility
	{
		// Token: 0x040057FE RID: 22526
		Never,
		// Token: 0x040057FF RID: 22527
		GodMode,
		// Token: 0x04005800 RID: 22528
		Always,
		// Token: 0x04005801 RID: 22529
		Boss
	}
}

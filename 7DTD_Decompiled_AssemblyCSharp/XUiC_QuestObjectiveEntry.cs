using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DB1 RID: 3505
[Preserve]
public class XUiC_QuestObjectiveEntry : XUiController
{
	// Token: 0x17000AFC RID: 2812
	// (get) Token: 0x06006DA6 RID: 28070 RVA: 0x002CB4C0 File Offset: 0x002C96C0
	// (set) Token: 0x06006DA7 RID: 28071 RVA: 0x002CB4C8 File Offset: 0x002C96C8
	public XUiC_QuestObjectiveList Owner { get; set; }

	// Token: 0x17000AFD RID: 2813
	// (get) Token: 0x06006DA8 RID: 28072 RVA: 0x002CB4D1 File Offset: 0x002C96D1
	// (set) Token: 0x06006DA9 RID: 28073 RVA: 0x002CB4DC File Offset: 0x002C96DC
	public BaseObjective Objective
	{
		get
		{
			return this.objective;
		}
		set
		{
			if (this.objective != null)
			{
				this.objective.ValueChanged -= this.Objective_ValueChanged;
			}
			this.objective = value;
			if (this.objective != null)
			{
				this.objective.ValueChanged += this.Objective_ValueChanged;
			}
			this.isDirty = true;
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06006DAA RID: 28074 RVA: 0x002BF216 File Offset: 0x002BD416
	[PublicizedFrom(EAccessModifier.Private)]
	public void Objective_ValueChanged()
	{
		base.RefreshBindings(true);
	}

	// Token: 0x06006DAB RID: 28075 RVA: 0x002CB53C File Offset: 0x002C973C
	public void SetIsTracker()
	{
		this.isTracker = true;
		this.completeColor = Utils.ColorToHex(StringParsers.ParseColor(this.completeColor));
		this.incompleteColor = Utils.ColorToHex(StringParsers.ParseColor(this.incompleteColor));
	}

	// Token: 0x17000AFE RID: 2814
	// (get) Token: 0x06006DAC RID: 28076 RVA: 0x002CB57B File Offset: 0x002C977B
	public static string OptionalKeyword
	{
		get
		{
			if (XUiC_QuestObjectiveEntry.optionalKeyword == "")
			{
				XUiC_QuestObjectiveEntry.optionalKeyword = Localization.Get("optional", false);
			}
			return XUiC_QuestObjectiveEntry.optionalKeyword;
		}
	}

	// Token: 0x06006DAD RID: 28077 RVA: 0x002CB5A4 File Offset: 0x002C97A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.objective != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 3046379438U)
		{
			if (num <= 1556015422U)
			{
				if (num != 612643325U)
				{
					if (num == 1556015422U)
					{
						if (bindingName == "hasobjective")
						{
							value = flag.ToString();
							return true;
						}
					}
				}
				else if (bindingName == "objectivestate")
				{
					value = (flag ? this.objective.StatusText : "");
					return true;
				}
			}
			else if (num != 1883252318U)
			{
				if (num == 3046379438U)
				{
					if (bindingName == "objectivecompletecolor")
					{
						if (this.objective != null)
						{
							if (this.objective.OwnerQuest.CurrentState == Quest.QuestState.Completed)
							{
								value = this.completeColor;
								return true;
							}
							switch (this.objective.ObjectiveState)
							{
							case BaseObjective.ObjectiveStates.NotStarted:
								value = this.originalColor;
								break;
							case BaseObjective.ObjectiveStates.InProgress:
								value = this.incompleteColor;
								break;
							case BaseObjective.ObjectiveStates.Warning:
								value = this.warningColor;
								break;
							case BaseObjective.ObjectiveStates.Complete:
								value = this.completeColor;
								break;
							case BaseObjective.ObjectiveStates.Failed:
								value = this.incompleteColor;
								break;
							}
						}
						else
						{
							value = this.completeColor;
						}
						return true;
					}
				}
			}
			else if (bindingName == "objectivephasecolor")
			{
				if (this.objective != null)
				{
					if (this.objective.OwnerQuest.CurrentState == Quest.QuestState.NotStarted)
					{
						value = this.originalColor;
					}
					else
					{
						value = (((this.objective.Phase == 0 || this.objective.Phase == this.objective.OwnerQuest.CurrentPhase) && this.objective.OwnerQuest.CurrentState == Quest.QuestState.InProgress) ? this.originalColor : this.inactiveColor);
					}
				}
				else
				{
					value = "FFFFFF";
				}
				return true;
			}
		}
		else if (num <= 3186090687U)
		{
			if (num != 3091161906U)
			{
				if (num == 3186090687U)
				{
					if (bindingName == "objectivephasehexcolor")
					{
						if (this.objective != null)
						{
							if (this.objective.OwnerQuest.CurrentState == Quest.QuestState.NotStarted)
							{
								value = this.Owner.activeHexColor;
							}
							else
							{
								value = ((this.objective.Phase == this.objective.OwnerQuest.CurrentPhase) ? this.Owner.activeHexColor : this.Owner.inactiveHexColor);
							}
						}
						else
						{
							value = "FFFFFF";
						}
						return true;
					}
				}
			}
			else if (bindingName == "objectivedescription")
			{
				value = (flag ? this.objective.Description : "");
				return true;
			}
		}
		else if (num != 3220755286U)
		{
			if (num == 3422073972U)
			{
				if (bindingName == "objectiveoptional")
				{
					value = (flag ? (this.objective.Optional ? this.objectiveOptionalFormatter.Format(XUiC_QuestObjectiveEntry.OptionalKeyword) : "") : "");
					return true;
				}
			}
		}
		else if (bindingName == "objectivecompletesprite")
		{
			if (this.objective != null)
			{
				Quest ownerQuest = this.objective.OwnerQuest;
				if (ownerQuest.CurrentState == Quest.QuestState.Completed)
				{
					value = this.completeIconName;
					return true;
				}
				if (ownerQuest.CurrentState == Quest.QuestState.NotStarted || this.objective.ObjectiveState == BaseObjective.ObjectiveStates.NotStarted)
				{
					value = this.notstartedIconName;
				}
				else
				{
					value = (this.objective.Complete ? this.completeIconName : this.incompleteIconName);
				}
			}
			else
			{
				value = "";
			}
			return true;
		}
		return false;
	}

	// Token: 0x06006DAE RID: 28078 RVA: 0x002CB930 File Offset: 0x002C9B30
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnCountChanged(XUiController _sender, OnCountChangedEventArgs _e)
	{
		this.isDirty = true;
	}

	// Token: 0x06006DAF RID: 28079 RVA: 0x002CB93C File Offset: 0x002C9B3C
	public override void Update(float _dt)
	{
		if (this.objective != null && this.objective.OwnerQuest.CurrentState == Quest.QuestState.InProgress && this.objective.UpdateUI && Time.time > this.updateTime)
		{
			this.updateTime = Time.time + 0.1f;
			base.RefreshBindings(this.isDirty);
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x06006DB0 RID: 28080 RVA: 0x002CB9AC File Offset: 0x002C9BAC
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		if (num <= 2036604981U)
		{
			if (num <= 466249841U)
			{
				if (num != 215154731U)
				{
					if (num == 466249841U)
					{
						if (name == "incomplete_color")
						{
							this.incompleteColor = value;
							return true;
						}
					}
				}
				else if (name == "incomplete_icon")
				{
					this.incompleteIconName = value;
					return true;
				}
			}
			else if (num != 1516116007U)
			{
				if (num == 2036604981U)
				{
					if (name == "notstarted_icon")
					{
						this.notstartedIconName = value;
						return true;
					}
				}
			}
			else if (name == "warning_color")
			{
				this.warningColor = value;
				return true;
			}
		}
		else if (num <= 3169435800U)
		{
			if (num != 2911778486U)
			{
				if (num == 3169435800U)
				{
					if (name == "inactive_color")
					{
						this.inactiveColor = value;
						return true;
					}
				}
			}
			else if (name == "complete_color")
			{
				this.completeColor = value;
				return true;
			}
		}
		else if (num != 3495370150U)
		{
			if (num == 4270887654U)
			{
				if (name == "complete_icon")
				{
					this.completeIconName = value;
					return true;
				}
			}
		}
		else if (name == "original_color")
		{
			this.originalColor = value;
			return true;
		}
		return base.ParseAttribute(name, value, _parent);
	}

	// Token: 0x0400533B RID: 21307
	[PublicizedFrom(EAccessModifier.Private)]
	public string notstartedIconName = "";

	// Token: 0x0400533C RID: 21308
	[PublicizedFrom(EAccessModifier.Private)]
	public string completeIconName = "";

	// Token: 0x0400533D RID: 21309
	[PublicizedFrom(EAccessModifier.Private)]
	public string incompleteIconName = "";

	// Token: 0x0400533E RID: 21310
	[PublicizedFrom(EAccessModifier.Private)]
	public string completeColor = "0,255,0,255";

	// Token: 0x0400533F RID: 21311
	[PublicizedFrom(EAccessModifier.Private)]
	public string incompleteColor = "255,0,0,255";

	// Token: 0x04005340 RID: 21312
	[PublicizedFrom(EAccessModifier.Private)]
	public string warningColor = "255,255,0,255";

	// Token: 0x04005341 RID: 21313
	[PublicizedFrom(EAccessModifier.Private)]
	public string inactiveColor = "160,160,160,255";

	// Token: 0x04005342 RID: 21314
	[PublicizedFrom(EAccessModifier.Private)]
	public string originalColor = "255,255,255,255";

	// Token: 0x04005343 RID: 21315
	[PublicizedFrom(EAccessModifier.Private)]
	public BaseObjective objective;

	// Token: 0x04005344 RID: 21316
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04005345 RID: 21317
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isTracker;

	// Token: 0x04005346 RID: 21318
	[PublicizedFrom(EAccessModifier.Private)]
	public static string optionalKeyword = "";

	// Token: 0x04005348 RID: 21320
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string> objectiveOptionalFormatter = new CachedStringFormatter<string>((string _s) => "(" + _s + ") ");

	// Token: 0x04005349 RID: 21321
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateTime;
}

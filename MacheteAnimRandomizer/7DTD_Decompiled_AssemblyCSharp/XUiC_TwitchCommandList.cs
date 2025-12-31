using System;
using System.Collections.Generic;
using Twitch;
using UniLinq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E84 RID: 3716
[Preserve]
public class XUiC_TwitchCommandList : XUiController
{
	// Token: 0x17000BED RID: 3053
	// (get) Token: 0x06007505 RID: 29957 RVA: 0x002F9323 File Offset: 0x002F7523
	// (set) Token: 0x06007506 RID: 29958 RVA: 0x002F932B File Offset: 0x002F752B
	public XUiC_TwitchWindow Owner { get; set; }

	// Token: 0x06007507 RID: 29959 RVA: 0x002F9334 File Offset: 0x002F7534
	public float GetHeight()
	{
		if (!this.twitchManager.IsReady || this.twitchManager.VotingManager.VotingIsActive)
		{
			return 0f;
		}
		if (this.commandLists.ContainsKey(this.CurrentKey))
		{
			return (float)(this.commandLists[this.CurrentKey].Count * 30);
		}
		return 0f;
	}

	// Token: 0x06007508 RID: 29960 RVA: 0x002F939C File Offset: 0x002F759C
	public override void Init()
	{
		base.Init();
		XUiC_TwitchCommandEntry[] childrenByType = base.GetChildrenByType<XUiC_TwitchCommandEntry>(null);
		for (int i = 0; i < childrenByType.Length; i++)
		{
			if (childrenByType[i] != null)
			{
				this.commandEntries.Add(childrenByType[i]);
			}
		}
		this.twitchManager = TwitchManager.Current;
		this.twitchManager.CommandsChanged -= this.TwitchManager_CommandsChanged;
		this.twitchManager.CommandsChanged += this.TwitchManager_CommandsChanged;
	}

	// Token: 0x06007509 RID: 29961 RVA: 0x002F9411 File Offset: 0x002F7611
	[PublicizedFrom(EAccessModifier.Private)]
	public void TwitchManager_CommandsChanged()
	{
		this.SetupCommandList();
		this.lastUpdate = 0f;
		this.commandListIndex = -1;
	}

	// Token: 0x0600750A RID: 29962 RVA: 0x002F942C File Offset: 0x002F762C
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetPrevCategory()
	{
		bool flag = false;
		int num = 0;
		while (!flag)
		{
			this.commandListIndex--;
			if (this.commandListIndex < 0)
			{
				this.commandListIndex = this.commandGroupList.Count - 1;
			}
			if (this.commandLists.ContainsKey(this.commandGroupList[this.commandListIndex].groupName))
			{
				flag = true;
			}
			num++;
			if (num > this.commandGroupList.Count)
			{
				break;
			}
		}
		this.ResetKey();
	}

	// Token: 0x0600750B RID: 29963 RVA: 0x002F94AC File Offset: 0x002F76AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetNextCategory()
	{
		bool flag = false;
		int num = 0;
		while (!flag)
		{
			this.commandListIndex++;
			if (this.commandListIndex >= this.commandGroupList.Count)
			{
				this.commandListIndex = 0;
			}
			if (this.commandGroupList.Count == 0 || this.commandLists.ContainsKey(this.commandGroupList[this.commandListIndex].groupName))
			{
				flag = true;
			}
			num++;
			if (num > this.commandGroupList.Count)
			{
				break;
			}
		}
		this.ResetKey();
	}

	// Token: 0x0600750C RID: 29964 RVA: 0x002F9534 File Offset: 0x002F7734
	public override void Update(float _dt)
	{
		if (!this.twitchManager.IsReady)
		{
			return;
		}
		if (Time.time - this.lastUpdate >= this.secondRotation)
		{
			this.isDirty = true;
			if (this.commandLists.Count > 0)
			{
				this.GetNextCategory();
			}
			this.lastUpdate = Time.time;
		}
		if (this.isDirty)
		{
			if (this.commandLists.Count == 0)
			{
				this.SetupCommandList();
			}
			if (this.commandLists.Count != 0 && this.commandLists.ContainsKey(this.CurrentKey))
			{
				TwitchAction[] array = (from a in this.commandLists[this.CurrentKey]
				orderby a.Command
				orderby a.PointType
				select a).ToArray<TwitchAction>();
				int num = 0;
				int num2 = 0;
				while (num2 < array.Length && num < this.commandEntries.Count)
				{
					if (this.commandEntries[num] != null)
					{
						this.commandEntries[num].Owner = this.Owner;
						this.commandEntries[num].Action = array[num2];
						num++;
					}
					num2++;
				}
				for (int i = num; i < this.commandEntries.Count; i++)
				{
					this.commandEntries[i].Action = null;
				}
				this.isDirty = false;
			}
		}
		base.Update(_dt);
	}

	// Token: 0x0600750D RID: 29965 RVA: 0x002F96BD File Offset: 0x002F78BD
	public void MoveForward()
	{
		this.GetNextCategory();
		this.lastUpdate = Time.time - 2f;
		this.isDirty = true;
	}

	// Token: 0x0600750E RID: 29966 RVA: 0x002F96DD File Offset: 0x002F78DD
	public void MoveBackward()
	{
		this.GetPrevCategory();
		this.lastUpdate = Time.time - 2f;
		this.isDirty = true;
	}

	// Token: 0x0600750F RID: 29967 RVA: 0x002F9700 File Offset: 0x002F7900
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchActionGroup AddCommandGroup(string groupName)
	{
		for (int i = 0; i < this.commandGroupList.Count; i++)
		{
			if (this.commandGroupList[i].groupName == groupName)
			{
				return this.commandGroupList[i];
			}
		}
		int categoryIndex = TwitchActionManager.Current.GetCategoryIndex(groupName);
		this.commandGroupList.Add(new TwitchActionGroup
		{
			ActionList = new List<TwitchAction>(),
			groupName = groupName,
			displayName = TwitchActionManager.Current.CategoryList[categoryIndex].DisplayName,
			index = categoryIndex
		});
		return this.commandGroupList[this.commandGroupList.Count - 1];
	}

	// Token: 0x06007510 RID: 29968 RVA: 0x002F97B4 File Offset: 0x002F79B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void RemoveCommandGroup(string groupName)
	{
		for (int i = 0; i < this.commandGroupList.Count; i++)
		{
			if (this.commandGroupList[i].groupName == groupName)
			{
				this.commandGroupList.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x06007511 RID: 29969 RVA: 0x002F9800 File Offset: 0x002F7A00
	public void SetupCommandList()
	{
		this.commandLists.Clear();
		string name = TwitchActionManager.Current.CategoryList[0].Name;
		int num = 0;
		foreach (string key in this.twitchManager.AvailableCommands.Keys)
		{
			TwitchAction twitchAction = this.twitchManager.AvailableCommands[key];
			if (!twitchAction.OnCooldown && twitchAction.OnlyUsableByType == TwitchAction.OnlyUsableTypes.Everyone)
			{
				num++;
			}
			if (num > 10)
			{
				break;
			}
		}
		bool flag = num <= 10;
		if (flag)
		{
			this.commandLists.Add(name, new List<TwitchAction>());
		}
		foreach (string key2 in this.twitchManager.AvailableCommands.Keys)
		{
			TwitchAction twitchAction2 = this.twitchManager.AvailableCommands[key2];
			if (twitchAction2.PointType != TwitchAction.PointTypes.Bits || !(this.twitchManager.BroadcasterType == ""))
			{
				if (flag)
				{
					if (!twitchAction2.OnCooldown && twitchAction2.OnlyUsableByType == TwitchAction.OnlyUsableTypes.Everyone)
					{
						twitchAction2.groupIndex = 0;
						this.commandLists[name].Add(twitchAction2);
						this.AddCommandGroup(name).ActionList.Add(twitchAction2);
					}
				}
				else if (twitchAction2.HasExtraConditions())
				{
					string text = twitchAction2.CategoryNames[0];
					if (!this.commandLists.ContainsKey(text))
					{
						this.commandLists.Add(text, new List<TwitchAction>());
					}
					this.AddCommandGroup(text).ActionList.Add(twitchAction2);
					twitchAction2.groupIndex = 0;
					this.commandLists[text].Add(twitchAction2);
				}
			}
		}
		this.commandListIndex = 0;
		this.lastUpdate = Time.time;
		if (!flag)
		{
			bool flag2 = true;
			while (flag2)
			{
				flag2 = false;
				foreach (string text2 in this.commandLists.Keys)
				{
					bool flag3 = false;
					if (this.commandLists[text2].Count > 10)
					{
						List<TwitchAction> list = this.commandLists[text2];
						this.commandLists.Remove(text2);
						this.RemoveCommandGroup(text2);
						for (int i = 0; i < list.Count; i++)
						{
							TwitchAction twitchAction3 = list[i];
							string text3 = twitchAction3.CategoryNames[twitchAction3.groupIndex];
							if (twitchAction3.CategoryNames.Count > twitchAction3.groupIndex + 1)
							{
								twitchAction3.groupIndex++;
								text3 = twitchAction3.CategoryNames[twitchAction3.groupIndex];
								flag3 = true;
							}
							if (!this.commandLists.ContainsKey(text3))
							{
								this.commandLists.Add(text3, new List<TwitchAction>());
							}
							this.AddCommandGroup(text3).ActionList.Add(twitchAction3);
							this.commandLists[text3].Add(twitchAction3);
						}
						if (flag3)
						{
							flag2 = true;
							break;
						}
						this.commandLists.Remove(text2);
						this.RemoveCommandGroup(text2);
						for (int j = 0; j < list.Count; j++)
						{
							TwitchAction twitchAction4 = list[j];
							int num2 = j / 10 + 1;
							string text4 = twitchAction4.CategoryNames[twitchAction4.groupIndex] + num2.ToString();
							if (!this.commandLists.ContainsKey(text4))
							{
								this.commandLists.Add(text4, new List<TwitchAction>());
							}
							this.AddCommandGroup(text4).ActionList.Add(twitchAction4);
							this.commandLists[text4].Add(twitchAction4);
						}
						flag2 = true;
						break;
					}
				}
			}
		}
		this.commandGroupList = (from x in this.commandGroupList
		orderby x.index, x.groupName
		select x).ToList<TwitchActionGroup>();
		this.ResetKey();
	}

	// Token: 0x06007512 RID: 29970 RVA: 0x002F9CBC File Offset: 0x002F7EBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetKey()
	{
		if (this.commandLists.Count <= 1)
		{
			this.CurrentTitle = (this.CurrentKey = Localization.Get("TwitchActionCategory_Commands", false));
			return;
		}
		this.CurrentKey = this.commandGroupList[this.commandListIndex].groupName;
		this.CurrentTitle = this.commandGroupList[this.commandListIndex].displayName;
	}

	// Token: 0x06007513 RID: 29971 RVA: 0x002F9D2A File Offset: 0x002F7F2A
	public override void OnOpen()
	{
		base.OnOpen();
		this.isDirty = true;
		this.twitchManager = TwitchManager.Current;
	}

	// Token: 0x06007514 RID: 29972 RVA: 0x002F9D44 File Offset: 0x002F7F44
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "complete_icon")
		{
			this.completeIconName = value;
			return true;
		}
		if (name == "incomplete_icon")
		{
			this.incompleteIconName = value;
			return true;
		}
		if (name == "complete_color")
		{
			Color32 color = StringParsers.ParseColor(value);
			this.completeColor = string.Format("{0},{1},{2},{3}", new object[]
			{
				color.r,
				color.g,
				color.b,
				color.a
			});
			this.completeHexColor = Utils.ColorToHex(color);
			return true;
		}
		if (!(name == "incomplete_color"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		Color32 color2 = StringParsers.ParseColor(value);
		this.incompleteColor = string.Format("{0},{1},{2},{3}", new object[]
		{
			color2.r,
			color2.g,
			color2.b,
			color2.a
		});
		this.incompleteHexColor = Utils.ColorToHex(color2);
		return true;
	}

	// Token: 0x04005903 RID: 22787
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_TwitchCommandEntry> commandEntries = new List<XUiC_TwitchCommandEntry>();

	// Token: 0x04005904 RID: 22788
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04005905 RID: 22789
	public string completeIconName = "";

	// Token: 0x04005906 RID: 22790
	public string incompleteIconName = "";

	// Token: 0x04005907 RID: 22791
	public string completeHexColor = "FF00FF00";

	// Token: 0x04005908 RID: 22792
	public string incompleteHexColor = "FFB400";

	// Token: 0x04005909 RID: 22793
	public string warningHexColor = "FFFF00FF";

	// Token: 0x0400590A RID: 22794
	public string inactiveHexColor = "888888FF";

	// Token: 0x0400590B RID: 22795
	public string activeHexColor = "FFFFFFFF";

	// Token: 0x0400590C RID: 22796
	public string completeColor = "0,255,0,255";

	// Token: 0x0400590D RID: 22797
	public string incompleteColor = "255, 180, 0, 255";

	// Token: 0x0400590E RID: 22798
	public string warningColor = "255,255,0,255";

	// Token: 0x0400590F RID: 22799
	public Dictionary<string, List<TwitchAction>> commandLists = new Dictionary<string, List<TwitchAction>>();

	// Token: 0x04005910 RID: 22800
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TwitchActionGroup> commandGroupList = new List<TwitchActionGroup>();

	// Token: 0x04005911 RID: 22801
	[PublicizedFrom(EAccessModifier.Private)]
	public int commandListIndex = -1;

	// Token: 0x04005912 RID: 22802
	public string CurrentKey = "";

	// Token: 0x04005913 RID: 22803
	public string CurrentTitle = "";

	// Token: 0x04005914 RID: 22804
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastUpdate;

	// Token: 0x04005915 RID: 22805
	[PublicizedFrom(EAccessModifier.Private)]
	public float secondRotation = 10f;

	// Token: 0x04005916 RID: 22806
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchManager twitchManager;
}

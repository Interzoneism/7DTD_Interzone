using System;
using UnityEngine.Scripting;

// Token: 0x02000DC5 RID: 3525
[Preserve]
public class XUiC_QuestTurnInDetailsWindow : XUiController
{
	// Token: 0x17000B1B RID: 2843
	// (get) Token: 0x06006E52 RID: 28242 RVA: 0x002CF9E0 File Offset: 0x002CDBE0
	// (set) Token: 0x06006E53 RID: 28243 RVA: 0x002CF9E8 File Offset: 0x002CDBE8
	public Quest CurrentQuest
	{
		get
		{
			return this.currentQuest;
		}
		set
		{
			this.currentQuest = value;
			this.questClass = ((value != null) ? QuestClass.GetQuest(this.currentQuest.ID) : null);
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06006E54 RID: 28244 RVA: 0x002CFA14 File Offset: 0x002CDC14
	public override void Init()
	{
		base.Init();
		this.xuiQuestDescriptionLabel = Localization.Get("xuiDescriptionLabel", false);
	}

	// Token: 0x06006E55 RID: 28245 RVA: 0x002CFA2D File Offset: 0x002CDC2D
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
		this.CurrentQuest = base.xui.Dialog.QuestTurnIn;
	}

	// Token: 0x06006E56 RID: 28246 RVA: 0x002CFA54 File Offset: 0x002CDC54
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2940790865U)
		{
			if (num <= 1468586257U)
			{
				if (num != 161079706U)
				{
					if (num == 1468586257U)
					{
						if (bindingName == "questsubtitle")
						{
							value = ((this.currentQuest != null) ? this.questClass.SubTitle : "");
							return true;
						}
					}
				}
				else if (bindingName == "sharedbyname")
				{
					if (this.currentQuest == null)
					{
						value = "";
					}
					else
					{
						PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.currentQuest.SharedOwnerID);
						if (playerDataFromEntityID != null)
						{
							value = GameUtils.SafeStringFormat(playerDataFromEntityID.PlayerName.DisplayName);
						}
						else
						{
							value = "";
						}
					}
					return true;
				}
			}
			else if (num != 1985695849U)
			{
				if (num == 2940790865U)
				{
					if (bindingName == "questdescription")
					{
						value = ((this.currentQuest != null) ? this.currentQuest.GetParsedText(this.questClass.Description) : "");
						return true;
					}
				}
			}
			else if (bindingName == "questcategory")
			{
				value = ((this.currentQuest != null) ? this.questClass.Category : "");
				return true;
			}
		}
		else if (num <= 3270262403U)
		{
			if (num != 3047389681U)
			{
				if (num == 3270262403U)
				{
					if (bindingName == "npcportrait")
					{
						if (this.currentQuest == null)
						{
							value = "";
						}
						else
						{
							value = this.NPC.NPCInfo.Portrait;
						}
						return true;
					}
				}
			}
			else if (bindingName == "questtitle")
			{
				value = ((this.currentQuest != null) ? this.questtitleFormatter.Format(this.questClass.Category, this.questClass.SubTitle) : this.xuiQuestDescriptionLabel);
				return true;
			}
		}
		else if (num != 3357817217U)
		{
			if (num != 3846167827U)
			{
				if (num == 4060322893U)
				{
					if (bindingName == "showempty")
					{
						value = (this.currentQuest == null).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "npcname")
			{
				if (this.currentQuest == null)
				{
					value = "";
				}
				else
				{
					value = Localization.Get(this.NPC.EntityName, false);
				}
				return true;
			}
		}
		else if (bindingName == "questcompletetext")
		{
			value = ((this.currentQuest != null) ? this.questClass.CompleteText : "Needs real complete text.");
			return true;
		}
		return false;
	}

	// Token: 0x040053D4 RID: 21460
	[PublicizedFrom(EAccessModifier.Private)]
	public QuestClass questClass;

	// Token: 0x040053D5 RID: 21461
	[PublicizedFrom(EAccessModifier.Private)]
	public string xuiQuestDescriptionLabel;

	// Token: 0x040053D6 RID: 21462
	public EntityNPC NPC;

	// Token: 0x040053D7 RID: 21463
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest currentQuest;

	// Token: 0x040053D8 RID: 21464
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, string> questtitleFormatter = new CachedStringFormatter<string, string>((string _s, string _s1) => string.Format("{0} : {1}", _s, _s1));
}

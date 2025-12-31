using System;
using System.Collections.Generic;
using Challenges;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001BF RID: 447
[Preserve]
public class ConsoleCmdChallenges : ConsoleCmdAbstract
{
	// Token: 0x06000D9B RID: 3483 RVA: 0x0005ADDE File Offset: 0x00058FDE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"challenges"
		};
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x0005ADEE File Offset: 0x00058FEE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Complete certain challenges";
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x0005ADF8 File Offset: 0x00058FF8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return string.Concat(new string[]
		{
			"\n\t\t\t|Usage:\n\t\t\t|  1. ",
			this.PrimaryCommand,
			" list [group name]\n\t\t\t|  2. ",
			this.PrimaryCommand,
			" complete all [\"redeem\"/\"r\"]\n\t\t\t|  3. ",
			this.PrimaryCommand,
			" complete first [\"redeem\"/\"r\"]\n\t\t\t|  4. ",
			this.PrimaryCommand,
			" complete challenge <challenge name> [\"redeem\"/\"r\"]\n\t\t\t|  5. ",
			this.PrimaryCommand,
			" complete group <group name> [\"redeem\"/\"r\"]\n\t\t\t|  6. ",
			this.PrimaryCommand,
			" complete category <category name> [\"redeem\"/\"r\"]\n\t\t\t|  7. ",
			this.PrimaryCommand,
			" groups [category name]\n\t\t\t|Short forms: \"list\"->\"l\", \"complete\"->\"c\", \"groups\"->\"g\".\n\t\t\t|1. List challenges - optionally limited to a specific group.\n\t\t\t|2. Set all challenges to completed.\n\t\t\t|3. Set challenges of the first defined group to completed.\n\t\t\t|4. Set given challenge to completed.\n\t\t\t|5. Set all challenges in the given group to completed.\n\t\t\t|6. Set all challenges in the given category to completed.\n\t\t\t|2.-6. If the optional \"redeem\" or \"r\" is specified it will automatically redeem the completed challenges.\n\t\t\t|7. List all groups - optionally limited to a specific category.\n\t\t\t"
		}).Unindent(true);
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000D9E RID: 3486 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000D9F RID: 3487 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x0005AE9C File Offset: 0x0005909C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.IsDedicatedServer)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Cannot execute " + this.PrimaryCommand + " on dedicated server, please execute as a client");
			return;
		}
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Missing arguments (see help)");
			return;
		}
		string text = _params[0];
		if (text.EqualsCaseInsensitive("list") || text.EqualsCaseInsensitive("l"))
		{
			this.executeList(_params);
			return;
		}
		if (text.EqualsCaseInsensitive("complete") || text.EqualsCaseInsensitive("c"))
		{
			this.executeComplete(_params);
			return;
		}
		if (text.EqualsCaseInsensitive("groups") || text.EqualsCaseInsensitive("g"))
		{
			this.executeListGroups(_params);
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown subcommand '" + text + "'");
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x0005AF74 File Offset: 0x00059174
	[PublicizedFrom(EAccessModifier.Private)]
	public void executeListGroups(List<string> _params)
	{
		Func<ChallengeGroup, bool> func = null;
		if (_params.Count > 1)
		{
			string filterValue = _params[1];
			if (!string.IsNullOrEmpty(filterValue))
			{
				func = ((ChallengeGroup _group) => _group.Category.EqualsCaseInsensitive(filterValue));
			}
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Challenge groups:");
		foreach (KeyValuePair<string, ChallengeGroup> keyValuePair in ChallengeGroup.s_ChallengeGroups)
		{
			string text;
			ChallengeGroup challengeGroup;
			keyValuePair.Deconstruct(out text, out challengeGroup);
			ChallengeGroup challengeGroup2 = challengeGroup;
			if (func == null || func(challengeGroup2))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
				{
					"  ",
					challengeGroup2.Name,
					" (category: ",
					challengeGroup2.Category,
					")"
				}));
			}
		}
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x0005B060 File Offset: 0x00059260
	[PublicizedFrom(EAccessModifier.Private)]
	public void executeList(List<string> _params)
	{
		Func<ChallengeClass, bool> func = null;
		if (_params.Count > 1)
		{
			string filterValue = _params[1];
			if (!string.IsNullOrEmpty(filterValue))
			{
				func = ((ChallengeClass _class) => _class.ChallengeGroup.Name.EqualsCaseInsensitive(filterValue));
			}
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Challenges:");
		foreach (KeyValuePair<string, ChallengeClass> keyValuePair in ChallengeClass.s_Challenges)
		{
			string text;
			ChallengeClass challengeClass;
			keyValuePair.Deconstruct(out text, out challengeClass);
			ChallengeClass challengeClass2 = challengeClass;
			if (func == null || func(challengeClass2))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
				{
					"  ",
					challengeClass2.Name,
					" (group: ",
					challengeClass2.ChallengeGroup.Name,
					", category: ",
					challengeClass2.ChallengeGroup.Category,
					")"
				}));
			}
		}
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x0005B16C File Offset: 0x0005936C
	[PublicizedFrom(EAccessModifier.Private)]
	public void executeComplete(List<string> _params)
	{
		if (_params.Count < 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Subcommand 'complete' expects at least one further argument");
			return;
		}
		string text = _params[1];
		string name = (_params.Count > 2) ? _params[2] : null;
		Func<ChallengeClass, bool> func;
		if (text.EqualsCaseInsensitive("all"))
		{
			func = null;
		}
		else
		{
			if (text.EqualsCaseInsensitive("first"))
			{
				func = null;
				using (Dictionary<string, ChallengeGroup>.Enumerator enumerator = ChallengeGroup.s_ChallengeGroups.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						goto IL_221;
					}
					KeyValuePair<string, ChallengeGroup> keyValuePair = enumerator.Current;
					string text2;
					ChallengeGroup group2;
					keyValuePair.Deconstruct(out text2, out group2);
					ChallengeGroup group = group2;
					func = ((ChallengeClass _class) => _class.ChallengeGroup.Name.Equals(group.Name));
					goto IL_221;
				}
			}
			if (text.EqualsCaseInsensitive("challenge"))
			{
				if (string.IsNullOrEmpty(name))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Subcommand 'complete' with argument 'challenge': Expects challenge name");
					return;
				}
				if (!ChallengeClass.s_Challenges.ContainsKey(name))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Challenge '" + name + "' does not exist");
					return;
				}
				func = ((ChallengeClass _class) => _class.Name.EqualsCaseInsensitive(name));
			}
			else if (text.EqualsCaseInsensitive("group"))
			{
				if (string.IsNullOrEmpty(name))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Subcommand 'complete' with argument 'group': Expects group name");
					return;
				}
				if (!ChallengeGroup.s_ChallengeGroups.ContainsKey(name))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Group '" + name + "' does not exist");
					return;
				}
				func = ((ChallengeClass _class) => _class.ChallengeGroup.Name.EqualsCaseInsensitive(name));
			}
			else
			{
				if (!text.EqualsCaseInsensitive("category"))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Subcommand 'complete': Invalid argument '" + text + "'");
					return;
				}
				if (string.IsNullOrEmpty(name))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Subcommand 'complete' with argument 'category': Expects category name");
					return;
				}
				if (!ChallengeCategory.s_ChallengeCategories.ContainsKey(name))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Category '" + name + "' does not exist");
					return;
				}
				func = ((ChallengeClass _class) => _class.ChallengeGroup.Category.EqualsCaseInsensitive(name));
			}
		}
		IL_221:
		string a = _params[_params.Count - 1];
		bool flag = a.EqualsCaseInsensitive("redeem") || a.EqualsCaseInsensitive("r");
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Marking challenges as completed:");
		foreach (Challenge challenge in GameManager.Instance.World.GetPrimaryPlayer().challengeJournal.Challenges)
		{
			ChallengeClass challengeClass = challenge.ChallengeClass;
			if (func == null || func(challengeClass))
			{
				if (!challenge.IsActive && (!flag || challenge.ChallengeState != Challenge.ChallengeStates.Completed))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
					{
						"  ",
						challengeClass.Name,
						" already complete (group: ",
						challengeClass.ChallengeGroup.Name,
						", category: ",
						challengeClass.ChallengeGroup.Category,
						")"
					}));
				}
				else
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
					{
						"  ",
						challengeClass.Name,
						" (group: ",
						challengeClass.ChallengeGroup.Name,
						", category: ",
						challengeClass.ChallengeGroup.Category,
						")"
					}));
					challenge.CompleteChallenge(flag);
				}
			}
		}
	}
}

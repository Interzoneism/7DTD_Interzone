using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000205 RID: 517
[Preserve]
public class ConsoleCmdLoot : ConsoleCmdAbstract
{
	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000F48 RID: 3912 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000F49 RID: 3913 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000F4A RID: 3914 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x00063DFC File Offset: 0x00061FFC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"loot"
		};
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x00063E0C File Offset: 0x0006200C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Loot commands";
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x00063E13 File Offset: 0x00062013
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Loot commands:\ncontainer [name] <count> <stage> <abundance> - list loot from named container for count times";
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x00063E1C File Offset: 0x0006201C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		string text = _params[0].ToLower();
		if (text == "c" || text == "container")
		{
			if (_params.Count >= 2)
			{
				if (!LootContainer.IsLoaded())
				{
					WorldStaticData.InitSync(true, false, false);
				}
				int count = 1;
				if (_params.Count >= 3)
				{
					int.TryParse(_params[2], out count);
				}
				int stage = 1;
				if (_params.Count >= 4)
				{
					int.TryParse(_params[3], out stage);
				}
				float abundance = 1f;
				if (_params.Count >= 5)
				{
					float.TryParse(_params[4], out abundance);
				}
				this.ContainerList(_params[1], count, stage, abundance);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown command " + text);
		}
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x00063EFC File Offset: 0x000620FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ContainerList(string _name, int _count, int _stage, float _abundance)
	{
		LootContainer lootContainer = LootContainer.GetLootContainer(_name, false);
		if (lootContainer != null)
		{
			GameRandom gameRandom = new GameRandom();
			gameRandom.SetSeed(0);
			int num = 0;
			List<ItemStack> list = new List<ItemStack>();
			for (int i = 0; i < _count; i++)
			{
				int num2 = this.CountItems(list);
				int num3 = 999999;
				LootContainer.SpawnLootItemsFromList(gameRandom, lootContainer.itemsToSpawn, 1, _abundance, list, ref num3, (float)_stage, 0f, lootContainer.lootQualityTemplate, null, FastTags<TagGroup.Global>.none, false, false, true, null);
				if (num2 == this.CountItems(list))
				{
					num++;
				}
			}
			list.Sort(delegate(ItemStack a, ItemStack b)
			{
				int num4 = b.count.CompareTo(a.count);
				if (num4 == 0)
				{
					num4 = a.itemValue.ItemClass.Name.CompareTo(b.itemValue.ItemClass.Name);
					if (num4 == 0)
					{
						num4 = a.itemValue.Quality.CompareTo(b.itemValue.Quality);
					}
				}
				return num4;
			});
			for (int j = list.Count - 1; j > 0; j--)
			{
				ItemStack itemStack = list[j];
				ItemStack itemStack2 = list[j - 1];
				if (itemStack.itemValue.type == itemStack2.itemValue.type && itemStack.itemValue.Quality == itemStack2.itemValue.Quality)
				{
					itemStack2.count += itemStack.count;
					list.RemoveAt(j);
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				ItemStack itemStack3 = list[k];
				this.Print("#{0} {1}, q{2}, count {3}", new object[]
				{
					k,
					itemStack3.itemValue.ItemClass.GetItemName(),
					itemStack3.itemValue.Quality,
					itemStack3.count
				});
			}
			this.Print("Loot Container {0}, unique items {1}, empties {2}", new object[]
			{
				lootContainer.Name,
				list.Count,
				num
			});
			return;
		}
		this.Print("Unknown container " + _name, Array.Empty<object>());
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x000640E0 File Offset: 0x000622E0
	[PublicizedFrom(EAccessModifier.Private)]
	public int CountItems(List<ItemStack> _list)
	{
		int num = 0;
		for (int i = 0; i < _list.Count; i++)
		{
			ItemStack itemStack = _list[i];
			num += itemStack.count;
		}
		return num;
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x00064114 File Offset: 0x00062314
	[PublicizedFrom(EAccessModifier.Private)]
	public void Print(string _s, params object[] _values)
	{
		string line = string.Format(_s, _values);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(line);
	}
}

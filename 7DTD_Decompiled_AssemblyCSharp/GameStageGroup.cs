using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x02000989 RID: 2441
public sealed class GameStageGroup
{
	// Token: 0x06004982 RID: 18818 RVA: 0x001D0E84 File Offset: 0x001CF084
	public GameStageGroup(GameStageDefinition _spawner)
	{
		this.spawner = _spawner;
	}

	// Token: 0x06004983 RID: 18819 RVA: 0x001D0E94 File Offset: 0x001CF094
	public static void AddGameStageGroup(string _fullName, GameStageGroup _group)
	{
		string key = GameStageGroup.CleanName(_fullName);
		GameStageGroup.groups.Add(key, _group);
		GameStageGroup.groupsFullName.Add(_fullName, _group);
	}

	// Token: 0x06004984 RID: 18820 RVA: 0x001D0EC0 File Offset: 0x001CF0C0
	public static GameStageGroup TryGet(string _name)
	{
		GameStageGroup result;
		if (GameStageGroup.groups.TryGetValue(_name, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06004985 RID: 18821 RVA: 0x001D0EDF File Offset: 0x001CF0DF
	public static void Clear()
	{
		GameStageGroup.groups.Clear();
		GameStageGroup.groupsFullName.Clear();
	}

	// Token: 0x170007BC RID: 1980
	// (get) Token: 0x06004986 RID: 18822 RVA: 0x001D0EF5 File Offset: 0x001CF0F5
	public static Dictionary<string, GameStageGroup> Groups
	{
		get
		{
			return GameStageGroup.groupsFullName;
		}
	}

	// Token: 0x06004987 RID: 18823 RVA: 0x001D0EFC File Offset: 0x001CF0FC
	public static string CleanName(string _name)
	{
		if (_name.Length > 0 && char.IsDigit(_name[0]))
		{
			_name = _name.Substring(1);
		}
		else if (_name.StartsWith("S_"))
		{
			int startIndex = 2;
			if (_name.StartsWith("S_-"))
			{
				startIndex = 3;
			}
			return _name.Substring(startIndex).Replace("_", "");
		}
		return _name;
	}

	// Token: 0x06004988 RID: 18824 RVA: 0x001D0F64 File Offset: 0x001CF164
	public static string MakeDisplayName(string _name)
	{
		bool flag = false;
		foreach (char c in _name)
		{
			if (!char.IsDigit(c))
			{
				if (char.IsUpper(c) && flag)
				{
					GameStageGroup.stringBuilder.Append(' ');
				}
				GameStageGroup.stringBuilder.Append(c);
				flag = true;
			}
		}
		_name = GameStageGroup.stringBuilder.ToString();
		GameStageGroup.stringBuilder.Clear();
		return _name;
	}

	// Token: 0x040038C7 RID: 14535
	public const string cDefaultGroupName = "GroupGenericZombie";

	// Token: 0x040038C8 RID: 14536
	public readonly GameStageDefinition spawner;

	// Token: 0x040038C9 RID: 14537
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, GameStageGroup> groups = new Dictionary<string, GameStageGroup>();

	// Token: 0x040038CA RID: 14538
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, GameStageGroup> groupsFullName = new Dictionary<string, GameStageGroup>();

	// Token: 0x040038CB RID: 14539
	[PublicizedFrom(EAccessModifier.Private)]
	public static StringBuilder stringBuilder = new StringBuilder();
}

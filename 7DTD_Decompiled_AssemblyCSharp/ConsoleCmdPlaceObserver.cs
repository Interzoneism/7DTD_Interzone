using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000220 RID: 544
[Preserve]
public class ConsoleCmdPlaceObserver : ConsoleCmdAbstract
{
	// Token: 0x06000FE5 RID: 4069 RVA: 0x000667D2 File Offset: 0x000649D2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Place a chunk observer on a given position.";
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x000667D9 File Offset: 0x000649D9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  1. chunkobserver add <x> <z> [size]\n  2. chunkobserver remove <x> <z>\n  3. chunkobserver list\n1. Place an observer on the chunk that contains the coordinate x/z.\n   Optionally specifying the box radius in chunks, defaulting to 1.\n2. Remove the observer from the chunk with the coordinate, if any.\n3. List all currently placed observers";
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x000667E0 File Offset: 0x000649E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"chunkobserver",
			"co"
		};
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x000667F8 File Offset: 0x000649F8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 1 && _params[0].EqualsCaseInsensitive("list"))
		{
			this.listObservers();
			return;
		}
		if ((!_params[0].EqualsCaseInsensitive("add") || (_params.Count != 3 && _params.Count != 4)) && (!_params[0].EqualsCaseInsensitive("remove") || _params.Count != 3))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Illegal arguments");
			return;
		}
		int x;
		if (!int.TryParse(_params[1], out x))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given x coordinate is not a valid integer");
			return;
		}
		int y;
		if (!int.TryParse(_params[2], out y))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given z coordinate is not a valid integer");
			return;
		}
		int num;
		if (_params.Count == 4)
		{
			if (!int.TryParse(_params[3], out num) || num < 1 || num > 15)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given size is not a valid integer or exceeds the allowed range of 1-15");
				return;
			}
		}
		else
		{
			num = 1;
		}
		Vector2i pos = new Vector2i(x, y);
		if (_params[0].EqualsCaseInsensitive("remove"))
		{
			if (this.removeObserver(pos))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Observer removed from " + pos.ToString());
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No observer on " + pos.ToString());
			return;
		}
		else
		{
			if (this.addObserver(pos, num))
			{
				int num2 = 2 * (num - 1) + 1;
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
				{
					"Observer added to ",
					pos.ToString(),
					" with radius ",
					num.ToString(),
					" (size ",
					num2.ToString(),
					"x",
					num2.ToString(),
					")"
				}));
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Already an observer on " + pos.ToString());
			return;
		}
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x00066A00 File Offset: 0x00064C00
	[PublicizedFrom(EAccessModifier.Private)]
	public bool addObserver(Vector2i _pos, int _radius)
	{
		if (this.observers.dict.ContainsKey(_pos))
		{
			return false;
		}
		Vector3 initialPosition = new Vector3((float)_pos.x, 0f, (float)_pos.y);
		ChunkManager.ChunkObserver value = GameManager.Instance.AddChunkObserver(initialPosition, false, _radius, -1);
		this.observers.Add(_pos, value);
		return true;
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x00066A5C File Offset: 0x00064C5C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool removeObserver(Vector2i _pos)
	{
		if (!this.observers.dict.ContainsKey(_pos))
		{
			return false;
		}
		ChunkManager.ChunkObserver observer = this.observers.dict[_pos];
		GameManager.Instance.RemoveChunkObserver(observer);
		this.observers.Remove(_pos);
		return true;
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x00066AAC File Offset: 0x00064CAC
	[PublicizedFrom(EAccessModifier.Private)]
	public void listObservers()
	{
		int num = 0;
		foreach (KeyValuePair<Vector2i, ChunkManager.ChunkObserver> keyValuePair in this.observers.dict)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format(" {0,3}: {1}", ++num, keyValuePair.Key.ToString()));
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(num.ToString() + " observers");
	}

	// Token: 0x04000B3B RID: 2875
	[PublicizedFrom(EAccessModifier.Private)]
	public MapVisitor mapVisitor;

	// Token: 0x04000B3C RID: 2876
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DictionaryList<Vector2i, ChunkManager.ChunkObserver> observers = new DictionaryList<Vector2i, ChunkManager.ChunkObserver>();
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200025D RID: 605
[Preserve]
public class ConsoleCmdSquareSpiral : ConsoleCmdAbstract
{
	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x0600114B RID: 4427 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x0006D0E4 File Offset: 0x0006B2E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"squarespiral",
			"sqs"
		};
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x0006D0FC File Offset: 0x0006B2FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Move the player chunk by chunk in a square spiral. Will start off paused and required un-pausing. Also gives god mode and flying at the start.";
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x0006D103 File Offset: 0x0006B303
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "<s[tart] [chunks per auto-pause]|p[ause]|r[eset]|waitmode [minimal|meshes|displayed]>";
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x0006D10A File Offset: 0x0006B30A
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!this.ExecuteInternal(_params, _senderInfo))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
		}
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x0006D128 File Offset: 0x0006B328
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ExecuteInternal(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!GameManager.Instance.World.GetPrimaryPlayer())
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No local player! (Are you in-game?)");
			return true;
		}
		if (_params.Count == 0)
		{
			return false;
		}
		string text = _params[0].ToLowerInvariant();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1697318111U)
		{
			if (num != 367725747U)
			{
				if (num != 1695364032U)
				{
					if (num != 1697318111U)
					{
						return false;
					}
					if (!(text == "start"))
					{
						return false;
					}
				}
				else
				{
					if (!(text == "reset"))
					{
						return false;
					}
					goto IL_17A;
				}
			}
			else
			{
				if (!(text == "waitmode"))
				{
					return false;
				}
				if (_params.Count > 1)
				{
					string a = _params[1].ToLowerInvariant();
					if (a == "minimal")
					{
						this.waitMode = ConsoleCmdSquareSpiral.WaitMode.SingleChunkDecorated;
						this.chunkCondition = ChunkConditions.Decorated;
						return true;
					}
					if (a == "meshes")
					{
						this.waitMode = ConsoleCmdSquareSpiral.WaitMode.SurroundingMeshesCopied;
						this.chunkCondition = ChunkConditions.MeshesCopied;
						return true;
					}
					if (a == "displayed")
					{
						this.waitMode = ConsoleCmdSquareSpiral.WaitMode.SurroundingChunksDisplayed;
						this.chunkCondition = ChunkConditions.Displayed;
						return true;
					}
				}
				return false;
			}
		}
		else
		{
			if (num <= 4111221743U)
			{
				if (num != 1887753101U)
				{
					if (num != 4111221743U)
					{
						return false;
					}
					if (!(text == "p"))
					{
						return false;
					}
				}
				else if (!(text == "pause"))
				{
					return false;
				}
				this.MacroPause();
				return true;
			}
			if (num != 4127999362U)
			{
				if (num != 4144776981U)
				{
					return false;
				}
				if (!(text == "r"))
				{
					return false;
				}
				goto IL_17A;
			}
			else if (!(text == "s"))
			{
				return false;
			}
		}
		if (_params.Count > 2)
		{
			return false;
		}
		int chunksPerAutoPause = 0;
		if (_params.Count > 1 && !int.TryParse(_params[1], out chunksPerAutoPause))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Failed to parse as int " + _params[1]);
			return false;
		}
		this.MacroStart(chunksPerAutoPause);
		return true;
		IL_17A:
		this.MacroReset();
		return true;
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x0006D338 File Offset: 0x0006B538
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroStart(int _chunksPerAutoPause)
	{
		if (this.m_coroutine != null)
		{
			return;
		}
		Coroutine coroutine = ThreadManager.StartCoroutine(this.CoroutineSpiral(_chunksPerAutoPause));
		if (this.m_isRunning)
		{
			this.m_coroutine = coroutine;
		}
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0006D36A File Offset: 0x0006B56A
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroPause()
	{
		if (this.m_isRunning)
		{
			this.m_isRunning = false;
			return;
		}
		this.m_isRunning = true;
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0006D383 File Offset: 0x0006B583
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroReset()
	{
		if (this.m_coroutine == null)
		{
			return;
		}
		this.m_isRunning = false;
		ThreadManager.StopCoroutine(this.m_coroutine);
		this.m_coroutine = null;
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x0006D3A7 File Offset: 0x0006B5A7
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator CoroutineSpiral(int _chunksPerAutoPause)
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			yield break;
		}
		EntityPlayerLocal player;
		if (!ProfilerGameUtils.TryGetFlyingPlayer(out player))
		{
			yield break;
		}
		this.m_isRunning = true;
		IEnumerator<Vector2i> spiralSequence = this.SpiralSequence();
		float lastY = player.position.y;
		int i = 0;
		DateTime lastPrintTime = DateTime.MinValue;
		while (world == GameManager.Instance.World && !(player != world.GetPrimaryPlayer()))
		{
			if (this.m_isRunning)
			{
				spiralSequence.MoveNext();
				Vector2i chunkPos = spiralSequence.Current;
				int num = i;
				i = num + 1;
				DateTime now = DateTime.Now;
				if (now.Subtract(lastPrintTime).TotalSeconds > 10.0)
				{
					Log.Out("Square Spiral: ({0}, {1}) #{2}", new object[]
					{
						chunkPos.x,
						chunkPos.y,
						i
					});
					lastPrintTime = now;
				}
				Vector2i centreOfChunk = new Vector2i(chunkPos.x * 16 + 8, chunkPos.y * 16 + 8);
				Vector3 rotationEuler = Quaternion.FromToRotation(Vector3.forward, new Vector3((float)(-(float)centreOfChunk.x), 0f, (float)(-(float)centreOfChunk.y))).eulerAngles;
				Chunk chunk = (Chunk)world.GetChunkSync(chunkPos.x, chunkPos.y);
				if (chunk == null)
				{
					player.SetPosition(new Vector3((float)centreOfChunk.x, lastY, (float)centreOfChunk.y), true);
					player.SetRotation(rotationEuler);
					while (chunk == null)
					{
						yield return null;
						chunk = (Chunk)world.GetChunkSync(chunkPos.x, chunkPos.y);
					}
				}
				float y = (float)(world.GetHeight(centreOfChunk.x, centreOfChunk.y) + 10);
				player.SetPosition(new Vector3((float)centreOfChunk.x, y, (float)centreOfChunk.y), true);
				player.SetRotation(rotationEuler);
				lastY = player.position.y;
				switch (this.waitMode)
				{
				case ConsoleCmdSquareSpiral.WaitMode.SingleChunkDecorated:
					yield return ProfilerGameUtils.WaitForSingleChunkToLoad(chunk, this.chunkCondition);
					break;
				case ConsoleCmdSquareSpiral.WaitMode.SurroundingMeshesCopied:
					yield return ProfilerGameUtils.WaitForChunksAroundObserverToLoad(player.ChunkObserver, this.chunkCondition);
					break;
				case ConsoleCmdSquareSpiral.WaitMode.SurroundingChunksDisplayed:
					yield return ProfilerGameUtils.WaitForChunksAroundObserverToLoad(player.ChunkObserver, this.chunkCondition);
					break;
				}
				yield return null;
				if ((_chunksPerAutoPause > 0 && i % _chunksPerAutoPause == 0) || i == 1)
				{
					Log.Out("Square Spiral: ({0}, {1}) #{2} (PAUSED)", new object[]
					{
						chunkPos.x,
						chunkPos.y,
						i
					});
					this.MacroPause();
				}
				rotationEuler = default(Vector3);
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
		this.MacroReset();
		yield break;
		yield break;
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0006D3BD File Offset: 0x0006B5BD
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator<Vector2i> SpiralSequence()
	{
		int x = 0;
		int y = 0;
		int rPos = 1;
		int rNeg = -1;
		ConsoleCmdSquareSpiral.SpiralSequenceState state = ConsoleCmdSquareSpiral.SpiralSequenceState.Left;
		for (;;)
		{
			yield return new Vector2i(x, y);
			switch (state)
			{
			case ConsoleCmdSquareSpiral.SpiralSequenceState.Left:
				if (x == rNeg)
				{
					state = ConsoleCmdSquareSpiral.SpiralSequenceState.Down;
					int num = y;
					y = num - 1;
				}
				else
				{
					int num = x;
					x = num - 1;
				}
				break;
			case ConsoleCmdSquareSpiral.SpiralSequenceState.Down:
				if (y == rNeg)
				{
					state = ConsoleCmdSquareSpiral.SpiralSequenceState.Right;
					int num = x;
					x = num + 1;
				}
				else
				{
					int num = y;
					y = num - 1;
				}
				break;
			case ConsoleCmdSquareSpiral.SpiralSequenceState.Right:
				if (x == rPos)
				{
					state = ConsoleCmdSquareSpiral.SpiralSequenceState.Up;
					int num = y;
					y = num + 1;
				}
				else
				{
					int num = x;
					x = num + 1;
				}
				break;
			case ConsoleCmdSquareSpiral.SpiralSequenceState.Up:
				if (y == rPos)
				{
					int num = rPos;
					rPos = num + 1;
					num = rNeg;
					rNeg = num - 1;
					state = ConsoleCmdSquareSpiral.SpiralSequenceState.Left;
					num = x;
					x = num - 1;
				}
				else
				{
					int num = y;
					y = num + 1;
				}
				break;
			}
		}
		yield break;
	}

	// Token: 0x04000BA3 RID: 2979
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_isRunning;

	// Token: 0x04000BA4 RID: 2980
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine m_coroutine;

	// Token: 0x04000BA5 RID: 2981
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkConditions.Delegate chunkCondition = ChunkConditions.Decorated;

	// Token: 0x04000BA6 RID: 2982
	[PublicizedFrom(EAccessModifier.Private)]
	public ConsoleCmdSquareSpiral.WaitMode waitMode;

	// Token: 0x0200025E RID: 606
	[PublicizedFrom(EAccessModifier.Private)]
	public enum WaitMode
	{
		// Token: 0x04000BA8 RID: 2984
		SingleChunkDecorated,
		// Token: 0x04000BA9 RID: 2985
		SurroundingMeshesCopied,
		// Token: 0x04000BAA RID: 2986
		SurroundingChunksDisplayed
	}

	// Token: 0x0200025F RID: 607
	[PublicizedFrom(EAccessModifier.Private)]
	public enum SpiralSequenceState
	{
		// Token: 0x04000BAC RID: 2988
		Left,
		// Token: 0x04000BAD RID: 2989
		Down,
		// Token: 0x04000BAE RID: 2990
		Right,
		// Token: 0x04000BAF RID: 2991
		Up
	}
}

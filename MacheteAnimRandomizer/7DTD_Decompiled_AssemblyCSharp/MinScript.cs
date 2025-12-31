using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

// Token: 0x0200067B RID: 1659
public class MinScript
{
	// Token: 0x060031B7 RID: 12727 RVA: 0x001533C3 File Offset: 0x001515C3
	public static string ConvertFromUIText(string _text)
	{
		return _text.Replace("\n", "^");
	}

	// Token: 0x060031B8 RID: 12728 RVA: 0x001533D5 File Offset: 0x001515D5
	public static string ConvertToUIText(string _text)
	{
		if (_text == null)
		{
			return string.Empty;
		}
		return _text.Replace("^", "\n");
	}

	// Token: 0x060031B9 RID: 12729 RVA: 0x001533F0 File Offset: 0x001515F0
	public void SetText(string _text)
	{
		int num = 0;
		int length = _text.Length;
		int num2;
		for (int i = 0; i < length; i = num2 + 1)
		{
			num2 = _text.IndexOf('^', i, length - i);
			if (num2 < 0)
			{
				num2 = length;
			}
			while (i < length && _text[i] == ' ')
			{
				i++;
			}
			int num3 = num2 - i;
			if (num3 > 0 && _text[i] != '/')
			{
				int num4 = _text.IndexOf(' ', i, num3);
				if (num4 < 0)
				{
					num4 = num2;
				}
				string key = _text.Substring(i, num4 - i);
				MinScript.CmdLine item;
				if (MinScript.nameToCmds.TryGetValue(key, out item.command))
				{
					num4++;
					item.parameters = null;
					int num5 = num2 - num4;
					if (num5 > 0)
					{
						item.parameters = _text.Substring(num4, num5);
					}
					this.commandList.Add(item);
				}
			}
			num++;
		}
	}

	// Token: 0x060031BA RID: 12730 RVA: 0x001534C7 File Offset: 0x001516C7
	public void Reset()
	{
		this.curIndex = -1;
	}

	// Token: 0x060031BB RID: 12731 RVA: 0x001534D0 File Offset: 0x001516D0
	public void Restart()
	{
		this.curIndex = 0;
		this.sleep = 0f;
	}

	// Token: 0x060031BC RID: 12732 RVA: 0x001534E4 File Offset: 0x001516E4
	public void Run(SleeperVolume _sv, EntityPlayer _player, float _countScale)
	{
		if (this.commandList == null)
		{
			return;
		}
		this.player = _player;
		this.countScale = _countScale;
		this.curIndex = 0;
		this.sleep = 0f;
	}

	// Token: 0x060031BD RID: 12733 RVA: 0x0015350F File Offset: 0x0015170F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsRunning()
	{
		return this.curIndex >= 0;
	}

	// Token: 0x060031BE RID: 12734 RVA: 0x00153520 File Offset: 0x00151720
	public void Tick(SleeperVolume _sv)
	{
		if (this.curIndex < 0)
		{
			return;
		}
		if (this.sleep > 0f)
		{
			this.sleep -= 0.05f;
			if (this.sleep > 0f)
			{
				return;
			}
		}
		for (;;)
		{
			MinScript.CmdLine cmdLine = this.commandList[this.curIndex];
			ushort command = cmdLine.command;
			int num;
			switch (command)
			{
			case 1:
				Log.Out("MinScript " + cmdLine.parameters);
				break;
			case 2:
				break;
			case 3:
				if (this.loopCount <= 0)
				{
					string[] array = cmdLine.parameters.Split(' ', StringSplitOptions.None);
					if (array.Length == 2)
					{
						this.loopToIndex = this.FindLabel(array[0]);
						if (this.loopToIndex < 0)
						{
							Log.Warning("MinScript loop label {0} missing: {1}", new object[]
							{
								array[0],
								_sv
							});
						}
						else
						{
							this.loopCount = int.Parse(array[1]);
						}
					}
					else
					{
						Log.Warning("MinScript loop needs 2 params: {0}", new object[]
						{
							_sv
						});
					}
				}
				num = this.loopCount - 1;
				this.loopCount = num;
				if (num > 0)
				{
					this.curIndex = this.loopToIndex;
				}
				break;
			case 4:
				this.sleep = float.Parse(cmdLine.parameters ?? "1");
				break;
			default:
				if (command != 40)
				{
					switch (command)
					{
					case 50:
						if (cmdLine.parameters != null)
						{
							string[] array2 = cmdLine.parameters.Split(' ', StringSplitOptions.None);
							if (array2.Length >= 1)
							{
								float num2 = 1f;
								float num3 = 1f;
								if (array2.Length >= 2)
								{
									num2 = float.Parse(array2[1]);
									num3 = num2;
									if (array2.Length >= 3)
									{
										num3 = float.Parse(array2[2]);
									}
								}
								_sv.AddSpawnCount(array2[0], num2 * this.countScale, num3 * this.countScale);
							}
						}
						break;
					case 51:
					{
						int num4 = int.Parse(cmdLine.parameters ?? "0");
						if (_sv.GetAliveCount() > num4)
						{
							return;
						}
						break;
					}
					case 52:
						if (this.player)
						{
							if (cmdLine.parameters != null)
							{
								byte trigger = (byte)int.Parse(cmdLine.parameters);
								this.player.world.triggerManager.Trigger(this.player, _sv.PrefabInstance, trigger);
							}
							else
							{
								Log.Warning("MinScript trigger !param {0}", new object[]
								{
									_sv
								});
							}
						}
						break;
					}
				}
				else
				{
					GameManager.Instance.PlaySoundAtPositionServer(_sv.Center, cmdLine.parameters, AudioRolloffMode.Linear, 100, _sv.GetPlayerTouchedToUpdateId());
				}
				break;
			}
			num = this.curIndex + 1;
			this.curIndex = num;
			if (num >= this.commandList.Count)
			{
				goto Block_20;
			}
			if (this.sleep > 0f)
			{
				return;
			}
		}
		return;
		Block_20:
		this.curIndex = -1;
	}

	// Token: 0x060031BF RID: 12735 RVA: 0x001537E4 File Offset: 0x001519E4
	[PublicizedFrom(EAccessModifier.Private)]
	public int FindLabel(string _name)
	{
		int count = this.commandList.Count;
		for (int i = 0; i < count; i++)
		{
			MinScript.CmdLine cmdLine = this.commandList[i];
			if (cmdLine.command == 2 && cmdLine.parameters == _name)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060031C0 RID: 12736 RVA: 0x00153830 File Offset: 0x00151A30
	public static MinScript Read(BinaryReader _br)
	{
		_br.ReadByte();
		MinScript minScript = new MinScript();
		minScript.curIndex = (int)_br.ReadInt16();
		if (minScript.curIndex >= 0)
		{
			minScript.sleep = _br.ReadSingle();
		}
		int num = (int)_br.ReadUInt16();
		for (int i = 0; i < num; i++)
		{
			MinScript.CmdLine item;
			item.command = _br.ReadUInt16();
			item.parameters = null;
			int num2 = (int)_br.ReadByte();
			if (num2 > 0)
			{
				_br.Read(MinScript.tempBytes, 0, num2);
				int chars = Encoding.UTF8.GetChars(MinScript.tempBytes, 0, num2, MinScript.tempChars, 0);
				item.parameters = new string(MinScript.tempChars, 0, chars);
			}
			minScript.commandList.Add(item);
		}
		return minScript;
	}

	// Token: 0x060031C1 RID: 12737 RVA: 0x001538EA File Offset: 0x00151AEA
	public bool HasData()
	{
		return this.commandList.Count > 0;
	}

	// Token: 0x060031C2 RID: 12738 RVA: 0x001538FC File Offset: 0x00151AFC
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(1);
		_bw.Write((short)this.curIndex);
		if (this.curIndex >= 0)
		{
			_bw.Write(this.sleep);
		}
		int num = this.commandList.Count;
		if (num >= 1000)
		{
			num = 1;
			Log.Error("MinScript Write error: {0}", new object[]
			{
				this
			});
		}
		_bw.Write((ushort)num);
		for (int i = 0; i < num; i++)
		{
			MinScript.CmdLine cmdLine = this.commandList[i];
			_bw.Write(cmdLine.command);
			if (cmdLine.parameters != null && cmdLine.parameters.Length > 0)
			{
				for (int j = 0; j < cmdLine.parameters.Length; j++)
				{
					MinScript.tempChars[j] = cmdLine.parameters[j];
				}
				byte b = (byte)Encoding.UTF8.GetBytes(MinScript.tempChars, 0, cmdLine.parameters.Length, MinScript.tempBytes, 0);
				_bw.Write(b);
				_bw.Write(MinScript.tempBytes, 0, (int)b);
			}
			else
			{
				_bw.Write(0);
			}
		}
	}

	// Token: 0x060031C3 RID: 12739 RVA: 0x00153A14 File Offset: 0x00151C14
	public override string ToString()
	{
		return string.Format("cmds {0}, index {1}, sleep {2}", this.commandList.Count, this.curIndex, this.sleep);
	}

	// Token: 0x060031C4 RID: 12740 RVA: 0x00153A46 File Offset: 0x00151C46
	[Conditional("DEBUG_MINSCRIPTLOG")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LogMS(string format, params object[] args)
	{
		format = string.Format("{0} {1} MinScript {2}", GameManager.frameTime.ToCultureInvariantString(), GameManager.frameCount, format);
		Log.Out(format, args);
	}

	// Token: 0x04002887 RID: 10375
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte cVersion = 1;

	// Token: 0x04002888 RID: 10376
	[PublicizedFrom(EAccessModifier.Private)]
	public const char cLineSepChar = '^';

	// Token: 0x04002889 RID: 10377
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cLineSepStr = "^";

	// Token: 0x0400288A RID: 10378
	[PublicizedFrom(EAccessModifier.Private)]
	public List<MinScript.CmdLine> commandList = new List<MinScript.CmdLine>();

	// Token: 0x0400288B RID: 10379
	[PublicizedFrom(EAccessModifier.Private)]
	public int curIndex;

	// Token: 0x0400288C RID: 10380
	[PublicizedFrom(EAccessModifier.Private)]
	public float sleep;

	// Token: 0x0400288D RID: 10381
	[PublicizedFrom(EAccessModifier.Private)]
	public int loopCount;

	// Token: 0x0400288E RID: 10382
	[PublicizedFrom(EAccessModifier.Private)]
	public int loopToIndex;

	// Token: 0x0400288F RID: 10383
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cCmdNop = 0;

	// Token: 0x04002890 RID: 10384
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cCmdLog = 1;

	// Token: 0x04002891 RID: 10385
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cCmdLabel = 2;

	// Token: 0x04002892 RID: 10386
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cCmdLoop = 3;

	// Token: 0x04002893 RID: 10387
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cCmdSleep = 4;

	// Token: 0x04002894 RID: 10388
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cCmdSound = 40;

	// Token: 0x04002895 RID: 10389
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cCmdSpawn = 50;

	// Token: 0x04002896 RID: 10390
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cCmdWaitAlive = 51;

	// Token: 0x04002897 RID: 10391
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cCmdTrigger = 52;

	// Token: 0x04002898 RID: 10392
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, ushort> nameToCmds = new Dictionary<string, ushort>
	{
		{
			"log",
			1
		},
		{
			"label",
			2
		},
		{
			"loop",
			3
		},
		{
			"sleep",
			4
		},
		{
			"sound",
			40
		},
		{
			"spawn",
			50
		},
		{
			"trigger",
			52
		},
		{
			"waitalive",
			51
		}
	};

	// Token: 0x04002899 RID: 10393
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte[] tempBytes = new byte[256];

	// Token: 0x0400289A RID: 10394
	[PublicizedFrom(EAccessModifier.Private)]
	public static char[] tempChars = new char[256];

	// Token: 0x0400289B RID: 10395
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer player;

	// Token: 0x0400289C RID: 10396
	[PublicizedFrom(EAccessModifier.Private)]
	public float countScale = 1f;

	// Token: 0x0200067C RID: 1660
	[PublicizedFrom(EAccessModifier.Private)]
	public struct CmdLine
	{
		// Token: 0x0400289D RID: 10397
		public ushort command;

		// Token: 0x0400289E RID: 10398
		public string parameters;
	}
}

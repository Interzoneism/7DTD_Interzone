using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using InControl;
using UnityEngine;

// Token: 0x02000F37 RID: 3895
public class ControllerDebugMacros : MonoBehaviour
{
	// Token: 0x06007C04 RID: 31748 RVA: 0x003233FB File Offset: 0x003215FB
	public ControllerDebugMacros()
	{
		this.m_debugMacros = new SortedDictionary<string, Action<ControllerDebugMacros.DebugDirection>>();
	}

	// Token: 0x06007C05 RID: 31749 RVA: 0x00323410 File Offset: 0x00321610
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.m_debugLabel = base.GetComponent<ControllerDebugLabel>();
		this.m_debugLabel.AddDebugProvider("Debug Macros", new Action<StringBuilder>(this.BuildDebugMacroStatus));
		this.AddDebugMacro("Do Nothing", delegate()
		{
		});
		this.AddDebugMacro("Open Console", new Action(this.MacroOpenConsole));
		this.AddDebugMacro("Toggle God Mode", new Action(this.MacroToggleGodMode));
		this.AddDebugMacro("Toggle Flying", new Action(this.MacroToggleFlying));
	}

	// Token: 0x06007C06 RID: 31750 RVA: 0x003234B4 File Offset: 0x003216B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.m_debugLabel.RemoveDebugProvider("Debug Macros");
		this.RemoveDebugMacro("Open Console");
		this.RemoveDebugMacro("Toggle God Mode");
		this.RemoveDebugMacro("Toggle Flying");
	}

	// Token: 0x06007C07 RID: 31751 RVA: 0x003234E8 File Offset: 0x003216E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.m_currentMacro == null || !this.m_debugMacros.ContainsKey(this.m_currentMacro))
		{
			if (this.m_lastIndex >= 0 && this.m_lastIndex < this.m_debugMacros.Keys.Count)
			{
				this.m_currentMacro = this.m_debugMacros.Keys.Skip(this.m_lastIndex).FirstOrDefault<string>();
			}
			else
			{
				this.m_currentMacro = this.m_debugMacros.Keys.LastOrDefault<string>();
				this.m_lastIndex = this.m_debugMacros.Keys.Count - 1;
			}
		}
		if (this.m_currentMacro == null)
		{
			return;
		}
		ControllerDebugMacros.DebugDirection? executeMacroKeybind = this.GetExecuteMacroKeybind();
		if (executeMacroKeybind != null)
		{
			ControllerDebugMacros.DebugDirection valueOrDefault = executeMacroKeybind.GetValueOrDefault();
			if (this.m_keybindPressed)
			{
				return;
			}
			this.m_keybindPressed = true;
			if (this.m_debugMacros.ContainsKey(this.m_currentMacro))
			{
				this.m_debugMacros[this.m_currentMacro](valueOrDefault);
				return;
			}
		}
		else
		{
			if (this.HasNextMacroKeybind())
			{
				if (this.m_keybindPressed)
				{
					return;
				}
				this.m_keybindPressed = true;
				bool flag = false;
				int num = 0;
				using (SortedDictionary<string, Action<ControllerDebugMacros.DebugDirection>>.KeyCollection.Enumerator enumerator = this.m_debugMacros.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						if (flag)
						{
							this.m_currentMacro = text;
							this.m_lastIndex = num;
							break;
						}
						if (text == this.m_currentMacro)
						{
							flag = true;
						}
						num++;
					}
					return;
				}
			}
			if (this.HasPreviousMacroKeybind())
			{
				if (this.m_keybindPressed)
				{
					return;
				}
				this.m_keybindPressed = true;
				string text2 = null;
				int num2 = 0;
				using (SortedDictionary<string, Action<ControllerDebugMacros.DebugDirection>>.KeyCollection.Enumerator enumerator = this.m_debugMacros.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text3 = enumerator.Current;
						if (text3 == this.m_currentMacro)
						{
							if (text2 != null)
							{
								this.m_currentMacro = text2;
								this.m_lastIndex = num2 - 1;
								break;
							}
							break;
						}
						else
						{
							text2 = text3;
							num2++;
						}
					}
					return;
				}
			}
			this.m_keybindPressed = false;
		}
	}

	// Token: 0x06007C08 RID: 31752 RVA: 0x0032370C File Offset: 0x0032190C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BuildDebugMacroStatus(StringBuilder builder)
	{
		foreach (string text in this.m_debugMacros.Keys)
		{
			if (builder.Length > 0)
			{
				builder.Append(' ');
			}
			bool flag = text == this.m_currentMacro;
			if (flag)
			{
				builder.Append('[');
			}
			builder.Append(text);
			if (flag)
			{
				builder.Append(']');
			}
		}
	}

	// Token: 0x06007C09 RID: 31753 RVA: 0x0032379C File Offset: 0x0032199C
	public void AddDebugMacro(string macroName, Action macro)
	{
		this.m_debugMacros[macroName] = delegate(ControllerDebugMacros.DebugDirection _)
		{
			macro();
		};
	}

	// Token: 0x06007C0A RID: 31754 RVA: 0x003237CE File Offset: 0x003219CE
	public void AddDebugMacro(string macroName, Action<ControllerDebugMacros.DebugDirection> macro)
	{
		this.m_debugMacros[macroName] = macro;
	}

	// Token: 0x06007C0B RID: 31755 RVA: 0x003237DD File Offset: 0x003219DD
	public void RemoveDebugMacro(string macroName)
	{
		this.m_debugMacros.Remove(macroName);
	}

	// Token: 0x06007C0C RID: 31756 RVA: 0x003237EC File Offset: 0x003219EC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasPreviousMacroKeybind()
	{
		return ControllerDebugMacros.<HasPreviousMacroKeybind>g__HasKeyboardKeybind|14_0() || ControllerDebugMacros.<HasPreviousMacroKeybind>g__HasControllerKeybind|14_1();
	}

	// Token: 0x06007C0D RID: 31757 RVA: 0x003237FC File Offset: 0x003219FC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasNextMacroKeybind()
	{
		return ControllerDebugMacros.<HasNextMacroKeybind>g__HasKeyboardKeybind|15_0() || ControllerDebugMacros.<HasNextMacroKeybind>g__HasControllerKeybind|15_1();
	}

	// Token: 0x06007C0E RID: 31758 RVA: 0x0032380C File Offset: 0x00321A0C
	[PublicizedFrom(EAccessModifier.Private)]
	public ControllerDebugMacros.DebugDirection? GetExecuteMacroKeybind()
	{
		ControllerDebugMacros.DebugDirection? result = ControllerDebugMacros.<GetExecuteMacroKeybind>g__GetKeyboardKeybind|16_0();
		if (result == null)
		{
			return ControllerDebugMacros.<GetExecuteMacroKeybind>g__GetControllerKeybind|16_1();
		}
		return result;
	}

	// Token: 0x06007C0F RID: 31759 RVA: 0x00323830 File Offset: 0x00321A30
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroOpenConsole()
	{
		GUIWindowManager guiwindowManager = UnityEngine.Object.FindObjectOfType<GUIWindowManager>();
		GameManager instance = GameManager.Instance;
		if (!guiwindowManager || !instance)
		{
			return;
		}
		GUIWindowConsole guiconsole = instance.m_GUIConsole;
		if (guiconsole != null)
		{
			guiwindowManager.Open(guiconsole, false, false, true);
			return;
		}
		guiwindowManager.Open(GUIWindowConsole.ID, false, false, true);
	}

	// Token: 0x06007C10 RID: 31760 RVA: 0x0032387D File Offset: 0x00321A7D
	[PublicizedFrom(EAccessModifier.Private)]
	public static EntityPlayerLocal GetPrimaryPlayer()
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return null;
		}
		return world.GetPrimaryPlayer();
	}

	// Token: 0x06007C11 RID: 31761 RVA: 0x00323894 File Offset: 0x00321A94
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroToggleGodMode()
	{
		EntityPlayerLocal primaryPlayer = ControllerDebugMacros.GetPrimaryPlayer();
		if (!primaryPlayer)
		{
			return;
		}
		DataItem<bool> isGodMode = primaryPlayer.IsGodMode;
		if (isGodMode.Value)
		{
			isGodMode.Value = false;
			primaryPlayer.Buffs.RemoveBuff("god", true);
		}
		else
		{
			isGodMode.Value = true;
			primaryPlayer.Buffs.AddBuff("god", -1, true, false, -1f);
		}
		primaryPlayer.bEntityAliveFlagsChanged = true;
	}

	// Token: 0x06007C12 RID: 31762 RVA: 0x00323900 File Offset: 0x00321B00
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroToggleFlying()
	{
		EntityPlayerLocal primaryPlayer = ControllerDebugMacros.GetPrimaryPlayer();
		if (!primaryPlayer)
		{
			return;
		}
		DataItem<bool> isFlyMode = primaryPlayer.IsFlyMode;
		isFlyMode.Value = !isFlyMode.Value;
		primaryPlayer.bEntityAliveFlagsChanged = true;
	}

	// Token: 0x06007C13 RID: 31763 RVA: 0x00323937 File Offset: 0x00321B37
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static bool <HasPreviousMacroKeybind>g__HasKeyboardKeybind|14_0()
	{
		return InputUtils.ControlKeyPressed && InputUtils.ShiftKeyPressed && InputUtils.AltKeyPressed && Input.GetKey(KeyCode.LeftArrow);
	}

	// Token: 0x06007C14 RID: 31764 RVA: 0x00323964 File Offset: 0x00321B64
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static bool <HasPreviousMacroKeybind>g__HasControllerKeybind|14_1()
	{
		return InputManager.Enabled && InputManager.ActiveDevice.LeftBumper.IsPressed && InputManager.ActiveDevice.RightBumper.IsPressed && InputManager.ActiveDevice.DPadLeft.IsPressed;
	}

	// Token: 0x06007C15 RID: 31765 RVA: 0x003239B4 File Offset: 0x00321BB4
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static bool <HasNextMacroKeybind>g__HasKeyboardKeybind|15_0()
	{
		return InputUtils.ControlKeyPressed && InputUtils.ShiftKeyPressed && InputUtils.AltKeyPressed && Input.GetKey(KeyCode.RightArrow);
	}

	// Token: 0x06007C16 RID: 31766 RVA: 0x003239E0 File Offset: 0x00321BE0
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static bool <HasNextMacroKeybind>g__HasControllerKeybind|15_1()
	{
		return InputManager.Enabled && InputManager.ActiveDevice.LeftBumper.IsPressed && InputManager.ActiveDevice.RightBumper.IsPressed && InputManager.ActiveDevice.DPadRight.IsPressed;
	}

	// Token: 0x06007C17 RID: 31767 RVA: 0x00323A30 File Offset: 0x00321C30
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static ControllerDebugMacros.DebugDirection? <GetExecuteMacroKeybind>g__GetKeyboardKeybind|16_0()
	{
		if (!InputUtils.ControlKeyPressed)
		{
			return null;
		}
		if (!InputUtils.ShiftKeyPressed)
		{
			return null;
		}
		if (!InputUtils.AltKeyPressed)
		{
			return null;
		}
		if (Input.GetKey(KeyCode.Menu))
		{
			return new ControllerDebugMacros.DebugDirection?(ControllerDebugMacros.DebugDirection.Neutral);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			return new ControllerDebugMacros.DebugDirection?(ControllerDebugMacros.DebugDirection.Down);
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			return new ControllerDebugMacros.DebugDirection?(ControllerDebugMacros.DebugDirection.Up);
		}
		return null;
	}

	// Token: 0x06007C18 RID: 31768 RVA: 0x00323AB4 File Offset: 0x00321CB4
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static ControllerDebugMacros.DebugDirection? <GetExecuteMacroKeybind>g__GetControllerKeybind|16_1()
	{
		if (!InputManager.Enabled)
		{
			return null;
		}
		if (!InputManager.ActiveDevice.LeftBumper.IsPressed)
		{
			return null;
		}
		if (!InputManager.ActiveDevice.RightBumper.IsPressed)
		{
			return null;
		}
		if (InputManager.ActiveDevice.Action1.IsPressed)
		{
			return new ControllerDebugMacros.DebugDirection?(ControllerDebugMacros.DebugDirection.Neutral);
		}
		if (InputManager.ActiveDevice.DPadDown.IsPressed)
		{
			return new ControllerDebugMacros.DebugDirection?(ControllerDebugMacros.DebugDirection.Down);
		}
		if (InputManager.ActiveDevice.DPadUp.IsPressed)
		{
			return new ControllerDebugMacros.DebugDirection?(ControllerDebugMacros.DebugDirection.Up);
		}
		return null;
	}

	// Token: 0x04005EDB RID: 24283
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly SortedDictionary<string, Action<ControllerDebugMacros.DebugDirection>> m_debugMacros;

	// Token: 0x04005EDC RID: 24284
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string m_currentMacro;

	// Token: 0x04005EDD RID: 24285
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int m_lastIndex;

	// Token: 0x04005EDE RID: 24286
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_keybindPressed;

	// Token: 0x04005EDF RID: 24287
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ControllerDebugLabel m_debugLabel;

	// Token: 0x02000F38 RID: 3896
	public enum DebugDirection
	{
		// Token: 0x04005EE1 RID: 24289
		Up,
		// Token: 0x04005EE2 RID: 24290
		Down,
		// Token: 0x04005EE3 RID: 24291
		Neutral
	}
}

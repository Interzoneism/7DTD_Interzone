using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200027C RID: 636
[Preserve]
public class ConsoleCmdWorkstationMaterials : ConsoleCmdAbstract
{
	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x0600120E RID: 4622 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x000710A5 File Offset: 0x0006F2A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"wsmats",
			"workstationmaterials"
		};
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x000710C0 File Offset: 0x0006F2C0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("This command requires exactly two parameters!");
			return;
		}
		int num;
		if (_params[0].EqualsCaseInsensitive("all"))
		{
			num = -1;
		}
		else if (!StringParsers.TryParseSInt32(_params[0], out num, 0, -1, NumberStyles.Integer) || num < -1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Parameter <slot> is not a valid positive integer or the keyword 'all'!");
			return;
		}
		int num2;
		if (!StringParsers.TryParseSInt32(_params[1], out num2, 0, -1, NumberStyles.Integer) || num2 < 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Parameter <count> is not a valid positive integer!");
			return;
		}
		if (!_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("This command has to be run from a game client!");
			return;
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("This command has to be run from a game client!");
			return;
		}
		foreach (XUiC_WorkstationWindowGroup xuiC_WorkstationWindowGroup in primaryPlayer.PlayerUI.xui.GetWindowsByType<XUiC_WorkstationWindowGroup>())
		{
			if (xuiC_WorkstationWindowGroup.IsOpen)
			{
				XUiM_Workstation workstationData = xuiC_WorkstationWindowGroup.WorkstationData;
				if (workstationData == null)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Open workstation window has no associated data!");
					return;
				}
				TileEntityWorkstation tileEntity = workstationData.TileEntity;
				ItemStack[] input = tileEntity.Input;
				int inputSlotCount = tileEntity.InputSlotCount;
				int num3 = input.Length - inputSlotCount;
				if (num == -1)
				{
					for (int i = 0; i < num3; i++)
					{
						this.setSlot(i, num2, num3, inputSlotCount, input, tileEntity.MaterialNames);
					}
				}
				else
				{
					this.setSlot(num, num2, num3, inputSlotCount, input, tileEntity.MaterialNames);
				}
				tileEntity.Input = input;
				xuiC_WorkstationWindowGroup.TileEntity_InputChanged();
				xuiC_WorkstationWindowGroup.SetAllChildrenDirty(false);
				return;
			}
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No open workstation window found!");
		float num4 = 0f;
		if (_params.Count > 0)
		{
			num4 = StringParsers.ParseFloat(_params[0], 0, -1, NumberStyles.Any);
		}
		primaryPlayer.Stats.Stamina.MaxModifier = -primaryPlayer.Stats.Stamina.Max + (float)Mathf.CeilToInt(num4 / 100f * primaryPlayer.Stats.Stamina.Max);
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x00071300 File Offset: 0x0006F500
	[PublicizedFrom(EAccessModifier.Private)]
	public void setSlot(int _targetSlot, int _count, int _storedSlotCount, int _smeltSlotCount, ItemStack[] _inputStacks, string[] _materialNames)
	{
		if (_targetSlot >= _storedSlotCount)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Parameter <slot> is larger than the amount of materials the currently open workstation knows ({0})!", _storedSlotCount));
			return;
		}
		ItemClass itemClass = ItemClass.GetItemClass("unit_" + _materialNames[_targetSlot], false);
		if (itemClass == null || itemClass.MadeOfMaterial.ForgeCategory == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Parameter <slot> specifies a slot that has no workstation materials!");
			return;
		}
		int num = _smeltSlotCount + _targetSlot;
		if (_inputStacks[num].itemValue.type == 0)
		{
			_inputStacks[num] = new ItemStack(new ItemValue(itemClass.Id, false), _inputStacks[num].count);
		}
		if (_count > itemClass.Stacknumber.Value)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Parameter <count> is larger than the max stack size for the material!");
			return;
		}
		_inputStacks[num].count = _count;
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x000713BC File Offset: 0x0006F5BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Set material counts on workstations.";
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x000713C3 File Offset: 0x0006F5C3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Set the material count for the given material slot on the currently opened workstation\nto the given value.\nUsage:\n   wsmats <slot> <count>\nSlot specifies the index of the material slot in the workstation, e.g. 0 on forge is iron, or \"all\"\nto change all slots at once.Count defines to what you want to set the material count with a maximum of 30000.\n";
	}
}

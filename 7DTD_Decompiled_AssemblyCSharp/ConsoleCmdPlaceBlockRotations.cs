using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200021E RID: 542
[Preserve]
public class ConsoleCmdPlaceBlockRotations : ConsoleCmdAbstract
{
	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06000FD7 RID: 4055 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x0006625C File Offset: 0x0006445C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Places all rotations of the currently held block";
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x00066263 File Offset: 0x00064463
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Places the block you currently hold in your hand in all supported rotations. Starts\nat the current selection box and spreads out towards the right relative to the\ncurrent view direction of the player. Spaces out each block by 1m meter.";
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x0006626A File Offset: 0x0006446A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"placeblockrotations",
			"pbr"
		};
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x00066284 File Offset: 0x00064484
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be used on clients");
			return;
		}
		if (!BlockToolSelection.Instance.SelectionActive)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No selection active. Running this command requires an active 1x1x1 selection box.");
			return;
		}
		Vector3i selectionSize = BlockToolSelection.Instance.SelectionSize;
		Vector3i selectionStart = BlockToolSelection.Instance.SelectionStart;
		if (selectionSize != Vector3i.one)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Selection box size is not 1x1x1.");
			return;
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		BlockValue blockValue = primaryPlayer.inventory.holdingItemItemValue.ToBlockValue();
		ItemClassBlock.ItemBlockInventoryData itemBlockInventoryData = primaryPlayer.inventory.holdingItemData as ItemClassBlock.ItemBlockInventoryData;
		if (blockValue.isair || itemBlockInventoryData == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Player is not holding a block.");
			return;
		}
		float y = primaryPlayer.rotation.y;
		Vector3i zero = Vector3i.zero;
		switch (GameUtils.GetClosestDirection(y, true))
		{
		case GameUtils.DirEightWay.N:
			zero.x = 2;
			goto IL_122;
		case GameUtils.DirEightWay.E:
			zero.z = -2;
			goto IL_122;
		case GameUtils.DirEightWay.S:
			zero.x = -2;
			goto IL_122;
		case GameUtils.DirEightWay.W:
			zero.z = 2;
			goto IL_122;
		}
		throw new ArgumentOutOfRangeException();
		IL_122:
		Vector3i vector3i = selectionStart;
		blockValue.rotation = 0;
		do
		{
			ConsoleCmdPlaceBlockRotations.PlaceBlock(blockValue, itemBlockInventoryData, vector3i, primaryPlayer);
			int num = 0;
			blockValue.rotation = blockValue.Block.BlockPlacementHelper.LimitRotation(BlockPlacement.EnumRotationMode.Advanced, ref num, default(HitInfoDetails), true, blockValue, blockValue.rotation);
			vector3i += zero;
		}
		while (blockValue.rotation != 0);
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x00066410 File Offset: 0x00064610
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PlaceBlock(BlockValue _blockValue, ItemClassBlock.ItemBlockInventoryData _holdingData, Vector3i _placementPos, EntityPlayerLocal _player)
	{
		BlockPlacement.Result result = new BlockPlacement.Result
		{
			clrIdx = 0,
			blockValue = _blockValue,
			blockPos = _placementPos
		};
		Block block = _blockValue.Block;
		block.OnBlockPlaceBefore(GameManager.Instance.World, ref result, _player, GameManager.Instance.World.GetGameRandom());
		_blockValue = result.blockValue;
		if (_holdingData.itemValue.TextureFullArray.IsDefault || Block.list[_holdingData.itemValue.type].SelectAlternates)
		{
			block.PlaceBlock(GameManager.Instance.World, result, _player);
			return;
		}
		BlockChangeInfo item = new BlockChangeInfo(0, _placementPos, _blockValue)
		{
			textureFull = _holdingData.itemValue.TextureFullArray,
			bChangeTexture = true
		};
		GameManager.Instance.World.SetBlocksRPC(new List<BlockChangeInfo>
		{
			item
		});
	}
}

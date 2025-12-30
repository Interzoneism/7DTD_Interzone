using System;
using UnityEngine.Scripting;
using XMLData;

// Token: 0x02000C9F RID: 3231
[Preserve]
public class XUiC_DoorEditor : XUiController
{
	// Token: 0x060063B2 RID: 25522 RVA: 0x00286C6C File Offset: 0x00284E6C
	public override void Init()
	{
		base.Init();
		XUiC_DoorEditor.ID = base.WindowGroup.ID;
		this.btnDowngrade = base.GetChildById("btnDowngrade").GetChildByType<XUiC_SimpleButton>();
		this.btnDowngrade.OnPressed += this.BtnDowngrade_OnPressed;
		this.btnUpgrade = base.GetChildById("btnUpgrade").GetChildByType<XUiC_SimpleButton>();
		this.btnUpgrade.OnPressed += this.BtnUpgrade_OnPressed;
		this.cbxColorPresetList = (XUiC_ComboBoxList<string>)base.GetChildById("cbxPresets");
		this.cbxColorPresetList.OnValueChanged += this.CbxColorPresetList_OnValueChanged;
		foreach (string item in ColorMappingData.Instance.IDFromName.Keys)
		{
			this.cbxColorPresetList.Elements.Add(item);
		}
		this.btnOpenClose = base.GetChildById("btnOpenClose").GetChildByType<XUiC_SimpleButton>();
		this.btnOpenClose.OnPressed += this.BtnOpenClose_OnPressed;
		this.btnCancel = base.GetChildById("btnCancel").GetChildByType<XUiC_SimpleButton>();
		this.btnCancel.OnPressed += this.BtnCancel_OnPressed;
		this.btnOk = base.GetChildById("btnOk").GetChildByType<XUiC_SimpleButton>();
		this.btnOk.OnPressed += this.BtnOk_OnPressed;
	}

	// Token: 0x060063B3 RID: 25523 RVA: 0x00286DF8 File Offset: 0x00284FF8
	public static void Open(LocalPlayerUI _playerUi, TileEntitySecureDoor _te, Vector3i _blockPos, World _world, int _cIdx)
	{
		XUiC_DoorEditor childByType = _playerUi.xui.FindWindowGroupByName(XUiC_DoorEditor.ID).GetChildByType<XUiC_DoorEditor>();
		childByType.world = _world;
		ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
		childByType.chunk = (Chunk)chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z));
		BlockEntityData blockEntity = childByType.chunk.GetBlockEntity(_blockPos);
		childByType.blockPos = _blockPos;
		childByType.initialColorIdx = blockEntity.blockValue.meta2;
		childByType.initialDamage = blockEntity.blockValue.damage;
		childByType.cbxColorPresetList.Value = ColorMappingData.Instance.NameFromID[(int)blockEntity.blockValue.meta2];
		childByType.bAcceptChanges = false;
		_playerUi.windowManager.Open(XUiC_DoorEditor.ID, true, false, true);
	}

	// Token: 0x060063B4 RID: 25524 RVA: 0x00286ED0 File Offset: 0x002850D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxColorPresetList_OnValueChanged(XUiController _sender, string _oldValue, string _newValue)
	{
		int num;
		if (ColorMappingData.Instance.IDFromName.TryGetValue(_newValue, out num) && ColorMappingData.Instance.ColorFromID.ContainsKey(num))
		{
			this.UpdateDoorColor(num);
		}
	}

	// Token: 0x060063B5 RID: 25525 RVA: 0x00286F0A File Offset: 0x0028510A
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDowngrade_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.UpdateDoorHealth(false);
	}

	// Token: 0x060063B6 RID: 25526 RVA: 0x00286F13 File Offset: 0x00285113
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnUpgrade_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.UpdateDoorHealth(true);
	}

	// Token: 0x060063B7 RID: 25527 RVA: 0x00286F1C File Offset: 0x0028511C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateDoorHealth(bool _upgrade)
	{
		BlockEntityData blockEntity = this.chunk.GetBlockEntity(this.blockPos);
		Block block = blockEntity.blockValue.Block;
		BlockShapeModelEntity blockShapeModelEntity = block.shape as BlockShapeModelEntity;
		if (blockShapeModelEntity == null)
		{
			Log.Warning(string.Format("block {0} does not have shape field. Cannot change damage state.", block));
			return;
		}
		int num = _upgrade ? ((int)blockShapeModelEntity.GetNextDamageStateUpHealth(blockEntity.blockValue)) : ((int)blockShapeModelEntity.GetNextDamageStateDownHealth(blockEntity.blockValue));
		blockEntity.blockValue.damage = block.MaxDamage - num;
		blockShapeModelEntity.UpdateDamageState(blockEntity.blockValue, blockEntity.blockValue, blockEntity, false);
		this.UpdateDoorColor((int)blockEntity.blockValue.meta2);
	}

	// Token: 0x060063B8 RID: 25528 RVA: 0x00286FBF File Offset: 0x002851BF
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateDoorColor(int _colorIdx)
	{
		this.chunk.GetBlockEntity(this.blockPos).blockValue.meta2 = (byte)_colorIdx;
	}

	// Token: 0x060063B9 RID: 25529 RVA: 0x00286FE0 File Offset: 0x002851E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetDoorDamage()
	{
		BlockEntityData blockEntity = this.chunk.GetBlockEntity(this.blockPos);
		Block block = blockEntity.blockValue.Block;
		BlockShapeModelEntity blockShapeModelEntity = block.shape as BlockShapeModelEntity;
		if (blockShapeModelEntity == null)
		{
			Log.Warning(string.Format("block {0} does not have shape field. Cannot change damage state.", block));
			return;
		}
		blockEntity.blockValue.damage = this.initialDamage;
		blockShapeModelEntity.UpdateDamageState(blockEntity.blockValue, blockEntity.blockValue, blockEntity, false);
		this.UpdateDoorColor((int)blockEntity.blockValue.meta2);
	}

	// Token: 0x060063BA RID: 25530 RVA: 0x00287064 File Offset: 0x00285264
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnOpenClose_OnPressed(XUiController _sender, int _mouseButton)
	{
		BlockEntityData blockEntity = this.chunk.GetBlockEntity(this.blockPos);
		blockEntity.blockValue.Block.OnBlockActivated("close", this.world, 0, this.blockPos, blockEntity.blockValue, this.world.GetPrimaryPlayer());
	}

	// Token: 0x060063BB RID: 25531 RVA: 0x00269150 File Offset: 0x00267350
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x060063BC RID: 25532 RVA: 0x002870B7 File Offset: 0x002852B7
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnOk_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.bAcceptChanges = true;
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x060063BD RID: 25533 RVA: 0x002870E0 File Offset: 0x002852E0
	public override void OnClose()
	{
		base.OnClose();
		if (this.bAcceptChanges)
		{
			BlockEntityData blockEntity = this.chunk.GetBlockEntity(this.blockPos);
			this.world.SetBlockRPC(this.blockPos, blockEntity.blockValue);
			return;
		}
		this.ResetDoorDamage();
		this.UpdateDoorColor((int)this.initialColorIdx);
	}

	// Token: 0x04004B15 RID: 19221
	public static string ID = "";

	// Token: 0x04004B16 RID: 19222
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDowngrade;

	// Token: 0x04004B17 RID: 19223
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnUpgrade;

	// Token: 0x04004B18 RID: 19224
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> cbxColorPresetList;

	// Token: 0x04004B19 RID: 19225
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnOpenClose;

	// Token: 0x04004B1A RID: 19226
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnCancel;

	// Token: 0x04004B1B RID: 19227
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnOk;

	// Token: 0x04004B1C RID: 19228
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04004B1D RID: 19229
	[PublicizedFrom(EAccessModifier.Private)]
	public Chunk chunk;

	// Token: 0x04004B1E RID: 19230
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x04004B1F RID: 19231
	[PublicizedFrom(EAccessModifier.Private)]
	public byte initialColorIdx;

	// Token: 0x04004B20 RID: 19232
	[PublicizedFrom(EAccessModifier.Private)]
	public int initialDamage;

	// Token: 0x04004B21 RID: 19233
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bAcceptChanges;
}

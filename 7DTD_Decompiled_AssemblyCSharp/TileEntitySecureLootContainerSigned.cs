using System;
using Platform;
using UnityEngine;

// Token: 0x02000B0E RID: 2830
public class TileEntitySecureLootContainerSigned : TileEntitySecureLootContainer, ITileEntitySignable, ITileEntity
{
	// Token: 0x060057B6 RID: 22454 RVA: 0x00238BE1 File Offset: 0x00236DE1
	public TileEntitySecureLootContainerSigned(Chunk _chunk) : base(_chunk)
	{
		this.signText = new AuthoredText();
		PlatformUserManager.BlockedStateChanged += this.UserBlockedStateChanged;
	}

	// Token: 0x060057B7 RID: 22455 RVA: 0x00238C0D File Offset: 0x00236E0D
	public override void OnUnload(World world)
	{
		base.OnUnload(world);
		PlatformUserManager.BlockedStateChanged -= this.UserBlockedStateChanged;
	}

	// Token: 0x060057B8 RID: 22456 RVA: 0x00238C27 File Offset: 0x00236E27
	public override void OnDestroy()
	{
		base.OnDestroy();
		PlatformUserManager.BlockedStateChanged -= this.UserBlockedStateChanged;
	}

	// Token: 0x060057B9 RID: 22457 RVA: 0x00238C40 File Offset: 0x00236E40
	public void SetBlockEntityData(BlockEntityData _blockEntityData)
	{
		if (_blockEntityData != null && _blockEntityData.bHasTransform && !GameManager.IsDedicatedServer)
		{
			Block.MultiBlockArray multiBlockPos = _blockEntityData.blockValue.Block.multiBlockPos;
			float num = (float)((multiBlockPos != null) ? multiBlockPos.dim.x : 1);
			TextMesh[] componentsInChildren = _blockEntityData.transform.GetComponentsInChildren<TextMesh>();
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				return;
			}
			this.smartTextMesh = new SmartTextMesh[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.smartTextMesh[i] = componentsInChildren[i].gameObject.AddComponent<SmartTextMesh>();
				this.smartTextMesh[i].MaxWidth = 0.4f * num;
				this.smartTextMesh[i].MaxLines = this.lineCount;
				this.smartTextMesh[i].ConvertNewLines = true;
				this.smartTextMesh[i].SeperatedLinesMode = false;
			}
			AuthoredText authoredText = this.signText;
			this.RefreshTextMesh((authoredText != null) ? authoredText.Text : null);
		}
	}

	// Token: 0x060057BA RID: 22458 RVA: 0x00238D2B File Offset: 0x00236F2B
	public void SetText(AuthoredText _authoredText, bool _syncData = true)
	{
		this.SetText((_authoredText != null) ? _authoredText.Text : null, _syncData, (_authoredText != null) ? _authoredText.Author : null);
	}

	// Token: 0x060057BB RID: 22459 RVA: 0x00238D4C File Offset: 0x00236F4C
	public void SetText(string _text, bool _syncData = true, PlatformUserIdentifierAbs _signingPlayer = null)
	{
		if (_signingPlayer == null)
		{
			_signingPlayer = PlatformManager.MultiPlatform.User.PlatformUserId;
		}
		if (GameManager.Instance.persistentPlayers.GetPlayerData(_signingPlayer) == null)
		{
			_signingPlayer = null;
			_text = "";
		}
		if (_text == this.signText.Text)
		{
			return;
		}
		this.signText.Update(_text, _signingPlayer);
		GeneratedTextManager.GetDisplayText(this.signText, new Action<string>(this.RefreshTextMesh), true, true, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.NotSupported);
		if (_syncData)
		{
			this.setModified();
		}
	}

	// Token: 0x060057BC RID: 22460 RVA: 0x00238DCE File Offset: 0x00236FCE
	public AuthoredText GetAuthoredText()
	{
		return this.signText;
	}

	// Token: 0x060057BD RID: 22461 RVA: 0x00238DD6 File Offset: 0x00236FD6
	public bool CanRenderString(string _text)
	{
		return this.smartTextMesh.Length != 0 && this.smartTextMesh[0].CanRenderString(_text);
	}

	// Token: 0x060057BE RID: 22462 RVA: 0x00238DF1 File Offset: 0x00236FF1
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.SecureLootSigned;
	}

	// Token: 0x060057BF RID: 22463 RVA: 0x00238DF8 File Offset: 0x00236FF8
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		int num = _br.ReadInt32();
		_br.ReadBoolean();
		_br.ReadBoolean();
		PlatformUserIdentifierAbs.FromStream(_br, false, false);
		if (num > 1)
		{
			this.SetText(AuthoredText.FromStream(_br), false);
		}
		_br.ReadString();
		int num2 = _br.ReadInt32();
		for (int i = 0; i < num2; i++)
		{
			PlatformUserIdentifierAbs.FromStream(_br, false, false);
		}
		if (num <= 1)
		{
			this.SetText(_br.ReadString(), false, null);
		}
	}

	// Token: 0x060057C0 RID: 22464 RVA: 0x00238E74 File Offset: 0x00237074
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(2);
		_bw.Write(this.bPlayerPlaced);
		_bw.Write(this.isLocked);
		this.ownerID.ToStream(_bw, false);
		AuthoredText.ToStream(this.signText, _bw);
		_bw.Write(this.password);
		_bw.Write(this.allowedUserIds.Count);
		for (int i = 0; i < this.allowedUserIds.Count; i++)
		{
			this.allowedUserIds[i].ToStream(_bw, false);
		}
	}

	// Token: 0x060057C1 RID: 22465 RVA: 0x00238F07 File Offset: 0x00237107
	[PublicizedFrom(EAccessModifier.Private)]
	public void UserBlockedStateChanged(IPlatformUserData userData, EBlockType blockType, EUserBlockState blockState)
	{
		if (!userData.PrimaryId.Equals(this.signText.Author) || blockType != EBlockType.TextChat)
		{
			return;
		}
		GeneratedTextManager.GetDisplayText(this.signText, new Action<string>(this.RefreshTextMesh), true, true, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.NotSupported);
	}

	// Token: 0x060057C2 RID: 22466 RVA: 0x00238F40 File Offset: 0x00237140
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshTextMesh(string _text)
	{
		if (this.smartTextMesh != null && _text != this.smartTextMesh[0].UnwrappedText && !GameManager.IsDedicatedServer)
		{
			for (int i = 0; i < this.smartTextMesh.Length; i++)
			{
				this.smartTextMesh[i].UnwrappedText = _text;
			}
		}
	}

	// Token: 0x060057C3 RID: 22467 RVA: 0x00238F92 File Offset: 0x00237192
	[PublicizedFrom(EAccessModifier.Private)]
	public int get_EntityId()
	{
		return base.EntityId;
	}

	// Token: 0x04004359 RID: 17241
	[PublicizedFrom(EAccessModifier.Private)]
	public new const int ver = 2;

	// Token: 0x0400435A RID: 17242
	[PublicizedFrom(EAccessModifier.Private)]
	public AuthoredText signText;

	// Token: 0x0400435B RID: 17243
	public int lineCount = 3;

	// Token: 0x0400435C RID: 17244
	[PublicizedFrom(EAccessModifier.Private)]
	public SmartTextMesh[] smartTextMesh;
}

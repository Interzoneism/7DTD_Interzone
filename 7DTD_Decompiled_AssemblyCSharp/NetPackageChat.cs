using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200070A RID: 1802
[Preserve]
public class NetPackageChat : NetPackage
{
	// Token: 0x060034FE RID: 13566 RVA: 0x00162214 File Offset: 0x00160414
	public NetPackageChat Setup(EChatType _chatType, int _senderEntityId, string _msg, List<int> _recipientEntityIds, EMessageSender _msgSender, GeneratedTextManager.BbCodeSupportMode _bbMode)
	{
		this.chatType = _chatType;
		this.senderEntityId = _senderEntityId;
		this.msg = (string.IsNullOrEmpty(_msg) ? string.Empty : _msg);
		this.msgSender = _msgSender;
		this.bbMode = _bbMode;
		this.recipientEntityIds = _recipientEntityIds;
		return this;
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x00162254 File Offset: 0x00160454
	public override void read(PooledBinaryReader _br)
	{
		this.chatType = (EChatType)_br.ReadByte();
		this.senderEntityId = _br.ReadInt32();
		this.msg = _br.ReadString();
		this.msgSender = (EMessageSender)_br.ReadByte();
		this.bbMode = (GeneratedTextManager.BbCodeSupportMode)_br.ReadByte();
		int num = _br.ReadInt32();
		if (num > 0)
		{
			this.recipientEntityIds = new List<int>();
			for (int i = 0; i < num; i++)
			{
				this.recipientEntityIds.Add(_br.ReadInt32());
			}
		}
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x001622D0 File Offset: 0x001604D0
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.chatType);
		_bw.Write(this.senderEntityId);
		_bw.Write(this.msg);
		_bw.Write((byte)this.msgSender);
		_bw.Write((byte)this.bbMode);
		_bw.Write((this.recipientEntityIds != null) ? this.recipientEntityIds.Count : 0);
		if (this.recipientEntityIds != null && this.recipientEntityIds.Count > 0)
		{
			for (int i = 0; i < this.recipientEntityIds.Count; i++)
			{
				_bw.Write(this.recipientEntityIds[i]);
			}
		}
	}

	// Token: 0x06003501 RID: 13569 RVA: 0x00162380 File Offset: 0x00160580
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			GameManager.Instance.ChatMessageServer(base.Sender, this.chatType, this.senderEntityId, this.msg, this.recipientEntityIds, this.msgSender, this.bbMode);
			return;
		}
		GameManager.Instance.ChatMessageClient(this.chatType, this.senderEntityId, this.msg, null, this.msgSender, this.bbMode);
	}

	// Token: 0x06003502 RID: 13570 RVA: 0x001623F8 File Offset: 0x001605F8
	public override int GetLength()
	{
		int num = (this.recipientEntityIds == null) ? 0 : this.recipientEntityIds.Count;
		return 7 + this.msg.Length + 4 * num;
	}

	// Token: 0x04002B3E RID: 11070
	[PublicizedFrom(EAccessModifier.Private)]
	public EChatType chatType;

	// Token: 0x04002B3F RID: 11071
	[PublicizedFrom(EAccessModifier.Private)]
	public int senderEntityId;

	// Token: 0x04002B40 RID: 11072
	[PublicizedFrom(EAccessModifier.Private)]
	public string msg;

	// Token: 0x04002B41 RID: 11073
	[PublicizedFrom(EAccessModifier.Private)]
	public EMessageSender msgSender;

	// Token: 0x04002B42 RID: 11074
	[PublicizedFrom(EAccessModifier.Private)]
	public GeneratedTextManager.BbCodeSupportMode bbMode;

	// Token: 0x04002B43 RID: 11075
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> recipientEntityIds;
}

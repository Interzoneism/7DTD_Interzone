using System;
using System.IO;
using UnityEngine;

// Token: 0x020004A6 RID: 1190
public class Faction
{
	// Token: 0x06002706 RID: 9990 RVA: 0x000FD543 File Offset: 0x000FB743
	public Faction()
	{
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x000FD55C File Offset: 0x000FB75C
	public Faction(string _name, bool _playerFaction = false, string _icon = "")
	{
		this.Name = _name;
		this.Icon = _icon;
		this.IsPlayerFaction = _playerFaction;
		for (int i = 0; i < 255; i++)
		{
			this.Relationships[i] = 400f;
		}
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x000FD5B4 File Offset: 0x000FB7B4
	public void ModifyRelationship(byte _factionId, float _modifier)
	{
		float num = this.Relationships[(int)_factionId];
		if (num != 255f)
		{
			num = Mathf.Clamp(num + _modifier, 0f, 1000f);
		}
		this.Relationships[(int)_factionId] = num;
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x000FD5EE File Offset: 0x000FB7EE
	public void SetRelationship(byte _factionId, float _value)
	{
		this.Relationships[(int)_factionId] = (float)((byte)Mathf.Clamp(_value, 0f, 1000f));
	}

	// Token: 0x0600270A RID: 9994 RVA: 0x000FD60A File Offset: 0x000FB80A
	public float GetRelationship(byte _factionId)
	{
		return this.Relationships[(int)_factionId];
	}

	// Token: 0x0600270B RID: 9995 RVA: 0x000FD614 File Offset: 0x000FB814
	public void SetAlly(byte _factionId)
	{
		this.Relationships[(int)_factionId] = 1000f;
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x000FD624 File Offset: 0x000FB824
	public void Write(BinaryWriter bw)
	{
		for (int i = 0; i < 255; i++)
		{
			bw.Write(this.Relationships[i]);
		}
		bw.Write(this.IsPlayerFaction);
	}

	// Token: 0x0600270D RID: 9997 RVA: 0x000FD65C File Offset: 0x000FB85C
	public void Read(BinaryReader br)
	{
		this.Relationships = new float[255];
		for (int i = 0; i < 255; i++)
		{
			this.Relationships[i] = br.ReadSingle();
		}
		this.IsPlayerFaction = br.ReadBoolean();
	}

	// Token: 0x0600270E RID: 9998 RVA: 0x000FD6A4 File Offset: 0x000FB8A4
	public override string ToString()
	{
		return string.Format("{0} : {1}", this.Name, string.Join(", ", Array.ConvertAll<float, string>(this.Relationships, (float x) => x.ToCultureInvariantString())));
	}

	// Token: 0x04001DD5 RID: 7637
	public byte ID;

	// Token: 0x04001DD6 RID: 7638
	public string Name;

	// Token: 0x04001DD7 RID: 7639
	public string Icon;

	// Token: 0x04001DD8 RID: 7640
	public bool IsPlayerFaction;

	// Token: 0x04001DD9 RID: 7641
	public float[] Relationships = new float[255];
}

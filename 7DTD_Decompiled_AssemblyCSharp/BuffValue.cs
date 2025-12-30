using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020005B9 RID: 1465
public class BuffValue
{
	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x06002F11 RID: 12049 RVA: 0x00142CD0 File Offset: 0x00140ED0
	public BuffClass BuffClass
	{
		get
		{
			if (this.cachedBuff == null && !BuffManager.Buffs.TryGetValue(this.buffName, out this.cachedBuff))
			{
				Log.Error("Buff Class not found for '{0}'", new object[]
				{
					this.buffName
				});
			}
			return this.cachedBuff;
		}
	}

	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x06002F12 RID: 12050 RVA: 0x00142D1C File Offset: 0x00140F1C
	// (set) Token: 0x06002F13 RID: 12051 RVA: 0x00142D29 File Offset: 0x00140F29
	public bool Remove
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.buffFlags & BuffValue.BuffFlags.Remove) > BuffValue.BuffFlags.None;
		}
		set
		{
			if (value)
			{
				this.buffFlags |= BuffValue.BuffFlags.Remove;
				return;
			}
			this.buffFlags &= (BuffValue.BuffFlags)251;
		}
	}

	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x06002F14 RID: 12052 RVA: 0x00142D4F File Offset: 0x00140F4F
	// (set) Token: 0x06002F15 RID: 12053 RVA: 0x00142D5C File Offset: 0x00140F5C
	public bool Finished
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.buffFlags & BuffValue.BuffFlags.Finished) > BuffValue.BuffFlags.None;
		}
		set
		{
			if (value)
			{
				this.buffFlags |= BuffValue.BuffFlags.Finished;
				return;
			}
			this.buffFlags &= (BuffValue.BuffFlags)253;
		}
	}

	// Token: 0x1700049D RID: 1181
	// (get) Token: 0x06002F16 RID: 12054 RVA: 0x00142D82 File Offset: 0x00140F82
	// (set) Token: 0x06002F17 RID: 12055 RVA: 0x00142D8F File Offset: 0x00140F8F
	public bool Started
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.buffFlags & BuffValue.BuffFlags.Started) > BuffValue.BuffFlags.None;
		}
		set
		{
			if (value)
			{
				this.buffFlags |= BuffValue.BuffFlags.Started;
				return;
			}
			this.buffFlags &= (BuffValue.BuffFlags)254;
		}
	}

	// Token: 0x1700049E RID: 1182
	// (get) Token: 0x06002F18 RID: 12056 RVA: 0x00142DB5 File Offset: 0x00140FB5
	// (set) Token: 0x06002F19 RID: 12057 RVA: 0x00142DC3 File Offset: 0x00140FC3
	public bool Invalid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.buffFlags & BuffValue.BuffFlags.Invalid) > BuffValue.BuffFlags.None;
		}
		set
		{
			if (value)
			{
				this.buffFlags |= BuffValue.BuffFlags.Invalid;
				return;
			}
			this.buffFlags &= (BuffValue.BuffFlags)239;
		}
	}

	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x06002F1A RID: 12058 RVA: 0x00142DEA File Offset: 0x00140FEA
	// (set) Token: 0x06002F1B RID: 12059 RVA: 0x00142DF7 File Offset: 0x00140FF7
	public bool Update
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.buffFlags & BuffValue.BuffFlags.Update) > BuffValue.BuffFlags.None;
		}
		set
		{
			if (value)
			{
				this.buffFlags |= BuffValue.BuffFlags.Update;
				return;
			}
			this.buffFlags &= (BuffValue.BuffFlags)247;
		}
	}

	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x06002F1C RID: 12060 RVA: 0x00142E1D File Offset: 0x0014101D
	// (set) Token: 0x06002F1D RID: 12061 RVA: 0x00142E2B File Offset: 0x0014102B
	public bool Paused
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.buffFlags & BuffValue.BuffFlags.Paused) > BuffValue.BuffFlags.None;
		}
		set
		{
			if (value)
			{
				this.buffFlags |= BuffValue.BuffFlags.Paused;
				return;
			}
			this.buffFlags &= (BuffValue.BuffFlags)223;
		}
	}

	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x06002F1E RID: 12062 RVA: 0x00142E52 File Offset: 0x00141052
	// (set) Token: 0x06002F1F RID: 12063 RVA: 0x00142E5A File Offset: 0x0014105A
	public int StackEffectMultiplier
	{
		get
		{
			return (int)this.stackEffectMultiplier;
		}
		set
		{
			this.stackEffectMultiplier = (byte)Mathf.Clamp(value, 0, 255);
		}
	}

	// Token: 0x170004A2 RID: 1186
	// (get) Token: 0x06002F20 RID: 12064 RVA: 0x00142E6F File Offset: 0x0014106F
	public float DurationInSeconds
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.durationTicks / 20f;
		}
	}

	// Token: 0x170004A3 RID: 1187
	// (get) Token: 0x06002F21 RID: 12065 RVA: 0x00142E7F File Offset: 0x0014107F
	// (set) Token: 0x06002F22 RID: 12066 RVA: 0x00142E88 File Offset: 0x00141088
	public uint DurationInTicks
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.durationTicks;
		}
		set
		{
			if (value == 0U)
			{
				this.durationTicks = 0U;
				this.timeSinceLastUpdate = 0;
				return;
			}
			this.timeSinceLastUpdate += (ushort)(value - this.durationTicks);
			this.durationTicks = value;
			if ((int)this.timeSinceLastUpdate == Mathf.FloorToInt(this.BuffClass.UpdateRate * 20f))
			{
				this.Update = true;
				this.timeSinceLastUpdate = 0;
			}
		}
	}

	// Token: 0x170004A4 RID: 1188
	// (get) Token: 0x06002F23 RID: 12067 RVA: 0x00142EF1 File Offset: 0x001410F1
	public string BuffName
	{
		get
		{
			return this.buffName;
		}
	}

	// Token: 0x170004A5 RID: 1189
	// (get) Token: 0x06002F24 RID: 12068 RVA: 0x00142EF9 File Offset: 0x001410F9
	public int InstigatorId
	{
		get
		{
			return this.instigatorId;
		}
	}

	// Token: 0x170004A6 RID: 1190
	// (get) Token: 0x06002F25 RID: 12069 RVA: 0x00142F01 File Offset: 0x00141101
	public Vector3i InstigatorPos
	{
		get
		{
			return this.instigatorPos;
		}
	}

	// Token: 0x06002F26 RID: 12070 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public BuffValue()
	{
	}

	// Token: 0x06002F27 RID: 12071 RVA: 0x00142F0C File Offset: 0x0014110C
	public BuffValue(string _buffEffectGroupId, Vector3i _instigatorPos, int _instigatorId = -1, BuffClass _buffClass = null)
	{
		this.buffName = _buffEffectGroupId;
		this.stackEffectMultiplier = 1;
		this.durationTicks = 0U;
		this.instigatorId = _instigatorId;
		this.buffFlags = BuffValue.BuffFlags.None;
		this.timeSinceLastUpdate = 0;
		this.instigatorPos = _instigatorPos;
		if (_buffClass == null)
		{
			this.cacheBuffClassPointer();
			return;
		}
		this.cachedBuff = _buffClass;
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x00142F63 File Offset: 0x00141163
	public void ClearBuffClassLink()
	{
		this.cachedBuff = null;
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x00142F6C File Offset: 0x0014116C
	[PublicizedFrom(EAccessModifier.Private)]
	public void cacheBuffClassPointer()
	{
		if (!BuffManager.Buffs.TryGetValue(this.buffName, out this.cachedBuff))
		{
			this.Remove = true;
		}
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x00142F90 File Offset: 0x00141190
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(this.buffName);
		_bw.Write(this.stackEffectMultiplier);
		_bw.Write(this.durationTicks);
		_bw.Write(this.instigatorId);
		_bw.Write((byte)this.buffFlags);
		_bw.Write(this.timeSinceLastUpdate);
		StreamUtils.Write(_bw, this.instigatorPos);
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x00142FF4 File Offset: 0x001411F4
	public void Read(BinaryReader _br, int _version)
	{
		if (_version < 2)
		{
			int num = _br.ReadInt32();
			using (Dictionary<string, BuffClass>.KeyCollection.Enumerator enumerator = BuffManager.Buffs.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string text = enumerator.Current;
					if (text.GetHashCode() == num)
					{
						this.buffName = BuffManager.Buffs[text].Name;
						break;
					}
				}
				goto IL_70;
			}
		}
		this.buffName = _br.ReadString().ToLower();
		IL_70:
		this.stackEffectMultiplier = _br.ReadByte();
		this.durationTicks = _br.ReadUInt32();
		this.instigatorId = _br.ReadInt32();
		this.buffFlags = (BuffValue.BuffFlags)_br.ReadByte();
		if (_version == 0)
		{
			this.timeSinceLastUpdate = (ushort)_br.ReadByte();
		}
		else
		{
			this.timeSinceLastUpdate = _br.ReadUInt16();
		}
		if (_version >= 3)
		{
			this.instigatorPos = StreamUtils.ReadVector3i(_br);
		}
		this.cacheBuffClassPointer();
	}

	// Token: 0x04002546 RID: 9542
	[PublicizedFrom(EAccessModifier.Private)]
	public BuffClass cachedBuff;

	// Token: 0x04002547 RID: 9543
	[PublicizedFrom(EAccessModifier.Private)]
	public string buffName;

	// Token: 0x04002548 RID: 9544
	[PublicizedFrom(EAccessModifier.Private)]
	public byte stackEffectMultiplier;

	// Token: 0x04002549 RID: 9545
	[PublicizedFrom(EAccessModifier.Private)]
	public uint durationTicks;

	// Token: 0x0400254A RID: 9546
	[PublicizedFrom(EAccessModifier.Private)]
	public int instigatorId;

	// Token: 0x0400254B RID: 9547
	[PublicizedFrom(EAccessModifier.Private)]
	public BuffValue.BuffFlags buffFlags;

	// Token: 0x0400254C RID: 9548
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort timeSinceLastUpdate;

	// Token: 0x0400254D RID: 9549
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i instigatorPos;

	// Token: 0x020005BA RID: 1466
	[PublicizedFrom(EAccessModifier.Private)]
	public enum BuffFlags : byte
	{
		// Token: 0x0400254F RID: 9551
		None,
		// Token: 0x04002550 RID: 9552
		Started,
		// Token: 0x04002551 RID: 9553
		Finished,
		// Token: 0x04002552 RID: 9554
		Remove = 4,
		// Token: 0x04002553 RID: 9555
		Update = 8,
		// Token: 0x04002554 RID: 9556
		Invalid = 16,
		// Token: 0x04002555 RID: 9557
		Paused = 32
	}
}

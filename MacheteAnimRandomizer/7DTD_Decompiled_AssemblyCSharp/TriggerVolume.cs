using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class TriggerVolume
{
	// Token: 0x06000C8E RID: 3214 RVA: 0x00055AAF File Offset: 0x00053CAF
	public static TriggerVolume Create(Prefab.PrefabTriggerVolume psv, Vector3i _boxMin, Vector3i _boxMax)
	{
		TriggerVolume triggerVolume = new TriggerVolume();
		triggerVolume.SetMinMax(_boxMin, _boxMax);
		triggerVolume.TriggersIndices = new List<byte>(psv.TriggersIndices);
		triggerVolume.AddToPrefabInstance();
		return triggerVolume;
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x00055AD8 File Offset: 0x00053CD8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMinMax(Vector3i _boxMin, Vector3i _boxMax)
	{
		this.BoxMin = _boxMin;
		this.BoxMax = _boxMax;
		this.Center = ((this.BoxMin + this.BoxMax) * 0.5f).ToVector3();
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06000C90 RID: 3216 RVA: 0x00055B1C File Offset: 0x00053D1C
	public PrefabInstance PrefabInstance
	{
		get
		{
			return this.prefabInstance;
		}
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00055B24 File Offset: 0x00053D24
	public void AddToPrefabInstance()
	{
		this.prefabInstance = GameManager.Instance.World.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator().GetPrefabAtPosition(this.Center, true);
		if (this.prefabInstance != null)
		{
			this.prefabInstance.AddTriggerVolume(this);
		}
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x00055B70 File Offset: 0x00053D70
	public void Reset()
	{
		this.isTriggered = false;
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x00055B79 File Offset: 0x00053D79
	public bool HasAnyTriggers()
	{
		return this.TriggersIndices.Count > 0;
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x00055B8C File Offset: 0x00053D8C
	public void CheckTouching(World _world, EntityPlayer _player)
	{
		if (this.isTriggered)
		{
			return;
		}
		Vector3 position = _player.position;
		position.y += 0.8f;
		if (position.x >= (float)this.BoxMin.x && position.x < (float)this.BoxMax.x && position.y >= (float)this.BoxMin.y && position.y < (float)this.BoxMax.y && position.z >= (float)this.BoxMin.z && position.z < (float)this.BoxMax.z)
		{
			this.Touch(_world, _player);
		}
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x00055C39 File Offset: 0x00053E39
	public bool Intersects(Bounds bounds)
	{
		return BoundsUtils.Intersects(bounds, this.BoxMin, this.BoxMax);
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x00055C57 File Offset: 0x00053E57
	[PublicizedFrom(EAccessModifier.Private)]
	public void Touch(World _world, EntityPlayer _player)
	{
		this.isTriggered = true;
		_world.triggerManager.TriggerBlocks(_player, this.prefabInstance, this);
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x00055C74 File Offset: 0x00053E74
	public static TriggerVolume Read(BinaryReader _br)
	{
		TriggerVolume triggerVolume = new TriggerVolume();
		int num = (int)_br.ReadByte();
		triggerVolume.SetMinMax(new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32()), new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32()));
		int num2 = (int)_br.ReadByte();
		triggerVolume.TriggersIndices.Clear();
		for (int i = 0; i < num2; i++)
		{
			triggerVolume.TriggersIndices.Add(_br.ReadByte());
		}
		if (num > 1)
		{
			triggerVolume.isTriggered = _br.ReadBoolean();
		}
		return triggerVolume;
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x00055D04 File Offset: 0x00053F04
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(2);
		_bw.Write(this.BoxMin.x);
		_bw.Write(this.BoxMin.y);
		_bw.Write(this.BoxMin.z);
		_bw.Write(this.BoxMax.x);
		_bw.Write(this.BoxMax.y);
		_bw.Write(this.BoxMax.z);
		_bw.Write((byte)this.TriggersIndices.Count);
		for (int i = 0; i < this.TriggersIndices.Count; i++)
		{
			_bw.Write(this.TriggersIndices[i]);
		}
		_bw.Write(this.isTriggered);
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x00055DC4 File Offset: 0x00053FC4
	[PublicizedFrom(EAccessModifier.Private)]
	public void DrawVolume()
	{
		Vector3 vector = this.BoxMin.ToVector3();
		vector -= Origin.position;
		Vector3 vector2 = this.BoxMax.ToVector3();
		vector2 -= Origin.position;
		Debug.DrawLine(vector, new Vector3(vector.x, vector.y, vector2.z), Color.blue, 1f);
		Debug.DrawLine(vector, new Vector3(vector2.x, vector.y, vector.z), Color.blue, 1f);
		Debug.DrawLine(new Vector3(vector.x, vector.y, vector2.z), new Vector3(vector2.x, vector.y, vector2.z), Color.blue, 1f);
		Debug.DrawLine(new Vector3(vector2.x, vector.y, vector.z), new Vector3(vector2.x, vector.y, vector2.z), Color.blue, 1f);
		Debug.DrawLine(new Vector3(vector.x, vector2.y, vector.z), new Vector3(vector.x, vector2.y, vector2.z), Color.cyan, 1f);
		Debug.DrawLine(new Vector3(vector.x, vector2.y, vector.z), new Vector3(vector2.x, vector2.y, vector.z), Color.cyan, 1f);
		Debug.DrawLine(new Vector3(vector.x, vector2.y, vector2.z), new Vector3(vector2.x, vector2.y, vector2.z), Color.cyan, 1f);
		Debug.DrawLine(new Vector3(vector2.x, vector2.y, vector.z), new Vector3(vector2.x, vector2.y, vector2.z), Color.cyan, 1f);
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x00055FC0 File Offset: 0x000541C0
	public void DrawDebugLines(float _duration)
	{
		string name = string.Format("TriggerVolume{0},{1}", this.BoxMin, this.BoxMax);
		Color color = new Color(0.1f, 0.1f, 1f);
		if (this.isTriggered)
		{
			color = new Color(0f, 0f, 0.5f, 0.16f);
		}
		Vector3 vector = this.BoxMin.ToVector3();
		Vector3 vector2 = this.BoxMax.ToVector3();
		vector += DebugLines.InsideOffsetV * 2f;
		vector2 -= DebugLines.InsideOffsetV * 2f;
		DebugLines.Create(name, GameManager.Instance.World.GetPrimaryPlayer().RootTransform, color, color, 0.03f, 0.03f, _duration).AddCube(vector, vector2);
	}

	// Token: 0x04000A6F RID: 2671
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte VERSION = 2;

	// Token: 0x04000A70 RID: 2672
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cPlayerYOffset = 0.8f;

	// Token: 0x04000A71 RID: 2673
	public static Vector3i chunkPadding = new Vector3i(12, 1, 12);

	// Token: 0x04000A72 RID: 2674
	[PublicizedFrom(EAccessModifier.Private)]
	public PrefabInstance prefabInstance;

	// Token: 0x04000A73 RID: 2675
	public List<byte> TriggersIndices = new List<byte>();

	// Token: 0x04000A74 RID: 2676
	public Vector3i BoxMin;

	// Token: 0x04000A75 RID: 2677
	public Vector3i BoxMax;

	// Token: 0x04000A76 RID: 2678
	public Vector3 Center;

	// Token: 0x04000A77 RID: 2679
	public bool isTriggered;
}

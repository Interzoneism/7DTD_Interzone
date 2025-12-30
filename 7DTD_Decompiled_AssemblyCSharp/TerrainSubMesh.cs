using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A4D RID: 2637
public struct TerrainSubMesh
{
	// Token: 0x0600508F RID: 20623 RVA: 0x002008D3 File Offset: 0x001FEAD3
	public TerrainSubMesh(List<TerrainSubMesh> _others, int _minSize = 0)
	{
		this.others = _others;
		this.textureIds = new ArrayDynamicFast<int>(TerrainSubMesh.vertexColors.Length);
		this.triangles = new ArrayListMP<int>(MemoryPools.poolInt, _minSize);
		this.needToAdd = new ArrayListMP<int>(MemoryPools.poolInt, 0);
	}

	// Token: 0x06005090 RID: 20624 RVA: 0x00200910 File Offset: 0x001FEB10
	public bool Contains(IList<int> _texIds)
	{
		for (int i = 0; i < _texIds.Count; i++)
		{
			if (_texIds[i] != -1 && this.textureIds.Contains(_texIds[i]) == -1)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06005091 RID: 20625 RVA: 0x00200950 File Offset: 0x001FEB50
	public bool CanAdd(IList<int> _texIds)
	{
		this.needToAdd.Clear();
		for (int i = 0; i < _texIds.Count; i++)
		{
			if (_texIds[i] != -1 && this.textureIds.Contains(_texIds[i]) == -1)
			{
				this.needToAdd.Add(_texIds[i]);
			}
		}
		if (this.needToAdd.Count == 0)
		{
			return true;
		}
		if (this.needToAdd.Count <= TerrainSubMesh.vertexColors.Length - this.textureIds.Count)
		{
			this.Add(this.needToAdd);
			return true;
		}
		return false;
	}

	// Token: 0x06005092 RID: 20626 RVA: 0x002009E8 File Offset: 0x001FEBE8
	public void Add(int[] _texIds)
	{
		foreach (int num in _texIds)
		{
			if (num != -1)
			{
				int num2 = -1;
				int num3 = 0;
				while (num2 == -1 && num3 < this.others.Count)
				{
					num2 = this.others[num3].textureIds.Contains(num);
					if (num2 != -1 && this.textureIds.DataAvail[num2])
					{
						num2 = -1;
					}
					num3++;
				}
				this.textureIds.Add(num2, num);
			}
		}
	}

	// Token: 0x06005093 RID: 20627 RVA: 0x00200A64 File Offset: 0x001FEC64
	public void Add(ArrayListMP<int> _texIds)
	{
		for (int i = 0; i < _texIds.Count; i++)
		{
			int num = _texIds[i];
			if (num != -1)
			{
				int num2 = -1;
				int num3 = 0;
				while (num2 == -1 && num3 < this.others.Count)
				{
					num2 = this.others[num3].textureIds.Contains(num);
					if (num2 != -1 && this.textureIds.DataAvail[num2])
					{
						num2 = -1;
					}
					num3++;
				}
				this.textureIds.Add(num2, num);
			}
		}
	}

	// Token: 0x06005094 RID: 20628 RVA: 0x00200AE4 File Offset: 0x001FECE4
	public Color GetColorForTextureId(int _texId)
	{
		int num = this.textureIds.Contains(_texId);
		if (num != -1)
		{
			return TerrainSubMesh.vertexColors[num];
		}
		return TerrainSubMesh.vertexColors[0];
	}

	// Token: 0x06005095 RID: 20629 RVA: 0x00200B19 File Offset: 0x001FED19
	public int GetTextureIdCount()
	{
		return this.textureIds.Count;
	}

	// Token: 0x06005096 RID: 20630 RVA: 0x00200B28 File Offset: 0x001FED28
	public int GetTextureId(int _idx)
	{
		for (int i = 0; i < this.textureIds.Size; i++)
		{
			if (this.textureIds.DataAvail[i] && _idx == 0)
			{
				return this.textureIds.Data[i];
			}
			_idx--;
		}
		return 0;
	}

	// Token: 0x04003DB1 RID: 15793
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Color[] vertexColors = new Color[]
	{
		new Color(0f, 0f, 0f, 0f),
		new Color(1f, 0f, 0f, 0f),
		new Color(0f, 1f, 0f, 0f)
	};

	// Token: 0x04003DB2 RID: 15794
	public ArrayDynamicFast<int> textureIds;

	// Token: 0x04003DB3 RID: 15795
	public ArrayListMP<int> triangles;

	// Token: 0x04003DB4 RID: 15796
	[PublicizedFrom(EAccessModifier.Private)]
	public ArrayListMP<int> needToAdd;

	// Token: 0x04003DB5 RID: 15797
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TerrainSubMesh> others;
}

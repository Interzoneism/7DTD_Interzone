using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000AC5 RID: 2757
public class DistantTerrainQuadTree
{
	// Token: 0x060054DE RID: 21726 RVA: 0x0022AB14 File Offset: 0x00228D14
	public DistantTerrainQuadTree(int AreaXSize, int AreaYSize, int ElementXSize, int ElementYSize)
	{
		this.AreaSize = new int[]
		{
			AreaXSize,
			AreaYSize
		};
		this.ElementSize = new int[]
		{
			ElementXSize,
			ElementYSize
		};
		this.NbTreeLevel = Mathf.Max(Mathf.CeilToInt((float)AreaXSize / (float)ElementXSize), Mathf.CeilToInt((float)AreaYSize / (float)ElementYSize));
		this.NbTreeLevel = Mathf.CeilToInt(Mathf.Log((float)this.NbTreeLevel, 2f));
		this.ExtendedElementSizeX = (int)(Mathf.Pow(2f, (float)this.NbTreeLevel) * (float)ElementXSize);
		this.ExtendedElementSizeY = (int)(Mathf.Pow(2f, (float)this.NbTreeLevel) * (float)ElementYSize);
		this.NbPosBit = Mathf.CeilToInt(Mathf.Log((float)this.ExtendedElementSizeX, 2f)) - 1;
		this.DataSize = ElementXSize * ElementYSize;
		this.ObjList = new List<object[]>();
		this.Root = new object[4];
	}

	// Token: 0x060054DF RID: 21727 RVA: 0x0022AC0D File Offset: 0x00228E0D
	public void AddChunk(int PosX, int PosY, byte[] Data)
	{
		this.AddElement(PosX * this.ElementSize[0] + this.AreaSize[0] / 2, PosY * this.ElementSize[1] + this.AreaSize[1] / 2, Data);
	}

	// Token: 0x060054E0 RID: 21728 RVA: 0x0022AC40 File Offset: 0x00228E40
	public void AddElement(int PosX, int PosY, byte[] Data)
	{
		int num = this.ExtendedElementSizeX;
		int num2 = this.ExtendedElementSizeY;
		int num3 = 0;
		int num4 = 0;
		this.CurNode = this.Root;
		this.NodeSeqList.Clear();
		int num5;
		for (int i = 0; i < this.NbTreeLevel - 1; i++)
		{
			num5 = 0;
			num >>= 1;
			if (PosX >= num3 + num)
			{
				num3 += num;
				num5++;
			}
			num2 >>= 1;
			if (PosY >= num4 + num2)
			{
				num4 += num2;
				num5 += 2;
			}
			if (this.CurNode[num5] == null)
			{
				this.CurNode[num5] = new object[4];
			}
			this.CurNode = (object[])this.CurNode[num5];
			this.NodeSeqList.Add(num5);
		}
		num5 = 0;
		num >>= 1;
		if (PosX >= num3 + num)
		{
			num5++;
		}
		num2 >>= 1;
		if (PosY >= num4 + num2)
		{
			num5 += 2;
		}
		this.NodeSeqList.Add(num5);
		if (this.CurNode[num5] == null)
		{
			this.CurNode[num5] = new QTDataElement(PosX, PosY, new byte[this.DataSize]);
			this.NbTreeElement++;
		}
		((QTDataElement)this.CurNode[num5]).Data = Data;
	}

	// Token: 0x060054E1 RID: 21729 RVA: 0x0022AD70 File Offset: 0x00228F70
	public QTDataElement GetElement(uint PosX, uint PosY)
	{
		int num = this.ExtendedElementSizeX;
		int num2 = this.ExtendedElementSizeY;
		int num3 = 0;
		int num4 = 0;
		this.CurNode = this.Root;
		int num5;
		for (int i = 0; i < this.NbTreeLevel - 1; i++)
		{
			num5 = 0;
			num >>= 1;
			if ((ulong)PosX >= (ulong)((long)(num3 + num)))
			{
				num3 += num;
				num5++;
			}
			num2 >>= 1;
			if ((ulong)PosY >= (ulong)((long)(num4 + num2)))
			{
				num4 += num2;
				num5 += 2;
			}
			if (this.CurNode[num5] == null)
			{
				return null;
			}
			this.CurNode = (object[])this.CurNode[num5];
		}
		num5 = 0;
		num >>= 1;
		if ((ulong)PosX >= (ulong)((long)(num3 + num)))
		{
			num5++;
		}
		num2 >>= 1;
		if ((ulong)PosY >= (ulong)((long)(num4 + num2)))
		{
			num5 += 2;
		}
		if (this.CurNode[num5] == null)
		{
			return null;
		}
		return (QTDataElement)this.CurNode[num5];
	}

	// Token: 0x060054E2 RID: 21730 RVA: 0x0022AE48 File Offset: 0x00229048
	public List<QTDataElement> GetAllElementFromLevelId(uint PosX, uint PosY, int LevelIdFromLeaf)
	{
		object nodeFromLevelId = this.GetNodeFromLevelId(PosX, PosY, LevelIdFromLeaf);
		if (nodeFromLevelId == null)
		{
			return null;
		}
		List<QTDataElement> list = new List<QTDataElement>();
		if (LevelIdFromLeaf == 0)
		{
			list.Add((QTDataElement)nodeFromLevelId);
			return list;
		}
		this.ObjList.Clear();
		this.ObjList.Add((object[])nodeFromLevelId);
		int num = 0;
		int count;
		for (int i = 0; i < LevelIdFromLeaf - 1; i++)
		{
			count = this.ObjList.Count;
			for (int j = num; j < count; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					if (this.ObjList[j][k] != null)
					{
						this.ObjList.Add((object[])this.ObjList[j][k]);
					}
				}
			}
			num = count;
		}
		count = this.ObjList.Count;
		for (int l = num; l < count; l++)
		{
			for (int m = 0; m < 4; m++)
			{
				if (this.ObjList[l][m] != null)
				{
					list.Add((QTDataElement)this.ObjList[l][m]);
				}
			}
		}
		return list;
	}

	// Token: 0x060054E3 RID: 21731 RVA: 0x0022AF68 File Offset: 0x00229168
	public object GetNodeFromLevelId(uint PosX, uint PosY, int LevelIdFromLeaf)
	{
		int num = this.ExtendedElementSizeX;
		int num2 = this.ExtendedElementSizeY;
		int num3 = 0;
		int num4 = 0;
		object obj = this.Root;
		this.NodeSeqList.Clear();
		for (int i = 0; i < this.NbTreeLevel - LevelIdFromLeaf; i++)
		{
			int num5 = 0;
			num >>= 1;
			if ((ulong)PosX >= (ulong)((long)(num3 + num)))
			{
				num3 += num;
				num5++;
			}
			num2 >>= 1;
			if ((ulong)PosY >= (ulong)((long)(num4 + num2)))
			{
				num4 += num2;
				num5 += 2;
			}
			this.NodeSeqList.Add(num5);
			if (((object[])obj)[num5] == null)
			{
				return null;
			}
			obj = ((object[])obj)[num5];
		}
		return obj;
	}

	// Token: 0x060054E4 RID: 21732 RVA: 0x0022B00C File Offset: 0x0022920C
	public byte[] GetAllElementFromLevelId(uint PosX, uint PosY)
	{
		PosX <<= 32 - this.NbPosBit;
		PosY <<= 32 - this.NbPosBit;
		this.CurNode = this.Root;
		for (int i = 0; i < this.NbTreeLevel - 1; i++)
		{
			uint num = ((PosX & 2147483648U) >> 31) + ((PosY & 2147483648U) >> 30);
			PosX <<= 1;
			PosY <<= 1;
			if (this.CurNode[(int)num] == null)
			{
				return null;
			}
			this.CurNode = (object[])this.CurNode[(int)num];
		}
		return (byte[])this.CurNode[(int)(((PosX & 2147483648U) >> 31) + ((PosY & 2147483648U) >> 30))];
	}

	// Token: 0x060054E5 RID: 21733 RVA: 0x0022B0B8 File Offset: 0x002292B8
	public List<QTDataElement> SetRandomData(byte DefaultHeight, int NbChunk)
	{
		int num = 50;
		int num2 = this.AreaSize[0] / this.ElementSize[0];
		int num3 = this.AreaSize[1] / this.ElementSize[1];
		List<QTDataElement> list = new List<QTDataElement>();
		GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(0);
		int[] array = new int[2];
		List<int> list2 = new List<int>();
		while (list.Count < NbChunk)
		{
			int num4 = Mathf.Min(gameRandom.RandomRange(1, num + 1), NbChunk - list.Count);
			array[0] = gameRandom.RandomRange(0, num2);
			array[1] = gameRandom.RandomRange(0, num3);
			list2.Clear();
			list2.Add(gameRandom.RandomRange(0, num2) + gameRandom.RandomRange(0, num3) * 1000);
			while (list2.Count < num4)
			{
				int index = gameRandom.RandomRange(0, list2.Count);
				int num5 = gameRandom.RandomRange(-1, 2);
				int num6 = gameRandom.RandomRange(-1, 2);
				if (list2[index] % 1000 + num5 >= 0 && list2[index] % 1000 + num5 < num2 && list2[index] / 1000 + num6 >= 0 && list2[index] / 1000 + num6 < num3)
				{
					list2.Add(list2[index] % 1000 + num5 + (list2[index] / 1000 + num6) * 1000);
					list2 = list2.Distinct<int>().ToList<int>();
				}
			}
			for (int i = 0; i < list2.Count; i++)
			{
				QTDataElement qtdataElement = new QTDataElement();
				qtdataElement.Data = new byte[256];
				for (int j = 0; j < 256; j++)
				{
					qtdataElement.Data[j] = DefaultHeight;
				}
				qtdataElement.Key = WorldChunkCache.MakeChunkKey(list2[i] % 1000 * this.ElementSize[0], list2[i] / 1000 * this.ElementSize[1]);
				list.Add(qtdataElement);
			}
		}
		return list;
	}

	// Token: 0x060054E6 RID: 21734 RVA: 0x0022B2E0 File Offset: 0x002294E0
	public void CreateFile(List<QTDataElement> NewData, string DirName, string FileName)
	{
		for (int i = 0; i < NewData.Count; i++)
		{
			this.DataBase.SetDS(NewData[i].Key, NewData[i].Data);
		}
		this.DataBase.Save(DirName, FileName);
	}

	// Token: 0x060054E7 RID: 21735 RVA: 0x0022B330 File Offset: 0x00229530
	public void SetQuadtreeFromDatabase(string DirName, string FileName)
	{
		this.DataBase = new ExtensionChunkDatabase(4660, this.AreaSize[0], this.AreaSize[1], this.ElementSize[0]);
		this.DataBase.Load(DirName, FileName);
		List<long> allKeys = this.DataBase.GetAllKeys();
		for (int i = 0; i < allKeys.Count; i++)
		{
			byte[] ds = this.DataBase.GetDS(allKeys[i]);
			int posX = WorldChunkCache.extractX(allKeys[i]);
			int posY = WorldChunkCache.extractZ(allKeys[i]);
			this.AddElement(posX, posY, ds);
		}
	}

	// Token: 0x040041B9 RID: 16825
	public object[] Root;

	// Token: 0x040041BA RID: 16826
	public int[] AreaSize;

	// Token: 0x040041BB RID: 16827
	public int[] ElementSize;

	// Token: 0x040041BC RID: 16828
	public int ExtendedElementSizeX;

	// Token: 0x040041BD RID: 16829
	public int ExtendedElementSizeY;

	// Token: 0x040041BE RID: 16830
	public int DataSize;

	// Token: 0x040041BF RID: 16831
	public int NbTreeLevel;

	// Token: 0x040041C0 RID: 16832
	public int NbPosBit;

	// Token: 0x040041C1 RID: 16833
	public int NbTreeElement;

	// Token: 0x040041C2 RID: 16834
	[PublicizedFrom(EAccessModifier.Private)]
	public object[] CurNode;

	// Token: 0x040041C3 RID: 16835
	[PublicizedFrom(EAccessModifier.Private)]
	public List<object[]> ObjList;

	// Token: 0x040041C4 RID: 16836
	[PublicizedFrom(EAccessModifier.Private)]
	public DatabaseWithFixedDS<long, byte[]> DataBase;

	// Token: 0x040041C5 RID: 16837
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> NodeSeqList = new List<int>();
}

using System;
using System.Collections.Generic;

// Token: 0x02001114 RID: 4372
public class ArrayDynamicFast<T>
{
	// Token: 0x06008977 RID: 35191 RVA: 0x0037BE0F File Offset: 0x0037A00F
	public ArrayDynamicFast(int _size)
	{
		this.Size = _size;
		this.Data = new T[_size];
		this.DataAvail = new bool[_size];
		this.Count = 0;
	}

	// Token: 0x06008978 RID: 35192 RVA: 0x0037BE40 File Offset: 0x0037A040
	public int Contains(T _v)
	{
		if (this.Count == 0)
		{
			return -1;
		}
		if (_v == null)
		{
			for (int i = 0; i < this.Data.Length; i++)
			{
				if (this.DataAvail[i] && this.Data[i] == null)
				{
					return i;
				}
			}
		}
		else
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int j = 0; j < this.Data.Length; j++)
			{
				if (this.DataAvail[j] && @default.Equals(this.Data[j], _v))
				{
					return j;
				}
			}
		}
		return -1;
	}

	// Token: 0x06008979 RID: 35193 RVA: 0x0037BED0 File Offset: 0x0037A0D0
	public void Clear()
	{
		for (int i = 0; i < this.Data.Length; i++)
		{
			this.DataAvail[i] = false;
		}
	}

	// Token: 0x0600897A RID: 35194 RVA: 0x0037BEFC File Offset: 0x0037A0FC
	public void Add(int _idx, T _texId)
	{
		if (_idx == -1)
		{
			for (int i = 0; i < this.Size; i++)
			{
				if (!this.DataAvail[i])
				{
					_idx = i;
					break;
				}
			}
		}
		if (_idx == -1)
		{
			return;
		}
		this.Data[_idx] = _texId;
		this.DataAvail[_idx] = true;
		this.Count++;
	}

	// Token: 0x04006BCF RID: 27599
	public T[] Data;

	// Token: 0x04006BD0 RID: 27600
	public bool[] DataAvail;

	// Token: 0x04006BD1 RID: 27601
	public int Count;

	// Token: 0x04006BD2 RID: 27602
	public int Size;
}

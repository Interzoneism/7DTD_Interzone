using System;
using System.Collections.Generic;

namespace SDF
{
	// Token: 0x020013D3 RID: 5075
	public class SdfData
	{
		// Token: 0x06009E92 RID: 40594 RVA: 0x003F0610 File Offset: 0x003EE810
		public SdfData()
		{
			this.Nodes = new Dictionary<string, SdfTag>();
		}

		// Token: 0x06009E93 RID: 40595 RVA: 0x003F0624 File Offset: 0x003EE824
		public bool Add(SdfTag sdfTag)
		{
			if (this.Nodes.ContainsKey(sdfTag.Name))
			{
				this.Nodes[sdfTag.Name].Value = sdfTag.Value;
			}
			else
			{
				this.Nodes.Add(sdfTag.Name, sdfTag);
			}
			return true;
		}

		// Token: 0x06009E94 RID: 40596 RVA: 0x003F0675 File Offset: 0x003EE875
		public bool Remove(string tagName)
		{
			if (!this.Nodes.ContainsKey(tagName))
			{
				return false;
			}
			this.Nodes.Remove(tagName);
			return true;
		}

		// Token: 0x06009E95 RID: 40597 RVA: 0x003F0698 File Offset: 0x003EE898
		public int? GetInt(string tagName)
		{
			if (!this.Nodes.ContainsKey(tagName))
			{
				return null;
			}
			if (this.Nodes[tagName].TagType != SdfTagType.Int)
			{
				return null;
			}
			return new int?(Convert.ToInt32(this.Nodes[tagName].Value));
		}

		// Token: 0x06009E96 RID: 40598 RVA: 0x003F06F8 File Offset: 0x003EE8F8
		public float? GetFloat(string tagName)
		{
			if (!this.Nodes.ContainsKey(tagName))
			{
				return null;
			}
			if (this.Nodes[tagName].TagType != SdfTagType.Float)
			{
				return null;
			}
			return new float?((float)this.Nodes[tagName].Value);
		}

		// Token: 0x06009E97 RID: 40599 RVA: 0x003F0756 File Offset: 0x003EE956
		public string GetString(string tagName)
		{
			if (!this.Nodes.ContainsKey(tagName))
			{
				return null;
			}
			if (this.Nodes[tagName].TagType != SdfTagType.String)
			{
				return null;
			}
			return this.Nodes[tagName].Value.ToString();
		}

		// Token: 0x06009E98 RID: 40600 RVA: 0x003F0794 File Offset: 0x003EE994
		public bool? GetBool(string tagName)
		{
			if (!this.Nodes.ContainsKey(tagName))
			{
				return null;
			}
			if (this.Nodes[tagName].TagType != SdfTagType.Bool)
			{
				return null;
			}
			return new bool?((bool)this.Nodes[tagName].Value);
		}

		// Token: 0x06009E99 RID: 40601 RVA: 0x003F07F2 File Offset: 0x003EE9F2
		public string GetBinary(string tagName)
		{
			if (!this.Nodes.ContainsKey(tagName))
			{
				return null;
			}
			if (this.Nodes[tagName].TagType != SdfTagType.Binary)
			{
				return null;
			}
			return (string)this.Nodes[tagName].Value;
		}

		// Token: 0x06009E9A RID: 40602 RVA: 0x003F0830 File Offset: 0x003EEA30
		public byte[] GetByteArray(string tagName)
		{
			if (!this.Nodes.ContainsKey(tagName))
			{
				throw new KeyNotFoundException();
			}
			if (this.Nodes[tagName].TagType == SdfTagType.ByteArray)
			{
				throw new InvalidCastException();
			}
			return (byte[])this.Nodes[tagName].Value;
		}

		// Token: 0x04007A05 RID: 31237
		public Dictionary<string, SdfTag> Nodes;
	}
}

using System;
using System.IO;

namespace SDF
{
	// Token: 0x020013C9 RID: 5065
	public class SdfFile
	{
		// Token: 0x06009E6A RID: 40554 RVA: 0x003EFF4C File Offset: 0x003EE14C
		public void Open(string path)
		{
			try
			{
				this.data = new SdfData();
				this.filePath = path;
				this.valuesChanged = false;
				if (!SdDirectory.Exists(Path.GetDirectoryName(path)))
				{
					SdDirectory.CreateDirectory(Path.GetDirectoryName(path));
				}
				using (Stream stream = SdFile.Open(this.filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
				{
					this.data.Nodes = SdfReader.Read(stream);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error opening SDF file: " + ex.Message);
			}
		}

		// Token: 0x06009E6B RID: 40555 RVA: 0x003EFFF0 File Offset: 0x003EE1F0
		public void Close()
		{
			try
			{
				if (this.valuesChanged)
				{
					using (Stream stream = SdFile.Open(this.filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
					{
						SdfWriter.Write(stream, this.data.Nodes);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error("Error opening SDF file:");
				Log.Exception(e);
			}
		}

		// Token: 0x06009E6C RID: 40556 RVA: 0x003F0064 File Offset: 0x003EE264
		public void SaveAndKeepOpen()
		{
			this.Close();
			this.Open(this.filePath);
		}

		// Token: 0x06009E6D RID: 40557 RVA: 0x003F0078 File Offset: 0x003EE278
		public void Set(string name, int val)
		{
			this.data.Add(new SdfInt(name, val));
			this.valuesChanged = true;
		}

		// Token: 0x06009E6E RID: 40558 RVA: 0x003F0094 File Offset: 0x003EE294
		public void Set(string name, float val)
		{
			this.data.Add(new SdfFloat(name, val));
			this.valuesChanged = true;
		}

		// Token: 0x06009E6F RID: 40559 RVA: 0x003F00B0 File Offset: 0x003EE2B0
		public void Set(string name, string val)
		{
			this.Set(name, val, false);
		}

		// Token: 0x06009E70 RID: 40560 RVA: 0x003F00BB File Offset: 0x003EE2BB
		public void Set(string name, string val, bool isBinary)
		{
			if (!isBinary)
			{
				this.data.Add(new SdfString(name, val));
			}
			else
			{
				this.data.Add(new SdfBinary(name, val));
			}
			this.valuesChanged = true;
		}

		// Token: 0x06009E71 RID: 40561 RVA: 0x003F00EF File Offset: 0x003EE2EF
		public void Set(string name, byte[] byteArray)
		{
			this.data.Add(new SdfByteArray(name, byteArray));
		}

		// Token: 0x06009E72 RID: 40562 RVA: 0x003F0104 File Offset: 0x003EE304
		public void Set(string name, bool val)
		{
			this.data.Add(new SdfBool(name, val));
			this.valuesChanged = true;
		}

		// Token: 0x06009E73 RID: 40563 RVA: 0x003F0120 File Offset: 0x003EE320
		public float? GetFloat(string name)
		{
			return this.data.GetFloat(name);
		}

		// Token: 0x06009E74 RID: 40564 RVA: 0x003F012E File Offset: 0x003EE32E
		public int? GetInt(string name)
		{
			return this.data.GetInt(name);
		}

		// Token: 0x06009E75 RID: 40565 RVA: 0x003F013C File Offset: 0x003EE33C
		public string GetString(string name)
		{
			return this.GetString(name, false);
		}

		// Token: 0x06009E76 RID: 40566 RVA: 0x003F0146 File Offset: 0x003EE346
		public string GetString(string name, bool isBinary)
		{
			if (!isBinary)
			{
				return this.data.GetString(name);
			}
			return Utils.FromBase64(this.data.GetString(name));
		}

		// Token: 0x06009E77 RID: 40567 RVA: 0x003F0169 File Offset: 0x003EE369
		public byte[] GetByteArray(string name)
		{
			return this.data.GetByteArray(name);
		}

		// Token: 0x06009E78 RID: 40568 RVA: 0x003F0177 File Offset: 0x003EE377
		public bool? GetBool(string name)
		{
			return this.data.GetBool(name);
		}

		// Token: 0x06009E79 RID: 40569 RVA: 0x003F0185 File Offset: 0x003EE385
		public void Remove(string name)
		{
			this.data.Remove(name);
			this.valuesChanged = true;
		}

		// Token: 0x06009E7A RID: 40570 RVA: 0x003F019C File Offset: 0x003EE39C
		public string[] GetKeys()
		{
			string[] array = new string[this.data.Nodes.Count];
			this.data.Nodes.CopyKeysTo(array);
			return array;
		}

		// Token: 0x06009E7B RID: 40571 RVA: 0x003F01D4 File Offset: 0x003EE3D4
		public string[] GetStoredGamePrefs()
		{
			string[] array = new string[this.data.Nodes.Count];
			this.data.Nodes.CopyKeysTo(array);
			return array;
		}

		// Token: 0x040079FF RID: 31231
		[PublicizedFrom(EAccessModifier.Private)]
		public SdfData data;

		// Token: 0x04007A00 RID: 31232
		[PublicizedFrom(EAccessModifier.Private)]
		public string filePath;

		// Token: 0x04007A01 RID: 31233
		[PublicizedFrom(EAccessModifier.Private)]
		public bool valuesChanged;
	}
}

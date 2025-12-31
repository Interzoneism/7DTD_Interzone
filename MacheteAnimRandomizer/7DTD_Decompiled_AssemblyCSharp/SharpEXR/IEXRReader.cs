using System;

namespace SharpEXR
{
	// Token: 0x02001403 RID: 5123
	public interface IEXRReader : IDisposable
	{
		// Token: 0x06009F7B RID: 40827
		byte ReadByte();

		// Token: 0x06009F7C RID: 40828
		int ReadInt32();

		// Token: 0x06009F7D RID: 40829
		uint ReadUInt32();

		// Token: 0x06009F7E RID: 40830
		Half ReadHalf();

		// Token: 0x06009F7F RID: 40831
		float ReadSingle();

		// Token: 0x06009F80 RID: 40832
		double ReadDouble();

		// Token: 0x06009F81 RID: 40833
		string ReadNullTerminatedString(int maxLength);

		// Token: 0x06009F82 RID: 40834
		string ReadString(int length);

		// Token: 0x06009F83 RID: 40835
		string ReadString();

		// Token: 0x06009F84 RID: 40836
		byte[] ReadBytes(int count);

		// Token: 0x06009F85 RID: 40837
		void CopyBytes(byte[] dest, int offset, int count);

		// Token: 0x17001156 RID: 4438
		// (get) Token: 0x06009F86 RID: 40838
		// (set) Token: 0x06009F87 RID: 40839
		int Position { get; set; }
	}
}

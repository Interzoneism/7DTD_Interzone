using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpEXR
{
	// Token: 0x020013FA RID: 5114
	public class ChannelList : IEnumerable<Channel>, IEnumerable
	{
		// Token: 0x17001140 RID: 4416
		// (get) Token: 0x06009F24 RID: 40740 RVA: 0x003F2209 File Offset: 0x003F0409
		// (set) Token: 0x06009F25 RID: 40741 RVA: 0x003F2211 File Offset: 0x003F0411
		public List<Channel> Channels { get; set; }

		// Token: 0x06009F26 RID: 40742 RVA: 0x003F221A File Offset: 0x003F041A
		public ChannelList()
		{
			this.Channels = new List<Channel>();
		}

		// Token: 0x06009F27 RID: 40743 RVA: 0x003F2230 File Offset: 0x003F0430
		public void Read(EXRFile file, IEXRReader reader, int size)
		{
			int num = 0;
			Channel item;
			int num2;
			while (this.ReadChannel(file, reader, out item, out num2))
			{
				this.Channels.Add(item);
				num += num2;
				if (num > size)
				{
					throw new EXRFormatException(string.Concat(new string[]
					{
						"Read ",
						num.ToString(),
						" bytes but Size was ",
						size.ToString(),
						"."
					}));
				}
			}
			num += num2;
			if (num != size)
			{
				throw new EXRFormatException(string.Concat(new string[]
				{
					"Read ",
					num.ToString(),
					" bytes but Size was ",
					size.ToString(),
					"."
				}));
			}
		}

		// Token: 0x06009F28 RID: 40744 RVA: 0x003F22E8 File Offset: 0x003F04E8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool ReadChannel(EXRFile file, IEXRReader reader, out Channel channel, out int bytesRead)
		{
			int position = reader.Position;
			string text = reader.ReadNullTerminatedString(255);
			if (text == "")
			{
				channel = null;
				bytesRead = reader.Position - position;
				return false;
			}
			channel = new Channel(text, (PixelType)reader.ReadInt32(), reader.ReadByte() > 0, reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadInt32(), reader.ReadInt32());
			bytesRead = reader.Position - position;
			return true;
		}

		// Token: 0x06009F29 RID: 40745 RVA: 0x003F2366 File Offset: 0x003F0566
		public IEnumerator<Channel> GetEnumerator()
		{
			return this.Channels.GetEnumerator();
		}

		// Token: 0x06009F2A RID: 40746 RVA: 0x003F2378 File Offset: 0x003F0578
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x17001141 RID: 4417
		public Channel this[int index]
		{
			get
			{
				return this.Channels[index];
			}
			set
			{
				this.Channels[index] = value;
			}
		}
	}
}

using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace mumblelib
{
	// Token: 0x020013E3 RID: 5091
	[PublicizedFrom(EAccessModifier.Internal)]
	public class WindowsLinkFile : ILinkFile, IDisposable
	{
		// Token: 0x06009EBF RID: 40639 RVA: 0x003F0B04 File Offset: 0x003EED04
		public unsafe WindowsLinkFile()
		{
			this.memoryMappedFile = MemoryMappedFile.CreateOrOpen("MumbleLink", (long)Marshal.SizeOf<WindowsLinkFile.WindowsLinkMemory>());
			byte* ptr = null;
			this.memoryMappedFile.CreateViewAccessor().SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
			this.ptr = (WindowsLinkFile.WindowsLinkMemory*)ptr;
		}

		// Token: 0x17001129 RID: 4393
		// (get) Token: 0x06009EC0 RID: 40640 RVA: 0x003F0B4E File Offset: 0x003EED4E
		// (set) Token: 0x06009EC1 RID: 40641 RVA: 0x003F0B5B File Offset: 0x003EED5B
		public unsafe uint UIVersion
		{
			get
			{
				return this.ptr->uiVersion;
			}
			set
			{
				this.ptr->uiVersion = value;
			}
		}

		// Token: 0x06009EC2 RID: 40642 RVA: 0x003F0B69 File Offset: 0x003EED69
		public unsafe void Tick()
		{
			this.ptr->uiTick += 1U;
		}

		// Token: 0x1700112A RID: 4394
		// (set) Token: 0x06009EC3 RID: 40643 RVA: 0x003F0B7B File Offset: 0x003EED7B
		public unsafe Vector3 AvatarPosition
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarPosition.FixedElementField, value);
			}
		}

		// Token: 0x1700112B RID: 4395
		// (set) Token: 0x06009EC4 RID: 40644 RVA: 0x003F0B94 File Offset: 0x003EED94
		public unsafe Vector3 AvatarForward
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarFront.FixedElementField, value);
			}
		}

		// Token: 0x1700112C RID: 4396
		// (set) Token: 0x06009EC5 RID: 40645 RVA: 0x003F0BAD File Offset: 0x003EEDAD
		public unsafe Vector3 AvatarTop
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarTop.FixedElementField, value);
			}
		}

		// Token: 0x1700112D RID: 4397
		// (set) Token: 0x06009EC6 RID: 40646 RVA: 0x003F0BC6 File Offset: 0x003EEDC6
		public unsafe string Name
		{
			set
			{
				Util.SetString<ushort>(&this.ptr->name.FixedElementField, value, 256, Encoding.Unicode);
			}
		}

		// Token: 0x1700112E RID: 4398
		// (set) Token: 0x06009EC7 RID: 40647 RVA: 0x003F0BE9 File Offset: 0x003EEDE9
		public unsafe Vector3 CameraPosition
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraPosition.FixedElementField, value);
			}
		}

		// Token: 0x1700112F RID: 4399
		// (set) Token: 0x06009EC8 RID: 40648 RVA: 0x003F0C02 File Offset: 0x003EEE02
		public unsafe Vector3 CameraForward
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraFront.FixedElementField, value);
			}
		}

		// Token: 0x17001130 RID: 4400
		// (set) Token: 0x06009EC9 RID: 40649 RVA: 0x003F0C1B File Offset: 0x003EEE1B
		public unsafe Vector3 CameraTop
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraTop.FixedElementField, value);
			}
		}

		// Token: 0x17001131 RID: 4401
		// (set) Token: 0x06009ECA RID: 40650 RVA: 0x003F0C34 File Offset: 0x003EEE34
		public unsafe string Identity
		{
			set
			{
				Util.SetString<ushort>(&this.ptr->identity.FixedElementField, value, 256, Encoding.Unicode);
			}
		}

		// Token: 0x17001132 RID: 4402
		// (set) Token: 0x06009ECB RID: 40651 RVA: 0x003F0C57 File Offset: 0x003EEE57
		public unsafe string Context
		{
			set
			{
				Util.SetContext(&this.ptr->context.FixedElementField, &this.ptr->context_len, value);
			}
		}

		// Token: 0x17001133 RID: 4403
		// (set) Token: 0x06009ECC RID: 40652 RVA: 0x003F0C7C File Offset: 0x003EEE7C
		public unsafe string Description
		{
			set
			{
				Util.SetString<ushort>(&this.ptr->description.FixedElementField, value, 2048, Encoding.Unicode);
			}
		}

		// Token: 0x06009ECD RID: 40653 RVA: 0x003F0C9F File Offset: 0x003EEE9F
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06009ECE RID: 40654 RVA: 0x003F0CB0 File Offset: 0x003EEEB0
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~WindowsLinkFile()
		{
			this.Dispose();
		}

		// Token: 0x06009ECF RID: 40655 RVA: 0x003F0CDC File Offset: 0x003EEEDC
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool _disposing)
		{
			Log.Out("[MumbleLF] Disposing shm");
			if (!this.disposed)
			{
				if (_disposing)
				{
					this.memoryMappedFile.Dispose();
				}
				this.disposed = true;
			}
		}

		// Token: 0x04007A37 RID: 31287
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MemoryMappedFile memoryMappedFile;

		// Token: 0x04007A38 RID: 31288
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe readonly WindowsLinkFile.WindowsLinkMemory* ptr;

		// Token: 0x04007A39 RID: 31289
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disposed;

		// Token: 0x020013E4 RID: 5092
		[PublicizedFrom(EAccessModifier.Private)]
		public struct WindowsLinkMemory
		{
			// Token: 0x04007A3A RID: 31290
			public uint uiVersion;

			// Token: 0x04007A3B RID: 31291
			public uint uiTick;

			// Token: 0x04007A3C RID: 31292
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fAvatarPosition>e__FixedBuffer fAvatarPosition;

			// Token: 0x04007A3D RID: 31293
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fAvatarFront>e__FixedBuffer fAvatarFront;

			// Token: 0x04007A3E RID: 31294
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fAvatarTop>e__FixedBuffer fAvatarTop;

			// Token: 0x04007A3F RID: 31295
			[FixedBuffer(typeof(ushort), 256)]
			public WindowsLinkFile.WindowsLinkMemory.<name>e__FixedBuffer name;

			// Token: 0x04007A40 RID: 31296
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fCameraPosition>e__FixedBuffer fCameraPosition;

			// Token: 0x04007A41 RID: 31297
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fCameraFront>e__FixedBuffer fCameraFront;

			// Token: 0x04007A42 RID: 31298
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fCameraTop>e__FixedBuffer fCameraTop;

			// Token: 0x04007A43 RID: 31299
			[FixedBuffer(typeof(ushort), 256)]
			public WindowsLinkFile.WindowsLinkMemory.<identity>e__FixedBuffer identity;

			// Token: 0x04007A44 RID: 31300
			public uint context_len;

			// Token: 0x04007A45 RID: 31301
			[FixedBuffer(typeof(byte), 256)]
			public WindowsLinkFile.WindowsLinkMemory.<context>e__FixedBuffer context;

			// Token: 0x04007A46 RID: 31302
			[FixedBuffer(typeof(ushort), 2048)]
			public WindowsLinkFile.WindowsLinkMemory.<description>e__FixedBuffer description;

			// Token: 0x020013E5 RID: 5093
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 256)]
			public struct <context>e__FixedBuffer
			{
				// Token: 0x04007A47 RID: 31303
				public byte FixedElementField;
			}

			// Token: 0x020013E6 RID: 5094
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 4096)]
			public struct <description>e__FixedBuffer
			{
				// Token: 0x04007A48 RID: 31304
				public ushort FixedElementField;
			}

			// Token: 0x020013E7 RID: 5095
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarFront>e__FixedBuffer
			{
				// Token: 0x04007A49 RID: 31305
				public float FixedElementField;
			}

			// Token: 0x020013E8 RID: 5096
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarPosition>e__FixedBuffer
			{
				// Token: 0x04007A4A RID: 31306
				public float FixedElementField;
			}

			// Token: 0x020013E9 RID: 5097
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarTop>e__FixedBuffer
			{
				// Token: 0x04007A4B RID: 31307
				public float FixedElementField;
			}

			// Token: 0x020013EA RID: 5098
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraFront>e__FixedBuffer
			{
				// Token: 0x04007A4C RID: 31308
				public float FixedElementField;
			}

			// Token: 0x020013EB RID: 5099
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraPosition>e__FixedBuffer
			{
				// Token: 0x04007A4D RID: 31309
				public float FixedElementField;
			}

			// Token: 0x020013EC RID: 5100
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraTop>e__FixedBuffer
			{
				// Token: 0x04007A4E RID: 31310
				public float FixedElementField;
			}

			// Token: 0x020013ED RID: 5101
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 512)]
			public struct <identity>e__FixedBuffer
			{
				// Token: 0x04007A4F RID: 31311
				public ushort FixedElementField;
			}

			// Token: 0x020013EE RID: 5102
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 512)]
			public struct <name>e__FixedBuffer
			{
				// Token: 0x04007A50 RID: 31312
				public ushort FixedElementField;
			}
		}
	}
}

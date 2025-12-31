using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace mumblelib
{
	// Token: 0x020013D7 RID: 5079
	[PublicizedFrom(EAccessModifier.Internal)]
	public class UnixLinkFile : ILinkFile, IDisposable
	{
		// Token: 0x06009EA9 RID: 40617
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public static extern int shm_open([MarshalAs(UnmanagedType.LPStr)] string name, int oflag, uint mode);

		// Token: 0x06009EAA RID: 40618
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public static extern uint getuid();

		// Token: 0x06009EAB RID: 40619
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public static extern int ftruncate(int fd, long length);

		// Token: 0x06009EAC RID: 40620
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public unsafe static extern void* mmap(void* addr, long length, int prot, int flags, int fd, long off);

		// Token: 0x06009EAD RID: 40621
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public unsafe static extern void* munmap(void* addr, long length);

		// Token: 0x06009EAE RID: 40622
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public static extern int close(int fd);

		// Token: 0x06009EAF RID: 40623 RVA: 0x003F08B0 File Offset: 0x003EEAB0
		public unsafe UnixLinkFile()
		{
			this.fd = UnixLinkFile.shm_open("/MumbleLink." + UnixLinkFile.getuid().ToString(), 66, 384U);
			if (this.fd < 0)
			{
				throw new Exception("[MumbleLF] Failed to open shm");
			}
			Log.Out("[MumbleLF] FD opened");
			int num = Marshal.SizeOf<UnixLinkFile.LinuxLinkMemory>();
			if (UnixLinkFile.ftruncate(this.fd, (long)num) != 0)
			{
				Log.Error("[MumbleLF] Failed resizing shm");
				return;
			}
			Log.Out("[MumbleLF] Resized");
			this.ptr = (UnixLinkFile.LinuxLinkMemory*)UnixLinkFile.mmap(null, (long)num, 3, 1, this.fd, 0L);
			Log.Out("[MumbleLF] MemMapped");
		}

		// Token: 0x1700111E RID: 4382
		// (get) Token: 0x06009EB0 RID: 40624 RVA: 0x003F0957 File Offset: 0x003EEB57
		// (set) Token: 0x06009EB1 RID: 40625 RVA: 0x003F0964 File Offset: 0x003EEB64
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

		// Token: 0x06009EB2 RID: 40626 RVA: 0x003F0972 File Offset: 0x003EEB72
		public unsafe void Tick()
		{
			this.ptr->uiTick += 1U;
		}

		// Token: 0x1700111F RID: 4383
		// (set) Token: 0x06009EB3 RID: 40627 RVA: 0x003F0984 File Offset: 0x003EEB84
		public unsafe Vector3 AvatarPosition
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarPosition.FixedElementField, value);
			}
		}

		// Token: 0x17001120 RID: 4384
		// (set) Token: 0x06009EB4 RID: 40628 RVA: 0x003F099D File Offset: 0x003EEB9D
		public unsafe Vector3 AvatarForward
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarFront.FixedElementField, value);
			}
		}

		// Token: 0x17001121 RID: 4385
		// (set) Token: 0x06009EB5 RID: 40629 RVA: 0x003F09B6 File Offset: 0x003EEBB6
		public unsafe Vector3 AvatarTop
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarTop.FixedElementField, value);
			}
		}

		// Token: 0x17001122 RID: 4386
		// (set) Token: 0x06009EB6 RID: 40630 RVA: 0x003F09CF File Offset: 0x003EEBCF
		public unsafe string Name
		{
			set
			{
				Util.SetString<uint>(&this.ptr->name.FixedElementField, value, 256, Encoding.UTF32);
			}
		}

		// Token: 0x17001123 RID: 4387
		// (set) Token: 0x06009EB7 RID: 40631 RVA: 0x003F09F2 File Offset: 0x003EEBF2
		public unsafe Vector3 CameraPosition
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraPosition.FixedElementField, value);
			}
		}

		// Token: 0x17001124 RID: 4388
		// (set) Token: 0x06009EB8 RID: 40632 RVA: 0x003F0A0B File Offset: 0x003EEC0B
		public unsafe Vector3 CameraForward
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraFront.FixedElementField, value);
			}
		}

		// Token: 0x17001125 RID: 4389
		// (set) Token: 0x06009EB9 RID: 40633 RVA: 0x003F0A24 File Offset: 0x003EEC24
		public unsafe Vector3 CameraTop
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraTop.FixedElementField, value);
			}
		}

		// Token: 0x17001126 RID: 4390
		// (set) Token: 0x06009EBA RID: 40634 RVA: 0x003F0A3D File Offset: 0x003EEC3D
		public unsafe string Identity
		{
			set
			{
				Util.SetString<uint>(&this.ptr->identity.FixedElementField, value, 256, Encoding.UTF32);
			}
		}

		// Token: 0x17001127 RID: 4391
		// (set) Token: 0x06009EBB RID: 40635 RVA: 0x003F0A60 File Offset: 0x003EEC60
		public unsafe string Context
		{
			set
			{
				Util.SetContext(&this.ptr->context.FixedElementField, &this.ptr->context_len, value);
			}
		}

		// Token: 0x17001128 RID: 4392
		// (set) Token: 0x06009EBC RID: 40636 RVA: 0x003F0A85 File Offset: 0x003EEC85
		public unsafe string Description
		{
			set
			{
				Util.SetString<uint>(&this.ptr->description.FixedElementField, value, 2048, Encoding.UTF32);
			}
		}

		// Token: 0x06009EBD RID: 40637 RVA: 0x003F0AA8 File Offset: 0x003EECA8
		public unsafe void Dispose()
		{
			if (!this.disposed)
			{
				UnixLinkFile.munmap((void*)this.ptr, (long)Marshal.SizeOf<UnixLinkFile.LinuxLinkMemory>());
				UnixLinkFile.close(this.fd);
				this.disposed = true;
			}
		}

		// Token: 0x06009EBE RID: 40638 RVA: 0x003F0AD8 File Offset: 0x003EECD8
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~UnixLinkFile()
		{
			this.Dispose();
		}

		// Token: 0x04007A10 RID: 31248
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_RDONLY = 0;

		// Token: 0x04007A11 RID: 31249
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_WRONLY = 1;

		// Token: 0x04007A12 RID: 31250
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_RDWR = 2;

		// Token: 0x04007A13 RID: 31251
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_CREAT = 64;

		// Token: 0x04007A14 RID: 31252
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_EXCL = 128;

		// Token: 0x04007A15 RID: 31253
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_TRUNC = 512;

		// Token: 0x04007A16 RID: 31254
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PROT_READ = 1;

		// Token: 0x04007A17 RID: 31255
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PROT_WRITE = 2;

		// Token: 0x04007A18 RID: 31256
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PROT_EXEC = 4;

		// Token: 0x04007A19 RID: 31257
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PROT_NONE = 0;

		// Token: 0x04007A1A RID: 31258
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MAP_SHARED = 1;

		// Token: 0x04007A1B RID: 31259
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MAP_PRIVATE = 2;

		// Token: 0x04007A1C RID: 31260
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MAP_SHARED_VALIDATE = 3;

		// Token: 0x04007A1D RID: 31261
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disposed;

		// Token: 0x04007A1E RID: 31262
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe readonly UnixLinkFile.LinuxLinkMemory* ptr;

		// Token: 0x04007A1F RID: 31263
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int fd;

		// Token: 0x020013D8 RID: 5080
		[PublicizedFrom(EAccessModifier.Private)]
		public struct LinuxLinkMemory
		{
			// Token: 0x04007A20 RID: 31264
			public uint uiVersion;

			// Token: 0x04007A21 RID: 31265
			public uint uiTick;

			// Token: 0x04007A22 RID: 31266
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fAvatarPosition>e__FixedBuffer fAvatarPosition;

			// Token: 0x04007A23 RID: 31267
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fAvatarFront>e__FixedBuffer fAvatarFront;

			// Token: 0x04007A24 RID: 31268
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fAvatarTop>e__FixedBuffer fAvatarTop;

			// Token: 0x04007A25 RID: 31269
			[FixedBuffer(typeof(uint), 256)]
			public UnixLinkFile.LinuxLinkMemory.<name>e__FixedBuffer name;

			// Token: 0x04007A26 RID: 31270
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fCameraPosition>e__FixedBuffer fCameraPosition;

			// Token: 0x04007A27 RID: 31271
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fCameraFront>e__FixedBuffer fCameraFront;

			// Token: 0x04007A28 RID: 31272
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fCameraTop>e__FixedBuffer fCameraTop;

			// Token: 0x04007A29 RID: 31273
			[FixedBuffer(typeof(uint), 256)]
			public UnixLinkFile.LinuxLinkMemory.<identity>e__FixedBuffer identity;

			// Token: 0x04007A2A RID: 31274
			public uint context_len;

			// Token: 0x04007A2B RID: 31275
			[FixedBuffer(typeof(byte), 256)]
			public UnixLinkFile.LinuxLinkMemory.<context>e__FixedBuffer context;

			// Token: 0x04007A2C RID: 31276
			[FixedBuffer(typeof(uint), 2048)]
			public UnixLinkFile.LinuxLinkMemory.<description>e__FixedBuffer description;

			// Token: 0x020013D9 RID: 5081
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 256)]
			public struct <context>e__FixedBuffer
			{
				// Token: 0x04007A2D RID: 31277
				public byte FixedElementField;
			}

			// Token: 0x020013DA RID: 5082
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 8192)]
			public struct <description>e__FixedBuffer
			{
				// Token: 0x04007A2E RID: 31278
				public uint FixedElementField;
			}

			// Token: 0x020013DB RID: 5083
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarFront>e__FixedBuffer
			{
				// Token: 0x04007A2F RID: 31279
				public float FixedElementField;
			}

			// Token: 0x020013DC RID: 5084
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarPosition>e__FixedBuffer
			{
				// Token: 0x04007A30 RID: 31280
				public float FixedElementField;
			}

			// Token: 0x020013DD RID: 5085
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarTop>e__FixedBuffer
			{
				// Token: 0x04007A31 RID: 31281
				public float FixedElementField;
			}

			// Token: 0x020013DE RID: 5086
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraFront>e__FixedBuffer
			{
				// Token: 0x04007A32 RID: 31282
				public float FixedElementField;
			}

			// Token: 0x020013DF RID: 5087
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraPosition>e__FixedBuffer
			{
				// Token: 0x04007A33 RID: 31283
				public float FixedElementField;
			}

			// Token: 0x020013E0 RID: 5088
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraTop>e__FixedBuffer
			{
				// Token: 0x04007A34 RID: 31284
				public float FixedElementField;
			}

			// Token: 0x020013E1 RID: 5089
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 1024)]
			public struct <identity>e__FixedBuffer
			{
				// Token: 0x04007A35 RID: 31285
				public uint FixedElementField;
			}

			// Token: 0x020013E2 RID: 5090
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 1024)]
			public struct <name>e__FixedBuffer
			{
				// Token: 0x04007A36 RID: 31286
				public uint FixedElementField;
			}
		}
	}
}

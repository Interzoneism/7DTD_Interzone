using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	// Token: 0x02000030 RID: 48
	[NativeHeader("Modules/Animation/HumanDescription.h")]
	[NativeType(CodegenOptions.Custom, "MonoHumanLimit")]
	[NativeHeader("Modules/Animation/ScriptBindings/AvatarBuilder.bindings.h")]
	public struct HumanLimit
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000220 RID: 544 RVA: 0x00003BBC File Offset: 0x00001DBC
		// (set) Token: 0x06000221 RID: 545 RVA: 0x00003BD7 File Offset: 0x00001DD7
		public bool useDefaultValues
		{
			get
			{
				return this.m_UseDefaultValues != 0;
			}
			set
			{
				this.m_UseDefaultValues = (value ? 1 : 0);
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000222 RID: 546 RVA: 0x00003BE8 File Offset: 0x00001DE8
		// (set) Token: 0x06000223 RID: 547 RVA: 0x00003C00 File Offset: 0x00001E00
		public Vector3 min
		{
			get
			{
				return this.m_Min;
			}
			set
			{
				this.m_Min = value;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000224 RID: 548 RVA: 0x00003C0C File Offset: 0x00001E0C
		// (set) Token: 0x06000225 RID: 549 RVA: 0x00003C24 File Offset: 0x00001E24
		public Vector3 max
		{
			get
			{
				return this.m_Max;
			}
			set
			{
				this.m_Max = value;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000226 RID: 550 RVA: 0x00003C30 File Offset: 0x00001E30
		// (set) Token: 0x06000227 RID: 551 RVA: 0x00003C48 File Offset: 0x00001E48
		public Vector3 center
		{
			get
			{
				return this.m_Center;
			}
			set
			{
				this.m_Center = value;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000228 RID: 552 RVA: 0x00003C54 File Offset: 0x00001E54
		// (set) Token: 0x06000229 RID: 553 RVA: 0x00003C6C File Offset: 0x00001E6C
		public float axisLength
		{
			get
			{
				return this.m_AxisLength;
			}
			set
			{
				this.m_AxisLength = value;
			}
		}

		// Token: 0x0400010E RID: 270
		private Vector3 m_Min;

		// Token: 0x0400010F RID: 271
		private Vector3 m_Max;

		// Token: 0x04000110 RID: 272
		private Vector3 m_Center;

		// Token: 0x04000111 RID: 273
		private float m_AxisLength;

		// Token: 0x04000112 RID: 274
		private int m_UseDefaultValues;
	}
}

using System;
using UnityEngine;

namespace WaterClippingTool
{
	// Token: 0x020013B8 RID: 5048
	[Serializable]
	public class ShapeSettings : IEquatable<ShapeSettings>
	{
		// Token: 0x170010FA RID: 4346
		// (get) Token: 0x06009DF3 RID: 40435 RVA: 0x003EDA50 File Offset: 0x003EBC50
		public bool hasPlane
		{
			get
			{
				return this.plane != WaterClippingPlanePlacer.DisabledPlaneVec;
			}
		}

		// Token: 0x06009DF4 RID: 40436 RVA: 0x003EDA62 File Offset: 0x003EBC62
		public ShapeSettings()
		{
			this.ResetToDefault();
		}

		// Token: 0x06009DF5 RID: 40437 RVA: 0x003EDA78 File Offset: 0x003EBC78
		public void ResetToDefault()
		{
			this.shapeName = string.Empty;
			this.shapeModel = null;
			this.modelOffset = WaterClippingPlanePlacer.DefaultModelOffset;
			this.plane = WaterClippingPlanePlacer.DisabledPlaneVec;
			this.waterFlowMask = BlockFaceFlag.All;
		}

		// Token: 0x06009DF6 RID: 40438 RVA: 0x003EDAAA File Offset: 0x003EBCAA
		public void CopyFrom(ShapeSettings other)
		{
			this.shapeName = other.shapeName;
			this.shapeModel = other.shapeModel;
			this.modelOffset = other.modelOffset;
			this.plane = other.plane;
			this.waterFlowMask = other.waterFlowMask;
		}

		// Token: 0x06009DF7 RID: 40439 RVA: 0x003EDAE8 File Offset: 0x003EBCE8
		public bool Equals(ShapeSettings other)
		{
			return !(this.plane != other.plane) && !(this.shapeName != other.shapeName) && !(this.shapeModel != other.shapeModel) && !(this.modelOffset != other.modelOffset) && this.waterFlowMask == other.waterFlowMask;
		}

		// Token: 0x040079CB RID: 31179
		public string shapeName;

		// Token: 0x040079CC RID: 31180
		public GameObject shapeModel;

		// Token: 0x040079CD RID: 31181
		public Vector3 modelOffset;

		// Token: 0x040079CE RID: 31182
		public Vector4 plane;

		// Token: 0x040079CF RID: 31183
		public BlockFaceFlag waterFlowMask = BlockFaceFlag.All;
	}
}

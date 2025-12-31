using System;
using System.IO;

// Token: 0x02000827 RID: 2087
public class QuestPositionData
{
	// Token: 0x06003BF8 RID: 15352 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public QuestPositionData()
	{
	}

	// Token: 0x06003BF9 RID: 15353 RVA: 0x00181619 File Offset: 0x0017F819
	public QuestPositionData(int questCode, Quest.PositionDataTypes positionDataType, Vector3i blockPosition)
	{
		this.questCode = questCode;
		this.positionDataType = positionDataType;
		this.blockPosition = blockPosition;
	}

	// Token: 0x06003BFA RID: 15354 RVA: 0x00181636 File Offset: 0x0017F836
	public static QuestPositionData Read(BinaryReader reader)
	{
		return new QuestPositionData
		{
			questCode = reader.ReadInt32(),
			positionDataType = (Quest.PositionDataTypes)reader.ReadInt32(),
			blockPosition = StreamUtils.ReadVector3i(reader)
		};
	}

	// Token: 0x06003BFB RID: 15355 RVA: 0x00181661 File Offset: 0x0017F861
	public void Write(BinaryWriter writer)
	{
		writer.Write(this.questCode);
		writer.Write((int)this.positionDataType);
		StreamUtils.Write(writer, this.blockPosition);
	}

	// Token: 0x0400309B RID: 12443
	public int questCode;

	// Token: 0x0400309C RID: 12444
	public Quest.PositionDataTypes positionDataType;

	// Token: 0x0400309D RID: 12445
	public Vector3i blockPosition;
}

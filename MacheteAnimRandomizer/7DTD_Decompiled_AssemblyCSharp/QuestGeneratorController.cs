using System;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class QuestGeneratorController : MonoBehaviour
{
	// Token: 0x06000532 RID: 1330 RVA: 0x00024ED8 File Offset: 0x000230D8
	public void SetGeneratorState(QuestGeneratorController.GeneratorStates state, bool isInit)
	{
		if (state != this.currentState)
		{
			this.currentState = state;
			this.updateStateDisplay();
			if (!isInit)
			{
				PrefabInstance.RefreshTriggersInContainingPoi(base.transform.position + Origin.position);
			}
		}
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x00024F10 File Offset: 0x00023110
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateStateDisplay()
	{
		switch (this.currentState)
		{
		case QuestGeneratorController.GeneratorStates.OnNoQuest:
			this.OffState.SetActive(false);
			this.RebootState.SetActive(false);
			this.EnteringOnState.SetActive(false);
			this.OnState.SetActive(true);
			return;
		case QuestGeneratorController.GeneratorStates.Off:
			this.OffState.SetActive(true);
			this.RebootState.SetActive(false);
			this.EnteringOnState.SetActive(false);
			this.OnState.SetActive(false);
			return;
		case QuestGeneratorController.GeneratorStates.RebootState:
			this.OffState.SetActive(false);
			this.RebootState.SetActive(true);
			this.EnteringOnState.SetActive(false);
			this.OnState.SetActive(false);
			return;
		case QuestGeneratorController.GeneratorStates.EnteringOnState:
			this.OffState.SetActive(false);
			this.RebootState.SetActive(false);
			this.EnteringOnState.SetActive(true);
			this.OnState.SetActive(false);
			return;
		case QuestGeneratorController.GeneratorStates.On:
			this.OffState.SetActive(false);
			this.RebootState.SetActive(false);
			this.EnteringOnState.SetActive(false);
			this.OnState.SetActive(true);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x00025033 File Offset: 0x00023233
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.updateStateDisplay();
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
	}

	// Token: 0x040005E4 RID: 1508
	public Light MainLight;

	// Token: 0x040005E5 RID: 1509
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public QuestGeneratorController.GeneratorStates currentState;

	// Token: 0x040005E6 RID: 1510
	public QuestGeneratorController.GeneratorStates TestingCurrentState = QuestGeneratorController.GeneratorStates.Off;

	// Token: 0x040005E7 RID: 1511
	public GameObject OffState;

	// Token: 0x040005E8 RID: 1512
	public GameObject RebootState;

	// Token: 0x040005E9 RID: 1513
	public GameObject EnteringOnState;

	// Token: 0x040005EA RID: 1514
	public GameObject OnState;

	// Token: 0x020000D2 RID: 210
	public enum GeneratorStates
	{
		// Token: 0x040005EC RID: 1516
		OnNoQuest,
		// Token: 0x040005ED RID: 1517
		Off,
		// Token: 0x040005EE RID: 1518
		RebootState,
		// Token: 0x040005EF RID: 1519
		EnteringOnState,
		// Token: 0x040005F0 RID: 1520
		On
	}
}

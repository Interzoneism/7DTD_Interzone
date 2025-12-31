using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x02001399 RID: 5017
	[Preserve]
	public class ItemActionData : IXMLData
	{
		// Token: 0x17001088 RID: 4232
		// (get) Token: 0x06009CD1 RID: 40145 RVA: 0x003E442F File Offset: 0x003E262F
		// (set) Token: 0x06009CD2 RID: 40146 RVA: 0x003E4437 File Offset: 0x003E2637
		public DataItem<float> Delay
		{
			get
			{
				return this.pDelay;
			}
			set
			{
				this.pDelay = value;
			}
		}

		// Token: 0x17001089 RID: 4233
		// (get) Token: 0x06009CD3 RID: 40147 RVA: 0x003E4440 File Offset: 0x003E2640
		// (set) Token: 0x06009CD4 RID: 40148 RVA: 0x003E4448 File Offset: 0x003E2648
		public DataItem<float> Range
		{
			get
			{
				return this.pRange;
			}
			set
			{
				this.pRange = value;
			}
		}

		// Token: 0x1700108A RID: 4234
		// (get) Token: 0x06009CD5 RID: 40149 RVA: 0x003E4451 File Offset: 0x003E2651
		// (set) Token: 0x06009CD6 RID: 40150 RVA: 0x003E4459 File Offset: 0x003E2659
		public DataItem<string> SoundStart
		{
			get
			{
				return this.pSoundStart;
			}
			set
			{
				this.pSoundStart = value;
			}
		}

		// Token: 0x1700108B RID: 4235
		// (get) Token: 0x06009CD7 RID: 40151 RVA: 0x003E4462 File Offset: 0x003E2662
		// (set) Token: 0x06009CD8 RID: 40152 RVA: 0x003E446A File Offset: 0x003E266A
		public DataItem<string> SoundRepeat
		{
			get
			{
				return this.pSoundRepeat;
			}
			set
			{
				this.pSoundRepeat = value;
			}
		}

		// Token: 0x1700108C RID: 4236
		// (get) Token: 0x06009CD9 RID: 40153 RVA: 0x003E4473 File Offset: 0x003E2673
		// (set) Token: 0x06009CDA RID: 40154 RVA: 0x003E447B File Offset: 0x003E267B
		public DataItem<string> SoundEnd
		{
			get
			{
				return this.pSoundEnd;
			}
			set
			{
				this.pSoundEnd = value;
			}
		}

		// Token: 0x1700108D RID: 4237
		// (get) Token: 0x06009CDB RID: 40155 RVA: 0x003E4484 File Offset: 0x003E2684
		// (set) Token: 0x06009CDC RID: 40156 RVA: 0x003E448C File Offset: 0x003E268C
		public DataItem<string> SoundEmpty
		{
			get
			{
				return this.pSoundEmpty;
			}
			set
			{
				this.pSoundEmpty = value;
			}
		}

		// Token: 0x1700108E RID: 4238
		// (get) Token: 0x06009CDD RID: 40157 RVA: 0x003E4495 File Offset: 0x003E2695
		// (set) Token: 0x06009CDE RID: 40158 RVA: 0x003E449D File Offset: 0x003E269D
		public DataItem<string> SoundReload
		{
			get
			{
				return this.pSoundReload;
			}
			set
			{
				this.pSoundReload = value;
			}
		}

		// Token: 0x1700108F RID: 4239
		// (get) Token: 0x06009CDF RID: 40159 RVA: 0x003E44A6 File Offset: 0x003E26A6
		// (set) Token: 0x06009CE0 RID: 40160 RVA: 0x003E44AE File Offset: 0x003E26AE
		public DataItem<string> SoundWarning
		{
			get
			{
				return this.pSoundWarning;
			}
			set
			{
				this.pSoundWarning = value;
			}
		}

		// Token: 0x17001090 RID: 4240
		// (get) Token: 0x06009CE1 RID: 40161 RVA: 0x003E44B7 File Offset: 0x003E26B7
		// (set) Token: 0x06009CE2 RID: 40162 RVA: 0x003E44BF File Offset: 0x003E26BF
		public DataItem<string> StaminaUsage
		{
			get
			{
				return this.pStaminaUsage;
			}
			set
			{
				this.pStaminaUsage = value;
			}
		}

		// Token: 0x17001091 RID: 4241
		// (get) Token: 0x06009CE3 RID: 40163 RVA: 0x003E44C8 File Offset: 0x003E26C8
		// (set) Token: 0x06009CE4 RID: 40164 RVA: 0x003E44D0 File Offset: 0x003E26D0
		public DataItem<string> UseTime
		{
			get
			{
				return this.pUseTime;
			}
			set
			{
				this.pUseTime = value;
			}
		}

		// Token: 0x17001092 RID: 4242
		// (get) Token: 0x06009CE5 RID: 40165 RVA: 0x003E44D9 File Offset: 0x003E26D9
		// (set) Token: 0x06009CE6 RID: 40166 RVA: 0x003E44E1 File Offset: 0x003E26E1
		public DataItem<string> FocusedBlockname1
		{
			get
			{
				return this.pFocusedBlockname1;
			}
			set
			{
				this.pFocusedBlockname1 = value;
			}
		}

		// Token: 0x17001093 RID: 4243
		// (get) Token: 0x06009CE7 RID: 40167 RVA: 0x003E44EA File Offset: 0x003E26EA
		// (set) Token: 0x06009CE8 RID: 40168 RVA: 0x003E44F2 File Offset: 0x003E26F2
		public DataItem<string> FocusedBlockname2
		{
			get
			{
				return this.pFocusedBlockname2;
			}
			set
			{
				this.pFocusedBlockname2 = value;
			}
		}

		// Token: 0x17001094 RID: 4244
		// (get) Token: 0x06009CE9 RID: 40169 RVA: 0x003E44FB File Offset: 0x003E26FB
		// (set) Token: 0x06009CEA RID: 40170 RVA: 0x003E4503 File Offset: 0x003E2703
		public DataItem<string> FocusedBlockname3
		{
			get
			{
				return this.pFocusedBlockname3;
			}
			set
			{
				this.pFocusedBlockname3 = value;
			}
		}

		// Token: 0x17001095 RID: 4245
		// (get) Token: 0x06009CEB RID: 40171 RVA: 0x003E450C File Offset: 0x003E270C
		// (set) Token: 0x06009CEC RID: 40172 RVA: 0x003E4514 File Offset: 0x003E2714
		public DataItem<string> FocusedBlockname4
		{
			get
			{
				return this.pFocusedBlockname4;
			}
			set
			{
				this.pFocusedBlockname4 = value;
			}
		}

		// Token: 0x17001096 RID: 4246
		// (get) Token: 0x06009CED RID: 40173 RVA: 0x003E451D File Offset: 0x003E271D
		// (set) Token: 0x06009CEE RID: 40174 RVA: 0x003E4525 File Offset: 0x003E2725
		public DataItem<string> FocusedBlockname5
		{
			get
			{
				return this.pFocusedBlockname5;
			}
			set
			{
				this.pFocusedBlockname5 = value;
			}
		}

		// Token: 0x17001097 RID: 4247
		// (get) Token: 0x06009CEF RID: 40175 RVA: 0x003E452E File Offset: 0x003E272E
		// (set) Token: 0x06009CF0 RID: 40176 RVA: 0x003E4536 File Offset: 0x003E2736
		public DataItem<string> FocusedBlockname6
		{
			get
			{
				return this.pFocusedBlockname6;
			}
			set
			{
				this.pFocusedBlockname6 = value;
			}
		}

		// Token: 0x17001098 RID: 4248
		// (get) Token: 0x06009CF1 RID: 40177 RVA: 0x003E453F File Offset: 0x003E273F
		// (set) Token: 0x06009CF2 RID: 40178 RVA: 0x003E4547 File Offset: 0x003E2747
		public DataItem<string> FocusedBlockname7
		{
			get
			{
				return this.pFocusedBlockname7;
			}
			set
			{
				this.pFocusedBlockname7 = value;
			}
		}

		// Token: 0x17001099 RID: 4249
		// (get) Token: 0x06009CF3 RID: 40179 RVA: 0x003E4550 File Offset: 0x003E2750
		// (set) Token: 0x06009CF4 RID: 40180 RVA: 0x003E4558 File Offset: 0x003E2758
		public DataItem<string> FocusedBlockname8
		{
			get
			{
				return this.pFocusedBlockname8;
			}
			set
			{
				this.pFocusedBlockname8 = value;
			}
		}

		// Token: 0x1700109A RID: 4250
		// (get) Token: 0x06009CF5 RID: 40181 RVA: 0x003E4561 File Offset: 0x003E2761
		// (set) Token: 0x06009CF6 RID: 40182 RVA: 0x003E4569 File Offset: 0x003E2769
		public DataItem<string> FocusedBlockname9
		{
			get
			{
				return this.pFocusedBlockname9;
			}
			set
			{
				this.pFocusedBlockname9 = value;
			}
		}

		// Token: 0x1700109B RID: 4251
		// (get) Token: 0x06009CF7 RID: 40183 RVA: 0x003E4572 File Offset: 0x003E2772
		// (set) Token: 0x06009CF8 RID: 40184 RVA: 0x003E457A File Offset: 0x003E277A
		public DataItem<string> ChangeItemTo
		{
			get
			{
				return this.pChangeItemTo;
			}
			set
			{
				this.pChangeItemTo = value;
			}
		}

		// Token: 0x1700109C RID: 4252
		// (get) Token: 0x06009CF9 RID: 40185 RVA: 0x003E4583 File Offset: 0x003E2783
		// (set) Token: 0x06009CFA RID: 40186 RVA: 0x003E458B File Offset: 0x003E278B
		public DataItem<string> ChangeBlockTo
		{
			get
			{
				return this.pChangeBlockTo;
			}
			set
			{
				this.pChangeBlockTo = value;
			}
		}

		// Token: 0x1700109D RID: 4253
		// (get) Token: 0x06009CFB RID: 40187 RVA: 0x003E4594 File Offset: 0x003E2794
		// (set) Token: 0x06009CFC RID: 40188 RVA: 0x003E459C File Offset: 0x003E279C
		public DataItem<string> DoBlockAction
		{
			get
			{
				return this.pDoBlockAction;
			}
			set
			{
				this.pDoBlockAction = value;
			}
		}

		// Token: 0x1700109E RID: 4254
		// (get) Token: 0x06009CFD RID: 40189 RVA: 0x003E45A5 File Offset: 0x003E27A5
		// (set) Token: 0x06009CFE RID: 40190 RVA: 0x003E45AD File Offset: 0x003E27AD
		public DataItem<float> GainHealth
		{
			get
			{
				return this.pGainHealth;
			}
			set
			{
				this.pGainHealth = value;
			}
		}

		// Token: 0x1700109F RID: 4255
		// (get) Token: 0x06009CFF RID: 40191 RVA: 0x003E45B6 File Offset: 0x003E27B6
		// (set) Token: 0x06009D00 RID: 40192 RVA: 0x003E45BE File Offset: 0x003E27BE
		public DataItem<float> GainFood
		{
			get
			{
				return this.pGainFood;
			}
			set
			{
				this.pGainFood = value;
			}
		}

		// Token: 0x170010A0 RID: 4256
		// (get) Token: 0x06009D01 RID: 40193 RVA: 0x003E45C7 File Offset: 0x003E27C7
		// (set) Token: 0x06009D02 RID: 40194 RVA: 0x003E45CF File Offset: 0x003E27CF
		public DataItem<float> GainWater
		{
			get
			{
				return this.pGainWater;
			}
			set
			{
				this.pGainWater = value;
			}
		}

		// Token: 0x170010A1 RID: 4257
		// (get) Token: 0x06009D03 RID: 40195 RVA: 0x003E45D8 File Offset: 0x003E27D8
		// (set) Token: 0x06009D04 RID: 40196 RVA: 0x003E45E0 File Offset: 0x003E27E0
		public DataItem<float> GainStamina
		{
			get
			{
				return this.pGainStamina;
			}
			set
			{
				this.pGainStamina = value;
			}
		}

		// Token: 0x170010A2 RID: 4258
		// (get) Token: 0x06009D05 RID: 40197 RVA: 0x003E45E9 File Offset: 0x003E27E9
		// (set) Token: 0x06009D06 RID: 40198 RVA: 0x003E45F1 File Offset: 0x003E27F1
		public DataItem<float> GainSickness
		{
			get
			{
				return this.pGainSickness;
			}
			set
			{
				this.pGainSickness = value;
			}
		}

		// Token: 0x170010A3 RID: 4259
		// (get) Token: 0x06009D07 RID: 40199 RVA: 0x003E45FA File Offset: 0x003E27FA
		// (set) Token: 0x06009D08 RID: 40200 RVA: 0x003E4602 File Offset: 0x003E2802
		public DataItem<float> GainWellness
		{
			get
			{
				return this.pGainWellness;
			}
			set
			{
				this.pGainWellness = value;
			}
		}

		// Token: 0x170010A4 RID: 4260
		// (get) Token: 0x06009D09 RID: 40201 RVA: 0x003E460B File Offset: 0x003E280B
		// (set) Token: 0x06009D0A RID: 40202 RVA: 0x003E4613 File Offset: 0x003E2813
		public DataItem<string> Buff
		{
			get
			{
				return this.pBuff;
			}
			set
			{
				this.pBuff = value;
			}
		}

		// Token: 0x170010A5 RID: 4261
		// (get) Token: 0x06009D0B RID: 40203 RVA: 0x003E461C File Offset: 0x003E281C
		// (set) Token: 0x06009D0C RID: 40204 RVA: 0x003E4624 File Offset: 0x003E2824
		public DataItem<string> BuffChance
		{
			get
			{
				return this.pBuffChance;
			}
			set
			{
				this.pBuffChance = value;
			}
		}

		// Token: 0x170010A6 RID: 4262
		// (get) Token: 0x06009D0D RID: 40205 RVA: 0x003E462D File Offset: 0x003E282D
		// (set) Token: 0x06009D0E RID: 40206 RVA: 0x003E4635 File Offset: 0x003E2835
		public DataItem<string> Debuff
		{
			get
			{
				return this.pDebuff;
			}
			set
			{
				this.pDebuff = value;
			}
		}

		// Token: 0x170010A7 RID: 4263
		// (get) Token: 0x06009D0F RID: 40207 RVA: 0x003E463E File Offset: 0x003E283E
		// (set) Token: 0x06009D10 RID: 40208 RVA: 0x003E4646 File Offset: 0x003E2846
		public DataItem<string> CreateItem
		{
			get
			{
				return this.pCreateItem;
			}
			set
			{
				this.pCreateItem = value;
			}
		}

		// Token: 0x170010A8 RID: 4264
		// (get) Token: 0x06009D11 RID: 40209 RVA: 0x003E464F File Offset: 0x003E284F
		// (set) Token: 0x06009D12 RID: 40210 RVA: 0x003E4657 File Offset: 0x003E2857
		public DataItem<int> ConditionRaycastBlock
		{
			get
			{
				return this.pConditionRaycastBlock;
			}
			set
			{
				this.pConditionRaycastBlock = value;
			}
		}

		// Token: 0x170010A9 RID: 4265
		// (get) Token: 0x06009D13 RID: 40211 RVA: 0x003E4660 File Offset: 0x003E2860
		// (set) Token: 0x06009D14 RID: 40212 RVA: 0x003E4668 File Offset: 0x003E2868
		public DataItem<int> GainGas
		{
			get
			{
				return this.pGainGas;
			}
			set
			{
				this.pGainGas = value;
			}
		}

		// Token: 0x170010AA RID: 4266
		// (get) Token: 0x06009D15 RID: 40213 RVA: 0x003E4671 File Offset: 0x003E2871
		// (set) Token: 0x06009D16 RID: 40214 RVA: 0x003E4679 File Offset: 0x003E2879
		public DataItem<bool> Consume
		{
			get
			{
				return this.pConsume;
			}
			set
			{
				this.pConsume = value;
			}
		}

		// Token: 0x170010AB RID: 4267
		// (get) Token: 0x06009D17 RID: 40215 RVA: 0x003E4682 File Offset: 0x003E2882
		// (set) Token: 0x06009D18 RID: 40216 RVA: 0x003E468A File Offset: 0x003E288A
		public DataItem<string> Blockname
		{
			get
			{
				return this.pBlockname;
			}
			set
			{
				this.pBlockname = value;
			}
		}

		// Token: 0x170010AC RID: 4268
		// (get) Token: 0x06009D19 RID: 40217 RVA: 0x003E4693 File Offset: 0x003E2893
		// (set) Token: 0x06009D1A RID: 40218 RVA: 0x003E469B File Offset: 0x003E289B
		public DataItem<float> ThrowStrengthDefault
		{
			get
			{
				return this.pThrowStrengthDefault;
			}
			set
			{
				this.pThrowStrengthDefault = value;
			}
		}

		// Token: 0x170010AD RID: 4269
		// (get) Token: 0x06009D1B RID: 40219 RVA: 0x003E46A4 File Offset: 0x003E28A4
		// (set) Token: 0x06009D1C RID: 40220 RVA: 0x003E46AC File Offset: 0x003E28AC
		public DataItem<float> ThrowStrengthMax
		{
			get
			{
				return this.pThrowStrengthMax;
			}
			set
			{
				this.pThrowStrengthMax = value;
			}
		}

		// Token: 0x170010AE RID: 4270
		// (get) Token: 0x06009D1D RID: 40221 RVA: 0x003E46B5 File Offset: 0x003E28B5
		// (set) Token: 0x06009D1E RID: 40222 RVA: 0x003E46BD File Offset: 0x003E28BD
		public DataItem<float> MaxStrainTime
		{
			get
			{
				return this.pMaxStrainTime;
			}
			set
			{
				this.pMaxStrainTime = value;
			}
		}

		// Token: 0x170010AF RID: 4271
		// (get) Token: 0x06009D1F RID: 40223 RVA: 0x003E46C6 File Offset: 0x003E28C6
		// (set) Token: 0x06009D20 RID: 40224 RVA: 0x003E46CE File Offset: 0x003E28CE
		public DataItem<int> MagazineSize
		{
			get
			{
				return this.pMagazineSize;
			}
			set
			{
				this.pMagazineSize = value;
			}
		}

		// Token: 0x170010B0 RID: 4272
		// (get) Token: 0x06009D21 RID: 40225 RVA: 0x003E46D7 File Offset: 0x003E28D7
		// (set) Token: 0x06009D22 RID: 40226 RVA: 0x003E46DF File Offset: 0x003E28DF
		public DataItem<string> MagazineItem
		{
			get
			{
				return this.pMagazineItem;
			}
			set
			{
				this.pMagazineItem = value;
			}
		}

		// Token: 0x170010B1 RID: 4273
		// (get) Token: 0x06009D23 RID: 40227 RVA: 0x003E46E8 File Offset: 0x003E28E8
		// (set) Token: 0x06009D24 RID: 40228 RVA: 0x003E46F0 File Offset: 0x003E28F0
		public DataItem<float> ReloadTime
		{
			get
			{
				return this.pReloadTime;
			}
			set
			{
				this.pReloadTime = value;
			}
		}

		// Token: 0x170010B2 RID: 4274
		// (get) Token: 0x06009D25 RID: 40229 RVA: 0x003E46F9 File Offset: 0x003E28F9
		// (set) Token: 0x06009D26 RID: 40230 RVA: 0x003E4701 File Offset: 0x003E2901
		public DataItem<string> BulletIcon
		{
			get
			{
				return this.pBulletIcon;
			}
			set
			{
				this.pBulletIcon = value;
			}
		}

		// Token: 0x170010B3 RID: 4275
		// (get) Token: 0x06009D27 RID: 40231 RVA: 0x003E470A File Offset: 0x003E290A
		// (set) Token: 0x06009D28 RID: 40232 RVA: 0x003E4712 File Offset: 0x003E2912
		public DataItem<int> RaysPerShot
		{
			get
			{
				return this.pRaysPerShot;
			}
			set
			{
				this.pRaysPerShot = value;
			}
		}

		// Token: 0x170010B4 RID: 4276
		// (get) Token: 0x06009D29 RID: 40233 RVA: 0x003E471B File Offset: 0x003E291B
		// (set) Token: 0x06009D2A RID: 40234 RVA: 0x003E4723 File Offset: 0x003E2923
		public DataItem<float> RaysSpread
		{
			get
			{
				return this.pRaysSpread;
			}
			set
			{
				this.pRaysSpread = value;
			}
		}

		// Token: 0x170010B5 RID: 4277
		// (get) Token: 0x06009D2B RID: 40235 RVA: 0x003E472C File Offset: 0x003E292C
		// (set) Token: 0x06009D2C RID: 40236 RVA: 0x003E4734 File Offset: 0x003E2934
		public DataItem<float> Sphere
		{
			get
			{
				return this.pSphere;
			}
			set
			{
				this.pSphere = value;
			}
		}

		// Token: 0x170010B6 RID: 4278
		// (get) Token: 0x06009D2D RID: 40237 RVA: 0x003E473D File Offset: 0x003E293D
		// (set) Token: 0x06009D2E RID: 40238 RVA: 0x003E4745 File Offset: 0x003E2945
		public DataItem<int> CrosshairMinDistance
		{
			get
			{
				return this.pCrosshairMinDistance;
			}
			set
			{
				this.pCrosshairMinDistance = value;
			}
		}

		// Token: 0x170010B7 RID: 4279
		// (get) Token: 0x06009D2F RID: 40239 RVA: 0x003E474E File Offset: 0x003E294E
		// (set) Token: 0x06009D30 RID: 40240 RVA: 0x003E4756 File Offset: 0x003E2956
		public DataItem<int> CrosshairMaxDistance
		{
			get
			{
				return this.pCrosshairMaxDistance;
			}
			set
			{
				this.pCrosshairMaxDistance = value;
			}
		}

		// Token: 0x170010B8 RID: 4280
		// (get) Token: 0x06009D31 RID: 40241 RVA: 0x003E475F File Offset: 0x003E295F
		// (set) Token: 0x06009D32 RID: 40242 RVA: 0x003E4767 File Offset: 0x003E2967
		public DataItem<int> DamageEntity
		{
			get
			{
				return this.pDamageEntity;
			}
			set
			{
				this.pDamageEntity = value;
			}
		}

		// Token: 0x170010B9 RID: 4281
		// (get) Token: 0x06009D33 RID: 40243 RVA: 0x003E4770 File Offset: 0x003E2970
		// (set) Token: 0x06009D34 RID: 40244 RVA: 0x003E4778 File Offset: 0x003E2978
		public DataItem<float> DamageBlock
		{
			get
			{
				return this.pDamageBlock;
			}
			set
			{
				this.pDamageBlock = value;
			}
		}

		// Token: 0x170010BA RID: 4282
		// (get) Token: 0x06009D35 RID: 40245 RVA: 0x003E4781 File Offset: 0x003E2981
		// (set) Token: 0x06009D36 RID: 40246 RVA: 0x003E4789 File Offset: 0x003E2989
		public DataItem<string> ParticlesMuzzleFire
		{
			get
			{
				return this.pParticlesMuzzleFire;
			}
			set
			{
				this.pParticlesMuzzleFire = value;
			}
		}

		// Token: 0x170010BB RID: 4283
		// (get) Token: 0x06009D37 RID: 40247 RVA: 0x003E4792 File Offset: 0x003E2992
		// (set) Token: 0x06009D38 RID: 40248 RVA: 0x003E479A File Offset: 0x003E299A
		public DataItem<string> ParticlesMuzzleSmoke
		{
			get
			{
				return this.pParticlesMuzzleSmoke;
			}
			set
			{
				this.pParticlesMuzzleSmoke = value;
			}
		}

		// Token: 0x170010BC RID: 4284
		// (get) Token: 0x06009D39 RID: 40249 RVA: 0x003E47A3 File Offset: 0x003E29A3
		// (set) Token: 0x06009D3A RID: 40250 RVA: 0x003E47AB File Offset: 0x003E29AB
		public DataItem<float> BlockRange
		{
			get
			{
				return this.pBlockRange;
			}
			set
			{
				this.pBlockRange = value;
			}
		}

		// Token: 0x170010BD RID: 4285
		// (get) Token: 0x06009D3B RID: 40251 RVA: 0x003E47B4 File Offset: 0x003E29B4
		// (set) Token: 0x06009D3C RID: 40252 RVA: 0x003E47BC File Offset: 0x003E29BC
		public DataItem<bool> AutoFire
		{
			get
			{
				return this.pAutoFire;
			}
			set
			{
				this.pAutoFire = value;
			}
		}

		// Token: 0x170010BE RID: 4286
		// (get) Token: 0x06009D3D RID: 40253 RVA: 0x003E47C5 File Offset: 0x003E29C5
		// (set) Token: 0x06009D3E RID: 40254 RVA: 0x003E47CD File Offset: 0x003E29CD
		public DataItem<float> HordeMeterRate
		{
			get
			{
				return this.pHordeMeterRate;
			}
			set
			{
				this.pHordeMeterRate = value;
			}
		}

		// Token: 0x170010BF RID: 4287
		// (get) Token: 0x06009D3F RID: 40255 RVA: 0x003E47D6 File Offset: 0x003E29D6
		// (set) Token: 0x06009D40 RID: 40256 RVA: 0x003E47DE File Offset: 0x003E29DE
		public DataItem<float> HordeMeterDistance
		{
			get
			{
				return this.pHordeMeterDistance;
			}
			set
			{
				this.pHordeMeterDistance = value;
			}
		}

		// Token: 0x170010C0 RID: 4288
		// (get) Token: 0x06009D41 RID: 40257 RVA: 0x003E47E7 File Offset: 0x003E29E7
		// (set) Token: 0x06009D42 RID: 40258 RVA: 0x003E47EF File Offset: 0x003E29EF
		public DataItem<string> HitmaskOverride
		{
			get
			{
				return this.pHitmaskOverride;
			}
			set
			{
				this.pHitmaskOverride = value;
			}
		}

		// Token: 0x170010C1 RID: 4289
		// (get) Token: 0x06009D43 RID: 40259 RVA: 0x003E47F8 File Offset: 0x003E29F8
		// (set) Token: 0x06009D44 RID: 40260 RVA: 0x003E4800 File Offset: 0x003E2A00
		public DataItem<bool> SingleMagazineUsage
		{
			get
			{
				return this.pSingleMagazineUsage;
			}
			set
			{
				this.pSingleMagazineUsage = value;
			}
		}

		// Token: 0x170010C2 RID: 4290
		// (get) Token: 0x06009D45 RID: 40261 RVA: 0x003E4809 File Offset: 0x003E2A09
		// (set) Token: 0x06009D46 RID: 40262 RVA: 0x003E4811 File Offset: 0x003E2A11
		public DataItem<string> BulletMaterial
		{
			get
			{
				return this.pBulletMaterial;
			}
			set
			{
				this.pBulletMaterial = value;
			}
		}

		// Token: 0x170010C3 RID: 4291
		// (get) Token: 0x06009D47 RID: 40263 RVA: 0x003E481A File Offset: 0x003E2A1A
		// (set) Token: 0x06009D48 RID: 40264 RVA: 0x003E4822 File Offset: 0x003E2A22
		public DataItem<bool> InfiniteAmmo
		{
			get
			{
				return this.pInfiniteAmmo;
			}
			set
			{
				this.pInfiniteAmmo = value;
			}
		}

		// Token: 0x170010C4 RID: 4292
		// (get) Token: 0x06009D49 RID: 40265 RVA: 0x003E482B File Offset: 0x003E2A2B
		// (set) Token: 0x06009D4A RID: 40266 RVA: 0x003E4833 File Offset: 0x003E2A33
		public DataItem<float> ZoomMaxOut
		{
			get
			{
				return this.pZoomMaxOut;
			}
			set
			{
				this.pZoomMaxOut = value;
			}
		}

		// Token: 0x170010C5 RID: 4293
		// (get) Token: 0x06009D4B RID: 40267 RVA: 0x003E483C File Offset: 0x003E2A3C
		// (set) Token: 0x06009D4C RID: 40268 RVA: 0x003E4844 File Offset: 0x003E2A44
		public DataItem<float> ZoomMaxIn
		{
			get
			{
				return this.pZoomMaxIn;
			}
			set
			{
				this.pZoomMaxIn = value;
			}
		}

		// Token: 0x170010C6 RID: 4294
		// (get) Token: 0x06009D4D RID: 40269 RVA: 0x003E484D File Offset: 0x003E2A4D
		// (set) Token: 0x06009D4E RID: 40270 RVA: 0x003E4855 File Offset: 0x003E2A55
		public DataItem<string> ZoomOverlay
		{
			get
			{
				return this.pZoomOverlay;
			}
			set
			{
				this.pZoomOverlay = value;
			}
		}

		// Token: 0x170010C7 RID: 4295
		// (get) Token: 0x06009D4F RID: 40271 RVA: 0x003E485E File Offset: 0x003E2A5E
		// (set) Token: 0x06009D50 RID: 40272 RVA: 0x003E4866 File Offset: 0x003E2A66
		public DataItem<int> Velocity
		{
			get
			{
				return this.pVelocity;
			}
			set
			{
				this.pVelocity = value;
			}
		}

		// Token: 0x170010C8 RID: 4296
		// (get) Token: 0x06009D51 RID: 40273 RVA: 0x003E486F File Offset: 0x003E2A6F
		// (set) Token: 0x06009D52 RID: 40274 RVA: 0x003E4877 File Offset: 0x003E2A77
		public DataItem<float> FlyTime
		{
			get
			{
				return this.pFlyTime;
			}
			set
			{
				this.pFlyTime = value;
			}
		}

		// Token: 0x170010C9 RID: 4297
		// (get) Token: 0x06009D53 RID: 40275 RVA: 0x003E4880 File Offset: 0x003E2A80
		// (set) Token: 0x06009D54 RID: 40276 RVA: 0x003E4888 File Offset: 0x003E2A88
		public DataItem<float> LifeTime
		{
			get
			{
				return this.pLifeTime;
			}
			set
			{
				this.pLifeTime = value;
			}
		}

		// Token: 0x170010CA RID: 4298
		// (get) Token: 0x06009D55 RID: 40277 RVA: 0x003E4891 File Offset: 0x003E2A91
		// (set) Token: 0x06009D56 RID: 40278 RVA: 0x003E4899 File Offset: 0x003E2A99
		public DataItem<float> CollisionRadius
		{
			get
			{
				return this.pCollisionRadius;
			}
			set
			{
				this.pCollisionRadius = value;
			}
		}

		// Token: 0x170010CB RID: 4299
		// (get) Token: 0x06009D57 RID: 40279 RVA: 0x003E48A2 File Offset: 0x003E2AA2
		// (set) Token: 0x06009D58 RID: 40280 RVA: 0x003E48AA File Offset: 0x003E2AAA
		public DataItem<int> ProjectileInitialVelocity
		{
			get
			{
				return this.pProjectileInitialVelocity;
			}
			set
			{
				this.pProjectileInitialVelocity = value;
			}
		}

		// Token: 0x170010CC RID: 4300
		// (get) Token: 0x06009D59 RID: 40281 RVA: 0x003E48B3 File Offset: 0x003E2AB3
		// (set) Token: 0x06009D5A RID: 40282 RVA: 0x003E48BB File Offset: 0x003E2ABB
		public DataItem<string> Fertileblock
		{
			get
			{
				return this.pFertileblock;
			}
			set
			{
				this.pFertileblock = value;
			}
		}

		// Token: 0x170010CD RID: 4301
		// (get) Token: 0x06009D5B RID: 40283 RVA: 0x003E48C4 File Offset: 0x003E2AC4
		// (set) Token: 0x06009D5C RID: 40284 RVA: 0x003E48CC File Offset: 0x003E2ACC
		public DataItem<string> Adjacentblock
		{
			get
			{
				return this.pAdjacentblock;
			}
			set
			{
				this.pAdjacentblock = value;
			}
		}

		// Token: 0x170010CE RID: 4302
		// (get) Token: 0x06009D5D RID: 40285 RVA: 0x003E48D5 File Offset: 0x003E2AD5
		// (set) Token: 0x06009D5E RID: 40286 RVA: 0x003E48DD File Offset: 0x003E2ADD
		public DataItem<int> RepairAmount
		{
			get
			{
				return this.pRepairAmount;
			}
			set
			{
				this.pRepairAmount = value;
			}
		}

		// Token: 0x170010CF RID: 4303
		// (get) Token: 0x06009D5F RID: 40287 RVA: 0x003E48E6 File Offset: 0x003E2AE6
		// (set) Token: 0x06009D60 RID: 40288 RVA: 0x003E48EE File Offset: 0x003E2AEE
		public DataItem<int> UpgradeHitOffset
		{
			get
			{
				return this.pUpgradeHitOffset;
			}
			set
			{
				this.pUpgradeHitOffset = value;
			}
		}

		// Token: 0x170010D0 RID: 4304
		// (get) Token: 0x06009D61 RID: 40289 RVA: 0x003E48F7 File Offset: 0x003E2AF7
		// (set) Token: 0x06009D62 RID: 40290 RVA: 0x003E48FF File Offset: 0x003E2AFF
		public DataItem<string> AllowedUpgradeItems
		{
			get
			{
				return this.pAllowedUpgradeItems;
			}
			set
			{
				this.pAllowedUpgradeItems = value;
			}
		}

		// Token: 0x170010D1 RID: 4305
		// (get) Token: 0x06009D63 RID: 40291 RVA: 0x003E4908 File Offset: 0x003E2B08
		// (set) Token: 0x06009D64 RID: 40292 RVA: 0x003E4910 File Offset: 0x003E2B10
		public DataItem<string> RestrictedUpgradeItems
		{
			get
			{
				return this.pRestrictedUpgradeItems;
			}
			set
			{
				this.pRestrictedUpgradeItems = value;
			}
		}

		// Token: 0x170010D2 RID: 4306
		// (get) Token: 0x06009D65 RID: 40293 RVA: 0x003E4919 File Offset: 0x003E2B19
		// (set) Token: 0x06009D66 RID: 40294 RVA: 0x003E4921 File Offset: 0x003E2B21
		public DataItem<string> UpgradeActionSound
		{
			get
			{
				return this.pUpgradeActionSound;
			}
			set
			{
				this.pUpgradeActionSound = value;
			}
		}

		// Token: 0x170010D3 RID: 4307
		// (get) Token: 0x06009D67 RID: 40295 RVA: 0x003E492A File Offset: 0x003E2B2A
		// (set) Token: 0x06009D68 RID: 40296 RVA: 0x003E4932 File Offset: 0x003E2B32
		public DataItem<string> RepairActionSound
		{
			get
			{
				return this.pRepairActionSound;
			}
			set
			{
				this.pRepairActionSound = value;
			}
		}

		// Token: 0x170010D4 RID: 4308
		// (get) Token: 0x06009D69 RID: 40297 RVA: 0x003E493B File Offset: 0x003E2B3B
		// (set) Token: 0x06009D6A RID: 40298 RVA: 0x003E4943 File Offset: 0x003E2B43
		public DataItem<string> ReferenceItem
		{
			get
			{
				return this.pReferenceItem;
			}
			set
			{
				this.pReferenceItem = value;
			}
		}

		// Token: 0x170010D5 RID: 4309
		// (get) Token: 0x06009D6B RID: 40299 RVA: 0x003E494C File Offset: 0x003E2B4C
		// (set) Token: 0x06009D6C RID: 40300 RVA: 0x003E4954 File Offset: 0x003E2B54
		public DataItem<string> Mesh
		{
			get
			{
				return this.pMesh;
			}
			set
			{
				this.pMesh = value;
			}
		}

		// Token: 0x170010D6 RID: 4310
		// (get) Token: 0x06009D6D RID: 40301 RVA: 0x003E495D File Offset: 0x003E2B5D
		// (set) Token: 0x06009D6E RID: 40302 RVA: 0x003E4965 File Offset: 0x003E2B65
		public DataItem<int> ActionIdx
		{
			get
			{
				return this.pActionIdx;
			}
			set
			{
				this.pActionIdx = value;
			}
		}

		// Token: 0x170010D7 RID: 4311
		// (get) Token: 0x06009D6F RID: 40303 RVA: 0x003E496E File Offset: 0x003E2B6E
		// (set) Token: 0x06009D70 RID: 40304 RVA: 0x003E4976 File Offset: 0x003E2B76
		public DataItem<string> Title
		{
			get
			{
				return this.pTitle;
			}
			set
			{
				this.pTitle = value;
			}
		}

		// Token: 0x170010D8 RID: 4312
		// (get) Token: 0x06009D71 RID: 40305 RVA: 0x003E497F File Offset: 0x003E2B7F
		// (set) Token: 0x06009D72 RID: 40306 RVA: 0x003E4987 File Offset: 0x003E2B87
		public DataItem<string> Description
		{
			get
			{
				return this.pDescription;
			}
			set
			{
				this.pDescription = value;
			}
		}

		// Token: 0x170010D9 RID: 4313
		// (get) Token: 0x06009D73 RID: 40307 RVA: 0x003E4990 File Offset: 0x003E2B90
		// (set) Token: 0x06009D74 RID: 40308 RVA: 0x003E4998 File Offset: 0x003E2B98
		public DataItem<string> RecipesToLearn
		{
			get
			{
				return this.pRecipesToLearn;
			}
			set
			{
				this.pRecipesToLearn = value;
			}
		}

		// Token: 0x170010DA RID: 4314
		// (get) Token: 0x06009D75 RID: 40309 RVA: 0x003E49A1 File Offset: 0x003E2BA1
		// (set) Token: 0x06009D76 RID: 40310 RVA: 0x003E49A9 File Offset: 0x003E2BA9
		public DataItem<string> InstantiateOnLoad
		{
			get
			{
				return this.pInstantiateOnLoad;
			}
			set
			{
				this.pInstantiateOnLoad = value;
			}
		}

		// Token: 0x170010DB RID: 4315
		// (get) Token: 0x06009D77 RID: 40311 RVA: 0x003E49B2 File Offset: 0x003E2BB2
		// (set) Token: 0x06009D78 RID: 40312 RVA: 0x003E49BA File Offset: 0x003E2BBA
		public DataItem<string> SoundDraw
		{
			get
			{
				return this.pSoundDraw;
			}
			set
			{
				this.pSoundDraw = value;
			}
		}

		// Token: 0x170010DC RID: 4316
		// (get) Token: 0x06009D79 RID: 40313 RVA: 0x003E49C3 File Offset: 0x003E2BC3
		// (set) Token: 0x06009D7A RID: 40314 RVA: 0x003E49CB File Offset: 0x003E2BCB
		public DataItem<DamageBonusData> DamageBonus
		{
			get
			{
				return this.pDamageBonus;
			}
			set
			{
				this.pDamageBonus = value;
			}
		}

		// Token: 0x170010DD RID: 4317
		// (get) Token: 0x06009D7B RID: 40315 RVA: 0x003E49D4 File Offset: 0x003E2BD4
		// (set) Token: 0x06009D7C RID: 40316 RVA: 0x003E49DC File Offset: 0x003E2BDC
		public DataItem<ExplosionData> Explosion
		{
			get
			{
				return this.pExplosion;
			}
			set
			{
				this.pExplosion = value;
			}
		}

		// Token: 0x06009D7D RID: 40317 RVA: 0x003E49E8 File Offset: 0x003E2BE8
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			List<IDataItem> list = new List<IDataItem>();
			if (_recursive && this.pDamageBonus != null)
			{
				list.AddRange(this.pDamageBonus.Value.GetDisplayValues(true));
			}
			if (_recursive && this.pExplosion != null)
			{
				list.AddRange(this.pExplosion.Value.GetDisplayValues(true));
			}
			return list;
		}

		// Token: 0x0400792B RID: 31019
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pDelay;

		// Token: 0x0400792C RID: 31020
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pRange;

		// Token: 0x0400792D RID: 31021
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundStart;

		// Token: 0x0400792E RID: 31022
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundRepeat;

		// Token: 0x0400792F RID: 31023
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundEnd;

		// Token: 0x04007930 RID: 31024
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundEmpty;

		// Token: 0x04007931 RID: 31025
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundReload;

		// Token: 0x04007932 RID: 31026
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundWarning;

		// Token: 0x04007933 RID: 31027
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pStaminaUsage;

		// Token: 0x04007934 RID: 31028
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pUseTime;

		// Token: 0x04007935 RID: 31029
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname1;

		// Token: 0x04007936 RID: 31030
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname2;

		// Token: 0x04007937 RID: 31031
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname3;

		// Token: 0x04007938 RID: 31032
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname4;

		// Token: 0x04007939 RID: 31033
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname5;

		// Token: 0x0400793A RID: 31034
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname6;

		// Token: 0x0400793B RID: 31035
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname7;

		// Token: 0x0400793C RID: 31036
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname8;

		// Token: 0x0400793D RID: 31037
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname9;

		// Token: 0x0400793E RID: 31038
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pChangeItemTo;

		// Token: 0x0400793F RID: 31039
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pChangeBlockTo;

		// Token: 0x04007940 RID: 31040
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pDoBlockAction;

		// Token: 0x04007941 RID: 31041
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainHealth;

		// Token: 0x04007942 RID: 31042
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainFood;

		// Token: 0x04007943 RID: 31043
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainWater;

		// Token: 0x04007944 RID: 31044
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainStamina;

		// Token: 0x04007945 RID: 31045
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainSickness;

		// Token: 0x04007946 RID: 31046
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainWellness;

		// Token: 0x04007947 RID: 31047
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBuff;

		// Token: 0x04007948 RID: 31048
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBuffChance;

		// Token: 0x04007949 RID: 31049
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pDebuff;

		// Token: 0x0400794A RID: 31050
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pCreateItem;

		// Token: 0x0400794B RID: 31051
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pConditionRaycastBlock;

		// Token: 0x0400794C RID: 31052
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pGainGas;

		// Token: 0x0400794D RID: 31053
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pConsume;

		// Token: 0x0400794E RID: 31054
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBlockname;

		// Token: 0x0400794F RID: 31055
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pThrowStrengthDefault;

		// Token: 0x04007950 RID: 31056
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pThrowStrengthMax;

		// Token: 0x04007951 RID: 31057
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pMaxStrainTime;

		// Token: 0x04007952 RID: 31058
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pMagazineSize;

		// Token: 0x04007953 RID: 31059
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pMagazineItem;

		// Token: 0x04007954 RID: 31060
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pReloadTime;

		// Token: 0x04007955 RID: 31061
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBulletIcon;

		// Token: 0x04007956 RID: 31062
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pRaysPerShot;

		// Token: 0x04007957 RID: 31063
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pRaysSpread;

		// Token: 0x04007958 RID: 31064
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pSphere;

		// Token: 0x04007959 RID: 31065
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pCrosshairMinDistance;

		// Token: 0x0400795A RID: 31066
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pCrosshairMaxDistance;

		// Token: 0x0400795B RID: 31067
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pDamageEntity;

		// Token: 0x0400795C RID: 31068
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pDamageBlock;

		// Token: 0x0400795D RID: 31069
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pParticlesMuzzleFire;

		// Token: 0x0400795E RID: 31070
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pParticlesMuzzleSmoke;

		// Token: 0x0400795F RID: 31071
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pBlockRange;

		// Token: 0x04007960 RID: 31072
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pAutoFire;

		// Token: 0x04007961 RID: 31073
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pHordeMeterRate;

		// Token: 0x04007962 RID: 31074
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pHordeMeterDistance;

		// Token: 0x04007963 RID: 31075
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pHitmaskOverride;

		// Token: 0x04007964 RID: 31076
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pSingleMagazineUsage;

		// Token: 0x04007965 RID: 31077
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBulletMaterial;

		// Token: 0x04007966 RID: 31078
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pInfiniteAmmo;

		// Token: 0x04007967 RID: 31079
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pZoomMaxOut;

		// Token: 0x04007968 RID: 31080
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pZoomMaxIn;

		// Token: 0x04007969 RID: 31081
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pZoomOverlay;

		// Token: 0x0400796A RID: 31082
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pVelocity;

		// Token: 0x0400796B RID: 31083
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pFlyTime;

		// Token: 0x0400796C RID: 31084
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pLifeTime;

		// Token: 0x0400796D RID: 31085
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pCollisionRadius;

		// Token: 0x0400796E RID: 31086
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pProjectileInitialVelocity;

		// Token: 0x0400796F RID: 31087
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFertileblock;

		// Token: 0x04007970 RID: 31088
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pAdjacentblock;

		// Token: 0x04007971 RID: 31089
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pRepairAmount;

		// Token: 0x04007972 RID: 31090
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pUpgradeHitOffset;

		// Token: 0x04007973 RID: 31091
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pAllowedUpgradeItems;

		// Token: 0x04007974 RID: 31092
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pRestrictedUpgradeItems;

		// Token: 0x04007975 RID: 31093
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pUpgradeActionSound;

		// Token: 0x04007976 RID: 31094
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pRepairActionSound;

		// Token: 0x04007977 RID: 31095
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pReferenceItem;

		// Token: 0x04007978 RID: 31096
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pMesh;

		// Token: 0x04007979 RID: 31097
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pActionIdx;

		// Token: 0x0400797A RID: 31098
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pTitle;

		// Token: 0x0400797B RID: 31099
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pDescription;

		// Token: 0x0400797C RID: 31100
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pRecipesToLearn;

		// Token: 0x0400797D RID: 31101
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pInstantiateOnLoad;

		// Token: 0x0400797E RID: 31102
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundDraw;

		// Token: 0x0400797F RID: 31103
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<DamageBonusData> pDamageBonus;

		// Token: 0x04007980 RID: 31104
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<ExplosionData> pExplosion;

		// Token: 0x0200139A RID: 5018
		public static class Parser
		{
			// Token: 0x06009D7F RID: 40319 RVA: 0x003E4A4C File Offset: 0x003E2C4C
			[PublicizedFrom(EAccessModifier.Private)]
			public static DataItem<string> ParseItem(string _string, PositionXmlElement _elem)
			{
				string startValue;
				try
				{
					startValue = stringParser.Parse(ParserUtils.ParseStringAttribute(_elem, "value", true, null));
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException(string.Concat(new string[]
					{
						"Could not parse attribute \"",
						_elem.Name,
						"\" value \"",
						ParserUtils.ParseStringAttribute(_elem, "value", true, null),
						"\""
					}), _elem.LineNumber, innerException);
				}
				return new DataItem<string>(_string, startValue);
			}

			// Token: 0x06009D80 RID: 40320 RVA: 0x003E4AD0 File Offset: 0x003E2CD0
			public static ItemAction Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "ItemAction";
				Type type = Type.GetType(typeof(ItemActionData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				ItemAction itemAction = (ItemAction)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing ItemAction", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!ItemActionData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing ItemAction", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
						if (num <= 2050383924U)
						{
							if (num <= 973038343U)
							{
								if (num <= 477934085U)
								{
									if (num <= 264004213U)
									{
										if (num <= 91459346U)
										{
											if (num != 32426346U)
											{
												if (num == 91459346U)
												{
													if (name == "CrosshairMinDistance")
													{
														int startValue;
														try
														{
															startValue = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
														}
														catch (Exception innerException)
														{
															throw new InvalidValueException(string.Concat(new string[]
															{
																"Could not parse attribute \"",
																positionXmlElement.Name,
																"\" value \"",
																ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
																"\""
															}), positionXmlElement.LineNumber, innerException);
														}
														DataItem<int> pCrosshairMinDistance = new DataItem<int>("CrosshairMinDistance", startValue);
														itemAction.pCrosshairMinDistance = pCrosshairMinDistance;
													}
												}
											}
											else if (name == "HitmaskOverride")
											{
												string startValue2;
												try
												{
													startValue2 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException2)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException2);
												}
												DataItem<string> pHitmaskOverride = new DataItem<string>("HitmaskOverride", startValue2);
												itemAction.pHitmaskOverride = pHitmaskOverride;
											}
										}
										else if (num != 132142556U)
										{
											if (num != 248841283U)
											{
												if (num == 264004213U)
												{
													if (name == "ProjectileInitialVelocity")
													{
														int startValue3;
														try
														{
															startValue3 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
														}
														catch (Exception innerException3)
														{
															throw new InvalidValueException(string.Concat(new string[]
															{
																"Could not parse attribute \"",
																positionXmlElement.Name,
																"\" value \"",
																ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
																"\""
															}), positionXmlElement.LineNumber, innerException3);
														}
														DataItem<int> pProjectileInitialVelocity = new DataItem<int>("ProjectileInitialVelocity", startValue3);
														itemAction.pProjectileInitialVelocity = pProjectileInitialVelocity;
													}
												}
											}
											else if (name == "RaysSpread")
											{
												float startValue4;
												try
												{
													startValue4 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException4)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException4);
												}
												DataItem<float> pRaysSpread = new DataItem<float>("RaysSpread", startValue4);
												itemAction.pRaysSpread = pRaysSpread;
											}
										}
										else if (name == "Mesh")
										{
											string startValue5;
											try
											{
												startValue5 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException5)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException5);
											}
											DataItem<string> pMesh = new DataItem<string>("Mesh", startValue5);
											itemAction.pMesh = pMesh;
										}
									}
									else if (num <= 388984571U)
									{
										if (num != 383713930U)
										{
											if (num == 388984571U)
											{
												if (name == "DamageEntity")
												{
													int startValue6;
													try
													{
														startValue6 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException6)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException6);
													}
													DataItem<int> pDamageEntity = new DataItem<int>("DamageEntity", startValue6);
													itemAction.pDamageEntity = pDamageEntity;
												}
											}
										}
										else if (name == "AutoFire")
										{
											bool startValue7;
											try
											{
												startValue7 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException7)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException7);
											}
											DataItem<bool> pAutoFire = new DataItem<bool>("AutoFire", startValue7);
											itemAction.pAutoFire = pAutoFire;
										}
									}
									else if (num != 422257095U)
									{
										if (num != 441323083U)
										{
											if (num == 477934085U)
											{
												if (name == "MaxStrainTime")
												{
													float startValue8;
													try
													{
														startValue8 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException8)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException8);
													}
													DataItem<float> pMaxStrainTime = new DataItem<float>("MaxStrainTime", startValue8);
													itemAction.pMaxStrainTime = pMaxStrainTime;
												}
											}
										}
										else if (name == "RaysPerShot")
										{
											int startValue9;
											try
											{
												startValue9 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException9)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException9);
											}
											DataItem<int> pRaysPerShot = new DataItem<int>("RaysPerShot", startValue9);
											itemAction.pRaysPerShot = pRaysPerShot;
										}
									}
									else if (name == "GainGas")
									{
										int startValue10;
										try
										{
											startValue10 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException10)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException10);
										}
										DataItem<int> pGainGas = new DataItem<int>("GainGas", startValue10);
										itemAction.pGainGas = pGainGas;
									}
								}
								else if (num <= 678358751U)
								{
									if (num <= 573162709U)
									{
										if (num != 550270072U)
										{
											if (num == 573162709U)
											{
												if (name == "SoundEmpty")
												{
													string startValue11;
													try
													{
														startValue11 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException11)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException11);
													}
													DataItem<string> pSoundEmpty = new DataItem<string>("SoundEmpty", startValue11);
													itemAction.pSoundEmpty = pSoundEmpty;
												}
											}
										}
										else if (name == "Delay")
										{
											float startValue12;
											try
											{
												startValue12 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException12)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException12);
											}
											DataItem<float> pDelay = new DataItem<float>("Delay", startValue12);
											itemAction.pDelay = pDelay;
										}
									}
									else if (num != 573416248U)
									{
										if (num != 617902505U)
										{
											if (num == 678358751U)
											{
												if (name == "GainStamina")
												{
													float startValue13;
													try
													{
														startValue13 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException13)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException13);
													}
													DataItem<float> pGainStamina = new DataItem<float>("GainStamina", startValue13);
													itemAction.pGainStamina = pGainStamina;
												}
											}
										}
										else if (name == "Title")
										{
											string startValue14;
											try
											{
												startValue14 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException14)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException14);
											}
											DataItem<string> pTitle = new DataItem<string>("Title", startValue14);
											itemAction.pTitle = pTitle;
										}
									}
									else if (name == "Sphere")
									{
										float startValue15;
										try
										{
											startValue15 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException15)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException15);
										}
										DataItem<float> pSphere = new DataItem<float>("Sphere", startValue15);
										itemAction.pSphere = pSphere;
									}
								}
								else if (num <= 859861643U)
								{
									if (num != 738624775U)
									{
										if (num != 843848237U)
										{
											if (num == 859861643U)
											{
												if (name == "SoundRepeat")
												{
													string startValue16;
													try
													{
														startValue16 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException16)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException16);
													}
													DataItem<string> pSoundRepeat = new DataItem<string>("SoundRepeat", startValue16);
													itemAction.pSoundRepeat = pSoundRepeat;
												}
											}
										}
										else if (name == "CollisionRadius")
										{
											float startValue17;
											try
											{
												startValue17 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException17)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException17);
											}
											DataItem<float> pCollisionRadius = new DataItem<float>("CollisionRadius", startValue17);
											itemAction.pCollisionRadius = pCollisionRadius;
										}
									}
									else if (name == "ThrowStrengthDefault")
									{
										float startValue18;
										try
										{
											startValue18 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException18)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException18);
										}
										DataItem<float> pThrowStrengthDefault = new DataItem<float>("ThrowStrengthDefault", startValue18);
										itemAction.pThrowStrengthDefault = pThrowStrengthDefault;
									}
								}
								else if (num != 883311634U)
								{
									if (num != 906214847U)
									{
										if (num == 973038343U)
										{
											if (name == "InfiniteAmmo")
											{
												bool startValue19;
												try
												{
													startValue19 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException19)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException19);
												}
												DataItem<bool> pInfiniteAmmo = new DataItem<bool>("InfiniteAmmo", startValue19);
												itemAction.pInfiniteAmmo = pInfiniteAmmo;
											}
										}
									}
									else if (name == "ChangeBlockTo")
									{
										string startValue20;
										try
										{
											startValue20 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException20)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException20);
										}
										DataItem<string> pChangeBlockTo = new DataItem<string>("ChangeBlockTo", startValue20);
										itemAction.pChangeBlockTo = pChangeBlockTo;
									}
								}
								else if (name == "ZoomOverlay")
								{
									string startValue21;
									try
									{
										startValue21 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException21)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException21);
									}
									DataItem<string> pZoomOverlay = new DataItem<string>("ZoomOverlay", startValue21);
									itemAction.pZoomOverlay = pZoomOverlay;
								}
							}
							else if (num <= 1725856265U)
							{
								if (num <= 1237474039U)
								{
									if (num <= 1173181999U)
									{
										if (num != 1157581142U)
										{
											if (num == 1173181999U)
											{
												if (name == "BlockRange")
												{
													float startValue22;
													try
													{
														startValue22 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException22)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException22);
													}
													DataItem<float> pBlockRange = new DataItem<float>("BlockRange", startValue22);
													itemAction.pBlockRange = pBlockRange;
												}
											}
										}
										else if (name == "SingleMagazineUsage")
										{
											bool startValue23;
											try
											{
												startValue23 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException23)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException23);
											}
											DataItem<bool> pSingleMagazineUsage = new DataItem<bool>("SingleMagazineUsage", startValue23);
											itemAction.pSingleMagazineUsage = pSingleMagazineUsage;
										}
									}
									else if (num != 1180803064U)
									{
										if (num != 1221226493U)
										{
											if (num == 1237474039U)
											{
												if (name == "DamageBonus")
												{
													DamageBonusData startValue24 = DamageBonusData.Parser.Parse(positionXmlElement, _updateLater);
													DataItem<DamageBonusData> pDamageBonus = new DataItem<DamageBonusData>("DamageBonus", startValue24);
													itemAction.pDamageBonus = pDamageBonus;
												}
											}
										}
										else if (name == "RecipesToLearn")
										{
											string startValue25;
											try
											{
												startValue25 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException24)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException24);
											}
											DataItem<string> pRecipesToLearn = new DataItem<string>("RecipesToLearn", startValue25);
											itemAction.pRecipesToLearn = pRecipesToLearn;
										}
									}
									else if (name == "SoundStart")
									{
										string startValue26;
										try
										{
											startValue26 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException25)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException25);
										}
										DataItem<string> pSoundStart = new DataItem<string>("SoundStart", startValue26);
										itemAction.pSoundStart = pSoundStart;
									}
								}
								else if (num <= 1330610628U)
								{
									if (num != 1259223448U)
									{
										if (num != 1292404073U)
										{
											if (num == 1330610628U)
											{
												if (name == "CrosshairMaxDistance")
												{
													int startValue27;
													try
													{
														startValue27 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException26)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException26);
													}
													DataItem<int> pCrosshairMaxDistance = new DataItem<int>("CrosshairMaxDistance", startValue27);
													itemAction.pCrosshairMaxDistance = pCrosshairMaxDistance;
												}
											}
										}
										else if (name == "SoundEnd")
										{
											string startValue28;
											try
											{
												startValue28 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException27)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException27);
											}
											DataItem<string> pSoundEnd = new DataItem<string>("SoundEnd", startValue28);
											itemAction.pSoundEnd = pSoundEnd;
										}
									}
									else if (name == "ZoomMaxOut")
									{
										float startValue29;
										try
										{
											startValue29 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException28)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException28);
										}
										DataItem<float> pZoomMaxOut = new DataItem<float>("ZoomMaxOut", startValue29);
										itemAction.pZoomMaxOut = pZoomMaxOut;
									}
								}
								else if (num != 1601156781U)
								{
									if (num != 1660679176U)
									{
										if (num == 1725856265U)
										{
											if (name == "Description")
											{
												string startValue30;
												try
												{
													startValue30 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException29)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException29);
												}
												DataItem<string> pDescription = new DataItem<string>("Description", startValue30);
												itemAction.pDescription = pDescription;
											}
										}
									}
									else if (name == "BuffChance")
									{
										string startValue31;
										try
										{
											startValue31 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException30)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException30);
										}
										DataItem<string> pBuffChance = new DataItem<string>("BuffChance", startValue31);
										itemAction.pBuffChance = pBuffChance;
									}
								}
								else if (name == "ReloadTime")
								{
									float startValue32;
									try
									{
										startValue32 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException31)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException31);
									}
									DataItem<float> pReloadTime = new DataItem<float>("ReloadTime", startValue32);
									itemAction.pReloadTime = pReloadTime;
								}
							}
							else if (num <= 1811911295U)
							{
								if (num <= 1744800819U)
								{
									if (num != 1728023200U)
									{
										if (num == 1744800819U)
										{
											if (name == "FocusedBlockname5")
											{
												string startValue33;
												try
												{
													startValue33 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException32)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException32);
												}
												DataItem<string> pFocusedBlockname = new DataItem<string>("FocusedBlockname5", startValue33);
												itemAction.pFocusedBlockname5 = pFocusedBlockname;
											}
										}
									}
									else if (name == "FocusedBlockname4")
									{
										string startValue34;
										try
										{
											startValue34 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException33)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException33);
										}
										DataItem<string> pFocusedBlockname2 = new DataItem<string>("FocusedBlockname4", startValue34);
										itemAction.pFocusedBlockname4 = pFocusedBlockname2;
									}
								}
								else if (num != 1761578438U)
								{
									if (num != 1778356057U)
									{
										if (num == 1811911295U)
										{
											if (name == "FocusedBlockname1")
											{
												string startValue35;
												try
												{
													startValue35 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException34)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException34);
												}
												DataItem<string> pFocusedBlockname3 = new DataItem<string>("FocusedBlockname1", startValue35);
												itemAction.pFocusedBlockname1 = pFocusedBlockname3;
											}
										}
									}
									else if (name == "FocusedBlockname7")
									{
										string startValue36;
										try
										{
											startValue36 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException35)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException35);
										}
										DataItem<string> pFocusedBlockname4 = new DataItem<string>("FocusedBlockname7", startValue36);
										itemAction.pFocusedBlockname7 = pFocusedBlockname4;
									}
								}
								else if (name == "FocusedBlockname6")
								{
									string startValue37;
									try
									{
										startValue37 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException36)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException36);
									}
									DataItem<string> pFocusedBlockname5 = new DataItem<string>("FocusedBlockname6", startValue37);
									itemAction.pFocusedBlockname6 = pFocusedBlockname5;
								}
							}
							else if (num <= 1929345774U)
							{
								if (num != 1828688914U)
								{
									if (num != 1845466533U)
									{
										if (num == 1929345774U)
										{
											if (name == "LifeTime")
											{
												float startValue38;
												try
												{
													startValue38 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException37)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException37);
												}
												DataItem<float> pLifeTime = new DataItem<float>("LifeTime", startValue38);
												itemAction.pLifeTime = pLifeTime;
											}
										}
									}
									else if (name == "FocusedBlockname3")
									{
										string startValue39;
										try
										{
											startValue39 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException38)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException38);
										}
										DataItem<string> pFocusedBlockname6 = new DataItem<string>("FocusedBlockname3", startValue39);
										itemAction.pFocusedBlockname3 = pFocusedBlockname6;
									}
								}
								else if (name == "FocusedBlockname2")
								{
									string startValue40;
									try
									{
										startValue40 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException39)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException39);
									}
									DataItem<string> pFocusedBlockname7 = new DataItem<string>("FocusedBlockname2", startValue40);
									itemAction.pFocusedBlockname2 = pFocusedBlockname7;
								}
							}
							else if (num != 1929354628U)
							{
								if (num != 1946132247U)
								{
									if (num == 2050383924U)
									{
										if (name == "RestrictedUpgradeItems")
										{
											string startValue41;
											try
											{
												startValue41 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException40)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException40);
											}
											DataItem<string> pRestrictedUpgradeItems = new DataItem<string>("RestrictedUpgradeItems", startValue41);
											itemAction.pRestrictedUpgradeItems = pRestrictedUpgradeItems;
										}
									}
								}
								else if (name == "FocusedBlockname9")
								{
									string startValue42;
									try
									{
										startValue42 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException41)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException41);
									}
									DataItem<string> pFocusedBlockname8 = new DataItem<string>("FocusedBlockname9", startValue42);
									itemAction.pFocusedBlockname9 = pFocusedBlockname8;
								}
							}
							else if (name == "FocusedBlockname8")
							{
								string startValue43;
								try
								{
									startValue43 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException42)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException42);
								}
								DataItem<string> pFocusedBlockname9 = new DataItem<string>("FocusedBlockname8", startValue43);
								itemAction.pFocusedBlockname8 = pFocusedBlockname9;
							}
						}
						else if (num <= 3213271394U)
						{
							if (num <= 2731875518U)
							{
								if (num <= 2292684416U)
								{
									if (num <= 2079371571U)
									{
										if (num != 2054944794U)
										{
											if (num == 2079371571U)
											{
												if (name == "Fertileblock")
												{
													string startValue44;
													try
													{
														startValue44 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException43)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException43);
													}
													DataItem<string> pFertileblock = new DataItem<string>("Fertileblock", startValue44);
													itemAction.pFertileblock = pFertileblock;
												}
											}
										}
										else if (name == "ParticlesMuzzleSmoke")
										{
											string startValue45;
											try
											{
												startValue45 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException44)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException44);
											}
											DataItem<string> pParticlesMuzzleSmoke = new DataItem<string>("ParticlesMuzzleSmoke", startValue45);
											itemAction.pParticlesMuzzleSmoke = pParticlesMuzzleSmoke;
										}
									}
									else if (num != 2205678605U)
									{
										if (num != 2214691755U)
										{
											if (num == 2292684416U)
											{
												if (name == "Explosion")
												{
													ExplosionData startValue46 = ExplosionData.Parser.Parse(positionXmlElement, _updateLater);
													DataItem<ExplosionData> pExplosion = new DataItem<ExplosionData>("Explosion", startValue46);
													itemAction.pExplosion = pExplosion;
												}
											}
										}
										else if (name == "GainSickness")
										{
											float startValue47;
											try
											{
												startValue47 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException45)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException45);
											}
											DataItem<float> pGainSickness = new DataItem<float>("GainSickness", startValue47);
											itemAction.pGainSickness = pGainSickness;
										}
									}
									else if (name == "ParticlesMuzzleFire")
									{
										string startValue48;
										try
										{
											startValue48 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException46)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException46);
										}
										DataItem<string> pParticlesMuzzleFire = new DataItem<string>("ParticlesMuzzleFire", startValue48);
										itemAction.pParticlesMuzzleFire = pParticlesMuzzleFire;
									}
								}
								else if (num <= 2391273097U)
								{
									if (num != 2296213333U)
									{
										if (num == 2391273097U)
										{
											if (name == "GainWater")
											{
												float startValue49;
												try
												{
													startValue49 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException47)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException47);
												}
												DataItem<float> pGainWater = new DataItem<float>("GainWater", startValue49);
												itemAction.pGainWater = pGainWater;
											}
										}
									}
									else if (name == "UseTime")
									{
										string startValue50;
										try
										{
											startValue50 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException48)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException48);
										}
										DataItem<string> pUseTime = new DataItem<string>("UseTime", startValue50);
										itemAction.pUseTime = pUseTime;
									}
								}
								else if (num != 2508081957U)
								{
									if (num != 2654093974U)
									{
										if (num == 2731875518U)
										{
											if (name == "ActionIdx")
											{
												int startValue51;
												try
												{
													startValue51 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException49)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException49);
												}
												DataItem<int> pActionIdx = new DataItem<int>("ActionIdx", startValue51);
												itemAction.pActionIdx = pActionIdx;
											}
										}
									}
									else if (name == "SoundDraw")
									{
										string startValue52;
										try
										{
											startValue52 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException50)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException50);
										}
										DataItem<string> pSoundDraw = new DataItem<string>("SoundDraw", startValue52);
										itemAction.pSoundDraw = pSoundDraw;
									}
								}
								else if (name == "RepairActionSound")
								{
									string startValue53;
									try
									{
										startValue53 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException51)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException51);
									}
									DataItem<string> pRepairActionSound = new DataItem<string>("RepairActionSound", startValue53);
									itemAction.pRepairActionSound = pRepairActionSound;
								}
							}
							else if (num <= 2951397452U)
							{
								if (num <= 2776146097U)
								{
									if (num != 2735859570U)
									{
										if (num == 2776146097U)
										{
											if (name == "GainWellness")
											{
												float startValue54;
												try
												{
													startValue54 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException52)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException52);
												}
												DataItem<float> pGainWellness = new DataItem<float>("GainWellness", startValue54);
												itemAction.pGainWellness = pGainWellness;
											}
										}
									}
									else if (name == "Range")
									{
										float startValue55;
										try
										{
											startValue55 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException53)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException53);
										}
										DataItem<float> pRange = new DataItem<float>("Range", startValue55);
										itemAction.pRange = pRange;
									}
								}
								else if (num != 2889970889U)
								{
									if (num != 2916596296U)
									{
										if (num == 2951397452U)
										{
											if (name == "ThrowStrengthMax")
											{
												float startValue56;
												try
												{
													startValue56 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException54)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException54);
												}
												DataItem<float> pThrowStrengthMax = new DataItem<float>("ThrowStrengthMax", startValue56);
												itemAction.pThrowStrengthMax = pThrowStrengthMax;
											}
										}
									}
									else if (name == "SoundWarning")
									{
										string startValue57;
										try
										{
											startValue57 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException55)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException55);
										}
										DataItem<string> pSoundWarning = new DataItem<string>("SoundWarning", startValue57);
										itemAction.pSoundWarning = pSoundWarning;
									}
								}
								else if (name == "Blockname")
								{
									string startValue58;
									try
									{
										startValue58 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException56)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException56);
									}
									DataItem<string> pBlockname = new DataItem<string>("Blockname", startValue58);
									itemAction.pBlockname = pBlockname;
								}
							}
							else if (num <= 3036802414U)
							{
								if (num != 2970340076U)
								{
									if (num != 3027266612U)
									{
										if (num == 3036802414U)
										{
											if (name == "GainFood")
											{
												float startValue59;
												try
												{
													startValue59 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException57)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException57);
												}
												DataItem<float> pGainFood = new DataItem<float>("GainFood", startValue59);
												itemAction.pGainFood = pGainFood;
											}
										}
									}
									else if (name == "HordeMeterRate")
									{
										float startValue60;
										try
										{
											startValue60 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException58)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException58);
										}
										DataItem<float> pHordeMeterRate = new DataItem<float>("HordeMeterRate", startValue60);
										itemAction.pHordeMeterRate = pHordeMeterRate;
									}
								}
								else if (name == "Buff")
								{
									string startValue61;
									try
									{
										startValue61 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException59)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException59);
									}
									DataItem<string> pBuff = new DataItem<string>("Buff", startValue61);
									itemAction.pBuff = pBuff;
								}
							}
							else if (num != 3118731031U)
							{
								if (num != 3124789842U)
								{
									if (num == 3213271394U)
									{
										if (name == "MagazineSize")
										{
											int startValue62;
											try
											{
												startValue62 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException60)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException60);
											}
											DataItem<int> pMagazineSize = new DataItem<int>("MagazineSize", startValue62);
											itemAction.pMagazineSize = pMagazineSize;
										}
									}
								}
								else if (name == "Velocity")
								{
									int startValue63;
									try
									{
										startValue63 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException61)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException61);
									}
									DataItem<int> pVelocity = new DataItem<int>("Velocity", startValue63);
									itemAction.pVelocity = pVelocity;
								}
							}
							else if (name == "DoBlockAction")
							{
								itemAction.pDoBlockAction = ItemActionData.Parser.ParseItem("DoBlockAction", positionXmlElement);
							}
						}
						else if (num <= 3880205310U)
						{
							if (num <= 3448204316U)
							{
								if (num <= 3261865014U)
								{
									if (num != 3239575924U)
									{
										if (num == 3261865014U)
										{
											if (name == "ConditionRaycastBlock")
											{
												int startValue64;
												try
												{
													startValue64 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException62)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException62);
												}
												DataItem<int> pConditionRaycastBlock = new DataItem<int>("ConditionRaycastBlock", startValue64);
												itemAction.pConditionRaycastBlock = pConditionRaycastBlock;
											}
										}
									}
									else if (name == "BulletMaterial")
									{
										string startValue65;
										try
										{
											startValue65 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException63)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException63);
										}
										DataItem<string> pBulletMaterial = new DataItem<string>("BulletMaterial", startValue65);
										itemAction.pBulletMaterial = pBulletMaterial;
									}
								}
								else if (num != 3297724363U)
								{
									if (num != 3370626489U)
									{
										if (num == 3448204316U)
										{
											if (name == "GainHealth")
											{
												float startValue66;
												try
												{
													startValue66 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException64)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException64);
												}
												DataItem<float> pGainHealth = new DataItem<float>("GainHealth", startValue66);
												itemAction.pGainHealth = pGainHealth;
											}
										}
									}
									else if (name == "SoundReload")
									{
										string startValue67;
										try
										{
											startValue67 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException65)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException65);
										}
										DataItem<string> pSoundReload = new DataItem<string>("SoundReload", startValue67);
										itemAction.pSoundReload = pSoundReload;
									}
								}
								else if (name == "ZoomMaxIn")
								{
									float startValue68;
									try
									{
										startValue68 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException66)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException66);
									}
									DataItem<float> pZoomMaxIn = new DataItem<float>("ZoomMaxIn", startValue68);
									itemAction.pZoomMaxIn = pZoomMaxIn;
								}
							}
							else if (num <= 3781194609U)
							{
								if (num != 3646476408U)
								{
									if (num != 3699885409U)
									{
										if (num == 3781194609U)
										{
											if (name == "ReferenceItem")
											{
												string startValue69;
												try
												{
													startValue69 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException67)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException67);
												}
												DataItem<string> pReferenceItem = new DataItem<string>("ReferenceItem", startValue69);
												itemAction.pReferenceItem = pReferenceItem;
											}
										}
									}
									else if (name == "Consume")
									{
										bool startValue70;
										try
										{
											startValue70 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException68)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException68);
										}
										DataItem<bool> pConsume = new DataItem<bool>("Consume", startValue70);
										itemAction.pConsume = pConsume;
									}
								}
								else if (name == "RepairAmount")
								{
									int startValue71;
									try
									{
										startValue71 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException69)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException69);
									}
									DataItem<int> pRepairAmount = new DataItem<int>("RepairAmount", startValue71);
									itemAction.pRepairAmount = pRepairAmount;
								}
							}
							else if (num != 3788546643U)
							{
								if (num != 3860496195U)
								{
									if (num == 3880205310U)
									{
										if (name == "CreateItem")
										{
											string startValue72;
											try
											{
												startValue72 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException70)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException70);
											}
											DataItem<string> pCreateItem = new DataItem<string>("CreateItem", startValue72);
											itemAction.pCreateItem = pCreateItem;
										}
									}
								}
								else if (name == "ChangeItemTo")
								{
									string startValue73;
									try
									{
										startValue73 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException71)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException71);
									}
									DataItem<string> pChangeItemTo = new DataItem<string>("ChangeItemTo", startValue73);
									itemAction.pChangeItemTo = pChangeItemTo;
								}
							}
							else if (name == "AllowedUpgradeItems")
							{
								string startValue74;
								try
								{
									startValue74 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException72)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException72);
								}
								DataItem<string> pAllowedUpgradeItems = new DataItem<string>("AllowedUpgradeItems", startValue74);
								itemAction.pAllowedUpgradeItems = pAllowedUpgradeItems;
							}
						}
						else if (num <= 4005067140U)
						{
							if (num <= 3894590787U)
							{
								if (num != 3893059105U)
								{
									if (num == 3894590787U)
									{
										if (name == "FlyTime")
										{
											float startValue75;
											try
											{
												startValue75 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException73)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException73);
											}
											DataItem<float> pFlyTime = new DataItem<float>("FlyTime", startValue75);
											itemAction.pFlyTime = pFlyTime;
										}
									}
								}
								else if (name == "StaminaUsage")
								{
									string startValue76;
									try
									{
										startValue76 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException74)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException74);
									}
									DataItem<string> pStaminaUsage = new DataItem<string>("StaminaUsage", startValue76);
									itemAction.pStaminaUsage = pStaminaUsage;
								}
							}
							else if (num != 3922517809U)
							{
								if (num != 3965826732U)
								{
									if (num == 4005067140U)
									{
										if (name == "MagazineItem")
										{
											string startValue77;
											try
											{
												startValue77 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException75)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException75);
											}
											DataItem<string> pMagazineItem = new DataItem<string>("MagazineItem", startValue77);
											itemAction.pMagazineItem = pMagazineItem;
										}
									}
								}
								else if (name == "UpgradeActionSound")
								{
									string startValue78;
									try
									{
										startValue78 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException76)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException76);
									}
									DataItem<string> pUpgradeActionSound = new DataItem<string>("UpgradeActionSound", startValue78);
									itemAction.pUpgradeActionSound = pUpgradeActionSound;
								}
							}
							else if (name == "DamageBlock")
							{
								float startValue79;
								try
								{
									startValue79 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException77)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException77);
								}
								DataItem<float> pDamageBlock = new DataItem<float>("DamageBlock", startValue79);
								itemAction.pDamageBlock = pDamageBlock;
							}
						}
						else if (num <= 4124220918U)
						{
							if (num != 4013882141U)
							{
								if (num != 4069726011U)
								{
									if (num == 4124220918U)
									{
										if (name == "InstantiateOnLoad")
										{
											string startValue80;
											try
											{
												startValue80 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException78)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException78);
											}
											DataItem<string> pInstantiateOnLoad = new DataItem<string>("InstantiateOnLoad", startValue80);
											itemAction.pInstantiateOnLoad = pInstantiateOnLoad;
										}
									}
								}
								else if (name == "UpgradeHitOffset")
								{
									int startValue81;
									try
									{
										startValue81 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException79)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException79);
									}
									DataItem<int> pUpgradeHitOffset = new DataItem<int>("UpgradeHitOffset", startValue81);
									itemAction.pUpgradeHitOffset = pUpgradeHitOffset;
								}
							}
							else if (name == "HordeMeterDistance")
							{
								float startValue82;
								try
								{
									startValue82 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException80)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException80);
								}
								DataItem<float> pHordeMeterDistance = new DataItem<float>("HordeMeterDistance", startValue82);
								itemAction.pHordeMeterDistance = pHordeMeterDistance;
							}
						}
						else if (num != 4129641136U)
						{
							if (num != 4139408471U)
							{
								if (num == 4265352596U)
								{
									if (name == "BulletIcon")
									{
										string startValue83;
										try
										{
											startValue83 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException81)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException81);
										}
										DataItem<string> pBulletIcon = new DataItem<string>("BulletIcon", startValue83);
										itemAction.pBulletIcon = pBulletIcon;
									}
								}
							}
							else if (name == "Debuff")
							{
								string startValue84;
								try
								{
									startValue84 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException82)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException82);
								}
								DataItem<string> pDebuff = new DataItem<string>("Debuff", startValue84);
								itemAction.pDebuff = pDebuff;
							}
						}
						else if (name == "Adjacentblock")
						{
							string startValue85;
							try
							{
								startValue85 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
							}
							catch (Exception innerException83)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException83);
							}
							DataItem<string> pAdjacentblock = new DataItem<string>("Adjacentblock", startValue85);
							itemAction.pAdjacentblock = pAdjacentblock;
						}
						if (!dictionary.ContainsKey(positionXmlElement.Name))
						{
							dictionary[positionXmlElement.Name] = 0;
						}
						Dictionary<string, int> dictionary2 = dictionary;
						name = positionXmlElement.Name;
						int num2 = dictionary2[name];
						dictionary2[name] = num2 + 1;
					}
				}
				foreach (KeyValuePair<string, Range<int>> keyValuePair in ItemActionData.Parser.knownAttributesMultiplicity)
				{
					int num3 = dictionary.ContainsKey(keyValuePair.Key) ? dictionary[keyValuePair.Key] : 0;
					if ((keyValuePair.Value.hasMin && num3 < keyValuePair.Value.min) || (keyValuePair.Value.hasMax && num3 > keyValuePair.Value.max))
					{
						throw new IncorrectAttributeOccurrenceException(string.Concat(new string[]
						{
							"Element has incorrect number of \"",
							keyValuePair.Key,
							"\" attribute instances, found ",
							num3.ToString(),
							", expected ",
							keyValuePair.Value.ToString()
						}), _elem.LineNumber);
					}
				}
				return itemAction;
			}

			// Token: 0x04007981 RID: 31105
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Delay",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Range",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundStart",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundRepeat",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundEnd",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundEmpty",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundReload",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundWarning",
					new Range<int>(true, 0, true, 1)
				},
				{
					"StaminaUsage",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UseTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname1",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname2",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname3",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname4",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname5",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname6",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname7",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname8",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname9",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ChangeItemTo",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ChangeBlockTo",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DoBlockAction",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainHealth",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainFood",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainWater",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainStamina",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainSickness",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainWellness",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Buff",
					new Range<int>(true, 0, true, 1)
				},
				{
					"BuffChance",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Debuff",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CreateItem",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ConditionRaycastBlock",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainGas",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Consume",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Blockname",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ThrowStrengthDefault",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ThrowStrengthMax",
					new Range<int>(true, 0, true, 1)
				},
				{
					"MaxStrainTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"MagazineSize",
					new Range<int>(true, 0, true, 1)
				},
				{
					"MagazineItem",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ReloadTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"BulletIcon",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RaysPerShot",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RaysSpread",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Sphere",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CrosshairMinDistance",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CrosshairMaxDistance",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DamageEntity",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DamageBlock",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ParticlesMuzzleFire",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ParticlesMuzzleSmoke",
					new Range<int>(true, 0, true, 1)
				},
				{
					"BlockRange",
					new Range<int>(true, 0, true, 1)
				},
				{
					"AutoFire",
					new Range<int>(true, 0, true, 1)
				},
				{
					"HordeMeterRate",
					new Range<int>(true, 0, true, 1)
				},
				{
					"HordeMeterDistance",
					new Range<int>(true, 0, true, 1)
				},
				{
					"HitmaskOverride",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SingleMagazineUsage",
					new Range<int>(true, 0, true, 1)
				},
				{
					"BulletMaterial",
					new Range<int>(true, 0, true, 1)
				},
				{
					"InfiniteAmmo",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ZoomMaxOut",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ZoomMaxIn",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ZoomOverlay",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Velocity",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FlyTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"LifeTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CollisionRadius",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ProjectileInitialVelocity",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Fertileblock",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Adjacentblock",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RepairAmount",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UpgradeHitOffset",
					new Range<int>(true, 0, true, 1)
				},
				{
					"AllowedUpgradeItems",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RestrictedUpgradeItems",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UpgradeActionSound",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RepairActionSound",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ReferenceItem",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Mesh",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ActionIdx",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Title",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Description",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RecipesToLearn",
					new Range<int>(true, 0, true, 1)
				},
				{
					"InstantiateOnLoad",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundDraw",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DamageBonus",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Explosion",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}

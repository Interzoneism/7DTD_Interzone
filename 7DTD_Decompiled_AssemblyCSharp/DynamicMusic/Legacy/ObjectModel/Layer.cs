using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMusic.Legacy.ObjectModel
{
	// Token: 0x02001784 RID: 6020
	public class Layer : Dictionary<int, InstrumentID>
	{
		// Token: 0x0600B481 RID: 46209 RVA: 0x0045BA35 File Offset: 0x00459C35
		public Layer()
		{
			if (Layer.Random == null)
			{
				Layer.Random = GameRandomManager.Instance.CreateGameRandom();
			}
		}

		// Token: 0x0600B482 RID: 46210 RVA: 0x0045BA53 File Offset: 0x00459C53
		public InstrumentID GetInstrumentID()
		{
			if (this.idQ == null || this.idQ.Count < 3)
			{
				this.PopulateQueue();
			}
			LayerReserve.AddLoad(this.idQ.ElementAt(1));
			return this.idQ.Dequeue();
		}

		// Token: 0x0600B483 RID: 46211 RVA: 0x0045BA90 File Offset: 0x00459C90
		public void PopulateQueue()
		{
			if (this.idQ == null)
			{
				this.idQ = new Queue<InstrumentID>(from e in base.Values
				orderby Layer.Random.RandomRange(int.MaxValue)
				select e);
				LayerReserve.AddLoad(this.idQ.Peek());
				return;
			}
			this.RefillQueue();
		}

		// Token: 0x0600B484 RID: 46212 RVA: 0x0045BAF1 File Offset: 0x00459CF1
		[PublicizedFrom(EAccessModifier.Private)]
		public void RefillQueue()
		{
			Array.ForEach<InstrumentID>(base.Values.OrderBy(delegate(InstrumentID e)
			{
				if (e.Name.Equals(this.idQ.Peek().Name) || e.Name.Equals(this.idQ.ElementAt(1).Name))
				{
					return int.MaxValue;
				}
				return Layer.Random.RandomRange(int.MaxValue);
			}).ToArray<InstrumentID>(), new Action<InstrumentID>(this.idQ.Enqueue));
		}

		// Token: 0x04008CE0 RID: 36064
		[PublicizedFrom(EAccessModifier.Private)]
		public Queue<InstrumentID> idQ;

		// Token: 0x04008CE1 RID: 36065
		[PublicizedFrom(EAccessModifier.Private)]
		public static GameRandom Random;
	}
}

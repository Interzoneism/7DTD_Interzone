using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;
using UniLinq;

namespace DynamicMusic
{
	// Token: 0x02001707 RID: 5895
	public abstract class AbstractConfiguration : IConfiguration
	{
		// Token: 0x170013B4 RID: 5044
		// (get) Token: 0x0600B1EF RID: 45551 RVA: 0x00454EEC File Offset: 0x004530EC
		// (set) Token: 0x0600B1F0 RID: 45552 RVA: 0x00454EF4 File Offset: 0x004530F4
		public virtual IList<SectionType> Sections { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x0600B1F1 RID: 45553
		public abstract int CountFor(LayerType _layer);

		// Token: 0x0600B1F2 RID: 45554 RVA: 0x00454EFD File Offset: 0x004530FD
		public AbstractConfiguration()
		{
			this.Sections = new List<SectionType>();
			AbstractConfiguration.AllConfigurations.Add(this);
		}

		// Token: 0x0600B1F3 RID: 45555 RVA: 0x00454F1B File Offset: 0x0045311B
		public static AbstractConfiguration CreateWrapper(string _type)
		{
			return (AbstractConfiguration)Activator.CreateInstance(ReflectionHelpers.GetTypeWithPrefix("DynamicMusic.", _type));
		}

		// Token: 0x0600B1F4 RID: 45556 RVA: 0x00454F34 File Offset: 0x00453134
		public virtual void ParseFromXml(XElement _xmlNode)
		{
			foreach (string name in _xmlNode.GetAttribute("sections").Split(',', StringSplitOptions.None))
			{
				this.Sections.Add(EnumUtils.Parse<SectionType>(name, false));
			}
		}

		// Token: 0x0600B1F5 RID: 45557 RVA: 0x00454F80 File Offset: 0x00453180
		public static T Get<T>(SectionType _sectionType) where T : IConfiguration
		{
			List<T> list = AbstractConfiguration.AllConfigurations.OfType<T>().ToList<T>().FindAll((T c) => c.Sections.Contains(_sectionType));
			if (list.Count <= 0)
			{
				return default(T);
			}
			return list[AbstractConfiguration.rng.RandomRange(list.Count)];
		}

		// Token: 0x0600B1F6 RID: 45558 RVA: 0x00454FE4 File Offset: 0x004531E4
		public static int GetBufferSize(SectionType _sectionType, LayerType _layerType)
		{
			IEnumerable<int> enumerable = from c in AbstractConfiguration.AllConfigurations.OfType<IConfiguration>()
			where c.Sections.Contains(_sectionType)
			select c.CountFor(_layerType);
			if (enumerable != null && enumerable.Count<int>() != 0)
			{
				return enumerable.Max();
			}
			return 0;
		}

		// Token: 0x04008B91 RID: 35729
		public static IList<AbstractConfiguration> AllConfigurations = new List<AbstractConfiguration>();

		// Token: 0x04008B92 RID: 35730
		[PublicizedFrom(EAccessModifier.Protected)]
		public static GameRandom rng = GameRandomManager.Instance.CreateGameRandom();
	}
}

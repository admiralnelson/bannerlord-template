using TaleWorlds.Localization;

namespace Modbed
{
	public class CharacterInfo
	{
		public string id;

		public string name;

		public string culture;

		public string defaultGroup;

		private string _name;

		public string Name
		{
			get
			{
				string text = new TextObject(name).ToString();
				if (id.EndsWith("troop"))
				{
					return text + " (Troop)";
				}
				if (!id.EndsWith("hero"))
				{
					return text;
				}
				return text + " (Hero)";
			}
		}
	}
}

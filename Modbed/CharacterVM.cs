using TaleWorlds.Library;

namespace Modbed
{
	public class CharacterVM : ViewModel
	{
		public CharacterInfo character;

		public string Name => character.Name;

		public CharacterVM(CharacterInfo character)
		{
			this.character = character;
		}
	}
}

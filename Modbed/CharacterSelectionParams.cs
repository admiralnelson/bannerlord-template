using System;
using System.Collections.Generic;

namespace Modbed
{
	public class CharacterSelectionParams
	{
		public List<CharacterInfo> characters;

		public int selectedIndex;

		public Action<CharacterInfo> setCharacter;
	}
}

using System;
using System.Collections.Generic;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace Modbed
{
	public class CharacterSelectionVM : ViewModel
	{
		private SortedDictionary<string, SortedDictionary<string, List<CharacterInfo>>> allCharacters;

		private CharacterSelectionParams _params;

		private bool _inChange;

		private MBBindingList<NameVM> _cultures;

		private MBBindingList<NameVM> _groups;

		private MBBindingList<CharacterVM> _characters;

		public int SelectedCultureIndex
		{
			get;
			set;
		}

		public int SelectedGroupIndex
		{
			get;
			set;
		}

		public int SelectedCharacterIndex
		{
			get;
			set;
		}

		[DataSourceProperty]
		public MBBindingList<NameVM> Cultures
		{
			get
			{
				return _cultures;
			}
			set
			{
				if (value != _cultures)
				{
					_cultures = value;
					OnPropertyChanged("Cultures");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<NameVM> Groups
		{
			get
			{
				return _groups;
			}
			set
			{
				if (value != _groups)
				{
					_groups = value;
					OnPropertyChanged("Groups");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CharacterVM> Characters
		{
			get
			{
				return _characters;
			}
			set
			{
				if (value != _characters)
				{
					_characters = value;
					OnPropertyChanged("Characters");
				}
			}
		}

		public CharacterSelectionVM(CharacterSelectionParams p)
		{
			ModuleLogger.Log("begin character selection vm construction");
			_params = p;
			allCharacters = new SortedDictionary<string, SortedDictionary<string, List<CharacterInfo>>>();
			foreach (CharacterInfo character in p.characters)
			{
				if (!allCharacters.ContainsKey(character.culture))
				{
					allCharacters.Add(character.culture, new SortedDictionary<string, List<CharacterInfo>>());
				}
				SortedDictionary<string, List<CharacterInfo>> sortedDictionary = allCharacters[character.culture];
				if (!sortedDictionary.ContainsKey(character.defaultGroup))
				{
					sortedDictionary.Add(character.defaultGroup, new List<CharacterInfo>());
				}
				sortedDictionary[character.defaultGroup].Add(character);
			}
			CharacterInfo c = p.characters[p.selectedIndex];
			Cultures = new MBBindingList<NameVM>();
			foreach (string key in allCharacters.Keys)
			{
				Cultures.Add(new NameVM
				{
					Name = key
				});
			}
			Groups = new MBBindingList<NameVM>();
			foreach (string key2 in allCharacters[c.culture].Keys)
			{
				Groups.Add(new NameVM
				{
					Name = key2
				});
			}
			Characters = new MBBindingList<CharacterVM>();
			foreach (CharacterInfo item in allCharacters[c.culture][c.defaultGroup])
			{
				Characters.Add(new CharacterVM(item));
			}
			SelectedCultureIndex = Cultures.FindIndex((NameVM n) => n.Name == c.culture);
			SelectedGroupIndex = Groups.FindIndex((NameVM n) => n.Name == c.defaultGroup);
			SelectedCharacterIndex = Characters.FindIndex((CharacterVM n) => n.character == c);
			ModuleLogger.Log("end character selection vm construction");
		}

		public void SelectedCultureChanged(ListPanel listPanel)
		{
			_inChange = true;
			try
			{
				int intValue = listPanel.IntValue;
				ModuleLogger.Log("SelectedCultureChanged {0}", intValue);
				string name = Cultures[intValue].Name;
				string text = null;
				Groups.Clear();
				Characters.Clear();
				foreach (string key in allCharacters[name].Keys)
				{
					if (text == null)
					{
						text = key;
					}
					Groups.Add(new NameVM
					{
						Name = key
					});
				}
				foreach (CharacterInfo item in allCharacters[name][text])
				{
					Characters.Add(new CharacterVM(item));
				}
				SelectedCultureIndex = intValue;
				SelectedGroupIndex = 0;
				SelectedCharacterIndex = 0;
			}
			catch (Exception ex)
			{
				ModuleLogger.Log("{0}", ex);
				throw;
			}
			_inChange = false;
		}

		public void SelectedGroupChanged(ListPanel listPanel)
		{
			int intValue = listPanel.IntValue;
			if (intValue >= 0 && !_inChange)
			{
				_inChange = true;
				ModuleLogger.Log("SelectedGroupChanged {0} {1}", intValue, Groups.Count);
				string name = Cultures[SelectedCultureIndex].Name;
				string name2 = Groups[intValue].Name;
				Characters.Clear();
				foreach (CharacterInfo item in allCharacters[name][name2])
				{
					Characters.Add(new CharacterVM(item));
				}
				SelectedGroupIndex = intValue;
				SelectedCharacterIndex = 0;
				_inChange = false;
			}
		}

		public void SelectedCharacterChanged(ListPanel listPanel)
		{
			int intValue = listPanel.IntValue;
			if (intValue >= 0 && !_inChange)
			{
				ModuleLogger.Log("SelectedCharacterChanged {0}", intValue);
				SelectedCharacterIndex = intValue;
			}
		}

		private void Done()
		{
			_params.setCharacter(Characters[SelectedCharacterIndex].character);
			ScreenManager.PopScreen();
		}
	}
}

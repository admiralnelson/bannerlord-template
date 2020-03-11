using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Modbed
{
	public class BattleTestVM : ViewModel
	{
		private struct LevelSuggestedProperties
		{
			public string Positions
			{
				get;
				set;
			}

			public string SunPower
			{
				get;
				set;
			}

			public string Rotation
			{
				get;
				set;
			}

			public LevelSuggestedProperties(string Pos, string Pow, string Rot)
			{
				Positions = Pos;
				SunPower = Pow;
				Rotation = Rot;
			}
		}

		private static BattleTestParams lastParams = BattleTestParams.createDefault();

		private string _text = "Welcome to test";

		private string _levelNumber;

		private string _playerSoldierCount;

		private string _enemySoldierCount;

		private string _distance;

		private string _soldierXInterval;

		private string _soldierYInterval;

		private string _soldiersPerRow;

		private string _tempVar;

		private string _playerCharacterName;

		private string _playerSoldierCharacterName;

		private string _enemySoldierCharacterName;

		private string _selectedCultureNamePlayer;

		private string[] _levelList;

		private string[] _cultureList;

		private int _selectedMapIndex;

		private int _selectedPlayerCultureIndex;

		private int _selectedEnemyCultureIndex;

		private CharacterInfo _playerCharacter;

		private CharacterInfo _playerSoldierCharacter;

		private CharacterInfo _enemySoldierCharacter;

		private List<CharacterInfo> _allCharacters;

		private LevelSuggestedProperties[] _suggestedSettings;

		private string _selectedCultureNameEnemy;

		[DataSourceProperty]
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				if (!(value == _text))
				{
					_text = value;
					OnPropertyChanged("Text");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerSoldierCount
		{
			get
			{
				return _playerSoldierCount;
			}
			set
			{
				if (!(value == _playerSoldierCount))
				{
					_playerSoldierCount = value;
					OnPropertyChanged("PlayerSoldierCount");
				}
			}
		}

		[DataSourceProperty]
		public string LevelNumber
		{
			get
			{
				return _levelNumber;
			}
			set
			{
				if (!(value == _levelNumber))
				{
					_levelNumber = value;
					OnPropertyChanged("LevelNumber");
				}
			}
		}

		[DataSourceProperty]
		public string EnemySoldierCount
		{
			get
			{
				return _enemySoldierCount;
			}
			set
			{
				if (!(value == _enemySoldierCount))
				{
					_enemySoldierCount = value;
					OnPropertyChanged("EnemySoldierCount");
				}
			}
		}

		[DataSourceProperty]
		public string Distance
		{
			get
			{
				return _distance;
			}
			set
			{
				if (!(value == _distance))
				{
					_distance = value;
					OnPropertyChanged("Distance");
				}
			}
		}

		[DataSourceProperty]
		public string SoldierXInterval
		{
			get
			{
				return _soldierXInterval;
			}
			set
			{
				if (!(value == _soldierXInterval))
				{
					_soldierXInterval = value;
					OnPropertyChanged("SoldierXInterval");
				}
			}
		}

		[DataSourceProperty]
		public string SoldierYInterval
		{
			get
			{
				return _soldierYInterval;
			}
			set
			{
				if (!(value == _soldierYInterval))
				{
					_soldierYInterval = value;
					OnPropertyChanged("SoldierYInterval");
				}
			}
		}

		[DataSourceProperty]
		public string SoldiersPerRow
		{
			get
			{
				return _soldiersPerRow;
			}
			set
			{
				if (!(value == _soldiersPerRow))
				{
					_soldiersPerRow = value;
					OnPropertyChanged("SoldiersPerRow");
				}
			}
		}

		[DataSourceProperty]
		public string TempVar
		{
			get
			{
				return _tempVar;
			}
			set
			{
				if (!(value == _tempVar))
				{
					_tempVar = value;
					OnPropertyChanged("TempVar");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerCharacterName
		{
			get
			{
				return _playerCharacterName;
			}
			set
			{
				if (!(value == _playerCharacterName))
				{
					_playerCharacterName = value;
					OnPropertyChanged("PlayerCharacterName");
				}
			}
		}

		public CharacterInfo PlayerCharacter
		{
			get
			{
				return _playerCharacter;
			}
			set
			{
				_playerCharacter = value;
				PlayerCharacterName = value?.Name;
			}
		}

		[DataSourceProperty]
		public string PlayerSoldierCharacterName
		{
			get
			{
				return _playerSoldierCharacterName;
			}
			set
			{
				if (!(value == _playerSoldierCharacterName))
				{
					_playerSoldierCharacterName = value;
					OnPropertyChanged("PlayerSoldierCharacterName");
				}
			}
		}

		public CharacterInfo PlayerSoldierCharacter
		{
			get
			{
				return _playerSoldierCharacter;
			}
			set
			{
				_playerSoldierCharacter = value;
				PlayerSoldierCharacterName = value?.Name;
			}
		}

		[DataSourceProperty]
		public string EnemySoldierCharacterName
		{
			get
			{
				return _enemySoldierCharacterName;
			}
			set
			{
				if (!(value == _enemySoldierCharacterName))
				{
					_enemySoldierCharacterName = value;
					OnPropertyChanged("EnemySoldierCharacterName");
				}
			}
		}

		public CharacterInfo EnemySoldierCharacter
		{
			get
			{
				return _enemySoldierCharacter;
			}
			set
			{
				_enemySoldierCharacter = value;
				EnemySoldierCharacterName = value?.Name;
			}
		}

		[DataSourceProperty]
		public string SkyBrightness
		{
			get;
			set;
		}

		[DataSourceProperty]
		public string TimeOfDay
		{
			get;
			set;
		}

		[DataSourceProperty]
		public string RainDensity
		{
			get;
			set;
		}

		[DataSourceProperty]
		public string FormationPosition
		{
			get;
			set;
		}

		[DataSourceProperty]
		public string FormationDirection
		{
			get;
			set;
		}

		public MBBindingList<AgentInfoVM> AgentInfoList
		{
			get;
			set;
		}

		public int SelectedIndex
		{
			get;
			set;
		}

		public bool UseFreeCamera
		{
			get;
			set;
		}

		public bool WantMoraleSystem
		{
			get;
			set;
		}

		[DataSourceProperty]
		public bool IAmFeelingLucky
		{
			get;
			set;
		}

		public float InfantryPercentage
		{
			get;
			set;
		}

		public float ArcherPercentage
		{
			get;
			set;
		}

		[DataSourceProperty]
		public string SelectedMapName
		{
			get
			{
				return _levelNumber;
			}
			set
			{
				_levelNumber = value;
				OnPropertyChanged("SelectedMapName");
			}
		}

		[DataSourceProperty]
		public string SelectedPlayerCulture
		{
			get
			{
				return _selectedCultureNamePlayer;
			}
			set
			{
				_selectedCultureNamePlayer = value;
				OnPropertyChanged("SelectedPlayerCulture");
			}
		}

		[DataSourceProperty]
		public string SelectedEnemyCulture
		{
			get
			{
				return _selectedCultureNameEnemy;
			}
			set
			{
				_selectedCultureNameEnemy = value;
				OnPropertyChanged("SelectedEnemyCulture");
			}
		}

		public BattleTestVM()
		{
			_playerSoldierCount = lastParams.playerSoldierCount.ToString();
			_enemySoldierCount = lastParams.enemySoldierCount.ToString();
			_levelNumber = lastParams.levelNumber.ToString();
			_distance = lastParams.distance.ToString();
			_soldierXInterval = lastParams.soldierXInterval.ToString();
			_soldierYInterval = lastParams.soldierYInterval.ToString();
			_soldiersPerRow = lastParams.soldiersPerRow.ToString();
			FormationPosition = $"{lastParams.FormationPosition.x},{lastParams.FormationPosition.y}";
			FormationDirection = $"{lastParams.formationDirection.x},{lastParams.formationDirection.y}";
			SkyBrightness = lastParams.skyBrightness.ToString();
			TimeOfDay = lastParams.timeOfDay.ToString();
			RainDensity = lastParams.rainDensity.ToString();
			_tempVar = lastParams.tempVar.ToString();
			UseFreeCamera = lastParams.useFreeCamera;
			IAmFeelingLucky = false;
			_selectedMapIndex = lastParams.levelNumber;
			_levelList = new string[35]
			{
				"mp_tdm_map_001",
				"mp_duel_001",
				"mp_duel_001_winter",
				"mp_duel_002",
				"mp_ruins_2",
				"mp_sergeant_map_001",
				"mp_sergeant_map_005",
				"mp_sergeant_map_007",
				"mp_sergeant_map_008",
				"mp_sergeant_map_009",
				"mp_sergeant_map_010",
				"mp_sergeant_map_011",
				"mp_sergeant_map_011s",
				"mp_sergeant_map_012",
				"mp_sergeant_map_013",
				"mp_sergeant_map_vlandia_01",
				"mp_siege_map_001",
				"mp_siege_map_002",
				"mp_siege_map_003",
				"mp_siege_map_004",
				"mp_siege_map_005",
				"mp_skirmish_map_001a",
				"mp_skirmish_map_002f",
				"mp_skirmish_map_002_winter",
				"mp_skirmish_map_004",
				"mp_skirmish_map_005",
				"mp_skirmish_map_006",
				"mp_skirmish_map_007",
				"mp_skirmish_map_007_winter",
				"mp_skirmish_map_008",
				"mp_skirmish_map_009",
				"mp_skirmish_map_010",
				"mp_skirmish_map_013",
				"mp_skirmish_map_battania_02",
				"mp_skirmish_map_battania_03"
			};
			_suggestedSettings = new LevelSuggestedProperties[_levelList.Length];
			for (int i = 0; i < _levelList.Length; i++)
			{
				_suggestedSettings[i] = new LevelSuggestedProperties($"{lastParams.FormationPosition.x},{lastParams.FormationPosition.y}", lastParams.skyBrightness.ToString(), $"{lastParams.formationDirection.x},{lastParams.formationDirection.y}");
			}
			_suggestedSettings[0] = new LevelSuggestedProperties("475,557", "100", "-90,1");
			_suggestedSettings[1] = new LevelSuggestedProperties("565,598", "150", "0,1");
			_suggestedSettings[3] = new LevelSuggestedProperties("152,184", "0", "0.6,-0.7");
			_suggestedSettings[4] = new LevelSuggestedProperties("457,544", "100", "1,1");
			_suggestedSettings[9] = new LevelSuggestedProperties("414,683", "100", "1,1");
			_suggestedSettings[10] = new LevelSuggestedProperties("428,388", "0", "1,1");
			_suggestedSettings[11] = new LevelSuggestedProperties("481,375", "100", "1,1");
			_suggestedSettings[13] = new LevelSuggestedProperties("620,522", "100", "90,10");
			_suggestedSettings[18] = new LevelSuggestedProperties("451, 626", "200", "1,1");
			_suggestedSettings[19] = new LevelSuggestedProperties("496,501", "150", "0.7,0.7");
			_suggestedSettings[20] = new LevelSuggestedProperties("656,653", "0", "0,0");
			SelectedMapName = _levelList[_selectedMapIndex];
			_cultureList = new string[5]
			{
				"Aserai",
				"Vlandia",
				"Sturgia",
				"Khuzait",
				"Empire"
			};
			_selectedEnemyCultureIndex = 3;
			_selectedPlayerCultureIndex = 2;
			SelectedEnemyCulture = _cultureList[_selectedEnemyCultureIndex];
			SelectedPlayerCulture = _cultureList[_selectedPlayerCultureIndex];
			AgentInfoList = new MBBindingList<AgentInfoVM>
			{
				new AgentInfoVM
				{
					Name = "hello1"
				},
				new AgentInfoVM
				{
					Name = "hello2"
				},
				new AgentInfoVM
				{
					Name = "hello3"
				},
				new AgentInfoVM
				{
					Name = "hello4"
				}
			};
			List<CharacterInfo> characters = getCharacters();
			foreach (CharacterInfo item in characters)
			{
				string name = new TextObject(item.name).ToString();
				AgentInfoList.Add(new AgentInfoVM
				{
					Name = name
				});
			}
			SelectedIndex = 0;
			PlayerCharacter = characters.Find((CharacterInfo c) => c.id == lastParams.playerCharacterId);
			PlayerSoldierCharacter = characters.Find((CharacterInfo c) => c.id == lastParams.playerSoldierCharacterId);
			EnemySoldierCharacter = characters.Find((CharacterInfo c) => c.id == lastParams.enemySoldierCharacterId);
			if (PlayerCharacter == null)
			{
				PlayerCharacter = characters[0];
			}
			if (PlayerSoldierCharacter == null)
			{
				PlayerSoldierCharacter = characters[0];
			}
			if (EnemySoldierCharacter == null)
			{
				EnemySoldierCharacter = characters[0];
			}
			_allCharacters = characters;
		}

		private void StartBattleTest()
		{
			BattleTestParams p = default(BattleTestParams);
			try
			{
				p.playerSoldierCount = Convert.ToInt32(PlayerSoldierCount);
				p.levelNumber = _selectedMapIndex;
				p.enemySoldierCount = Convert.ToInt32(EnemySoldierCount);
				p.distance = Convert.ToSingle(Distance);
				p.soldierXInterval = Convert.ToSingle(SoldierXInterval);
				p.soldierYInterval = Convert.ToSingle(SoldierYInterval);
				p.soldiersPerRow = Convert.ToInt32(SoldiersPerRow);
				string[] array = FormationPosition.Split(',');
				string[] array2 = FormationDirection.Split(',');
				p.FormationPosition = new Vec2(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]));
				p.formationDirection = new Vec2(Convert.ToSingle(array2[0]), Convert.ToSingle(array2[1])).Normalized();
				p.skyBrightness = Convert.ToSingle(SkyBrightness);
				p.timeOfDay = Convert.ToSingle(TimeOfDay);
				p.rainDensity = Convert.ToSingle(RainDensity);
				p.tempVar = Convert.ToSingle(TempVar);
				p.playerCharacterId = PlayerCharacter.id;
				p.playerSoldierCharacterId = PlayerSoldierCharacter.id;
				p.enemySoldierCharacterId = EnemySoldierCharacter.id;
				p.useFreeCamera = UseFreeCamera;
			}
			catch
			{
				return;
			}
			if (p.validate())
			{
				lastParams = p;
				ModuleLogger.Writer.WriteLine("StartBattleTest");
				MBGameManager.StartNewGame(new BattleTestGameManager(p));
			}
		}

		private void GoBack()
		{
			ScreenManager.PopScreen();
		}

		private void MultipleUnitsClicked()
		{
			IAmFeelingLucky = !IAmFeelingLucky;
			OnPropertyChanged("MultipleUnitsClicked");
		}

		private void SelectPlayerCharacter()
		{
			ModuleLogger.Log("SelectPlayerCharacter");
			ScreenManager.PushScreen(new CharacterSelectionScreen(new CharacterSelectionParams
			{
				characters = _allCharacters,
				selectedIndex = _allCharacters.IndexOf(PlayerCharacter),
				setCharacter = delegate(CharacterInfo c)
				{
					PlayerCharacter = c;
				}
			}));
		}

		private void PreviousMap()
		{
			if (_selectedMapIndex != 0)
			{
				_selectedMapIndex--;
				SelectedMapName = _levelList[_selectedMapIndex];
				LevelSuggestedProperties levelSuggestedProperties = _suggestedSettings[_selectedMapIndex];
				FormationPosition = levelSuggestedProperties.Positions;
				FormationDirection = levelSuggestedProperties.Rotation;
				SkyBrightness = levelSuggestedProperties.SunPower;
				OnPropertyChanged("SelectedMapName");
				OnPropertyChanged("FormationPosition");
				OnPropertyChanged("SkyBrightness");
			}
		}

		private void NextMap()
		{
			if (_selectedMapIndex != _levelList.Length - 1)
			{
				_selectedMapIndex++;
				SelectedMapName = _levelList[_selectedMapIndex];
				LevelSuggestedProperties levelSuggestedProperties = _suggestedSettings[_selectedMapIndex];
				FormationPosition = levelSuggestedProperties.Positions;
				FormationDirection = levelSuggestedProperties.Rotation;
				SkyBrightness = levelSuggestedProperties.SunPower;
				OnPropertyChanged("SelectedMapName");
				OnPropertyChanged("FormationPosition");
				OnPropertyChanged("SkyBrightness");
			}
		}

		private void PreviousCultureEnemy()
		{
			if (_selectedEnemyCultureIndex != 0)
			{
				_selectedEnemyCultureIndex--;
				SelectedEnemyCulture = _cultureList[_selectedEnemyCultureIndex];
			}
		}

		private void PreviousCulturePlayer()
		{
			if (_selectedPlayerCultureIndex != 0)
			{
				_selectedPlayerCultureIndex--;
				SelectedPlayerCulture = _cultureList[_selectedPlayerCultureIndex];
			}
		}

		private void NextCultureEnemy()
		{
			if (_selectedEnemyCultureIndex != _cultureList.Length - 1)
			{
				_selectedEnemyCultureIndex++;
				SelectedEnemyCulture = _cultureList[_selectedEnemyCultureIndex];
			}
		}

		private void NextCulturePlayer()
		{
			if (_selectedPlayerCultureIndex != _cultureList.Length - 1)
			{
				_selectedPlayerCultureIndex++;
				SelectedPlayerCulture = _cultureList[_selectedPlayerCultureIndex];
			}
		}

		private void SelectPlayerSoldierCharacter()
		{
			ModuleLogger.Log("SelectPlayerSoldierCharacter");
			ScreenManager.PushScreen(new CharacterSelectionScreen(new CharacterSelectionParams
			{
				characters = _allCharacters,
				selectedIndex = _allCharacters.IndexOf(PlayerSoldierCharacter),
				setCharacter = delegate(CharacterInfo c)
				{
					PlayerSoldierCharacter = c;
				}
			}));
		}

		private void SelectEnemySoldierCharacter()
		{
			ModuleLogger.Log("SelectPlayerCharacter");
			ScreenManager.PushScreen(new CharacterSelectionScreen(new CharacterSelectionParams
			{
				characters = _allCharacters,
				selectedIndex = _allCharacters.IndexOf(EnemySoldierCharacter),
				setCharacter = delegate(CharacterInfo c)
				{
					EnemySoldierCharacter = c;
				}
			}));
		}

		private List<CharacterInfo> getCharacters()
		{
			List<CharacterInfo> list = new List<CharacterInfo>();
			StreamReader txtReader = new StreamReader(BasePath.Name + "Modules/BattleTest/ModuleData/MPCharacters.xml");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(txtReader);
			XmlNode xmlNode = xmlDocument.ChildNodes[1];
			ModuleLogger.Writer.WriteLine("rootName.name {0}", xmlNode.Name);
			ModuleLogger.Writer.Flush();
			if (xmlNode.Name != "MPCharacters")
			{
				throw new Exception(xmlNode.Name);
			}
			for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode2 = xmlNode.ChildNodes[i];
				if (!(xmlNode2.Name != "NPCCharacter"))
				{
					string value = xmlNode2.Attributes["id"].Value;
					string value2 = xmlNode2.Attributes["name"].Value;
					string text = xmlNode2.Attributes["culture"]?.Value;
					string text2 = xmlNode2.Attributes["default_group"]?.Value;
					if (text == null)
					{
						text = "Other";
					}
					else if (text.StartsWith("Culture."))
					{
						text = text.Substring("Culture.".Length);
					}
					if (text2 == null)
					{
						text2 = "Other";
					}
					CharacterInfo item = new CharacterInfo
					{
						id = value,
						name = value2,
						culture = text.First().ToString().ToUpper() + text.Substring(1),
						defaultGroup = text2.First().ToString().ToUpper() + text2.Substring(1)
					};
					list.Add(item);
				}
			}
			return list;
		}
	}
}

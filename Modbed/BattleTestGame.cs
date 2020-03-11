using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Modbed
{
	public class BattleTestGame : GameType
	{
		private int _stepNo;

		public override bool IsBannerWindowAccessibleAtMission => true;

		public static BattleTestGame Current => Game.Current.GameType as BattleTestGame;

		protected override void OnInitialize()
		{
			base.OnInitialize();
			Game currentGame = base.CurrentGame;
			currentGame.FirstInitialize();
			IGameStarter gameStarter = new BasicGameStarter();
			InitializeGameModels(gameStarter);
			base.GameManager.OnGameStart(base.CurrentGame, gameStarter);
			MBObjectManager objectManager = currentGame.ObjectManager;
			currentGame.SecondInitialize(gameStarter.Models);
			currentGame.CreateGameManager();
			base.GameManager.BeginGameStart(base.CurrentGame);
			base.CurrentGame.RegisterBasicTypes();
			base.CurrentGame.ThirdInitialize();
			currentGame.CreateObjects();
			currentGame.InitializeDefaultGameObjects();
			currentGame.LoadBasicFiles(isLoad: false);
			base.ObjectManager.LoadXML("Items");
			base.ObjectManager.LoadXML("MPCharacters");
			base.ObjectManager.LoadXML("BasicCultures");
			base.ObjectManager.LoadXML("MPClassDivisions");
			objectManager.ClearEmptyObjects();
			currentGame.SetDefaultEquipments(new Dictionary<string, Equipment>());
			ModuleLogger.Writer.WriteLine(currentGame.BasicModels);
			ModuleLogger.Writer.Flush();
			if (currentGame.BasicModels.SkillList == null)
			{
				throw new Exception("haha");
			}
			currentGame.CreateLists();
			objectManager.ClearEmptyObjects();
			AddGameTexts();
			base.GameManager.OnCampaignStart(base.CurrentGame, null);
			base.GameManager.OnAfterCampaignStart(base.CurrentGame);
			base.GameManager.OnGameInitializationFinished(base.CurrentGame);
		}

		protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
		{
			ModuleLogger.Writer.WriteLine("BattleTestGame.DoLoadingForGameType {0}", gameTypeLoadingState);
			ModuleLogger.Writer.Flush();
			nextState = GameTypeLoadingStates.None;
			switch (gameTypeLoadingState)
			{
			case GameTypeLoadingStates.InitializeFirstStep:
				base.CurrentGame.Initialize();
				nextState = GameTypeLoadingStates.WaitSecondStep;
				break;
			case GameTypeLoadingStates.WaitSecondStep:
				nextState = GameTypeLoadingStates.LoadVisualsThirdState;
				break;
			case GameTypeLoadingStates.LoadVisualsThirdState:
				nextState = GameTypeLoadingStates.PostInitializeFourthState;
				break;
			}
		}

		public override void OnDestroy()
		{
		}

		private void InitializeGameModels(IGameStarter basicGameStarter)
		{
			basicGameStarter.AddModel(new MultiplayerAgentDecideKilledOrUnconsciousModel());
			basicGameStarter.AddModel(new CustomBattleAgentStatCalculateModel());
			basicGameStarter.AddModel(new MultiplayerAgentApplyDamageModel());
			basicGameStarter.AddModel(new DefaultRidingModel());
			basicGameStarter.AddModel(new DefaultStrikeMagnitudeModel());
			basicGameStarter.AddModel(new TestSkillList());
			basicGameStarter.AddModel(new CustomBattleMoraleModel());
		}

		protected override void OnRegisterTypes()
		{
			base.OnRegisterTypes();
			base.ObjectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "MPCharacters");
			base.ObjectManager.RegisterType<BasicCultureObject>("Culture", "BasicCultures");
			base.ObjectManager.RegisterType<MultiplayerClassDivisions.MPHeroClass>("MPClassDivision", "MPClassDivisions");
		}

		private void AddGameTexts()
		{
			base.CurrentGame.GameTextManager.LoadGameTexts(BasePath.Name + "Modules/Native/ModuleData/multiplayer_strings.xml");
			base.CurrentGame.GameTextManager.LoadGameTexts(BasePath.Name + "Modules/Native/ModuleData/global_strings.xml");
			base.CurrentGame.GameTextManager.LoadGameTexts(BasePath.Name + "Modules/Native/ModuleData/module_strings.xml");
		}
	}
}

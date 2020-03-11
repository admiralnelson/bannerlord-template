using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace Modbed
{
	public class BattleTestGameManager : MBGameManager
	{
		private int _stepNo;

		private BattleTestParams _params;

		private int levelNumber;

		public BattleTestGameManager(BattleTestParams p)
		{
			_params = p;
			levelNumber = p.levelNumber;
		}

		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			ModuleLogger.Writer.WriteLine("BattleTestGameManager.DoLoadingForGameManager {0}", gameManagerLoadingStep);
			ModuleLogger.Writer.Flush();
			nextStep = GameManagerLoadingSteps.None;
			switch (gameManagerLoadingStep)
			{
			case GameManagerLoadingSteps.PreInitializeZerothStep:
				MBGameManager.LoadModuleData(isLoadGame: false);
				MBGlobals.InitializeReferences();
				new Game(new BattleTestGame(), this).DoLoading();
				nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
				break;
			case GameManagerLoadingSteps.FirstInitializeFirstStep:
			{
				bool flag = true;
				foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
				{
					flag = (flag && subModule.DoLoading(Game.Current));
				}
				nextStep = ((!flag) ? GameManagerLoadingSteps.FirstInitializeFirstStep : GameManagerLoadingSteps.WaitSecondStep);
				break;
			}
			case GameManagerLoadingSteps.WaitSecondStep:
				MBGameManager.StartNewGame();
				nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
				break;
			case GameManagerLoadingSteps.SecondInitializeThirdState:
				nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState);
				break;
			case GameManagerLoadingSteps.PostInitializeFourthState:
				nextStep = GameManagerLoadingSteps.FinishLoadingFifthStep;
				break;
			case GameManagerLoadingSteps.FinishLoadingFifthStep:
				nextStep = GameManagerLoadingSteps.None;
				break;
			}
		}

		public override void OnLoadFinished()
		{
			ModuleLogger.Writer.WriteLine("BattleTestGameManager.OnLoadFinished");
			ModuleLogger.Writer.Flush();
			string[] array = new string[35]
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
			MissionState.OpenNew("BattleTest", new MissionInitializerRecord(array[levelNumber]), (Mission missionController) => new MissionBehaviour[7]
			{
				new BattleTestMissionController(_params),
				new AgentBattleAILogic(),
				new AgentVictoryLogic(),
				new AgentMoraleInteractionLogic(),
				new HighlightsController(),
				new BattleHighlightsController(),
				new FieldBattleController()
			});
		}
	}
}

using TaleWorlds.Engine.Screens;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.View.Screen;

namespace Modbed
{
	public class BattleTestMissionView : MissionView
	{
		private Mission _mission;

		public BattleTestMissionView(Mission mission)
		{
			_mission = mission;
		}

		public override void OnMissionScreenActivate()
		{
			foreach (MissionLogic missionLogic in _mission.MissionLogics)
			{
				BattleTestMissionController battleTestMissionController = missionLogic as BattleTestMissionController;
				if (battleTestMissionController != null)
				{
					(ScreenManager.TopScreen as MissionScreen).CombatCamera.Position = battleTestMissionController.freeCameraPosition;
					break;
				}
			}
		}
	}
}

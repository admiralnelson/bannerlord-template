using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.LegacyGUI.Missions;
using TaleWorlds.MountAndBlade.View.Missions;

namespace Modbed
{
	[ViewCreatorModule]
	public class BattleTestMissionViews
	{
		[ViewMethod("BattleTest")]
		public static MissionView[] OpenTestMission(Mission mission)
		{
			return new List<MissionView>
			{
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateOrderTroopPlacerView(mission),
				ViewCreator.CreateMissionKillNotificationUIHandler(),
				ViewCreator.CreateMissionLeaveView(),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(),
				ViewCreator.CreateMissionFlagMarkerUIHandler(),
				ViewCreator.CreateMissionOrderUIHandler(),
				new BattleTestMissionView(mission),
				new TestBattleMusicHandler()
			}.ToArray();
		}
	}
}

namespace TaleWorlds.MountAndBlade.View.Missions
{
	public class TestBattleMusicHandler : MissionView, IMusicHandler
	{
		bool IMusicHandler.IsPausable => false;

		public TestBattleMusicHandler()
		{
			if (MBMusicManager.Current == null)
			{
				MBMusicManager.Initialize();
				MBMusicManager.Current.StartTheme(MusicTheme.EmpireVictory, 100f);
			}
			else
			{
				MBMusicManager.Current.StartTheme(MusicTheme.EmpireVictory, 100f);
			}
		}

		public override void OnBehaviourInitialize()
		{
			base.OnBehaviourInitialize();
			MBMusicManager.Current.OnBattleMusicHandlerInit(this);
			MBMusicManager.Current.StartTheme(MusicTheme.EmpireVictory, 100f);
		}

		public override void OnMissionScreenFinalize()
		{
		}

		public void OnUpdated(float dt)
		{
		}
	}
}

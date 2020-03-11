using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Modbed
{
	public class BattleTestScreen : ScreenBase
	{
		private BattleTestVM _datasource;

		private GauntletLayer _gauntletLayer;

		private GauntletMovie _movie;

		private bool _firstRender;

		protected override void OnInitialize()
		{
			base.OnInitialize();
			_datasource = new BattleTestVM();
			_gauntletLayer = new GauntletLayer(100);
			_gauntletLayer.IsFocusLayer = true;
			AddLayer(_gauntletLayer);
			_gauntletLayer.InputRestrictions.SetInputRestrictions();
			ScreenManager.TrySetFocus(_gauntletLayer);
			ModuleLogger.Writer.WriteLine(BasePath.Name + " before load movie");
			ModuleLogger.Writer.Flush();
			_movie = _gauntletLayer.LoadMovie("BattleTestScreen", _datasource);
			ModuleLogger.Writer.WriteLine("after load movie");
			ModuleLogger.Writer.Flush();
			_firstRender = true;
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			InputContext input = _gauntletLayer.Input;
			if (_firstRender)
			{
				if (MBMusicManager.Current == null)
				{
					MBMusicManager.Initialize();
				}
				_firstRender = false;
				MBMusicManager.Current.StartTheme(MusicTheme.BattaniaVictory, 10f);
			}
			if (input.IsKeyReleased(InputKey.Escape))
			{
				ScreenManager.PopScreen();
			}
			else if (input.IsKeyPressed(InputKey.F5))
			{
				_movie.WidgetFactory.CheckForUpdates();
				_movie = _gauntletLayer.LoadMovie("BattleTestScreen", _datasource);
			}
		}
	}
}

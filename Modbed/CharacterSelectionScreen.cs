using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;

namespace Modbed
{
	public class CharacterSelectionScreen : ScreenBase
	{
		private CharacterSelectionVM _datasource;

		private GauntletLayer _gauntletLayer;

		private GauntletMovie _movie;

		private CharacterSelectionParams _params;

		public CharacterSelectionScreen(CharacterSelectionParams p)
		{
			_params = p;
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			_datasource = new CharacterSelectionVM(_params);
			_gauntletLayer = new GauntletLayer(100);
			_gauntletLayer.IsFocusLayer = true;
			AddLayer(_gauntletLayer);
			_gauntletLayer.InputRestrictions.SetInputRestrictions();
			ScreenManager.TrySetFocus(_gauntletLayer);
			HandleLoadMovie();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			InputContext input = _gauntletLayer.Input;
			if (input.IsKeyReleased(InputKey.Escape))
			{
				ScreenManager.PopScreen();
			}
			else if (input.IsKeyPressed(InputKey.F5))
			{
				_movie.WidgetFactory.CheckForUpdates();
				HandleLoadMovie();
			}
		}

		private void HandleLoadMovie()
		{
			CharacterSelectionVM vm = _datasource;
			_movie = _gauntletLayer.LoadMovie("CharacterSelectionScreen", _datasource);
			ListPanel listPanel = _movie.RootView.Target.FindChild("Cultures", includeAllChildren: true) as ListPanel;
			ListPanel groupsListPanel = _movie.RootView.Target.FindChild("Groups", includeAllChildren: true) as ListPanel;
			ListPanel charactersListPanel = _movie.RootView.Target.FindChild("Characters", includeAllChildren: true) as ListPanel;
			listPanel.IntValue = vm.SelectedCultureIndex;
			groupsListPanel.IntValue = vm.SelectedGroupIndex;
			charactersListPanel.IntValue = vm.SelectedCharacterIndex;
			ModuleLogger.Log("vm.SelectedCharacterIndex {0}", vm.SelectedCharacterIndex);
			listPanel.SelectEventHandlers.Add(delegate(Widget w)
			{
				vm.SelectedCultureChanged(w as ListPanel);
				groupsListPanel.IntValue = vm.SelectedGroupIndex;
				charactersListPanel.IntValue = vm.SelectedCharacterIndex;
			});
			groupsListPanel.SelectEventHandlers.Add(delegate(Widget w)
			{
				vm.SelectedGroupChanged(w as ListPanel);
				charactersListPanel.IntValue = vm.SelectedCharacterIndex;
			});
			charactersListPanel.SelectEventHandlers.Add(delegate(Widget w)
			{
				vm.SelectedCharacterChanged(w as ListPanel);
			});
		}
	}
}

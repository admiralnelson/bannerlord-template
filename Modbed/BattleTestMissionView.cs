using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.View.Screen;

namespace Modbed
{
	public class BattleTestMissionView : MissionView
	{
		private Mission _mission;
		private GauntletLayer _gauntletLayer;
		private GauntletMovie _gauntletMovie;
		private BattleTestVM _vm;
		public BattleTestMissionView(Mission mission)
		{
			_mission = mission;
		}

		public override void OnAfterMissionCreated()
		{
			base.OnAfterMissionCreated();

		}

		public override void OnAgentShootMissile(Agent shooterAgent, EquipmentIndex weaponIndex, Vec3 position, Vec3 velocity, Mat3 orientation, bool hasRigidBody, int forcedMissileIndex)
		{
			base.OnAgentShootMissile(shooterAgent, weaponIndex, position, velocity, orientation, hasRigidBody, forcedMissileIndex);
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override bool OnEscape()
		{
			return base.OnEscape();
		}

		public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
		{
			base.OnFocusGained(agent, focusableObject, isInteractable);
		}

		public override void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			base.OnFocusLost(agent, focusableObject);
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

		public override void OnMissionScreenDeactivate()
		{
			base.OnMissionScreenDeactivate();
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			InitializeLayer();
		}

		public override void OnMissionScreenPreLoad()
		{
			base.OnMissionScreenPreLoad();
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			InputContext input = _gauntletLayer.Input;

			if (input.IsKeyReleased(InputKey.Escape))
			{
				//ScreenManager.PopScreen();
				MissionScreen.RemoveLayer(_gauntletLayer);
			}
			else if (input.IsKeyPressed(InputKey.F5))
			{
				_gauntletMovie.WidgetFactory.CheckForUpdates();
				_gauntletMovie = _gauntletLayer.LoadMovie("ArmourScreen", _vm);
			}
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);			
		}

		public void InitializeLayer()
		{
			_gauntletLayer = new GauntletLayer(ViewOrderPriorty);
			_vm = new BattleTestVM();
			_gauntletMovie = _gauntletLayer.LoadMovie("ArmourScreen", _vm);
			base.MissionScreen.AddLayer(_gauntletLayer);
			_gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.Mouse);
		}
	}
}

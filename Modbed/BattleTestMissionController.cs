using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Modbed
{
	public class BattleTestMissionController : MissionLogic
	{
		private Game _game;

		public BattleTestParams battleTestParams;

		public Vec3 freeCameraPosition;

		public Agent _playerAgent;

		private Agent _controlledBot;

		private Scene _currentScene;

		private Random rnd;

		private SoundEvent _musicSoundEvent;

		private bool _started;

		private bool _flyActive;

		private bool _gameIsOver;

		private float _soundTimeTrigger;

		private float _makeSound;

		private int hooarayTimes;

		private int currentMusic = 1;

		public BattleTestMissionController(BattleTestParams p)
		{
			_game = Game.Current;
			_flyActive = false;
			_gameIsOver = false;
			battleTestParams = p;
			rnd = new Random();
			hooarayTimes = 0;
			_makeSound = 0f;
			_soundTimeTrigger = 10f;
		}

		public override void AfterStart()
		{
			try
			{
				AfterStart2();
			}
			catch (Exception ex)
			{
				ModuleLogger.Log("{0}", ex);
			}
		}

		public void AfterStart2()
		{
			_started = true;
			Scene scene = base.Mission.Scene;
			rnd = new Random();
			_currentScene = scene;
			if (rnd.Next(2) == 0)
			{
				SetRandomizedWeather();
			}
			if (battleTestParams.skyBrightness >= 0f)
			{
				scene.SetSkyBrightness(battleTestParams.skyBrightness);
			}
			base.Mission.MissionTeamAIType = Mission.MissionTeamAITypeEnum.FieldBattle;
			base.Mission.SetMissionMode(MissionMode.Battle, atStart: true);
			float soldierXInterval = battleTestParams.soldierXInterval;
			float soldierYInterval = battleTestParams.soldierYInterval;
			int soldiersPerRow = battleTestParams.soldiersPerRow;
			Vec2 formationPosition = battleTestParams.FormationPosition;
			Vec2 formationDirection = battleTestParams.formationDirection;
			Vec2 v = battleTestParams.formationDirection.LeftVec();
			Vec2 vec = new Vec2(0f, 1f);
			bool useFreeCamera = battleTestParams.useFreeCamera;
			BasicCharacterObject @object = _game.ObjectManager.GetObject<BasicCharacterObject>(battleTestParams.playerSoldierCharacterId);
			FormationClass currentFormationClass = @object.CurrentFormationClass;
			Banner banner = Banner.CreateRandomBanner(rnd.Next(-1, 100));
			banner = new Banner("11." + rnd.Next(1, 2000).ToString() + ".123." + rnd.Next(1, 7000).ToString() + "." + rnd.Next(1, 2000).ToString() + ".768.768.1.0.0.160.0.15.512.512.769.764.1.0.0");
			uint color = 4291404800u;
			uint num = 4283826176u;
			uint color2 = uint.MaxValue;
			banner.ChangeIconColors(color);
			banner.ChangePrimaryColor(num);
			Team team = base.Mission.Teams.Add(BattleSideEnum.Attacker, num, color2, banner);
			team.AddTeamAI(new TeamAIGeneral(base.Mission, team));
			team.AddTacticOption(new TacticCharge(team));
			team.ExpireAIQuerySystem();
			team.ResetTactic();
			base.Mission.PlayerTeam = team;
			Vec2 vec2 = formationPosition + formationDirection * -10f + v * -10f;
			Vec3 o = new Vec3(vec2.x, vec2.y, 30f);
			if (!useFreeCamera)
			{
				Mat3 identity = Mat3.Identity;
				identity.RotateAboutUp(vec.AngleBetween(formationDirection));
				AgentBuildData agentBuildData = new AgentBuildData(new BasicBattleAgentOrigin(_game.ObjectManager.GetObject<BasicCharacterObject>(battleTestParams.playerCharacterId))).ClothingColor1(num).Banner(team.Banner).InitialFrame(new MatrixFrame(identity, o));
				Agent agent = base.Mission.SpawnAgent(agentBuildData);
				agent.Controller = Agent.ControllerType.Player;
				agent.WieldInitialWeapons();
				agent.AllowFirstPersonWideRotation();
				base.Mission.MainAgent = agent;
				agent.SetTeam(team, sync: true);
				team.GetFormation(currentFormationClass).PlayerOwner = agent;
				try
				{
					team.GetFormation(FormationClass.Infantry).PlayerOwner = agent;
					team.GetFormation(FormationClass.Ranged).PlayerOwner = agent;
					team.GetFormation(FormationClass.Bodyguard).PlayerOwner = agent;
					team.GetFormation(FormationClass.HeavyCavalry).PlayerOwner = agent;
					team.GetFormation(FormationClass.Cavalry).PlayerOwner = agent;
					team.GetFormation(FormationClass.HeavyCavalry).PlayerOwner = agent;
					team.GetFormation(FormationClass.HorseArcher).PlayerOwner = agent;
					team.GetFormation(FormationClass.LightCavalry).PlayerOwner = agent;
					team.GetFormation(FormationClass.NumberOfDefaultFormations).PlayerOwner = agent;
				}
				catch (Exception ex)
				{
					displayMessage("Ooupsy" + ex.Message);
				}
				team.SetPlayerRole(isPlayerGeneral: true, isPlayerSergeant: false);
				team.PlayerOrderController.Owner = agent;
				team.MasterOrderController.Owner = agent;
				team.OnOrderIssued += PlayerTeam_OnOrderIssued;
				_playerAgent = agent;
			}
			else
			{
				int playerSoldierCount = battleTestParams.playerSoldierCount;
				if (playerSoldierCount <= 0)
				{
					freeCameraPosition = new Vec3(formationPosition.x, formationPosition.y, 30f);
				}
				else
				{
					int num2 = (playerSoldierCount + soldiersPerRow - 1) / soldiersPerRow;
					Vec2 vec3 = formationPosition + (float)((Math.Min(soldiersPerRow, playerSoldierCount) - 1) / 2) * soldierYInterval * v - (float)num2 * soldierXInterval * formationDirection;
					freeCameraPosition = new Vec3(vec3.x, vec3.y, 5f);
				}
			}
			bool flag = false;
			Formation formation = team.GetFormation(currentFormationClass);
			float initialFormationWidth = getInitialFormationWidth(team, currentFormationClass);
			WorldPosition value = new WorldPosition(scene, (formationPosition + v * (initialFormationWidth / 2f)).ToVec3());
			formation.SetPositioning(value, formationDirection);
			formation.FormOrder = FormOrder.FormOrderCustom(initialFormationWidth);
			flag = (value.GetNavMesh() != UIntPtr.Zero);
			for (int i = 0; i < battleTestParams.playerSoldierCount; i++)
			{
				Formation formation2 = team.GetFormation(currentFormationClass);
				AgentBuildData agentBuildData2 = new AgentBuildData(new BasicBattleAgentOrigin(@object)).ClothingColor1(team.Color).ClothingColor2(team.Color2).Banner(team.Banner)
					.IsFemale(isFemale: false)
					.EquipmentSeed(rnd.Next(0, 150))
					.Team(team)
					.Formation(formation2);
				if (!flag)
				{
					int num3 = i / soldiersPerRow;
					int num4 = i % soldiersPerRow;
					Mat3 identity2 = Mat3.Identity;
					Vec2 vec4 = formationPosition + formationDirection * ((0f - soldierXInterval) * (float)num3) + v * soldierYInterval * num4;
					identity2.RotateAboutUp(vec.AngleBetween(formationDirection));
					MatrixFrame frame = new MatrixFrame(identity2, new Vec3(vec4.x, vec4.y, 30f));
					agentBuildData2.InitialFrame(frame);
				}
				Agent agent2 = base.Mission.SpawnAgent(agentBuildData2);
				agent2.TryToWieldWeaponInSlot(EquipmentIndex.Weapon4, Agent.WeaponWieldActionType.WithAnimationUninterruptible, isWieldedOnSpawn: true);
				agent2.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
			}
			BasicCharacterObject object2 = _game.ObjectManager.GetObject<BasicCharacterObject>(battleTestParams.enemySoldierCharacterId);
			Banner banner2 = Banner.CreateRandomBanner(rnd.Next(-1, 100));
			banner2.ChangePrimaryColor(4287526228u);
			Team team2 = base.Mission.Teams.Add(BattleSideEnum.Defender, banner2.GetPrimaryColor(), 4280492835u, banner2);
			TeamAIGeneral teamAIGeneral = new TeamAIGeneral(base.Mission, team2, 2f);
			teamAIGeneral.AddTacticOption(new TacticArchersOnTheHill(team2));
			team2.AddTeamAI(teamAIGeneral);
			team2.AddTacticOption(new TacticCharge(team2));
			if (rnd.Next(4) > 0)
			{
				DisplayMessanger("Sir! Enemy have an experienced commander", new Color(0.247f, 0.949f, 0.435f));
				team2.AddTacticOption(new TacticFullScaleAttack(team2));
				team2.AddTacticOption(new TacticHoldTheHill(team2));
				team2.AddTacticOption(new TacticDefensiveEngagement(team2));
				team2.AddTacticOption(new TacticDefensiveRing(team2));
				team2.AddTacticOption(new TacticDefensiveLine(team2));
				team2.AddTacticOption(new TacticFrontalCavalryCharge(team2));
				team2.AddTacticOption(new TacticHoldChokePoint(team2));
				team2.AddTacticOption(new TacticRangedHarrassmentOffensive(team2));
				team2.AddTacticOption(new TacticPerimeterDefense(team2));
			}
			team2.SetIsEnemyOf(team, isEnemyOf: true);
			team.SetIsEnemyOf(team2, isEnemyOf: true);
			team2.ExpireAIQuerySystem();
			team2.ResetTactic();
			team2.OnFormationAIActiveBehaviourChanged += EnemyTeam_OnFormationAIActiveBehaviourChanged;
			team2.OnOrderIssued += EnemyTeam_OnOrderIssued;
			team2.OnFormationsChanged += EnemyTeam_OnFormationsChanged;
			FormationClass currentFormationClass2 = object2.CurrentFormationClass;
			Formation formation3 = team2.GetFormation(object2.CurrentFormationClass);
			float initialFormationWidth2 = getInitialFormationWidth(team2, currentFormationClass2);
			WorldPosition value2 = new WorldPosition(scene, (formationPosition + v * (initialFormationWidth2 / 2f) + formationDirection * battleTestParams.distance).ToVec3());
			formation3.SetPositioning(value2, -formationDirection);
			formation3.FormOrder = FormOrder.FormOrderCustom(initialFormationWidth2);
			for (int j = 0; j < battleTestParams.enemySoldierCount; j++)
			{
				AgentBuildData agentBuildData3 = new AgentBuildData(new BasicBattleAgentOrigin(object2)).ClothingColor1(team2.Color).ClothingColor2(team2.Color2).Banner(team2.Banner)
					.EquipmentSeed(rnd.Next(0, 4))
					.Formation(formation3);
				if (!flag)
				{
					int num5 = j / soldiersPerRow;
					int num6 = j % soldiersPerRow;
					Mat3.Identity.RotateAboutUp(vec.AngleBetween(-formationDirection));
					Vec2 vec5 = formationPosition + formationDirection * battleTestParams.distance + formationDirection * soldierXInterval * num5 + v * soldierYInterval * num6;
					MatrixFrame frame2 = new MatrixFrame(Mat3.Identity, new Vec3(vec5.x, vec5.y, 30f));
					agentBuildData3.InitialFrame(frame2);
				}
				Agent agent3 = base.Mission.SpawnAgent(agentBuildData3);
				agent3.TryToWieldWeaponInSlot(EquipmentIndex.Weapon4, Agent.WeaponWieldActionType.WithAnimationUninterruptible, isWieldedOnSpawn: true);
				agent3.SetTeam(team2, sync: true);
				agent3.Formation = formation3;
				agent3.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
			}
			bool flag2 = base.Mission.IsOrderShoutingAllowed();
			bool flag3 = base.Mission.IsAgentInteractionAllowed();
			bool isClientOrReplay = GameNetwork.IsClientOrReplay;
			bool flag4 = team.PlayerOrderController.Owner == null;
			ModuleLogger.Log("Mission allowed shouting: {0} interaction: {1} {2} {3}", flag2, flag3, isClientOrReplay, flag4);
			MBMusicManager.Current.StartTheme((MusicTheme)currentMusic, 100f);
			_musicSoundEvent = SoundEvent.CreateEvent(SoundEvent.GetEventIdFromString("event:/mission/ambient/area/multiplayer/city_mp_02"), _currentScene);
			_musicSoundEvent.Play();
			displayMessage("Loading " + base.Mission.SceneName);
		}

		private void EnemyTeam_OnFormationsChanged(Team arg1, Formation arg2)
		{
			DisplayMessanger("Sir! Enemy team has changed its formation! ", new Color(0.247f, 0.949f, 0.435f));
		}

		private void EnemyTeam_OnOrderIssued(OrderType orderType, IEnumerable<Formation> appliedFormations, params object[] delegateParams)
		{
			DisplayMessanger("Sir! Enemy general has issued an order! Order looks like an " + orderType.ToString() + " order", new Color(1f, 0.847f, 0.42f));
		}

		private void EnemyTeam_OnFormationAIActiveBehaviourChanged(Formation obj)
		{
			DisplayMessanger("Sir, enemy team has changed active behaviour! ", new Color(0.267f, 0.969f, 0.455f));
		}

		private void PlayerTeam_OnOrderIssued(OrderType orderType, IEnumerable<Formation> appliedFormations, params object[] delegateParams)
		{
			SoundEvent soundEvent = SoundEvent.CreateEvent(SoundEvent.GetEventIdFromString("event:/ui/mission/horns/attack"), _currentScene);
			switch (orderType)
			{
			case OrderType.Move:
				soundEvent = SoundEvent.CreateEvent(SoundEvent.GetEventIdFromString("event:/ui/mission/horns/move"), _currentScene);
				hooarayTimes = 3;
				_makeSound = base.Mission.Time;
				_soundTimeTrigger = _makeSound + 0.1f;
				break;
			case OrderType.FollowMe:
				soundEvent = SoundEvent.CreateEvent(SoundEvent.GetEventIdFromString("event:/ui/mission/horns/retreat"), _currentScene);
				break;
			case OrderType.Charge:
				hooarayTimes = 6;
				_makeSound = base.Mission.Time;
				_soundTimeTrigger = _makeSound + 0.1f;
				break;
			}
			soundEvent.Play();
		}

		private void Mission_OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			displayMessage("Main agent changed!");
		}

		public override void OnBattleEnded()
		{
			displayMessage("Battle ended!");
		}

		public override void OnMissionTick(float dt)
		{
			try
			{
				if (_started)
				{
					try
					{
						AdvanceSounds(dt);
					}
					catch (Exception ex)
					{
						displayMessage("Problem with advance sounds" + ex.Message);
					}
					if (!_gameIsOver)
					{
						try
						{
							CheckIsEnemySideRetreatingOrOneSideDepleted();
						}
						catch (Exception ex2)
						{
							displayMessage("You are dead! - And that's why you have this error: " + ex2.Message);
						}
					}
					if (!_flyActive && (_playerAgent == null || !_playerAgent.IsActive()))
					{
						_flyActive = true;
					}
					if (base.Mission.InputManager.IsKeyPressed(InputKey.L) && _flyActive)
					{
						TeleportPlayer();
					}
					else if (base.Mission.InputManager.IsKeyPressed(InputKey.B))
					{
						MBEditor.EnterEditMissionMode(base.Mission);
					}
					else if (base.Mission.InputManager.IsKeyReleased(InputKey.Tab))
					{
						ReportGameStatus();
					}
					else if (base.Mission.InputManager.IsKeyPressed(InputKey.P))
					{
						MissionState.Current.Paused = !MissionState.Current.Paused;
						displayMessage("Paused Game. To leave the battle, hold TAB..");
					}
					else if (base.Mission.InputManager.IsKeyPressed(InputKey.I))
					{
						Vec3 vec = base.Mission.MainAgent?.Position ?? base.Mission.Scene.LastFinalRenderCameraPosition;
						string text = new WorldPosition(base.Mission.Scene, vec).GetNavMesh().ToString() ?? "";
						displayMessage($"Position: {vec} | Navmesh: {text} | Trigger: {_soundTimeTrigger} | Time: {base.Mission.Time}");
						ModuleLogger.Log("INFO Position: {0}, Navigation Mesh: {1}", vec, text);
					}
					else if (base.Mission.InputManager.IsKeyPressed(InputKey.F) && _flyActive)
					{
						try
						{
							Vec3 groundVec = new WorldPosition(base.Mission.Scene, base.Mission.Scene.LastFinalRenderCameraPosition).GetGroundVec3();
							Agent closestAllyAgent = base.Mission.GetClosestAllyAgent(base.Mission.PlayerTeam, groundVec, 20f);
							if (closestAllyAgent != null)
							{
								if (closestAllyAgent.IsRunningAway)
								{
									displayMessage("This soldier is running away!");
								}
								else
								{
									displayMessage("Taking control of ally soldier nearby..");
									base.Mission.MainAgent = null;
									if (_playerAgent != null && _playerAgent.IsActive())
									{
										_playerAgent.Controller = Agent.ControllerType.None;
									}
									if (_controlledBot != null)
									{
										_controlledBot.Controller = Agent.ControllerType.AI;
									}
									closestAllyAgent.Controller = Agent.ControllerType.Player;
									_controlledBot = closestAllyAgent;
									_flyActive = false;
								}
							}
							else
							{
								displayMessage("No ally soldier nearby..");
							}
						}
						catch (Exception ex3)
						{
							displayMessage(ex3.Message);
						}
					}
					else if (base.Mission.InputManager.IsKeyPressed(InputKey.C))
					{
						if (_flyActive && _playerAgent != null && _playerAgent.IsActive())
						{
							if (_controlledBot != null)
							{
								_controlledBot.Controller = Agent.ControllerType.AI;
							}
							_playerAgent.Controller = Agent.ControllerType.AI;
							_playerAgent.Controller = Agent.ControllerType.Player;
							_flyActive = false;
						}
						else
						{
							SwitchCamera();
						}
					}
				}
			}
			catch (Exception ex4)
			{
				displayMessage(ex4.Message + " - " + ex4.StackTrace);
			}
		}

		public override void OnClearScene()
		{
		}

		private void SetRandomizedWeather()
		{
			displayMessage("Setting foggy field..");
			_currentScene.SetFogAdvanced(500f, 0f, rnd.Next(40, 500));
			Vec3 fogColor = new Vec3(0.7f, 0.652f, 0.511f);
			_currentScene.SetFog(rnd.Next(2, 8), ref fogColor, 0.2f);
		}

		private void CheckIsEnemySideRetreatingOrOneSideDepleted()
		{
			if (_gameIsOver)
			{
				return;
			}
			IReadOnlyList<Agent> activeAgents = base.Mission.PlayerTeam.ActiveAgents;
			IReadOnlyList<Agent> activeAgents2 = base.Mission.PlayerEnemyTeam.ActiveAgents;
			AgentVictoryLogic missionBehaviour = base.Mission.GetMissionBehaviour<AgentVictoryLogic>();
			bool flag = false;
			foreach (Agent item in activeAgents)
			{
				if (item.Health > 0f && !item.IsRunningAway)
				{
					flag = true;
				}
				if (item.IsRunningAway)
				{
					try
					{
						if (item.HasWeapon())
						{
							item.MovementFlags = Agent.MovementControlFlag.DefendRight;
							item.RemoveEquippedWeapon(EquipmentIndex.WeaponItemBeginSlot);
							item.RemoveEquippedWeapon(EquipmentIndex.Weapon1);
							item.RemoveEquippedWeapon(EquipmentIndex.Weapon2);
							item.RemoveEquippedWeapon(EquipmentIndex.Weapon3);
						}
					}
					catch (Exception)
					{
					}
				}
			}
			if (!flag)
			{
				_gameIsOver = true;
				foreach (Agent item2 in activeAgents2)
				{
					missionBehaviour.SetTimersOfVictoryReactions(item2, 1f, 2f);
				}
				displayMessage("You have lost the battle! You killed " + _playerAgent.KillCount.ToString() + " enemies. Hold TAB to exit.");
				return;
			}
			flag = false;
			foreach (Agent item3 in activeAgents2)
			{
				if (item3.Health > 0f && !item3.IsRunningAway)
				{
					flag = true;
				}
				if (item3.IsRunningAway)
				{
					try
					{
						if (item3.HasWeapon())
						{
							item3.MovementFlags = Agent.MovementControlFlag.DefendRight;
							item3.RemoveEquippedWeapon(EquipmentIndex.WeaponItemBeginSlot);
							item3.RemoveEquippedWeapon(EquipmentIndex.Weapon1);
							item3.RemoveEquippedWeapon(EquipmentIndex.Weapon2);
							item3.RemoveEquippedWeapon(EquipmentIndex.Weapon3);
						}
					}
					catch (Exception)
					{
					}
				}
			}
			if (!flag)
			{
				_gameIsOver = true;
				foreach (Agent item4 in activeAgents)
				{
					missionBehaviour.SetTimersOfVictoryReactions(item4, 1f, 2f);
				}
				displayMessage("You have won the battle! You killed " + _playerAgent.KillCount.ToString() + " enemies. Hold TAB to exit.");
			}
		}

		private void ReportGameStatus()
		{
			int count = base.Mission.PlayerTeam.ActiveAgents.Count;
			int count2 = base.Mission.PlayerEnemyTeam.ActiveAgents.Count;
			DisplayMessanger("Currently we have " + count.ToString() + " soldiers alive in the battlefield.", new Color(1f, 0.847f, 0.42f));
			DisplayMessanger("Enemy has " + count2.ToString() + " soldiers alive in the battlefield.", new Color(1f, 0.847f, 0.42f));
			DisplayMessanger("And you alone killed " + _playerAgent.KillCount.ToString() + " foes", new Color(1f, 0.847f, 0.42f));
		}

		private float getInitialFormationWidth(Team team, FormationClass fc)
		{
			BattleTestParams battleTestParams = this.battleTestParams;
			team.GetFormation(fc);
			bool num = fc == FormationClass.Cavalry || fc == FormationClass.HorseArcher;
			float defaultUnitDiameter = Formation.GetDefaultUnitDiameter(num);
			int unitSpacing = 1;
			float num2 = num ? Formation.CavalryInterval(unitSpacing) : Formation.InfantryInterval(unitSpacing);
			return (float)(Math.Min(battleTestParams.soldiersPerRow, battleTestParams.playerSoldierCount) - 1) * (defaultUnitDiameter + num2) + defaultUnitDiameter + 0.1f;
		}

		public void AdvanceSounds(float dt)
		{
			try
			{
				if (hooarayTimes > 0)
				{
					_makeSound += dt;
					if (!(_makeSound < _soundTimeTrigger))
					{
						hooarayTimes--;
						_soundTimeTrigger += 1.3f;
						try
						{
							foreach (Agent activeAgent in base.Mission.PlayerTeam.ActiveAgents)
							{
								if (!activeAgent.IsPlayerControlled)
								{
									if (activeAgent.IsCharging)
									{
										hooarayTimes = 0;
										break;
									}
									activeAgent.MakeVoice(SkinVoiceManager.VoiceType.Grunt, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
								}
							}
						}
						catch (Exception ex)
						{
							displayMessage(ex.Message);
						}
					}
				}
			}
			catch (Exception)
			{
				displayMessage("Not cool!");
			}
		}

		private void TeleportPlayer()
		{
			try
			{
				Vec3 groundVec = new WorldPosition(base.Mission.Scene, base.Mission.Scene.LastFinalRenderCameraPosition).GetGroundVec3();
				Vec3 lookDirection = -base.Mission.Scene.LastFinalRenderCameraFrame.rotation.u;
				lookDirection.Normalize();
				if (_playerAgent.HasMount)
				{
					BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
					Mat3 identity = Mat3.Identity;
					identity.RotateAboutUp(new Vec2(0f, 1f).AngleBetween(lookDirection.AsVec2));
					MatrixFrame matrixFrame = new MatrixFrame(identity, groundVec);
					Agent mountAgent = _playerAgent.MountAgent;
					mountAgent.GetType().GetProperty("InitialFrame", bindingAttr).SetValue(mountAgent, matrixFrame);
				}
				_playerAgent.LookDirection = lookDirection;
				_playerAgent.TeleportToPosition(groundVec);
				_playerAgent.Controller = Agent.ControllerType.AI;
				_playerAgent.Controller = Agent.ControllerType.Player;
				displayMessage($"Teleport player to {groundVec}");
				_flyActive = false;
			}
			catch (Exception ex)
			{
				displayMessage(ex.Message);
			}
		}

		private void SwitchCamera()
		{
			ModuleLogger.Log("SwitchCamera");
			try
			{
				if (_playerAgent == null || !_playerAgent.IsActive())
				{
					displayMessage("You are dead! Press F to control your soldiers");
					_flyActive = true;
				}
				else if (base.Mission.MainAgent == null)
				{
					if (_controlledBot != null)
					{
						_controlledBot.Controller = Agent.ControllerType.AI;
						base.Mission.MainAgent = null;
						_flyActive = true;
					}
				}
				else
				{
					_flyActive = true;
					base.Mission.MainAgent = null;
					_playerAgent.Controller = Agent.ControllerType.None;
					displayMessage("Switch camera");
				}
			}
			catch (Exception ex)
			{
				displayMessage("ERROR: " + ex.Message);
			}
		}

		private void displayMessage(string msg)
		{
			InformationManager.DisplayMessage(new InformationMessage(new TextObject(msg).ToString()));
			ModuleLogger.Log("LOG-{0}", msg);
		}

		private void DisplayMessanger(string msg, Color col)
		{
			InformationManager.DisplayMessage(new InformationMessage(msg, col));
		}

		private void SetDefensiveArrangementMoveBehaviorValues(Agent unit)
		{
			unit.SetAIBehaviorValues(AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
			unit.SetAIBehaviorValues(AISimpleBehaviorKind.Melee, 0f, 7f, 0f, 20f, 0f);
			unit.SetAIBehaviorValues(AISimpleBehaviorKind.Ranged, 0f, 7f, 0f, 20f, 0f);
			unit.SetAIBehaviorValues(AISimpleBehaviorKind.ChargeHorseback, 0f, 7f, 0f, 30f, 0f);
			unit.SetAIBehaviorValues(AISimpleBehaviorKind.RangedHorseback, 0f, 15f, 0f, 30f, 0f);
			unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 4f);
			unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
		}

		private void testBatchUnitPositionAvailabilities(LineFormation lf)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			Formation formation = lf.GetType().GetField("owner", bindingAttr).GetValue(lf) as Formation;
			object[] parameters = new object[6]
			{
				lf.GetType().GetField("cachedOrderedUnitPositionIndices", bindingAttr).GetValue(lf),
				lf.GetType().GetField("cachedOrderedLocalPositions", bindingAttr).GetValue(lf),
				lf.GetType().GetField("unitPositionAvailabilities", bindingAttr).GetValue(lf),
				lf.GetType().GetField("globalPositions", bindingAttr).GetValue(lf),
				lf.GetType().GetProperty("FileCount", bindingAttr).GetValue(lf),
				lf.GetType().GetProperty("RankCount", bindingAttr).GetValue(lf)
			};
			ModuleLogger.Log("test batch : {0}", formation.GetType().GetInterface("IFormation").GetMethod("BatchUnitPositions", bindingAttr)
				.Invoke(formation, parameters));
		}
	}
}

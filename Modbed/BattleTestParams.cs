using TaleWorlds.Library;

namespace Modbed
{
	public struct BattleTestParams
	{
		public int playerSoldierCount;

		public int enemySoldierCount;

		public int levelNumber;

		public float distance;

		public float soldierXInterval;

		public float soldierYInterval;

		public int soldiersPerRow;

		public Vec2 FormationPosition;

		public Vec2 formationDirection;

		public float skyBrightness;

		public float timeOfDay;

		public float rainDensity;

		public float tempVar;

		public float infantryPercentage;

		public float archerPercentage;

		public string playerCharacterId;

		public string playerSoldierCharacterId;

		public string enemySoldierCharacterId;

		public bool useFreeCamera;

		public bool wantMoraleSystem;

		public bool iAmFeelingLucky;

		public static BattleTestParams createDefault()
		{
			BattleTestParams result = default(BattleTestParams);
			result.levelNumber = 4;
			result.timeOfDay = 5f;
			result.playerSoldierCount = 200;
			result.enemySoldierCount = 200;
			result.distance = 150f;
			result.soldierXInterval = 2f;
			result.soldierYInterval = 1.5f;
			result.soldiersPerRow = 50;
			result.FormationPosition = new Vec2(418f, 393f);
			result.formationDirection = new Vec2(1f, 0f);
			result.skyBrightness = 200f;
			result.rainDensity = 0f;
			result.tempVar = 0f;
			result.playerCharacterId = "mp_heavy_cavalry_vlandia_hero";
			result.playerSoldierCharacterId = "mp_heavy_infantry_vlandia_troop";
			result.enemySoldierCharacterId = "mp_light_infantry_vlandia_troop";
			result.useFreeCamera = false;
			result.wantMoraleSystem = true;
			result.iAmFeelingLucky = false;
			result.archerPercentage = 50f;
			result.infantryPercentage = 70f;
			return result;
		}

		public bool validate()
		{
			if (playerSoldierCount >= 0 && enemySoldierCount >= 0 && (double)distance > 0.0 && (double)soldierXInterval > 0.0 && (double)soldierYInterval > 0.0 && soldiersPerRow > 0 && (double)formationDirection.Length > 0.0 && playerCharacterId != null && playerSoldierCharacterId != null)
			{
				return enemySoldierCharacterId != null;
			}
			return false;
		}
	}
}

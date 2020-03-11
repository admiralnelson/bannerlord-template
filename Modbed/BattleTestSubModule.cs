using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.TwoDimension;

namespace Modbed
{
	public class BattleTestSubModule : MBSubModuleBase
	{
		private static BattleTestSubModule _instance;

		private bool _initialized;

		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			ModuleLogger.Writer.WriteLine("BattleTestSubModule::OnSubModuleLoad");
			_instance = this;
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("battletest", new TextObject("{=battletest}Custom Battle"), 9996, delegate
			{
				ScreenManager.PushScreen(new BattleTestScreen());
			}, isDisabled: false));
		}

		protected override void OnSubModuleUnloaded()
		{
			ModuleLogger.Writer.WriteLine("BattleTestSubModule::OnSubModuleUnloaded");
			ModuleLogger.Writer.Close();
			_instance = null;
			base.OnSubModuleUnloaded();
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			if (!_initialized)
			{
				_initialized = true;
			}
		}

		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
		}

		private void enableChinese()
		{
			ModuleLogger.Writer.WriteLine("enableChinese");
			ModuleLogger.Writer.Flush();
			try
			{
				addChineseFont();
				TaleWorlds.Engine.Texture texture = (UIResourceManager.FontFactory.GetFont("Galahad").FontSprite.Texture.PlatformTexture as EngineTexture).Texture;
				ModuleLogger.Writer.WriteLine("texture1.name {0} {1} {2} {3} {4}", texture.Name, texture.IsValid, texture.MemorySize, texture.Width, texture.Height);
				ModuleLogger.Writer.Flush();
				Font font = UIResourceManager.FontFactory.GetFont("warbandfont");
				TaleWorlds.Engine.Texture texture2 = TaleWorlds.Engine.Texture.LoadTextureFromPath("fonts_6.png", "D:\\SteamLibrary\\steamapps\\common\\Mount & Blade II Bannerlord - Beta\\GUI\\GauntletUI\\SpriteSheets\\fonts");
				ModuleLogger.Writer.WriteLine("{0} {1} {2}", font == null, texture2 == null, font.FontSprite == null);
				ModuleLogger.Writer.Flush();
				font.FontSprite.Category.SpriteSheets[font.FontSprite.SheetID - 1] = new TaleWorlds.TwoDimension.Texture(new EngineTexture(texture2));
				ModuleLogger.Writer.WriteLine("texture.name {0} {1} {2} {3} {4}", texture2.Name, texture2.IsValid, texture2.MemorySize, texture2.Width, texture2.Height);
				ModuleLogger.Writer.Flush();
				UIResourceManager.FontFactory.DefaultFont = UIResourceManager.FontFactory.GetFont("warbandfont");
			}
			catch (Exception ex)
			{
				ModuleLogger.Writer.WriteLine(ex);
				ModuleLogger.Writer.WriteLine(ex.StackTrace);
				ModuleLogger.Writer.Flush();
				throw;
			}
		}

		private void addChineseFont()
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			SpriteCategory spriteCategory = new SpriteCategory("customfonts", spriteData, 1);
			spriteCategory.SheetSizes = new Vec2i[1]
			{
				new Vec2i(2048, 16384)
			};
			spriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
			spriteData.SpriteCategories.Add(spriteCategory.Name, spriteCategory);
			SpritePart spritePart = new SpritePart("warbandfont", spriteCategory, 2048, 16384)
			{
				SheetID = 1,
				SheetX = 0
			};
			spritePart.SheetX = 0;
			spriteData.SpritePartNames.Add(spritePart.Name, spritePart);
			SpriteGeneric spriteGeneric = new SpriteGeneric("warbandfont", spritePart);
			spriteData.SpriteNames.Add(spriteGeneric.Name, spriteGeneric);
			Font font = UIResourceManager.FontFactory.GetFont("warbandfont");
			font.GetType().GetProperty("FontSprite").SetValue(font, spritePart);
		}
	}
}

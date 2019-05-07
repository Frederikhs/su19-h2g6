using System;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;
using OpenTK;
using SpaceTaxiGame;

namespace SpaceTaxi_2.SpaceTaxiStates {
    public class GameLevelPicker : IGameState {
        private static GameLevelPicker instance;

        //Colors
        private Vec3F active;

        private Entity backGroundImage;
        private Vec3F inactive;
        private Text[] menuButtons;

        private int selectedMenu;

        public GameLevelPicker() {
            InitializeGameState();
        }

        public void RenderState() {
            //Render background image
            backGroundImage.RenderEntity();

            //Render each menu button
            foreach (var aButton in menuButtons) {
                aButton.RenderText();
            }
        }

        private void SetActiveMenu() {
            foreach (var button in menuButtons) {
                button.SetColor(inactive);
                button.SetFontSize(50);
            }
            menuButtons[selectedMenu].SetColor(active);
            menuButtons[selectedMenu].SetFontSize(70);
        }

        private void UpMenu() {
            if (selectedMenu + 1 < menuButtons.Length) {
                selectedMenu++;
            } else {
                selectedMenu = menuButtons.Length -1 ;
            }
            SetActiveMenu();
        }

        private void DownMenu() {
            if (selectedMenu - 1 > 0) {
                selectedMenu--;
            } else {
                selectedMenu = 0;
            }
            SetActiveMenu();
        }

        public void HandleKeyEvent(string keyValue, string keyAction) {
            if (keyAction == "KEY_PRESS") {
                switch (keyValue) {
                case "KEY_UP":
                    UpMenu();
                    break;

                case "KEY_DOWN":
                    DownMenu();
                    break;

                case "KEY_ENTER":
                    switch (selectedMenu) {
                    //If Main Menu is chosen we return to main menu
                    case 0: // Main menu
                        SpaceTaxiBus.GetBus().RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.GameStateEvent,
                                this,
                                "CHANGE_STATE",
                                "MAIN_MENU", "wut"));
                        break;
                    
                    case 1: // Level 1
                        SpaceTaxiBus.GetBus().RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.GameStateEvent,
                                this,
                                "CHANGE_STATE",
                                "GAME_RUNNING", "short-n-sweet"));
                        break;
                    case 2: // Level 2
                        SpaceTaxiBus.GetBus().RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.GameStateEvent,
                                this,
                                "CHANGE_STATE",
                                "GAME_RUNNING", "the-beach"));
                        break;
                    }

                    break;
                }
            }
        }

        //Not used
        public void GameLoop() { }

        public void InitializeGameState() {
            //Creating colors
            active = new Vec3F(0.0f, 1.0f, 0.0f); // Green
            inactive = new Vec3F(1.0f, 0.0f, 0.0f); // Red

            //Create background image entity, fills entire screen
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0.0f, 0.0f),
                    new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png")));

            //Creating new array and adding buttons to it.
            menuButtons = new[] {
                new Text("Back", new Vec2F(0.2f, 0.1f), new Vec2F(0.3f, 0.3f)),
                new Text("Level 1", new Vec2F(0.2f, 0.2f), new Vec2F(0.5f, 0.3f)),
                new Text("Level 2", new Vec2F(0.2f, 0.3f), new Vec2F(0.5f, 0.3f))
            };

            //Setting button vars
            selectedMenu = 0;

            //Iterating over buttons and setting their default color and size
            foreach (var button in menuButtons) {
                button.SetColor(inactive);
                button.SetFontSize(50);
            }

            //Setting color and font size of active button
            menuButtons[selectedMenu].SetColor(active);
            menuButtons[selectedMenu].SetFontSize(70);
        }

        //Not used
        public void UpdateGameLogic() { }

        //Return an instance, or creates a new one
        public static GameLevelPicker GetInstance() {
            return GameLevelPicker.instance ?? (GameLevelPicker.instance = new GameLevelPicker());
        }
    }
}
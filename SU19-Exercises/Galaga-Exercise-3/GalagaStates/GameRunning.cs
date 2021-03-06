using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.State;
using GalagaGame;
using Galaga_Exercise_3.GalagaEntities.Enemy;

namespace Galaga_Exercise_3.GalagaStates {
    public class GameRunning : IGameState {
        private static GameRunning instance;

        //Background
        private Entity backGroundImage;

        //Enemy vars
        private EntityContainer<Enemy> enemies;
        private ImageStride enemyAnimation;

        //Enemy formation vars
        private ISquadron enemyFormation;
        private List<Image> enemyStrides;

        //Function for displaying explosions
        private int explosionLength;
        private AnimationContainer explosions;
        private ImageStride explosionStride;

        //Explosion vars
        private List<Image> explosionStrides;
        
        //Formation
        private LineFormation Form_LineFormation;
        private PairsFormation Form_PairsFormation;
        private ZigZagFormation Form_ZigZag;

        //Enemy movement vars
        private NoMove Move_No;
        private ZigZagDown Move_Zig;
        private Down Move_Down;
        private Player player;

        private Score score;

        public GameRunning() { 
            InitializeGameState();
        }

        public List<PlayerShot> playerShots { get; private set; }

        public void UpdateGameLogic() {
            IterateShots();
            player.Move();
            Move_Zig.MoveEnemies(enemyFormation.Enemies);
        }

        public void HandleKeyEvent(string keyValue, string keyAction) {
            if (keyAction == "KEY_PRESS") {
                switch (keyValue) {
                case "KEY_RIGHT":
                    KeyPress(keyValue);
                    break;
                case "KEY_LEFT":
                    KeyPress(keyValue);
                    break;
                case "KEY_SPACE":
                    KeyPress(keyValue);
                    break;
                case "KEY_ESCAPE":
                    KeyPress(keyValue);
                    break;
                }
            } else if (keyAction == "KEY_RELEASE") {
                switch (keyValue) {
                case "KEY_RIGHT":
                    KeyRelease(keyValue);
                    break;
                case "KEY_LEFT":
                    KeyRelease(keyValue);
                    break;
                }
            }
        }

        public void InitializeGameState() {
            Move_No = new NoMove();
            Move_Zig = new ZigZagDown();
            Move_Down = new Down();
            explosionLength = 500;
            //Background image while playing
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0.0f, 0.0f),
                    new Vec2F(1.0f, 1.0f)),
                new Image(Path.Combine("Assets", "Images", "SpaceBackground.png")));

            //Create the player
            player = new Player(this,
                new DynamicShape(new Vec2F(0.45f, 0.1f), new Vec2F(0.1f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "Player.png")));

            //Create 4 strides of 1 image for monsters
            enemyStrides = ImageStride.CreateStrides(4,
                Path.Combine("Assets", "Images", "BlueMonster.png"));

            //Create new animation based on image list for monsters
            enemyAnimation = new ImageStride(80, enemyStrides);

            //Decisions for enemy position:
            //Choose to uncomment your choice of formation (default is ZigZag):
            enemyFormation = new ZigZagFormation(5);
            //enemyFormation = new LineFormation(7);
            //enemyFormation = new PairsFormation(7);
            enemyFormation.CreateEnemies(enemyStrides);
            enemies = new EntityContainer<Enemy>();
            enemies = enemyFormation.Enemies;

            //Create list of playershots
            playerShots = new List<PlayerShot>();

            //Animation stride for explosions
            explosionStrides = ImageStride.CreateStrides(8,
                Path.Combine("Assets", "Images", "Explosion.png"));
            explosions = new AnimationContainer(enemyFormation.MaxEnemies);
            explosionStride = new ImageStride(explosionLength / 8, explosionStrides);

            //Create score
            score = new Score(new Vec2F(0.45f, -0.12f), new Vec2F(0.2f, 0.2f));
        }

        public void GameLoop() { }

        public void RenderState() {
            backGroundImage.RenderEntity();
            //Render the player
            player.RenderEntity();

            //Render each shot in the air
            foreach (var aShot in playerShots) {
                aShot.RenderEntity();
            }

            //Render all enemies and their explosions if there is any
            enemies.RenderEntities();
            explosions.RenderAnimations();
            score.RenderScore();
        }

        public static GameRunning GetInstance() {
            return GameRunning.instance ?? (GameRunning.instance = new GameRunning());
        }

        private void IterateShots() {
            //Bliver kaldt
            //Console.WriteLine("Called IterateShots");
            foreach (var shot in playerShots) {
                shot.Shape.Move();
                if (shot.Shape.Position.Y > 1.0f) {
                    shot.DeleteEntity();
                }

                foreach (Enemy enemy in enemyFormation.Enemies) {
                    //Create dynamic shapes
                    var shotDyn = shot.Shape.AsDynamicShape();

                    //Checks if there's a collision
                    if (CollisionDetection.Aabb(shotDyn, enemy.Shape).Collision) {
                        //Delete both enemy and shot if so
                        enemy.DeleteEntity();
                        shot.DeleteEntity();

                        //Create explosion at coordinates
                        AddExplosion(enemy.Shape.Position.X, enemy.Shape.Position.Y, 0.1f, 0.1f);

                        //Adds a point to the score
                        score.AddPoint();
                        break;
                    }
                }
            }

            //If collision happened add enemies to new enemy container 
            var aliveEnemies = new List<Enemy>();
            foreach (Enemy enemy in enemyFormation.Enemies) {
                if (!enemy.IsDeleted()) {
                    aliveEnemies.Add(enemy);
                }
            }

            enemyFormation.Enemies.ClearContainer();
            foreach (var enemy in aliveEnemies) {
                if (!enemy.IsDeleted()) {
                    enemyFormation.Enemies.AddDynamicEntity(enemy);
                }
            }

            //If collision happened remove playershot from old list and create new list
            var newPlayerShots = new List<PlayerShot>();
            foreach (var aShot in playerShots) {
                if (!aShot.IsDeleted()) {
                    newPlayerShots.Add(aShot);
                }
            }

            playerShots = newPlayerShots;
        }

        private void AddExplosion(float posX, float posY,
            float extentX, float extentY) {
            explosions.AddAnimation(
                new StationaryShape(posX, posY, extentX, extentY), explosionLength,
                explosionStride);
        }

        private void KeyPress(string key) {
            switch (key) {
            case "KEY_ESCAPE":
                GalagaBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent,
                        this,
                        "CHANGE_STATE",
                        "GAME_PAUSED", ""));
                break;
            case "KEY_LEFT":
                GalagaBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "move_left", "", ""));
                break;
            case "KEY_RIGHT":
                GalagaBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "move_right", "", ""));
                break;
            case "KEY_SPACE":
                GalagaBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "shoot", "", ""));
                break;
            }
        }

        private void KeyRelease(string key) {
            switch (key) {
            case "KEY_LEFT":
                GalagaBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "stop_left", "", ""));
                break;

            case "KEY_RIGHT":
                GalagaBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "stop_right", "", ""));
                break;
            }
        }
    }
}
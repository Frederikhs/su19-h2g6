using System;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using SpaceTaxiGame;

namespace SpaceTaxi_2.Customer {
    public class Customer : IGameEventProcessor<object> {

        public string name; // Name of the costumer
        public int spawnAfter; // Number of seconds that should pass in the level, before the customer appears (is spawned).
        public char spawnPlatform; // Determining on which platform the customer should be spawned.
        public string destinationPlatform; // Destination platform of the customer
        public int taxiDuration; // Number of seconds you have to drop off the customer at the correct platform
        public int points; // Number of points a correct drop off of the customer is worth.

        public bool visible;

        public bool IsInTransit;
        public bool HasTravled;

        private Image imageStandLeft;
        private Image imageStandRight;

        private float Start;
        private float End;

        public bool OnPath;
        private Direction walkingDirection;

        private enum Direction {
            StandLeft,
            StandRight,
            WalkLeft,
            WalkRight
        }

        public Entity entity { get; private set; }
        private Shape shape;

        public Customer(string name, int spawnAfter, char spawnPlatform, string destinationPlatform,
            int taxiDuration, int points) {
            this.name = name;
            this.spawnAfter = spawnAfter;
            this.spawnPlatform = spawnPlatform;
            this.destinationPlatform = destinationPlatform;
            this.taxiDuration = taxiDuration;
            this.points = points;
            this.walkingDirection = Direction.WalkRight;

            this.visible = false;
            this.HasTravled = false;

            GenerateImage();

            shape.AsDynamicShape().ChangeDirection(new Vec2F(0.0005f, 0.0f));
        }

        private void GenerateImage() {
            imageStandLeft =
                new Image(Path.Combine("Assets", "Images", "CustomerStandLeft.png"));
            imageStandRight =
                new Image(Path.Combine("Assets", "Images", "CustomerStandRight.png"));
            
            shape = new DynamicShape(new Vec2F(), new Vec2F(0.05f,0.08f));
            
            this.entity = new Entity(shape, imageStandLeft);
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.TimedEvent, this);
        }

        private float GetPosX() {
            return this.entity.Shape.Position.X;
        }
        
        
        public void WalkCustomer() {
            if (walkingDirection == Direction.WalkRight &&
                GetPosX() >= Platform.GetWidth(this.spawnPlatform)[1]) {
                Console.WriteLine("CHanging direction");
                shape.AsDynamicShape().ChangeDirection(new Vec2F(-0.0005f, 0.0f));
                this.walkingDirection = Direction.WalkLeft;
            } else if (walkingDirection == Direction.WalkLeft &&
                       GetPosX() <= Platform.GetWidth(this.spawnPlatform)[0]) {
                shape.AsDynamicShape().ChangeDirection(new Vec2F(0.0005f, 0.0f));
                this.walkingDirection = Direction.WalkRight;
            }

//            Console.WriteLine("-----\nCustomer: "+name);
//            Console.WriteLine("WalkingDirection: "+walkingDirection);
//            Console.WriteLine("GetPos: "+GetPosX());
//            Console.WriteLine("Start: "+Platform.GetWidth(this.spawnPlatform)[0]);
//            Console.WriteLine("End: "+Platform.GetWidth(this.spawnPlatform)[1]);
//            Console.WriteLine("––––––––––––");
//                switch (walkingDirection) {
//                case Direction.WalkRight:
//                    shape.AsDynamicShape().ChangeDirection(new Vec2F(0.0005f, 0.0f));
//                    walkingDirection = Direction.WalkLeft;
////                    Console.WriteLine("moving left");
//                    break;
//                case Direction.WalkLeft:
//                    shape.AsDynamicShape().ChangeDirection(new Vec2F(-0.0005f, 0.0f));
//                    walkingDirection = Direction.WalkRight;
////                    Console.WriteLine("moving right");
//                    break;
//                }
                
//            shape.Move();
        }
            
            
        
        
        public void Hide() {
            this.visible = false;
            entity.Shape.Extent = new Vec2F(0f, 0f);
        }

        public void Show() {
            this.visible = true;
            entity.Shape.Extent = new Vec2F(0.05f, 0.08f);
        }

        public void SetPos(Vec2F pos) {
            shape.SetPosition(pos);
        }
        
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            // TODO: Fill
        }
        
        public void RenderCustomer() {
            entity.RenderEntity();
        }
    }
}
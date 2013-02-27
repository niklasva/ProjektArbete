﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Library;

namespace Library
{
    /// <summary>
    /// Spelar karaktären
    /// </summary>
    public class Player
    {
        //Texturer och animation av spelaren
        public Vector2 position = new Vector2(36, 39);
        //De olika texturerna
        //private Texture2D vanligTexture;
        //private Texture2D militarTexture;
        //private Texture2D spionTexture;
        //private Texture2D kvinnaTexture;
        //private Texture2D jkeaTexture;
        //De olika spritarna
        private AnimatedSprite vanligSprite;
        private AnimatedSprite militarSprite;
        private AnimatedSprite spionSprite;
        private AnimatedSprite kvinnaSprite;
        private AnimatedSprite jkeaSprite;
        //De spriten som används vid utritning
        private AnimatedSprite currentSprite;
        int yBounds = 5;

        private Point leftCurrentFrame = new Point(0, 0);
        private Point rightCurrentFrame = new Point(0, 1);
        private Point downCurrentFrame = new Point(0, 2);
        private Point upCurrentFrame = new Point(0, 3);
        private double deltaX;
        private double deltaY;
        private bool isMoving;
        //Variabler för att bestämma vilka kläder spelaren ska ha
        enum WhichClothes { vanliga, militar, kvinna, spion, jkea };
        private WhichClothes whichClothes = WhichClothes.vanliga;
        //Lagerposition
        private float layerPosition = 0;

        //Skala spriten
        float scale;

        //Inventory
        Inventory inventory;
        
        //Kontroller
        private MouseState mouseState;
        private MouseState oldState;
        private Vector2 mousePosition;
        private Vector2 target = new Vector2(36, 39);
        private Vector2 direction;
        private int speed = 1;

        //Konstruktor
        public Player(Game game, Texture2D vanligTexture, Texture2D militarTexture, Texture2D kvinnaTexture, Texture2D spionTexture, Texture2D jkeaTexture, Texture2D invBackGround, Rectangle clientBounds)
        {
            this.vanligSprite = new AnimatedSprite(vanligTexture, position, 0, new Point(34, 68), new Point(0, 0), new Point(4, 5), 100);
            this.militarSprite = new AnimatedSprite(militarTexture, position, 0, new Point(34, 68), new Point(0, 0), new Point(4, 5), 100);
            this.kvinnaSprite = new AnimatedSprite(kvinnaTexture, position, 0, new Point(34, 68), new Point(0, 0), new Point(4, 5), 100);
            this.spionSprite = new AnimatedSprite(spionTexture, position, 0, new Point(34, 68), new Point(0, 0), new Point(4, 5), 100);
            this.jkeaSprite = new AnimatedSprite(jkeaTexture, position, 0, new Point(34, 68), new Point(0, 0), new Point(4, 5), 100);
            this.inventory = new Inventory(invBackGround, clientBounds, game);
            currentSprite = vanligSprite;
            this.scale = 1f;
        }

        public void Update(Game game, GameTime gameTime, Rectangle clientBounds)
        {
            if (whichClothes == WhichClothes.vanliga)
                currentSprite = vanligSprite;   
            else if (whichClothes == WhichClothes.militar)
                currentSprite = militarSprite;
            else if (whichClothes == WhichClothes.spion)
                currentSprite = spionSprite;
            else if (whichClothes == WhichClothes.jkea)
                currentSprite = jkeaSprite;
            else if (whichClothes == WhichClothes.kvinna)
                currentSprite = kvinnaSprite;

            //scaleToPosition(clientBounds);
            //Är inventoryn öppen ska spelaren inte röra på sig
            if (Registry.changingRoom)
            {
                position = Registry.nextRoomDoorPosition;
                target = position;
                Registry.changingRoom = false;

            }
            if (!inventory.InventoryInUse || Registry.changingRoom)
            {
                //Rör spelaren på sig?
                isMoving = true;
                //styrning av spelare
                mouseState = Mouse.GetState();

                //mousePosition = mouseState/3 för att mouseState inte har något med upplösningen (som tredubblas) att göra
                mousePosition.X = (mouseState.X / 3);
                mousePosition.Y = (mouseState.Y / 3);

                if (mouseState.LeftButton == ButtonState.Pressed && oldState.LeftButton != ButtonState.Pressed)
                {
                    target.X = mousePosition.X - 15;
                    target.Y = mousePosition.Y - 67;
                }
                direction = target - position;
                direction.Normalize();
                if (position.X != target.X && position.Y != target.Y)
                {
                    position.X += direction.X * speed;
                    position.Y += direction.Y * speed;
                }
                if (position.X < target.X + 1 && position.X > target.X - 1 && position.Y < target.Y + 1 && position.Y > target.Y - 1)
                {
                    speed = 0;
                    //Spelaren rör inte på sig
                    isMoving = false;
                }
                else speed = 2;

                oldState = mouseState;
            }
            //Rör spelaren på sig ska den animeras som vanilgt
            if (isMoving)
            {
                //Skillnad i X-led och skillnad i Y-led
                if (position.Y <= target.Y)
                    deltaY = target.Y - position.Y;
                else
                    deltaY = position.Y - target.Y;

                if (position.X < target.X)
                    deltaX = target.X - position.X;
                else
                    deltaX = position.X - target.X;

                //Bestämmning av vilken animation som ska användas av spelaren
                //Är man påväg åt höger, vänster, up eller ner?
                //Går man mest vertikalt eller horisontellt?
                if (position.X <= target.X && deltaX > deltaY)
                {
                    yBounds = 1;
                    leftCurrentFrame = new Point(0, 0);
                    downCurrentFrame = new Point(0, 2);
                    upCurrentFrame = new Point(0, 3);
                }
                else if (position.X >= target.X && deltaX > deltaY)
                {
                    yBounds = 0;
                    rightCurrentFrame = new Point(0, 1);
                    downCurrentFrame = new Point(0, 2);
                    upCurrentFrame = new Point(0, 3);
                }
                else if (position.Y >= target.Y && deltaX < deltaY)
                {
                    yBounds = 3;
                    leftCurrentFrame = new Point(0, 0);
                    downCurrentFrame = new Point(0, 2);
                    rightCurrentFrame = new Point(0, 1);
                }
                else
                {
                    yBounds = 2;
                    leftCurrentFrame = new Point(0, 0);
                    rightCurrentFrame = new Point(0, 1);
                    upCurrentFrame = new Point(0, 3);
                }
            }
            //Rör spelaren inte på sig så ska han ha en stillastående sprite
            else
            {
                yBounds = 4;
                leftCurrentFrame = new Point(0, 0);
                rightCurrentFrame = new Point(0, 1);
                downCurrentFrame = new Point(0, 2);
                upCurrentFrame = new Point(0, 3);
            }

            if (inventory.InteractingWithItem && !inventory.InventoryInUse)
            {
                if (Mousecontrol.clicked() && Registry.currentRoom.giveNPCItem(inventory.getItemClickedon))
                {
                    inventory.removeItem(inventory.getItemClickedon);
                }
                else if (Mousecontrol.clicked())
                    addItem(inventory.getItemClickedon);
            }

            inventory.Update();
            currentSprite.Position = position;
            Registry.playerPosition = position;
            updateLayerDepth();
            if(!inventory.InventoryInUse)
                 currentSprite.Update(gameTime, clientBounds, yBounds);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            currentSprite.Draw(gameTime, spriteBatch, scale, layerPosition);
            inventory.Draw(gameTime, spriteBatch);
        }

        public void addItem(Item item)
        {
            inventory.addItem(item);
        }
        public void removeItem(Item item)
        {
            inventory.removeItem(item);
        }

        /// <summary>
        /// Stannar spelaren
        /// </summary>
        /// <param name="move">Var närmsta vägg är</param>
        public void Stop(Vector2 move)
        {
            // Spelaren stannas....typ. Spelaren flyttas ett steg ifrån det som blockerar vägen så att hen inte fastnar
            position = position - move;

            // för att gånganimationen inte ska fortsätta efter spelaren stoppas
            target = position;
        }

        private void updateLayerDepth()
        {
            layerPosition = (1 - (position.Y + 68) / 180) / 3;
        }

        //private void scaleToPosition(Rectangle clientBounds)
        //{
        //    //Skalan blir 1 - skilladen mellan rutans storlek och positionen på karaktären.
        //    //Gör att när man är närmast "kameran" blir karaktären som störst och när man rör sig därifrån blir karaktären mindre.
        //    float temp = (clientBounds.Height / 3) - position.Y * 2f;
        //    double doubleScale = 1 - (temp * 0.001);
        //    scale = float.Parse(doubleScale.ToString());
        //}

        public bool IsMoving
        {
            get
            {
                return isMoving;
            }
        }

        public bool InventoryInUse
        {
            get
            {
                return inventory.InventoryInUse;
            }
        }
    }
}

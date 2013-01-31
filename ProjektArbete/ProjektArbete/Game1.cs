﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Library;

namespace ProjektArbete
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        Item[] items;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 320 * 3;
            graphics.PreferredBackBufferHeight = 180 * 3;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            items = Content.Load<Library.Item[]>(@"Data/ItemXML");
            foreach (Item item in items)
            {
                item.Initialize(Content.Load<Texture2D>(@item.TextureString));
            }
            Registry.items = items;

            player = new Player(this, Content.Load<Texture2D>(@"Images/AnimatedSprites/leftTexture"), Content.Load<Texture2D>(@"Images/AnimatedSprites/rightTexture"), 
                Content.Load<Texture2D>(@"Images/AnimatedSprites/downTexture"), Content.Load<Texture2D>(@"Images/AnimatedSprites/upTexture"), 
                Content.Load<Texture2D>(@"Images/AnimatedSprites/stillTexture"), Content.Load<Texture2D>(@"Images/Sprites/invBackground"), Window.ClientBounds);

            Registry.npcs = Content.Load<Library.NPC[]>(@"Data/npcs");
            Registry.dialogs = Content.Load<Library.Dialog[]>(@"Data/dialogs");
            Registry.rooms = Content.Load<Library.Room[]>(@"Data/rooms");
            Registry.currentRoom = Registry.rooms[0];
            Registry.currentRoom.LoadContent(this);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Mousecontrol.update();
            player.Update(this, gameTime, Window.ClientBounds);
            Registry.currentRoom.Update(gameTime, Window.ClientBounds);
            
            //Muskontroll
            if (!Registry.playerIsMoving && !Registry.inventoryInUse && Registry.currentRoom.isItemClickedInRoom())
            {
                Sprite item = Registry.currentRoom.getClickedItem().getSprite();
                if (Mousecontrol.inProximityToItem(item.Position, item.FrameSize))
                {
                    player.addItem(Registry.currentRoom.getClickedItem());
                    Registry.currentRoom.removeItem();
                    Registry.currentRoom.itemWasClicked();
                }
            }

            // Kollar om spelaren rör sig utanför spelområdet
            if (IntersectMask(Registry.currentRoom.getMask()) != Vector2.Zero)
            {
                // Rättar till spelarens position så att man inte går utanför spelområdet (det icke-transparenta i masken)
                player.Stop(IntersectMask(Registry.currentRoom.getMask()));
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AntiqueWhite);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.CreateScale(3f));
            Registry.currentRoom.Draw(gameTime, spriteBatch);
            player.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Kolla om spelaren är där hen inte borde vara
        /// </summary>
        /// <param name="data">2D-färgarray</param>
        /// <returns>Var "väggen" är jämfört med spelaren</returns>
        static Vector2 IntersectMask(Color[,] data)
        {
            Vector2 where = Vector2.Zero;
            // Konverterar spelarpositionen (Vector2/float) till int för att kunna använda den som arrayposition
            int x = (int)Math.Round(Registry.playerPosition.X);
            int y = (int)Math.Round(Registry.playerPosition.Y + 39);
            if (x < 1) x = 1;
            if (x > 298) x = 298;
            if (y < 1) y = 1;

            // Letar efter genomsynlighet i närheten av spelaren
            if (data[x - 1, y].A == 0)
            {
                where.X = -1;
            } 
            if (data[x + 21, y].A == 0)
            {
                where.X = 1;
            }
            if (data[x, y + 1].A == 0)
            {
                where.Y = 1;
            }
            if (data[x, y - 1].A == 0)
            {
                where.Y = -1;
            }

            // Returnerar var "väggen" är jämfört med spelaren
            return where;
        }




    }
}
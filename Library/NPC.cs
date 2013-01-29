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


namespace Library
{
    /// <summary>
    ///  Icke-spelarkaraktärer.
    /// </summary>
    public class NPC
    {
        public String name;
        public Vector2 position;
        private Dialog dialog;
        public int dialogID;
        private Texture2D stillTexture;
        private Texture2D talkTexture;
        private AnimatedSprite stillSprite;
        private AnimatedSprite talkSprite;
        private AnimatedSprite activeSprite;
        private Boolean isTalking = true;

        public void loadContent(Game game)
        {
            this.talkTexture = game.Content.Load<Texture2D>(@"Images/Characters/" + name + "talk");
            this.stillTexture = game.Content.Load<Texture2D>(@"Images/Characters/" + name);
            //stillSprite = new AnimatedSprite(stillTexture, position, 10, new Point(20, 40), new Point(0, 0), new Point(3, 1), 200);
            stillSprite = new AnimatedSprite(stillTexture, position, 0, new Point(stillTexture.Width / 3, stillTexture.Height), new Point(0, 0), new Point(3, 1), 200);
            talkSprite = new AnimatedSprite(talkTexture, position, 0, new Point(talkTexture.Width / 3, talkTexture.Height), new Point(0, 0), new Point(3, 1), 200);
            dialog = Registry.dialogs[dialogID];
            dialog.setFont(game.Content.Load<SpriteFont>(@"textfont"));
            this.activeSprite = stillSprite;
        }
        public void Update(GameTime gameTime, Rectangle clientBounds)
        {
            if (dialog.getActiveLine() == "0")
            {
                isTalking = false;
            }

            if (dialog.getSpeaker() == "NPC")
            {
                activeSprite = talkSprite;
            }
            else
            {
                activeSprite = stillSprite;
            }
            activeSprite.Update(gameTime, clientBounds);
        }

        public void Talk()
        {
            isTalking = true;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 playerPosition)
        {
            activeSprite.Draw(gameTime, spriteBatch, 1f, 0);
            if (isTalking == true)
            {
                dialog.Speak(gameTime, spriteBatch, position);
            }
        }
    }

}

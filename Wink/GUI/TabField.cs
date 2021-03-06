﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class TabField : GameObjectList
    {
        private class TabButton : Button
        {
            Color bgColor;
            Point size;
            public Tab tab;

            public TabButton(Point size, Tab tab, SpriteFont titleFont, Color textColor, Color bgColor, int layer = 0, string id = "", int sheetIndex = 0, float scale = 1) : base("", tab.Title, titleFont, textColor)
            {
                this.size = size;
                this.tab = tab;
                this.bgColor = bgColor;
            }

            public override int Width
            {
                get { return size.X; }
            }
            public override int Height
            {
                get { return size.Y; }
            }

            public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
            {
                Texture2D white = GameEnvironment.AssetManager.GetSingleColorPixel(tab.Active ? bgColor : Color.LightGray);
                spriteBatch.Draw(white, new Rectangle(Position.ToPoint(), size), Color.White);
                base.Draw(gameTime, spriteBatch, camera);
            }
        }

        public class Tab
        {
            public Tab(string title, GameObjectList content)
            {
                this.Title = title;
                this.Content = content;
            }

            public bool Active { get; set; } 
            public string Title { get; private set; }
            public GameObjectList Content { get; private set; }
        }

        private List<Tab> tabs;
        private int width;
        private int height;

        private Color titleColor;
        private Color bgColor;
        private SpriteFont titleFont;

        public TabField(List<Tab> tabs, Color titleColor, Color bgColor, SpriteFont titleFont, int width, int height)
        {
            GameObjectList tabTitles = new GameObjectList(0, "TabTitles");
            Add(tabTitles);

            GameObjectList content = new GameObjectList(0, "Content");
            Add(content);

            this.titleColor = titleColor;
            this.bgColor = bgColor;
            this.titleFont = titleFont;
            this.width = width;
            this.height = height;
            this.tabs = tabs;
            foreach (Tab t in tabs)
            {
                AddTab(t);
            }

            ((this.Find("TabTitles") as GameObjectList).Children[0] as TabButton).Action.Invoke();
        }

        private void AddTab(Tab tab)
        {
            int tabTitleWidth = width / tabs.Count - 4;
            int index = tabs.FindIndex(t => t.Equals(tab));

            TabButton b = new TabButton(new Point(width / tabs.Count - 4, 50), tab, titleFont, titleColor, bgColor);
            b.Action = () =>
            {
                Tab prevActive = tabs.Find(t => t.Active);
                if (prevActive != null)
                    prevActive.Active = false;

                b.tab.Active = true;

                GameObjectList content = this.Find("Content") as GameObjectList;
                Remove(content);

                GameObjectList newContent = new GameObjectList(0, "Content");
                newContent.Add(b.tab.Content);
                newContent.Position = new Vector2(0, 50);
                Add(newContent);
            };
            b.Position = new Vector2(width / tabs.Count * index + 2, 2);

            (this.Find("TabTitles") as GameObjectList).Add(b);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            Texture2D bgTex = GameEnvironment.AssetManager.GetSingleColorPixel(bgColor);
            Point screen = GameEnvironment.Screen;
            spriteBatch.Draw(bgTex, null, new Rectangle(0, 52, width, height - 54));

            base.Draw(gameTime, spriteBatch, camera);
        }
    }
}

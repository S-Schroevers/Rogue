﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class MultiplayerMenu : GameObjectList
    {
        Button back;

        private class OnlineTab : GameObjectList
        {   
            public OnlineTab()
            {
                Point screen = GameEnvironment.Screen;
                Table availableHosts = new Table();
                availableHosts.AddColumn();
                availableHosts.AddColumn();

                availableHosts.AddRow(new List<object>() { "test", "192.168.153.1" });
                availableHosts.AddRow(new List<object>() { "test2", "192.168.153.2" });

                availableHosts.Position = new Vector2((screen.X - availableHosts.Width)/2, 50);
                Add(availableHosts);
            }
        }

        private class LANTab : GameObjectList
        {
            Button connectButton;

            public LANTab()
            {
                Point screen = GameEnvironment.Screen;
                SpriteFont buttonFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");

                //Create a text field for the ip address
                TextField ipAddress = new TextField(buttonFont, Color.Red);
                ipAddress.Position = new Vector2((screen.X - ipAddress.Width) / 2, screen.Y / 2);
                ipAddress.Editable = true;
                Add(ipAddress);

                //Create a button to start connecting.
                connectButton = new Button("button", "Connect", buttonFont, Color.Black);
                connectButton.Position = new Vector2((screen.X - connectButton.Width) / 2, screen.Y - 300);
                Add(connectButton);
            }

            public override void HandleInput(InputHelper inputHelper)
            {
                base.HandleInput(inputHelper);

                if (connectButton.Pressed)
                {
                    //Connect here...
                }
            }
        }

        public MultiplayerMenu()
        {
            SpriteFont tabTitleFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");
            
            //Define two tabs
            TabField.Tab onlineTab = new TabField.Tab("Online", new OnlineTab());
            TabField.Tab lanTab = new TabField.Tab("LAN", new LANTab());

            Point screen = GameEnvironment.Screen;
            //Make a TabField and add the two tabs.
            TabField tabField = new TabField(new List<TabField.Tab> { lanTab, onlineTab }, Color.Black, tabTitleFont, screen.X, screen.Y - 200);
            Add(tabField);

            //Create a button to go back to the main menu.
            back = new Button("button", "Back", tabTitleFont, Color.Black);
            back.Position = new Vector2(100, screen.Y - 100);
            Add(back);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (back.Pressed)
            {
                GameEnvironment.GameStateManager.SwitchTo("mainMenuState");
            }
        }
    }
}
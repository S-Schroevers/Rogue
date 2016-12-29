﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class NextLevelEvent : Event
    {
        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            Level level = new Level(server.LevelIndex + 1);
            List<GameObject> playerlist = server.Level.FindAll(obj => obj is Player);
            foreach (GameObject obj in playerlist)
            {
                level.Add(obj);
                (obj as Player).InitPosition();
            }
            server.Level = level;
            server.LevelChanged();
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
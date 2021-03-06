﻿using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class PlayerMoveEvent : ActionEvent
    {
        private Tile tile;

        public PlayerMoveEvent(Player player, Tile tile) : base(player)
        {
            this.tile = tile;
        }

        #region Serialization
        public PlayerMoveEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            tile = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("tileGUID"))) as Tile;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This event can only be sent from client to server, therefore ID based serialization is used.
            info.AddValue("tileGUID", tile.GUID.ToString());
            base.GetObjectData(info, context);
        }
        #endregion

        public override bool GUIDSerialization
        {
            get { return true; }
        }

        protected override int Cost
        {
            get
            {
                int mc = Living.BaseActionCost;
                if ((player.EquipmentSlots.Find("bodySlot") as EquipmentSlot).SlotItem != null)
                {
                    mc = (int)(mc * ((player.EquipmentSlots.Find("bodySlot") as EquipmentSlot).SlotItem as BodyEquipment).WalkCostMod);
                }

                return mc;
            }
        }

        protected override void DoAction(LocalServer server)
        {
            server.ChangedObjects.Add(player.Tile);
            server.ChangedObjects.Add(tile);
            player.MoveTo(tile);
            player.ComputeVisibility();
        }

        protected override bool ValidateAction(Level level)
        {
            if (player.Tile == null)
                return false;
            int dx = (int)Math.Abs(player.Tile.Position.X - tile.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - tile.Position.Y);

            bool theSame = dx == 0 && dy == 0;
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            return withinReach && !theSame;
        }
    }
}

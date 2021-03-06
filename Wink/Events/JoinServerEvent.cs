﻿using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class JoinServerEvent : Event
    {
        private string clientName;

        public JoinServerEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            clientName = info.GetString("clientName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("clientName", clientName);
            base.GetObjectData(info, context);
        }

        public JoinServerEvent(string clientName)
        {
            this.clientName = clientName;
        }

        public override bool GUIDSerialization
        {
            get { return false; }
        }

        public override bool OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override bool OnServerReceive(LocalServer server)
        {
            (Sender as RemoteClient).ClientName = clientName;
            return true;
        }

        public override bool Validate(Level level)
        {
            //TODO: Implement Validation.
            return true;
        }
    }
}

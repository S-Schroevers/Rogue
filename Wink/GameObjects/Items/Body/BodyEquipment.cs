﻿using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    [Serializable]
    public class BodyEquipment : Equipment
    {
        int physicalValue;
        int magicValue;
        int reqPenalty;

        public BodyEquipment(string assetName, string id, int physicalValue=0, int magicValue=0,int reqPenalty=0, int strRequirement = 0, int dexRequirement = 0, int intRequirement = 0, int layer = 0, int stackSize = 1) : base(assetName, id, layer, stackSize, strRequirement, dexRequirement, intRequirement)
        {
            this.physicalValue = physicalValue;
            this.magicValue = magicValue;
            this.reqPenalty = reqPenalty;
        }

        public BodyEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            physicalValue = info.GetInt32("physicalValue");
            magicValue = info.GetInt32("magicValue");
            reqPenalty = info.GetInt32("reqPenalty");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("physicalValue", physicalValue);
            info.AddValue("magicValue", magicValue);
            info.AddValue("reqPenalty", reqPenalty);
        }

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            TextGameObject armorinfo = new TextGameObject("Arial12", 0, 0, "ArmorValueTypeInfo." + this);
            armorinfo.Text = "ArmorValue: " + physicalValue + " physical, " + magicValue + " magic";
            armorinfo.Color = Color.Red;
            armorinfo.Parent = infoList;
            infoList.Children.Insert(1, armorinfo);
        }

        public  int Value(Living l,DamageType damageType)
        {
            int value;
            switch (damageType)
            {
                case DamageType.Physical:
                    if (MeetsRequirements(l))
                    {
                        value = physicalValue;
                        return value;
                    }
                    else
                    {
                        // now it's a armorvalue reduction as a penalty, it might do something compleetly different
                        value = (int)l.CalculateValue(physicalValue, penaltyDif(l), reqPenalty);
                        return value;
                    }
                case DamageType.Magic:
                    if (MeetsRequirements(l))
                    {
                        value = magicValue;
                        return value;
                    }
                    else
                    {                        
                        value = (int)l.CalculateValue(magicValue, penaltyDif(l), reqPenalty);
                        return value;
                    }
                default:
                    throw new Exception("invalide damageType");
            }
        }

        private int penaltyDif(Living l)
        {
            int dif = 0;
            if (l.Strength < strRequirement)
            {
                dif += strRequirement - l.Strength;
            }
            if (l.Dexterity < dexRequirement)
            {
                dif += dexRequirement - l.Dexterity;
            }
            if (l.Intelligence < intRequirement)
            {
                dif += intRequirement - l.Intelligence;
            }
            return dif;
        }
        /*
        #region Serialization
        public BodyEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            armorValue = info.GetInt32("armorValue");
            strRequirement = info.GetInt32("strRequirement");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("armorValue", armorValue);
            info.AddValue("strRequirement", strRequirement);
        }
        #endregion
        */
    }
}
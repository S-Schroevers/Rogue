﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    public enum RingType
    {
        vitality,
        strength,
        dexterity,
        luck,
        intelligence,
        wisdom,
        // Make sure types of magic effects or other types that shouldn't be randomly put on rings are at the end of the enum
        reflection
    }

    class RingEquipment : Equipment
    {
        // mulchance is the chance a ring effect will be a multiplier, I'm keeping this relatively high because
        // a multiplier is much easier to balance than a set value
        protected double mulchance = 0.7;
        // acceptablePower is the maximum power for a single stat boost on a ring, can be changed for balancing
        // This should eventually be a function based on the floor the ring was found on
        protected int acceptablePower = 500;

        // This dictionary will hold all of the multipliers for balancing rings for the different stats
        // for now these will all be set to 1
        protected Dictionary<RingType, double> balanceMultiplier = new Dictionary<RingType, double>(); 

        private void setBalance()
        {
            balanceMultiplier.Add(RingType.strength, 0.8);
            balanceMultiplier.Add(RingType.vitality, 1);
            balanceMultiplier.Add(RingType.dexterity, 0.8);
            balanceMultiplier.Add(RingType.luck, 1);
            balanceMultiplier.Add(RingType.intelligence, 1.2);
            balanceMultiplier.Add(RingType.wisdom, 1);
        }

        protected List<RingEffect> ringEffects = new List<RingEffect>();

        // the amount of magic effects in the RingType enum, these will not be put on randomly generated rings
        protected int magicEffects = 1;

        public RingEquipment(double ringValue, RingType ringType, string assetName, bool multiplier = false, int stackSize = 1, int layer = 0, string id = "") : base(assetName, id, stackSize, layer)
        {
            AddEffect(ringType, ringValue, multiplier);
        }

        public RingEquipment(string assetName, int maxEffects = 3, int stackSize = 1, int layer = 0, string id = "", bool reflectEffect = false) : base(assetName, id, stackSize, layer)
        {
            setBalance();
            GenerateRing(maxEffects + 1);
            if (reflectEffect) AddReflection();
        }

        public RingEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // IK SNAP SERIALIZATION NIET!
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // I REPEAT, ME NO UNDERSTANDO
        }

        protected void AddEffect(RingType effectType, double effectValue, bool multiplier)
        {
            RingEffect effect = new RingEffect(effectType, multiplier, effectValue);
            ringEffects.Add(effect);
        }

        protected void AddReflection()
        {
            // This method adds a special effect to a ring, this being the "Reflection" effect.
            // It's the first attempt of magic in the game
            ReflectionEffect reflectionEffect = new ReflectionEffect(2.5, 1.5);
            ringEffects.Add(reflectionEffect);
        }

        protected void GenerateRing(int maxEffects = 3)
        {
            for(int i = 0; i < GameEnvironment.Random.Next(0, maxEffects); i++)
            {
                GenerateEffect();
            }
        }

        protected void GenerateEffect()
        {
            Array values = RingType.GetValues(typeof(RingType));
            RingType effectType = (RingType)values.GetValue(GameEnvironment.Random.Next(values.Length - magicEffects));

            bool multiplier = true;
            if (GameEnvironment.Random.NextDouble() < mulchance) multiplier = false;

            Dictionary<string, object> ret = new Dictionary<string, object>();

            double effectValue = 0;

            if (multiplier)
            {
                effectValue = 1 + Math.Round(GameEnvironment.Random.NextDouble(), 2);
            }
            else
            {
                effectValue = Math.Round(acceptablePower * GameEnvironment.Random.NextDouble());
            }
            effectValue *= balanceMultiplier[effectType];
            AddEffect(effectType, effectValue, multiplier);
        }

        public List<RingEffect> RingEffects
        {
            get { return ringEffects; }
        }

        // Change in code broke this part

        /*public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            if (Multiplier)
            {
                TextGameObject ringInfo = new TextGameObject("Arial12", 0, 0, "RingInfo." + this);
                ringInfo.Text = "Multiplies " + RingType + " by " + RingValue;
                ringInfo.Color = Color.Red;
                ringInfo.Parent = infoList;
                infoList.Children.Insert(1, ringInfo);
            }
            else
            {
                TextGameObject ringInfo = new TextGameObject("Arial12", 0, 0, "RingInfo." + this);
                ringInfo.Text = "Adds " + RingValue + " to " + RingType;
                ringInfo.Color = Color.Red;
                ringInfo.Parent = infoList;
                infoList.Children.Insert(1, ringInfo);
            }
        }*/
    }

    class RingEffect
    {
        protected RingType effectType;
        protected bool multiplier;
        protected double effectValue;
        public RingEffect(RingType effectType, bool multiplier, double effectValue)
        {
            this.effectType = effectType;
            this.multiplier = multiplier;
            this.effectValue = effectValue;
        }

        public RingType EffectType { get { return effectType; } }
        public bool Multiplier { get { return multiplier; } }
        public double EffectValue { get { return effectValue; } }
    }

    class ReflectionEffect : RingEffect
    {
        protected double power;
        protected double chance;
        public ReflectionEffect(double power, double chance) : base(RingType.reflection, false, -1)
        {
            this.power = power;
            this.chance = chance;
        }

        public double Power { get { return power; } }
        public double Chance { get { return chance; } }
        public double this[string val]
        {
            get
            {
                if (val == "power")
                {
                    return Power;
                }
                else if (val == "chance")
                {
                    return Chance;
                }
                else
                {
                    throw new KeyNotFoundException("The value " + val + " is not in ReflectionEffect");
                }
            }
        }
    }
}

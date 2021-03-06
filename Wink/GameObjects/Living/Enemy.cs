﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    public enum EnemyType {warrior,archer,mage,random}
    [Serializable]

    public class Enemy : Living, IGUIGameObject
    {
        private Bar<Enemy> hpBar;
        string enemySprite;
        
        /// <summary>
        /// Return True if health > 0 meaning the enemy is blocking the tile it's standing on
        /// </summary>
        public override bool BlocksTile
        {
            get { return Health > 0; }
        }

        /// <summary>
        /// Create a new Enemy object
        /// </summary>
        /// <param name="layer">The layer for drawing the object</param>
        /// <param name="floorNumber">The floor the Enemy is placed on</param>
        /// <param name="type">The enemy type</param>
        /// <param name="id">The (unique) object ID</param>
        /// <param name="FOVlength">The view distance for the Enemy</param>
        /// <param name="scale">The scale (multiplier) for the sprite size</param>
        public Enemy(int layer, int floorNumber, EnemyType type = EnemyType.random, string id = "Enemy", float FOVlength = 8.5f) : base(layer, id, FOVlength)
        {
            if(floorNumber < 1)
            {
                floorNumber = 1;
            }
            SetupType(type, floorNumber);
            InitAnimation(enemySprite);
        }

        /// <summary>
        /// Set up the enemy
        /// </summary>
        /// <param name="etype">The Enemy type</param>
        /// <param name="floorNumber">The floor the enemy is on</param>
        private void SetupType(EnemyType etype, int floorNumber)
        {
            if (etype == EnemyType.random)
            {
                //select random armorType
                Array eTypeValues = Enum.GetValues(typeof(EnemyType));
                etype = (EnemyType)eTypeValues.GetValue(GameEnvironment.Random.Next(eTypeValues.Length - 1));
            }
            id += " : " + etype.ToString();
            int eLvl = GameEnvironment.Random.Next(1,floorNumber);
            int weaponChance = 15 * floorNumber; // higher chance the deeper you go
            int armorChance = 15  *floorNumber;  //

            switch (etype)
            {
                case EnemyType.warrior:
                    if(weaponChance < GameEnvironment.Random.Next(100))
                    {
                        EquipmentSlot weaponslot = EquipmentSlots.Find("weaponSlot") as EquipmentSlot;
                        weaponslot.ChangeItem(new WeaponEquipment(floorNumber,WeaponType.melee));
                    }
                    if (armorChance < GameEnvironment.Random.Next(100))
                    {
                        EquipmentSlot bodyslot = EquipmentSlots.Find("bodySlot") as EquipmentSlot;
                       // bodyslot.ChangeItem(new BodyEquipment(floorNumber, 2, ArmorType.normal));
                    }
                    SetStats(eLvl, 3 + (eLvl), 3 + (eLvl), 2 + (eLvl / 2), 1 + (eLvl / 2), 1 + (eLvl / 2), 2 + (eLvl / 2), 20 + eLvl * 3, 2, 1);
                    enemySprite = "empty:65:65:12:Brown";
                    break;

                case EnemyType.archer:
                    if (weaponChance < GameEnvironment.Random.Next(100))
                    {
                        EquipmentSlot weaponslot = EquipmentSlots.Find("weaponSlot") as EquipmentSlot;
                        weaponslot.ChangeItem(new WeaponEquipment(floorNumber, WeaponType.bow));
                    }
                    if (armorChance < GameEnvironment.Random.Next(100))
                    {
                        EquipmentSlot bodyslot = EquipmentSlots.Find("bodySlot") as EquipmentSlot;
                        //bodyslot.ChangeItem(new BodyEquipment(floorNumber, 2, ArmorType.normal));
                    }
                    SetStats(eLvl, 2 + (eLvl/2), 1 + (eLvl/2), 3 + (eLvl), 1 + (eLvl / 2), 1 + (eLvl / 2), 3 + (eLvl), 20 + eLvl * 3, 2, 1);
                    enemySprite = "empty:65:65:12:Yellow";
                    break;
                case EnemyType.mage:
                    if (weaponChance < GameEnvironment.Random.Next(100))
                    {
                        EquipmentSlot weaponslot = EquipmentSlots.Find("weaponSlot") as EquipmentSlot;
                        weaponslot.ChangeItem(new WeaponEquipment(floorNumber, WeaponType.staff));
                    }
                    if (armorChance < GameEnvironment.Random.Next(100))
                    {
                        EquipmentSlot bodyslot = EquipmentSlots.Find("bodySlot") as EquipmentSlot;
                        //bodyslot.ChangeItem(new BodyEquipment(floorNumber, 2, ArmorType.robes));
                    }
                    SetStats(eLvl, 1 + (eLvl/2), 1 + (eLvl/2), 1 + (eLvl / 2), 3 + (eLvl), 3 + (eLvl), 1 + (eLvl / 2), 20 + eLvl * 3, 2, 2);
                    enemySprite = "empty:65:65:12:CornflowerBlue";
                    break;
                default:
                    throw new Exception("invalid enemy type");
            }
        }


        #region Serialization
        public Enemy(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            enemySprite = info.GetString("enemySprite");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("enemySprite", enemySprite);
            base.GetObjectData(info, context);
        }
        #endregion

        protected override void InitAnimation(string idleColor)
        {
            base.InitAnimation(idleColor);
            PlayAnimation("idle");
        }

        protected override void Death()
        {
            // call recive exp for every player
            base.Death();
        }

        protected override void DoBehaviour(List<GameObject> changedObjects)
        {
            GoTo(changedObjects, GameWorld.Find(Player.LocalPlayerName) as Player);
        }

        /// <summary>
        /// Pathfind towards a given player
        /// </summary>
        /// <param name="player">The player to target with Pathfinding</param>
        public void GoTo(List<GameObject> changedObjects, Player player)
        {
            TileField tf = GameWorld.Find("TileField") as TileField;

            if (player.Tile.SeenBy.ContainsKey(this))
            {
                bool ableToHit = AttackEvent.AbleToHit(this, player);
                if (ableToHit)
                {
                    Attack(player);

                    int cost = BaseActionCost;
                    if((EquipmentSlots.Find("bodySlot") as EquipmentSlot).SlotItem != null)
                    {
                        cost =(int)(cost * ((EquipmentSlots.Find("bodySlot") as EquipmentSlot).SlotItem as BodyEquipment).WalkCostMod);
                    }
                    actionPoints -= cost;
                    changedObjects.Add(player);
                }
                else
                {
                    PathFinder pf = new PathFinder(tf);
                    List<Tile> path = pf.ShortestPath(Tile, player.Tile);
                    // TODO?:(assuming there are tiles that cannot be walked over but can be fired over)
                    // check if there is a path to a spot that can hit the player (move closer water to fire over it)
                    if (path.Count > 0)
                    {
                        changedObjects.Add(this);
                        changedObjects.Add(Tile);
                        changedObjects.Add(path[0]);

                        MoveTo(path[0]);
                        actionPoints -= BaseActionCost;
                    }
                    else
                    {
                        Idle();
                    }
                }
            }
            else
            {
                Idle();
            }
        }

        private void Idle()
        {
            //TODO: implement idle behaviour (seeing the player part done)
            actionPoints=0;//if this is reached the enemy has no other options than to skip its turn (reduces number of GoTo loops executed) compared to actionpoints--;
        }
        
        public override void HandleInput(InputHelper inputHelper)
        {
            if (Health > 0)
            {
                Action onClick = () =>
                {
                    Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                    AttackEvent aE = new AttackEvent(player, this);
                    Server.Send(aE);
                };
            
                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

                base.HandleInput(inputHelper);
            }
        }

        /// <summary>
        /// Position the HP bar directly above the Enemy
        /// </summary>
        private void PositionHPBar()
        {
            hpBar.Position = Tile.GlobalPosition - new Vector2(Math.Abs(Tile.Width - hpBar.Width) / 2, 0);
        }

        public void InitGUI(Dictionary<string, object> guiState)
        {
            if (GameWorld.Find("HealthBar" + guid.ToString()) == null)
            {
                SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");
                hpBar = new Bar<Enemy>(this, e => e.Health, e => e.MaxHealth, textfieldFont, Color.Red, 2, "HealthBar" + guid.ToString(), 1.0f, 1f, false);
                (GameWorld.Find("PlayingGui") as PlayingGUI).Add(hpBar);
            }
            else
            {
                hpBar = GameWorld.Find("HealthBar" + guid.ToString()) as Bar<Enemy>;
                hpBar.SetValueObject(this);
            }
            hpBar.Visible = !Tile.Visible ? false : Visible;
            PositionHPBar();
        }

        public void CleanupGUI(Dictionary<string, object> guiState)
        {

        }
    }
}
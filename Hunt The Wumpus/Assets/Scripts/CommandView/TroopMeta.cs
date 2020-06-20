﻿using System;

namespace CommandView
{
    public enum TroopType
    {
        Marine,
        // Sniper,
        // Tank,
        // Defender
    }
    public class TroopMeta
    {
        public String resourceString;

        public TroopType type = TroopType.Marine;
        public String name;

        public int damage;

        public bool sendToBattle = false;
        
        public int battlesFought;

        public int UpgradeLvl;

        public TroopMeta(TroopType type, String name)
        {
            this.type = type;
            this.name = name;
            damage = 10;
            

            switch (type)
            {
                case TroopType.Marine:
                    resourceString = "Objects/Soldier";
                    //Debug.Log(resourceString);
                    break;
            }
        }
    }
}

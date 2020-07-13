using System;

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
        public readonly String ResourceString;

        public readonly TroopType Type = TroopType.Marine;
        public readonly String Name;

        public int Damage;

        public bool SendToBattle = false;
        
        public int BattlesFought;

        public int UpgradeLvl;

        public TroopMeta(TroopType type, String name)
        {
            this.Type = type;
            this.Name = name;
            Damage = 10;
            

            switch (type)
            {
                case TroopType.Marine:
                    ResourceString = "Objects/Soldier";
                    //Debug.Log(resourceString);
                    break;
            }
        }
    }
}

using System;
using UnityEngine;
using Object = UnityEngine.Object;

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

        public int battlesFought;

        public TroopMeta(TroopType type, String name)
        {
            this.type = type;
            this.name = name;

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

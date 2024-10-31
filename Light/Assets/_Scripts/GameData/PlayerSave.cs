using System;
using UnityEngine;

namespace GameData
{
    public class PlayerSave
    {
        public float PosX = 3.86f;
        public float PosZ = -0.797f;
        public int Hp = 10;
        public int Lantern = 3;
        public int StageTime = 0;
        public int SkeletonKilled;
        public PlayerSpell[] Spells = Array.Empty<PlayerSpell>();

        public PlayerSave(int hp, int lantern, int stageTime,Vector3 position ,int skeletonKilled, PlayerSpell[] spells)
        {
            Hp = hp;
            Lantern = lantern;
            StageTime = stageTime;
            PosX = position.x;
            PosZ = position.z;
            Spells = spells;
            SkeletonKilled = skeletonKilled;
        }
    }

    public class PlayerSpell
    {
        public string SpellName;
        public int Remain;

        public PlayerSpell()
        {
            
        }
        public PlayerSpell(string spellName, int remain)
        {
            SpellName = spellName;
            Remain = remain;
        }
    }
}
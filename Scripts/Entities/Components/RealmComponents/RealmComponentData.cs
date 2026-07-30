using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Components.RealmComponents
{
    public class RealmData
    {
        public required string Name;
        public required float MaxHpBonus;//血
        public required float MaxQiBonus;//气
        public required float MaxExp;//修
        public required float AtkBonus;//攻
        public required float DefBonus;//御
    }

    public class RealmComponentData : ComponentData
    {
        public RealmType Realm;
        public required Dictionary<RealmType, RealmData> RealmDataMap;
    }
}
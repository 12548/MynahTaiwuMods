using Config;
using BehaviorType = GameData.Domains.Character.BehaviorType;

namespace EventDslFramework;

public enum 立场
{
    刚正 = BehaviorType.Just,
    仁善 = BehaviorType.Kind,
    中庸 = BehaviorType.Even,
    叛逆 = BehaviorType.Rebel,
    唯我 = BehaviorType.Egoistic
}
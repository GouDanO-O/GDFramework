///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GTagLib
    {
        public static GameplayTag Ability { get; } = new GameplayTag("Ability");
        public static GameplayTag Ability_Curing { get; } = new GameplayTag("Ability.Curing");
        public static GameplayTag Test { get; } = new GameplayTag("Test");

        public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["Ability"] = Ability,
            ["Ability.Curing"] = Ability_Curing,
            ["Test"] = Test,
        };
    }
}
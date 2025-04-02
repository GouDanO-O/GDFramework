using System.Collections.Generic;
using GAS.Runtime;
using GDFramework;
using QFramework;

namespace GDFramework_Core.Skill.General.Core
{
    public class GameplayAbilitySystem : Singleton<GameplayAbilitySystem>,ISystem
    {
        public bool Initialized { get; set; }

        private const int _abilityCapacity = 1024;
        
        public List<AbilitySystemComponent> AbilitySystemComponentList { get; }

        private readonly List<AbilitySystemComponent> _cachedAbilitySystemComponentList;
        
        private GameplayAbilitySystem()
        {
            AbilitySystemComponentList = new List<AbilitySystemComponent>(_abilityCapacity);
            _cachedAbilitySystemComponentList= new List<AbilitySystemComponent>(_abilityCapacity);
            
            
        }
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            
        }

        public void Init()
        {
            
            
        }

        public void Deinit()
        {
            
        }

    }
}
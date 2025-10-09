using System;
using Core.World.Object.PlayerTeam;
using GDFrameworkCore;

namespace Core.Game.Condition
{
    [Serializable]
    public struct ConditionCheckInventory
    {
        public Goods.Goods conditionGood;

        public int checkCount;
    }
    
    public class CheckInventoryCondition : BaseCondition,ICanGetModel
    {
        public ConditionCheckInventory[] conditionGoods;
        
        public override bool CheckCondition()
        {
            bool canPaskCondition = true;
            PlayerTeamInventory teamInventory = this.GetModel<PlayerTeamInventory>();

            for (int i = 0; i < conditionGoods.Length; i++)
            {
                ConditionCheckInventory  conditionGood = conditionGoods[i];
                if (!teamInventory.DoesInventoryHasThisGoods(conditionGood.conditionGood,conditionGood.checkCount))
                {
                    canPaskCondition = false;
                    break;
                }
            }
            
            return canPaskCondition;
        }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
    }
}
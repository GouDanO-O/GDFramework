using System.Collections.Generic;
using GDFramework_Core.Cheater;
using QFramework;

namespace GDFramework_Core.Models
{
    public class CheatDataModel : AbstractModel
    {
        public Dictionary<string,AddCheatCommand> cheatModules = new Dictionary<string,AddCheatCommand>();
        
        protected override void OnInit()
        {
            
        }
        
        public void AddCheatModule(string name, AddCheatCommand command)
        {
            if (!cheatModules.ContainsKey(name))
            {
                cheatModules.Add(name,command);
            }
        }

        public void RemoveCheatModule(string name)
        {
            if (cheatModules.ContainsKey(name))
            {
                cheatModules.Remove(name);  
            }
        }
        
        public Dictionary<string,AddCheatCommand> GetCheaterDatas()
        {
            return cheatModules;
        }
    } 
}


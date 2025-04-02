using System.Collections.Generic;
using GDFramework_Core.Command.Cheat;
using QFramework;

namespace GDFramework_Core.Models
{
    public class CheatData_Model : AbstractModel
    {
        public Dictionary<string,AddCheat_Command> cheatModules = new Dictionary<string,AddCheat_Command>();
        
        protected override void OnInit()
        {
            
        }
        
        public void AddCheatModule(string name, AddCheat_Command command)
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
        
        public Dictionary<string,AddCheat_Command> GetCheaterDatas()
        {
            return cheatModules;
        }
    } 
}


using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace GDFramework.Mission.Actions
{
    public class ActionBase : ActionTask
    {
        [RequiredField]
        public BBParameter<string> TaskId;
        
        protected override void OnExecute()
        {
            base.OnExecute();
        }
    }
}
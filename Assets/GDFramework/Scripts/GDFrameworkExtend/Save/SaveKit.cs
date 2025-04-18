using System.Collections;
using Cysharp.Threading.Tasks;
using GDFrameworkCore;

namespace GDFrameworkExtend.Save
{
    public class SaveKit : AbstractSystem
    {
        private SaveModel _saveModel;
        
        protected override void OnInit()
        {
            
        }

        private void InitSaveKit()
        {
            this.RegisterEvent<SSaveEvent>((data) =>
            {
                StartSave();
            });
        }

        private void DeInitSaveKit()
        {
            
        }

        private void StartSave()
        {
            
        }
        
    }
}
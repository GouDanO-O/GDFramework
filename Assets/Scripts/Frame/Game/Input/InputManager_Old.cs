using QFramework;

namespace Frame.Game.Input
{
    public class InputManager_Old : MonoSingleton<InputManager_Old>,IController
    {
        public IArchitecture GetArchitecture()
        {
            throw new System.NotImplementedException();
        }

        private void Update()
        {
            CheckKeyCode();
        }

        private void CheckKeyCode()
        {
            
        }
    }
}


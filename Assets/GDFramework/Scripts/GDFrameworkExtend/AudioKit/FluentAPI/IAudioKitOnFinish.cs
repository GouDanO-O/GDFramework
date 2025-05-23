using System;

namespace GDFrameworkExtend.AudioKit
{
    public interface IAudioKitOnFinish
    {
        void OnFinish(Action onFinish);
    }
    
    public static class AudioKitOnFinishExtensions
    {
        public static void OnFinish(this IAudioKitOnFinish self,Action onFinish)
        {
            self?.OnFinish(onFinish);
        }
    }
}
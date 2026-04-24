namespace ei8.Prototypes.HelloWorm
{
    internal interface ITemporal
    {
        void ProcessTick();

        bool IsPlaying { get; }
        
        void Play();

        void Pause();
    }
}

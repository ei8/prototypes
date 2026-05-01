namespace ei8.Prototypes.HelloWorm
{
    internal interface ITemporal
    {
        void ProcessTick();

        bool IsPlaying { get; }
        
        void Play();

        void Pause();
    }

    internal interface ITemporal<T> : ITemporal where T : Enum
    {
        T Mode { get; set; }
    }
}

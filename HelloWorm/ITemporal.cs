namespace ei8.Prototypes.HelloWorm
{
    public interface ITemporal
    {
        void ProcessTick();

        bool IsPlaying { get; }
        
        void Play();

        void Pause();
    }
}

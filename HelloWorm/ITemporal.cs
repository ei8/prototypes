namespace ei8.Prototypes.HelloWorm
{
    internal interface ITemporal
    {
        event EventHandler IsPlayingChanged;

        bool IsPlaying { get; }
        
        void Play();

        void Pause();
    }
}

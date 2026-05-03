namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class FiredEventArgs(FireInfo fireInfo, int charge) : EventArgs
    {
        public FireInfo FireInfo { get; } = fireInfo;
        public int Charge { get; } = charge;
    }
}

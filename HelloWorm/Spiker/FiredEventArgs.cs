namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class FiredEventArgs(FireInfo fireInfo, ChargeInfo charge) : EventArgs
    {
        public FireInfo FireInfo { get; } = fireInfo;
        public ChargeInfo Charge { get; } = charge;
    }
}

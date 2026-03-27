namespace HelloWorm.Spiker.Neurons
{
    public class FiredEventArgs : EventArgs
    {
        public FiredEventArgs(FireInfo fireInfo, int charge)
        {
            this.FireInfo = fireInfo;
            this.Charge = charge;
        }

        public FireInfo FireInfo { get; private set; }
        public int Charge { get; private set; }
    }
}

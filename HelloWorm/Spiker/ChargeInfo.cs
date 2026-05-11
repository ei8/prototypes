namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class ChargeInfo
    {
        public const float RestingPotential = -70;
        public const float SpikeDepolarizationAmount = 15;

        public ChargeInfo(IEnumerable<TriggerInfo> triggers)
        {
            this.Excitations = triggers.Where(ti => ti.Effect == Cortex.Coding.NeurotransmitterEffect.Excite);
            this.Inhibitions = triggers.Where(ti => ti.Effect == Cortex.Coding.NeurotransmitterEffect.Inhibit);
        }

        public IEnumerable<TriggerInfo> Excitations { get; private set; }

        public IEnumerable<TriggerInfo> Inhibitions { get; private set; }

        public float Result =>  
            ChargeInfo.RestingPotential + 
            this.Excitations.Sum(ChargeInfo.GetCharge) +
            this.Inhibitions.Sum(ChargeInfo.GetCharge);

        public static float GetCharge(TriggerInfo trigger) => ChargeInfo.SpikeDepolarizationAmount * trigger.Strength * (int) trigger.Effect;
    }
}

namespace ei8.Prototypes.HelloWorm.Spiker.Neurons
{
    public struct SpikeOrigin
    {
        public SpikeOrigin(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; set; }

        public override string ToString()
        {
            return $"SpikeOrigin: '{this.Id}'";
        }
    }
}

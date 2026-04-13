namespace ei8.Prototypes.HelloWorm.Spiker
{
    public struct SpikeOrigin
    {
        public SpikeOrigin(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

        public override string ToString()
        {
            return $"SpikeOrigin: '{Id}'";
        }
    }
}

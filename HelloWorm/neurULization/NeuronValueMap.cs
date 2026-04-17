namespace ei8.Prototypes.HelloWorm.neurULization
{
    public  class NeuronValueMap<T>(Guid neuronId, T value)
    {
        public Guid NeuronId { get; } = neuronId;
        public T Value { get; } = value;
    }
}

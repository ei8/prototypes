namespace ei8.Prototypes.HelloWorm.neurULization
{
    public  class FauxNeurULizationMap<T>(string neuronId, T value)
    {
        public string NeuronId { get; } = neuronId;
        public T Value { get; } = value;
    }
}

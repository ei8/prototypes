namespace HelloWorm.Spiker.Neurons
{
    public class NeuronCollection : ObservableKeyedCollection<string, Neuron>
    {
        protected override string GetKeyForItem(Neuron item) => item.Id;
    }
}

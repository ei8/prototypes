namespace ei8.Prototypes.HelloWorm
{
    public class InputInfo
    {
        public InputInfo(BinaryNeuronInfo input1, BinaryNeuronInfo input2)
        {
            this.Input1 = input1;
            this.Input2 = input2;
        }
        
        public BinaryNeuronInfo Input1 { get; }
        public BinaryNeuronInfo Input2 { get; }
    }
}

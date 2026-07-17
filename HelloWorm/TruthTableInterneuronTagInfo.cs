namespace ei8.Prototypes.HelloWorm
{
    public class TruthTableInterneuronTagInfo(string input1TagPrefix, string input2TagPrefix, string typeTagPrefix)
    {
        public TruthTableInterneuronTagInfo(string inputTagPrefix) : this(inputTagPrefix, inputTagPrefix, inputTagPrefix)
        {
        }

        public string Input1TagPrefix { get; } = input1TagPrefix;
        public string Input2TagPrefix { get; } = input2TagPrefix;
        public string TypeTagPrefix { get; } = typeTagPrefix;
    }
}

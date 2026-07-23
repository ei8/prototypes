namespace ei8.Prototypes.HelloWorm.Math.Logic
{
    public class LogicGateInterneuronTagInfo(string[] inputTagPrefixes, string typeTagPrefix)
    {
        public static LogicGateInterneuronTagInfo CreateSameTagForSingleInput(string inputTagPrefix) =>
             new([inputTagPrefix], inputTagPrefix);

        public static LogicGateInterneuronTagInfo CreateSameTagForDualInput(string inputTagPrefix) =>
             new([inputTagPrefix, inputTagPrefix], inputTagPrefix);

        public string[] InputTagPrefixes { get; } = inputTagPrefixes;

        public string TypeTagPrefix { get; } = typeTagPrefix;
    }
}

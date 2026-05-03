namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class StimulusInfo(StimulusType type, object value)
    {
        public StimulusType Type { get; } = type;

        public object Value { get; } = value;
    }
}
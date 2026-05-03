namespace ei8.Prototypes.HelloWorm.Spiker
{
    public class StimulusParser(StimulusType type, Predicate<object> evaluator, Func<object, Guid> idConverter)
    {
        public StimulusType Type { get; } = type;

        public Predicate<object> Evaluator { get; } = evaluator;

        public Func<object, Guid> IdConverter { get; } = idConverter;
    } 
}

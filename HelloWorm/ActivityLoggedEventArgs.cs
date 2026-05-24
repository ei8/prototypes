namespace ei8.Prototypes.HelloWorm
{
    public class ActivityLoggedEventArgs
    (
        IEnumerable<Guid> presynapticIds,
        Guid neuronId,
        NeuronStatusValue newStatus
    )
        : EventArgs
    {
        public IEnumerable<Guid> PresynapticIds { get; } = presynapticIds;
        public Guid NeuronId { get; } = neuronId;
        public NeuronStatusValue NewStatus { get; } = newStatus;
    }
}

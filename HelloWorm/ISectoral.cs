namespace HelloWorm
{
    internal interface ISectoral : IRectangleBounded
    {
        float StartAngle { get; set; }
        float SweepAngle { get; set; }
    }
}

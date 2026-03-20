namespace HelloWorm
{
    internal interface ISectoral : IRectangleBound
    {
        float StartAngle { get; set; }
        float SweepAngle { get; set; }
    }
}

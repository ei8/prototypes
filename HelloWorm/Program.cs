namespace HelloWorm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var world = new World();
            world.Location = new Point(0, 0);
            world.Size = new Size(600, 400);
            world.Add(
                new Food()
                {
                    Location = new Point(50, 50),
                    Size = new Size(5, 5)
                }
            );
            world.Add(
                new Worm()
                {
                    Direction = 0,
                    Location = new Point(100, 100),
                    Size = new Size(Constants.Worm.Width, 90),
                    Speed = Constants.Worm.Speed,
                    Components = [
                        new Nose()
                        {
                            Location = new Point(0, 0),
                            Size = new Size(Constants.Worm.Width, Constants.Worm.Width),
                            Components = InitializeSectors()
                        }
                    ]
                }
            );
            //world.Add(
            //    new Worm()
            //    {
            //        Direction = 135,
            //        Location = new Point(200, 100),
            //        Size = new Size(10, 80),
            //        Speed = 1
            //    }
            //);
            Application.Run(new Form1(world));
        }

        private static IEnumerable<Sector> InitializeSectors()
        {
            var sects = new List<Sector>();

            for (int i = 0; i < Constants.Worm.SectorRenderCount; i++)
                sects.Add(new Sector()
                {
                    Name = (i + 1).ToString(),
                    StartAngle = (i * Constants.Worm.SectorSweepAngle) + 1,
                    SweepAngle = Constants.Worm.SectorSweepAngle
                });

            return sects;
        }
    }
}
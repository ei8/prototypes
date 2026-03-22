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
                    Size = new Size(5, 5),
                    StartAngle = 0f,
                    SweepAngle = 90f
                }
            );

            world.Add(
                new Worm()
                {
                    Direction = 370,
                    Location = new Point(500, 20),
                    Size = new Size(Constants.Worm.Width, 100),
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
            world.Add(
                new Worm()
                {
                    Direction = 135,
                    Location = new Point(20, 300),
                    Size = new Size(15, 80),
                    Speed = 8,
                    Components = [
                        new Nose()
                        {
                            Location = new Point(0, 0),
                            Size = new Size(15, 15),
                            Components = InitializeSectors()
                        }
                    ]
                }
            );

            Application.Run(new Form1(world));
        }

        private static IEnumerable<Sector> InitializeSectors()
        {
            var sects = new List<Sector>();

            for (int i = 0; i < Constants.Worm.SectorRenderCount; i++)
                sects.Add(new Sector()
                {
                    StartAngle = (i * Constants.Worm.SectorSweepAngle) + 1,
                    SweepAngle = Constants.Worm.SectorSweepAngle
                });

            return sects;
        }
    }
}
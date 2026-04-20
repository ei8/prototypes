namespace ei8.Prototypes.HelloWorm
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

            #region Network retrieval
            //var rp = new RequestProvider();
            //rp.SetHttpClientHandler(new HttpClientHandler());
            //var client = new ei8.Cortex.Library.Client.Out.HttpNeuronQueryClient(rp);
            //var queryResult = client.GetNeurons(
            //    "http://fibona.cc/worm1/av8r/",
            //    new NeuronQuery()
            //    {
            //        Depth = 5,
            //        DirectionValues = DirectionValues.Outbound
            //    },
            //    "Guest"
            //).Result;
            //var net = queryResult.ToNetwork();
            #endregion

            // TODO: Prepare data
            // Create neurons
            //var rp = new RequestProvider();
            //rp.SetHttpClientHandler(new HttpClientHandler());
            //var client = new HttpNeuronClient(rp);
            //client.CreateNeuron(
            //    "http://fibona.cc/worm1/av8r/",
            //    Guid.NewGuid().ToString(),
            //    "Hello worm",
            //    null,
            //    "http://fibona.cc/worm1/gogogo/",
            //    "bearerToken"
            //).Wait();

            var world = new World();
            world.Location = new Point(0, 0);
            world.Size = new Size(600, 400);

            Application.Run(new Form1(world));
        }
    }
}
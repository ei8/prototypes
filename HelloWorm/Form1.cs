using HelloWorld.Spiker.Spikes;

namespace ei8.Prototypes.HelloWorm
{
    public partial class Form1 : Form
    {
        private readonly WorldPanel worldPanel;
        private bool loading = true;

        public Form1(World world)
        {
            InitializeComponent();

            this.worldPanel = new WorldPanel(world);
            this.worldPanel.Left = 0;
            this.worldPanel.Top = 0;
            this.worldPanel.DoubleClick += this.WorldPanel_DoubleClick;
            this.UpdatePanelSize();

            this.Controls.Add(this.worldPanel);

            this.loading = false;
        }

        private void WorldPanel_DoubleClick(object? sender, EventArgs e)
        {
            // TODO: var worm = this.worldPanel.World.Components.OfType<Worm>().Single();
            //worm.SpikeService.Spike(
            //    [
            //        new SpikeTarget(typeof(Constants.NeuronId).GetField("Sector5")!.GetValue(null)!.ToString()!),
            //        new SpikeTarget(Constants.NeuronId.Odor)
            //    ],
            //    worm.Neurons
            //);

            var f = this.worldPanel.World.Components.OfType<Food>().SingleOrDefault();
            if (f != null)
                this.worldPanel.World.Remove(f);
            else
                this.worldPanel.World.Add(new Food().Create(this.worldPanel.World.Size));
        }

        private void UpdatePanelSize()
        {
            this.worldPanel.Height = this.Height;
            this.worldPanel.Width = this.Width;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.worldPanel.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.worldPanel.Invalidate();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            this.UpdatePanelSize();
        }
    }
}

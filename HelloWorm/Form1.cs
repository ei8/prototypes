using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Model.Reflection;

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
            //var worm = this.worldPanel.World.Components.OfType<Worm>().Single();
            //if (
            //    worm.Network!.TryGetByMirrorUrl(worm.MirrorConfigs!.Single(mc => mc.Keys.Contains(Worm.SectorValues.Sector1.ToEnumKeyString())).Url, out Neuron? sectorNeuron) &&
            //    worm.Network!.TryGetByMirrorUrl(worm.MirrorConfigs!.Single(mc => mc.Keys.Contains(typeof(Odor).ToKeyString())).Url, out Neuron? odorNeuron)
            //)
            //worm.SpikeService!.Spike(
            //    [
            //        sectorNeuron.Id,
            //        odorNeuron.Id
            //    ]
            //);
            //worm.Collide(new CollisionInfo(new Odor(), worm.Components.OfType<Nose>().Single().Components.OfType<Sector>().First(), 1));

            var f = this.worldPanel.World.Components.OfType<Food>().FirstOrDefault();
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
            this.worldPanel.InvalidateRectangularComposite(this.worldPanel.World);
            // TODO: this.worldPanel.Invalidate();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            this.UpdatePanelSize();
        }
    }
}

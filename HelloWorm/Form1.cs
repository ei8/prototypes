using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Model.Reflection;

namespace ei8.Prototypes.HelloWorm
{
    public partial class Form1 : Form
    {
        private bool loading = true;

        public Form1(World world)
        {
            InitializeComponent();

            this.worldPanel.World = world;

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

            if (this.worldPanel.World != null)
            {
                var f = this.worldPanel.World.Components.OfType<Food>().FirstOrDefault();
                if (f != null)
                    this.worldPanel.World.Remove(f);
                else
                    this.worldPanel.World.Add(new Food().Create(this.worldPanel.World.Size));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.worldPanel.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.worldPanel.World != null)
                this.worldPanel.InvalidateRectangularComposite(this.worldPanel.World);
        }

        private void mnuWormInitialize_Click(object sender, EventArgs e)
        {
            string avatarUrl = Form1.ShowDialog(this, "Avatar URL");
        }

        public static string ShowDialog(IWin32Window? owner, string caption)
        {
            using (InputBox prompt = new InputBox() { Text = caption, StartPosition = FormStartPosition.CenterScreen })
            {
                // ... add controls and configure ...
                return prompt.ShowDialog(owner) == DialogResult.OK ? prompt.txtInput.Text : "";
            }
        }
    }
}

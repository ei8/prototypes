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
            this.UpdatePanelSize();

            this.Controls.Add(this.worldPanel);

            this.loading = false;
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

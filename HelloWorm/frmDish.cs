using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmDish : DockContent
    {
        private readonly DishPanel dishPanel;
        public frmDish(ISelectionService selectionService)
        {
            InitializeComponent();

            this.dishPanel = new DishPanel(selectionService);
            this.dishPanel.Dish = null;
            this.dishPanel.Dock = DockStyle.Fill;
            this.dishPanel.Location = new Point(0, 0);
            this.dishPanel.Name = "dishPanel";
            this.dishPanel.Size = new Size(800, 450);
            this.dishPanel.TabIndex = 1;
            this.Controls.Add(dishPanel);
        }

        private void Dish_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var dish = (Dish)sender!;
            switch (e.PropertyName)
            {
                case nameof(Dish.Name):
                    this.Text = dish.Name;
                    break;
                case nameof(Dish.TimerResolution):
                    this.timer1.Interval = dish.TimerResolution;
                    break;
                case nameof(Dish.IsPlaying):
                    if (dish.IsPlaying)
                        this.timer1.Start();
                    else
                        this.timer1.Stop();
                    break;
            }
        }

        public Dish? Dish
        {
            get => this.dishPanel.Dish;
            set
            {
                if (this.dishPanel.Dish != value)
                {
                    if (this.dishPanel.Dish != null)
                    {
                        this.dishPanel.Dish.Pause();
                        this.dishPanel.Dish.PropertyChanged -= this.Dish_PropertyChanged;
                    }

                    this.dishPanel.Dish = value;

                    if (this.dishPanel.Dish != null)
                    {
                        this.dishPanel.Dish.PropertyChanged += this.Dish_PropertyChanged;
                        this.timer1.Interval = this.dishPanel.Dish.TimerResolution;
                    }

                    this.dishPanel.Invalidate();
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (this.Dish != null)
            {
                this.Dish.ProcessTick();
                this.dishPanel.InvalidateDish();
            }
        }

        private void frmDish_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dish = null;
        }

        private void frmDish_Load(object sender, EventArgs e)
        {
            if (this.dishPanel.Dish != null && string.IsNullOrEmpty(this.dishPanel.Dish.Name))
            {
                this.dishPanel.Dish.Name = ExtensionMethods.CreateUnusedName(
                    (i) => $"{typeof(Dish).Name}{i}",
                    (s) => this.DockPanel.Contents.OfType<frmDish>().Any(fd => fd.Dish?.Name == s)
                );
            }
            else 
                this.Text = this.Dish?.Name;
        }
    }
}

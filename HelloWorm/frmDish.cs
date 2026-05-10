using Microsoft.Extensions.DependencyInjection;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmDish : DockContent
    {
        private readonly IServiceProvider serviceProvider;
        private readonly DishPanel dishPanel;
        public frmDish(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            this.dishPanel = serviceProvider.GetRequiredService<DishPanel>();
            this.dishPanel.Dish = null;
            this.dishPanel.Dock = DockStyle.Fill;
            this.dishPanel.Location = new Point(0, 0);
            this.dishPanel.Name = "dishPanel";
            this.dishPanel.Size = new Size(800, 450);
            this.dishPanel.TabIndex = 1;
            this.Controls.Add(dishPanel);

            this.serviceProvider = serviceProvider;
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

        public Dish Dish
        {
            get => this.dishPanel.Dish!;
            private set
            {
                if (this.dishPanel.Dish != value)
                {
                    if (this.dishPanel.Dish != null)
                        this.dishPanel.Dish.PropertyChanged -= this.Dish_PropertyChanged;

                    this.dishPanel.Dish = value;

                    if (this.dishPanel.Dish != null)
                    {
                        this.dishPanel.Dish.PropertyChanged += this.Dish_PropertyChanged;
                        this.dishPanel.Dish.Name = ExtensionMethods.CreateUnusedName(
                            (i) => $"{typeof(Dish).Name}{i.ToString()}",
                            (s) => this.DockPanel.Contents.OfType<frmDish>().Any(fd => fd.Dish.Name == s)
                        );
                        this.timer1.Interval = this.dishPanel.Dish.TimerResolution;
                    }

                    this.dishPanel.Invalidate();
                }
            }
        }

        private void frmDish_Load(object sender, EventArgs e)
        {
            this.Dish = serviceProvider.GetRequiredService<Dish>();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.dishPanel.Dish!.ProcessTick();
            this.dishPanel.InvalidateDish();
        }
    }
}

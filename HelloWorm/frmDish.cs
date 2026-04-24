using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmDish : DockContent
    {
        public frmDish()
        {
            InitializeComponent();

            this.dishPanel.Dish = new Dish();
            this.dishPanel.Dish.PropertyChanged += this.Dish_PropertyChanged;

            this.timer1.Interval = this.dishPanel.Dish.TimerResolution;
        }

        private void Dish_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var dish = (Dish) sender!;
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

        public Dish Dish => this.dishPanel.Dish!;

        private void DishPanel_DoubleClick(object? sender, EventArgs e)
        {
            if (this.dishPanel.Dish != null)
            {
                var f = this.dishPanel.Dish.Components.OfType<Food>().FirstOrDefault();
                if (f != null)
                    this.dishPanel.Dish.Remove(f);
                else
                    this.dishPanel.Dish.Add(new Food().Create(this.dishPanel.Dish.Size));
            }
        }

        private void frmDish_Load(object sender, EventArgs e)
        {
            this.dishPanel.Invalidate();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.dishPanel.Dish!.ProcessTick();
            this.dishPanel.InvalidateDish();
        }
    }
}

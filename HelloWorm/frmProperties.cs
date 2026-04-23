using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmProperties : DockContent
    {
        private readonly ISelectionService selectionService;

        public frmProperties(ISelectionService selectionService)
        {
            InitializeComponent();
            this.selectionService = selectionService;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
        }

        private void SelectionService_SelectionChanged(object? sender, EventArgs e)
        {
            this.propertyGrid1.SelectedObject = this.selectionService.PrimarySelection;
        }
    }
}

using System.Collections.Specialized;
using System.ComponentModel.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace ei8.Prototypes.HelloWorm
{
    public partial class frmProjectExplorer : DockContent
    {
        private readonly IProjectService projectService;
        private readonly ISelectionService selectionService;
        private readonly Func<IObject, TreeNode?, string> textRetriever;
        private readonly Predicate<IObject> includeChecker;
        private readonly Func<IObject, int> imageIndexRetriever;

        public frmProjectExplorer(IProjectService projectService, ISelectionService selectionService)
        {
            InitializeComponent();

            this.HideOnClose = true;
            this.projectService = projectService;
            this.projectService.ProjectChanged += this.ProjectService_ProjectChanged;
            this.selectionService = selectionService;

            this.textRetriever = (o, pn) =>
            {
                var result = string.Empty;

                // TODO: use INamed
                if (o is Project)
                    result = "Project";
                else if (o is Dish d)
                    result = d.Name;
                else if (o is Worm w)
                    result = "Worm";
                else if (o is Food f)
                    result = "Food";
                else if (o is Nose)
                    result = "Nose";
                else if (
                    o is ISectoral s &&
                    pn != null
                )
                    result = "Sector" + ((IRectangularComposite)pn.Tag).GetSectorId(s).ToString();

                return result;
            };
            this.includeChecker = (o) => o is not Odor;
            this.imageIndexRetriever = (o) =>
            {
                var result = -1;

                if (o is Project)
                    result = 0;
                else if (o is Dish)
                    result = 1;
                else if (o is Food)
                    result = 2;
                else if (o is Worm)
                    result = 3;
                else if (o is Nose)
                    result = 4;
                else if (o is Sector)
                    result = 5;

                return result;
            };

        }

        private void ProjectService_ProjectChanged(object? sender, EventArgs e)
        {
            this.ReloadProjectTreeView();
        }

        private void ReloadProjectTreeView()
        {
            this.treeView.Nodes.Clear();
            var p = this.projectService.GetProject();
            if (p != null)
            {
                frmProjectExplorer.Fill(
                    this.treeView,
                    p,
                    this.textRetriever,
                    this.includeChecker,
                    this.imageIndexRetriever,
                    this.Composite_NotifyCollectionChanged
                );

                this.treeView.ExpandAll();
            }
        }

        private static void Fill(
            TreeView treeView, 
            IObject @object, 
            Func<IObject, TreeNode?, string> textRetriever, 
            Predicate<IObject> includeChecker,
            Func<IObject, int> imageIndexRetriever,
            NotifyCollectionChangedEventHandler collectionHandler, 
            TreeNode? parent = null
        )
        {
            if (includeChecker(@object))
            {
                var newNode = new TreeNode() { 
                    Text = textRetriever(@object, parent), 
                    Tag = @object, 
                    ImageIndex = imageIndexRetriever(@object)
                };
                if (parent == null)
                    treeView.Nodes.Add(newNode);
                else
                    parent.Nodes.Add(newNode);

                if (@object is IComposite composite)
                {
                    composite.NotifyCollectionChanged += collectionHandler;
                    collectionHandler(composite, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        private void Composite_NotifyCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {

            if (
                // sender is composite and 
                sender is IComposite composite &&
                (
                    // is not a dish
                    ( 
                        sender is not Dish
                    ) ||
                    // or if it is, dish is not being added to but not an odor particle
                    (
                        e.Action == NotifyCollectionChangedAction.Add &&
                        e.NewItems != null &&
                        e.NewItems.Count == 1 &&
                        e.NewItems[0] is not Odor
                    ) ||
                    // or dish is being removed from but not an odor particle
                    (
                        e.Action == NotifyCollectionChangedAction.Remove &&
                        e.OldItems != null &&
                        e.OldItems.Count == 1 &&
                        e.OldItems[0] is not Odor
                    ) ||
                    e.Action == NotifyCollectionChangedAction.Reset
                )
            )
            {
                TreeNode parentNode = this.treeView.GetAllNodes().Single(tn => tn.Tag == composite);
                parentNode.Nodes.Clear();
                foreach (var c in composite.Components)
                {
                    frmProjectExplorer.Fill(
                        this.treeView,
                        c,
                        this.textRetriever,
                        this.includeChecker,
                        this.imageIndexRetriever,
                        this.Composite_NotifyCollectionChanged,
                        parentNode
                    );
                }
                parentNode.Expand();
            }
        }

        private void tsbReload_Click(object sender, EventArgs e)
        {
            this.ReloadProjectTreeView();
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.selectionService.SetSelectedComponents(new[] { e.Node.Tag });
        }
    }
}
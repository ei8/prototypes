using NLog;
using Reactor.Neurons;
using Reactor.Persistence;
using Reactor.ResultMarkers;
using Reactor.SpikeResults;
using Reactor.Spikes;
using Reactor.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TinyIoC;
using WeifenLuo.WinFormsUI.Docking;
using works.ei8.Cortex.Graph.Domain.Model;
using works.ei8.Cortex.Graph.Port.Adapter.IO.Process.Events.Standard;

using SpikerNeuron = Reactor.Neurons.Neuron;

namespace Reactor
{
    public partial class MainForm : Form
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const int AnimateCount = 5;

        private TinyIoCContainer container;
        private DockPanel dockPanel;
        private System.Threading.Timer timer;

        public MainForm()
        {
            InitializeComponent();

            this.dockPanel = new DockPanel();
            this.dockPanel.Dock = DockStyle.Fill;
            this.dockPanel.BackColor = SystemColors.AppWorkspace;
            this.dockPanel.Theme = new VS2015BlueTheme();
            this.Controls.Add(this.dockPanel);
            this.statusStrip1.SendToBack();
            this.toolStrip1.SendToBack();
            this.menuStrip1.SendToBack();

            this.refractoryPeriodToolStripProgressBar.Value = 100;
            this.refractoryPeriodToolStripStatusLabel.Text = $"Refractory Period ({Constants.RefractoryPeriod.Milliseconds} ms):";
            this.timer = new System.Threading.Timer(new System.Threading.TimerCallback((o) =>
            {
                if (this.refractoryPeriodToolStripProgressBar.Value < 100)
                {
                    var newValue = this.refractoryPeriodToolStripProgressBar.Value + (100 / MainForm.AnimateCount);
                    this.Invoke(new MethodInvoker(() => this.refractoryPeriodToolStripProgressBar.SetProgressNoAnimation(newValue)));                    
                }                    
                else if (this.refractoryPeriodToolStripProgressBar.Value == 100)
                {
                    this.timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
            }));

            this.container = new TinyIoCContainer();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //if (!this.DisplayEula())
            //    return;

            this.container.Register(new NeuronCollection());
            this.container.Resolve<NeuronCollection>().CollectionChanged += this.MainForm_CollectionChanged;

            #region Legacy Data
            // TODO: var allNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "All"));
            //var hasNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Has"));
            //var ofNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Of"));
            //var getNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Get"));
            //var byNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "By"));
            //var roleNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Role"));
            //var identifierNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Identifier"));

            //var hasRoleNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Role",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(roleNeuron.Id)
            //    ));
            //var subjectIdNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Subject Id"));
            //var hasSubjectIdNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Subject Id",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(subjectIdNeuron.Id)
            //    ));
            //var ofSubjectIdNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Of, Subject Id",
            //    new Terminal(ofNeuron.Id),
            //    new Terminal(subjectIdNeuron.Id)
            //    ));

            //var hasIdentifierOfSubjectIdNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Identifier, Of Subject Id",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(subjectIdNeuron.Id),
            //    new Terminal(ofSubjectIdNeuron.Id)
            //    ));

            //var userNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(), 
            //    "User",
            //    new Terminal(hasIdentifierOfSubjectIdNeuron.Id, 0.5f),
            //    new Terminal(hasSubjectIdNeuron.Id, 0.5f),
            //    new Terminal(hasRoleNeuron.Id, 0.5f)
            //    ));

            //var heightNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Height"));
            //var hasHeightNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Height",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(heightNeuron.Id)
            //    ));

            //var personNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(), 
            //    "Person",
            //    new Terminal(hasHeightNeuron.Id, 0.5f)
            //    ));

            //var inheritsNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Inherits"));

            //var inheritsUserNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Inherits, User",
            //    new Terminal(inheritsNeuron.Id),
            //    new Terminal(userNeuron.Id)
            //    ));

            //var inheritsPersonNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Inherits, Person",
            //    new Terminal(inheritsNeuron.Id),
            //    new Terminal(personNeuron.Id)
            //    ));

            //var inheritsPersonInheritsUserNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Inherits Person, Inherits User",
            //    new Terminal(inheritsPersonNeuron.Id),
            //    new Terminal(inheritsUserNeuron.Id)                
            //    ));

            //var instantiatesNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Instantiates"));
            //var instantiatesInheritsPersonInheritsUserNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Instantiates, Inherits Person Inherits User",
            //    new Terminal(instantiatesNeuron.Id),
            //    new Terminal(inheritsPersonInheritsUserNeuron.Id)
            //    ));

            //#region Subject
            //// Inherent properties should always fire
            //// Of, 123
            //var _123Neuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "123"));
            //var of123Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Of, 123",
            //    new Terminal(ofNeuron.Id), 
            //    new Terminal(_123Neuron.Id)
            //    ));

            //// Has, Subject Id, Of, 123
            //var hasSubjectIdOf123Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Subject Id, Of 123",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(subjectIdNeuron.Id),
            //    new Terminal(of123Neuron.Id)
            //    ));

            //// Of, 456
            //var _456Neuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "456"));
            //var of456Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Of, 456",
            //    new Terminal(ofNeuron.Id), 
            //    new Terminal(_456Neuron.Id)
            //    ));

            //// Has, Subject Id, Of, 456
            //var hasSubjectIdOf456Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Subject Id, Of 456",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(subjectIdNeuron.Id),
            //    new Terminal(of456Neuron.Id)
            //    ));

            //// by, Subject Id
            //var bySubjectIdNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "By, Subject Id",
            //    new Terminal(byNeuron.Id),
            //    new Terminal(subjectIdNeuron.Id)
            //    ));
            //#endregion

            //#region Role   
            //// Of, Admin
            //var adminNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Admin"));
            //var ofAdminNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Of, Admin",
            //    new Terminal(ofNeuron.Id), 
            //    new Terminal(adminNeuron.Id)
            //    ));

            //// Has, Role, Of, Admin
            //var hasRoleOfAdminNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Role, Of Admin",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(roleNeuron.Id),
            //    new Terminal(ofAdminNeuron.Id)
            //    ));

            //// Of, Writer
            //var writerNeuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "Writer"));
            //var ofWriterNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Of, Writer",
            //    new Terminal(ofNeuron.Id),
            //    new Terminal(writerNeuron.Id)
            //    ));

            //// Has, Role, Of, Writer
            //var hasRoleOfWriterNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Role, Of Writer",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(roleNeuron.Id),
            //    new Terminal(ofWriterNeuron.Id)
            //    ));
            //#endregion

            //#region Height
            //// Of, 69
            //var _69Neuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "69"));
            //var of69Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Of, 69",
            //    new Terminal(ofNeuron.Id), new Terminal(_69Neuron.Id)
            //    ));

            //// Has, Height, Of, 69
            //var hasHeightOf69Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Height, Of 69",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(heightNeuron.Id),
            //    new Terminal(of69Neuron.Id)
            //    ));

            //// Of, 61
            //var _61Neuron = this.AddNeuron(new Neuron(Helper.GetNewShortGuid(), "61"));
            //var of61Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Of, 61",
            //    new Terminal(ofNeuron.Id), new Terminal(_61Neuron.Id)
            //    ));

            //// Has, Height, Of, 61
            //var hasHeightOf61Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Has, Height, Of 61",
            //    new Terminal(hasNeuron.Id),
            //    new Terminal(heightNeuron.Id),
            //    new Terminal(of61Neuron.Id)
            //    ));
            //#endregion

            //#region Elmer
            //var elmerNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Elmer",
            //    new Terminal(instantiatesInheritsPersonInheritsUserNeuron.Id),
            //    new Terminal(hasSubjectIdOf123Neuron.Id),
            //    new Terminal(hasRoleOfAdminNeuron.Id, 0.5f),
            //    new Terminal(hasHeightOf69Neuron.Id, 0.5f)
            //    ));
            //#endregion

            //#region Rose
            //var roseNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Rose",
            //    new Terminal(instantiatesInheritsPersonInheritsUserNeuron.Id),
            //    new Terminal(hasSubjectIdOf456Neuron.Id),
            //    new Terminal(hasRoleOfWriterNeuron.Id, 0.5f),
            //    new Terminal(hasHeightOf61Neuron.Id, 0.5f)
            //    ));
            //#endregion

            //#region User
            //var getUserBySubjectIdNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Get, User, By Subject Id"                
            //    ));

            //getNeuron.AddTerminal(new Terminal(getUserBySubjectIdNeuron.Id, 1f/3));
            //userNeuron.AddTerminal(new Terminal(getUserBySubjectIdNeuron.Id, 1f/3));
            //bySubjectIdNeuron.AddTerminal(new Terminal(getUserBySubjectIdNeuron.Id, 1f/3));

            //var ofUserNeuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Of, User",
            //    new Terminal(ofNeuron.Id),
            //    new Terminal(userNeuron.Id)
            //    ));
            //#endregion

            //#region Get User By Subject Id, Of 123
            //// Get User By Subject Id, Of 123
            //var getUserBySubjectIdOf123Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Get User By Subject Id, Of 123",
            //    new Terminal(elmerNeuron.Id)
            //    ));

            //getUserBySubjectIdNeuron.AddTerminal(new Terminal(getUserBySubjectIdOf123Neuron.Id, 0.5f));
            //of123Neuron.AddTerminal(new Terminal(getUserBySubjectIdOf123Neuron.Id, 0.5f));
            //#endregion

            //#region Get User By Subject Id, Of 456
            //// Get User By Subject Id, Of 456
            //var getUserBySubjectIdOf456Neuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Get User By Subject Id, Of 456",
            //    new Terminal(roseNeuron.Id)
            //    ));

            //getUserBySubjectIdNeuron.AddTerminal(new Terminal(getUserBySubjectIdOf456Neuron.Id, 0.5f));
            //of456Neuron.AddTerminal(new Terminal(getUserBySubjectIdOf456Neuron.Id, 0.5f));
            //#endregion

            //#region Get Role Of User

            //var getRoleOfUserNeuron = this.AddNeuron(new Neuron(
            //   Helper.GetNewShortGuid(),
            //   "Get, Role, Of User",
            //   new Terminal(hasRoleOfAdminNeuron.Id, 0.5f),
            //   new Terminal(hasRoleOfWriterNeuron.Id, 0.5f)               
            //   ));

            //getNeuron.AddTerminal(new Terminal(getRoleOfUserNeuron.Id, 1f/3));
            //roleNeuron.AddTerminal(new Terminal(getRoleOfUserNeuron.Id, 1f/3));
            //ofUserNeuron.AddTerminal(new Terminal(getRoleOfUserNeuron.Id, 1f/3));
            //#endregion

            //#region Get Height Of User

            //var getHeightOfUserNeuron = this.AddNeuron(new Neuron(
            //   Helper.GetNewShortGuid(),
            //   "Get, Height, Of User",
            //   new Terminal(hasHeightOf61Neuron.Id, 0.5f),
            //   new Terminal(hasHeightOf69Neuron.Id, 0.5f)
            //   ));

            //getNeuron.AddTerminal(new Terminal(getHeightOfUserNeuron.Id, 1f/3));
            //heightNeuron.AddTerminal(new Terminal(getHeightOfUserNeuron.Id, 1f/3));
            //ofUserNeuron.AddTerminal(new Terminal(getHeightOfUserNeuron.Id, 1f/3));
            //#endregion

            //#region Get All Users
            //// Fires when "Get", "All", and "User" are fired
            //var getAllUserneuron = this.AddNeuron(new Neuron(
            //    Helper.GetNewShortGuid(),
            //    "Get, All, User",
            //    new Terminal(elmerNeuron.Id),
            //    new Terminal(roseNeuron.Id)
            //    ));

            //getNeuron.AddTerminal(new Terminal(getAllUserneuron.Id, 1f/3));
            //allNeuron.AddTerminal(new Terminal(getAllUserneuron.Id, 1f/3));
            //userNeuron.AddTerminal(new Terminal(getAllUserneuron.Id, 1f/3));
            //#endregion
            #endregion

            this.container.Register<ISpikeResultsService, SpikeResultsService>().AsSingleton();
            this.container.Register<ISpikeService, SpikeService>().AsSingleton();
            this.container.Register<ISpikeTargetListService, SpikeTargetListService>().AsSingleton();
            this.container.Register<ISelectionService, SelectionService>().AsSingleton();
            this.container.Register<IResultMarkerService, ResultMarkerService>().AsSingleton();
            this.container.Register<ISettingsService, SettingsService>().AsSingleton();
            
            this.container.Register<IRepository<works.ei8.Cortex.Graph.Domain.Model.Neuron>, InMemoryNeuronRepository>();
            this.container.Register<IRepository<works.ei8.Cortex.Graph.Domain.Model.Settings>, InMemorySettingsRepository>();
            this.container.Register<INotificationLogClient, StandardNotificationLogClient>(new StandardNotificationLogClient(
                    Properties.Settings.Default.NotificationLogBaseUrl,
                    int.Parse(Properties.Settings.Default.PollInterval),
                    this.container.Resolve<IRepository<works.ei8.Cortex.Graph.Domain.Model.Settings>>(),
                    this.container.Resolve<IRepository<works.ei8.Cortex.Graph.Domain.Model.Neuron>>()
                    )
                );

            this.container.Register<FiredForm>().AsSingleton();
            this.container.Register<TriggeredForm>().AsSingleton();
            this.container.Register<LogsForm>().AsSingleton();
            this.container.Register<CortexGraphView>().AsSingleton();
            this.container.Register<DendritesForm>().AsSingleton();
            this.container.Register<ResultMarkersForm>().AsSingleton();
            this.container.Register<ServersForm>().AsSingleton();
            this.container.Register<SpikeTargetsForm>().AsSingleton();
            this.container.Register<PropertyGridForm>().AsSingleton();

            this.container.Resolve<ISpikeTargetListService>().Removed += this.targets_Changed;
            this.container.Resolve<ISpikeTargetListService>().Added += this.targets_Changed;
            this.container.Resolve<ISpikeService>().Spiking += this.MainForm_Spiking;

            this.viewServersToolStripMenuItem_Click(this, EventArgs.Empty);
            this.viewGraphToolStripMenuItem_Click(this, EventArgs.Empty);
            this.viewSpikeTargetsToolStripMenuItem_Click(this, EventArgs.Empty);

            this.container.Resolve<ResultMarkersForm>().Show(
                this.container.Resolve<SpikeTargetsForm>().Pane,
                DockAlignment.Right,
                0.8);
            this.container.Resolve<LogsForm>().Show(
                this.container.Resolve<ResultMarkersForm>().Pane,
                DockAlignment.Right,
                0.6);
            this.container.Resolve<TriggeredForm>().Show(
                this.container.Resolve<LogsForm>().Pane,
                this.container.Resolve<LogsForm>()
                );
            this.container.Resolve<FiredForm>().Show(
                this.container.Resolve<TriggeredForm>().Pane,
                this.container.Resolve<TriggeredForm>()
                );
            this.container.Resolve<DendritesForm>().Show(
                this.container.Resolve<ServersForm>().Pane,
                this.container.Resolve<ServersForm>()
                );
            this.container.Resolve<PropertyGridForm>().Show(
                this.container.Resolve<DendritesForm>().Pane,
                DockAlignment.Bottom,
                0.5);

            // Defaults
            // TODO: this.container.Resolve<ISpikeService>().Add(new SpikeTarget(getNeuron.Id));
            //this.container.Resolve<ISpikeService>().Add(new SpikeTarget(roleNeuron.Id));
            //this.container.Resolve<ISpikeService>().Add(new SpikeTarget(ofUserNeuron.Id));
            //this.container.Resolve<ISpikeService>().Add(new SpikeTarget(bySubjectIdNeuron.Id));
            //this.container.Resolve<ISpikeService>().Add(new SpikeTarget(of123Neuron.Id));
            //this.container.Resolve<IResultMarkerService>().Add(new ResultMarker(adminNeuron.Id));
            //this.container.Resolve<IResultMarkerService>().Add(new ResultMarker(writerNeuron.Id));
            //this.container.Resolve<IResultMarkerService>().Add(new ResultMarker(_61Neuron.Id));
            //this.container.Resolve<IResultMarkerService>().Add(new ResultMarker(_69Neuron.Id));
            //this.container.Resolve<IResultMarkerService>().Add(new ResultMarker(elmerNeuron.Id));
            //this.container.Resolve<IResultMarkerService>().Add(new ResultMarker(roseNeuron.Id));
        }

        private void MainForm_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var n = this.container.Resolve<NeuronCollection>()[e.NewStartingIndex];
                n.Triggered += this.neuron_Triggered;
                n.Fired += this.neuron_Fired;

                this.graphCountToolStripStatusLabel.Text = $"{this.container.Resolve<NeuronCollection>().Count} neurons loaded";
            }
            else if(e.Action == NotifyCollectionChangedAction.Replace && this.reactiveToolStripButton.Checked)
            {
                var old = e.OldItems.Cast<Neurons.Neuron>().First();
                var newItem = e.NewItems.Cast<Neurons.Neuron>().First();

                if (!MainForm.IsAnalyzed(old) && MainForm.IsAnalyzed(newItem))
                {
                    var targets = new SpikeTarget[] { new SpikeTarget(newItem.Id) }; 
                    var newTargets = targets.Concat(this.container.Resolve<ISpikeTargetListService>().Targets);

                    this.InitializeSpike();
                    this.container.Resolve<ISpikeService>().Spike(newTargets);
                }
            }
        }

        private static bool IsAnalyzed(SpikerNeuron neuron)
        {
            bool result = false;
            if (neuron != null)
            {
                foreach (Neurons.Terminal t in neuron.Terminals)
                {
                    if (t.TargetId == Properties.Settings.Default.AnalyzedId)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private void MainForm_Spiking(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() => { 
                this.refractoryPeriodToolStripProgressBar.SetProgressNoAnimation(0);
                this.timer.Change(0, Constants.RefractoryPeriod.Milliseconds/MainForm.AnimateCount);
                }
            ));
        }

        private void targets_Changed(object sender, SpikeTargetEventArgs e)
        {
            this.spikeToolStripButton.Enabled = this.container.Resolve<ISpikeTargetListService>().Targets.Any();
        }

        private void neuron_Fired(object sender, FiredEventArgs e)
        {
            var n = (SpikerNeuron)sender;
            this.container.Resolve<ISpikeResultsService>().AddFired(n, e);
            this.container.Resolve<IResultMarkerService>().UpdateIfMarker(n.Id);
        }

        private void neuron_Triggered(object sender, TriggeredEventArgs e)
        {
            this.container.Resolve<ISpikeResultsService>().AddTriggered((SpikerNeuron)sender, e);
        }

        private void spikeButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.InitializeSpike();

                var ss = this.container.Resolve<ISpikeService>();
                ss.Spike(this.container.Resolve<ISpikeTargetListService>().Targets);
            }
            catch (Exception ex)
            {
                MainForm.logger.Error("The following error occurred: " + Environment.NewLine + ex.ToString());
            }
        }

        private void InitializeSpike()
        {
            this.container.Resolve<ISpikeResultsService>().Clear();
            this.container.Resolve<IResultMarkerService>().Initialize();
            this.container.Resolve<ISpikeService>().SetSpikeCount(int.Parse(this.spikeCountToolStripTextBox.Text));
        }

        private void viewLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<LogsForm>().Show(this.dockPanel, DockState.DockBottom);
        }

        private void viewTriggeredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<TriggeredForm>().Show(this.dockPanel, DockState.DockBottom);
        }

        private void viewFiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<FiredForm>().Show(this.dockPanel, DockState.DockBottom);
        }

        private void viewGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<CortexGraphView>().Show(this.dockPanel, DockState.Document);
        }

        private void viewDendritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<DendritesForm>().Show(this.dockPanel, DockState.DockRight);
        }

        private void viewResultMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<ResultMarkersForm>().Show(this.dockPanel, DockState.DockBottom);
        }

        private void viewServersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<ServersForm>().Show(this.dockPanel, DockState.DockRight);
        }

        private void enableLogsToolStripButton_Click(object sender, EventArgs e)
        {
            this.container.Resolve<ISpikeResultsService>().Enable(this.enableLogsToolStripButton.Checked);
        }

        private void viewSpikeTargetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<SpikeTargetsForm>().Show(this.dockPanel, DockState.DockBottom);
        }

        private void viewPropertiesWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<PropertyGridForm>().Show(this.dockPanel, DockState.DockRight);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.container.Resolve<ISelectionService>().SetSelectedObjects(
                new ISettingsService[] { this.container.Resolve<ISettingsService>() }
                );
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO: Workaround to prevent null reference exception when WhenActivated of View binding
            this.container.Resolve<CortexGraphView>().Close();
        }
    }
}
using HeartRateSensor;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.RequestProvider;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;

namespace HeartRateAnalyzer
{
    public partial class MainForm : Form
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly SerialQueue queue = new SerialQueue();

        public class HeartRateData
        {
            public string Timestamp { get; set; }
            public string HeartRate { get; set; }
        }

        private NeuronClient neuronClient;
        private NeuronApplicationService neuronApplicationService;
        private NeuronQueryService neuronQueryService;
        private NeuronGraphQueryClient neuronGraphQueryClient;
        private RequestProvider requestProvider;
        private SettingsService settingsService;
        private DependencyService dependencyService;
        private ObservableCollection<Candidate> candidateList;
        private IEnumerable<Neuron> heartRateValues;
        
        public MainForm()
        {
            InitializeComponent();

            this.requestProvider = new RequestProvider();
            this.dependencyService = new DependencyService();
            this.settingsService = new SettingsService(this.dependencyService);
            this.neuronGraphQueryClient = new NeuronGraphQueryClient(this.requestProvider, this.settingsService);
            this.neuronQueryService = new NeuronQueryService(this.neuronGraphQueryClient);
            this.neuronClient = new NeuronClient(this.settingsService);
            this.neuronApplicationService = new NeuronApplicationService(this.neuronClient);

            this.candidateList = new ObservableCollection<Candidate>();
            this.candidateList.CollectionChanged += this.CandidateList_CollectionChanged;

            this.propertyGrid1.SelectedObject = Properties.Settings.Default;

            MainForm.logger.Info("Initializing...");
        }

        private void CandidateList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    var c = item as Candidate;
                    if (c != null)
                    {
                        //Added items
                        c.PropertyChanged += this.Candidate_PropertyChanged;

                        var li = this.candidateListView.Items.Add(c.Neuron.NeuronId, c.Neuron.Tag, 0);
                        li.SubItems.Add(c.Status.ToString());
                        li.SubItems.Add(c.Neuron.NeuronId);
                        li.ToolTipText = c.Neuron.Tag;
                        li.Tag = c;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged)
                        //Removed items
                        ((INotifyPropertyChanged)item).PropertyChanged -= this.Candidate_PropertyChanged;

                    var c = item as Candidate;
                    if (c != null)
                        this.candidateListView.Items.RemoveByKey(c.Neuron.NeuronId);
                }                
            }
        }

        private void Candidate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Candidate.Status))
            {
                var c = (Candidate)sender;
                this.Invoke(new MethodInvoker(() =>
                {
                    this.candidateListView.Items[c.Neuron.NeuronId].SubItems[1].Text = c.Status.ToString();
                }
                ));
            }
        }

        private async void activateToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.heartRateValues == null)
            {
                MainForm.logger.Info("Loading HR values...");

                var relaxedNs = await this.neuronQueryService.GetNeurons(
                    Properties.Settings.Default.AvatarUrl,
                    filter: $"Postsynaptic={Properties.Settings.Default.RelaxedId}"
                    );

                var warningNs = await this.neuronQueryService.GetNeurons(
                    Properties.Settings.Default.AvatarUrl,
                    filter: $"Postsynaptic={Properties.Settings.Default.WarningId}"
                    );

                this.heartRateValues = relaxedNs.Concat(warningNs);
            }

            this.timer1.Enabled = activateToolStripButton.Checked;
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            MainForm.logger.Info("Checking for candidates...");

            // hope as far as one can see
            var cns = await this.neuronQueryService.GetNeurons(
                Properties.Settings.Default.AvatarUrl, 
                filter: $"Postsynaptic={Properties.Settings.Default.HeartRateId}&Postsynaptic!={Properties.Settings.Default.AnalyzedId}"
                );

            foreach (var original in cns)
            {
                if (!this.candidateList.Any(c => c.Neuron.NeuronId == original.NeuronId))
                {
                    var c = new Candidate(original);
                    this.candidateList.Add(c);
                    MainForm.logger.Info($"Added {c.Neuron.Tag} to candidate list.");

                    await this.queue.Enqueue(() => this.AnalyzeCandidate(c));
                    // var _ = Task.Run(() => this.AnalyzeCandidate(c));
                }
            }

            MainForm.logger.Info($"Found total of {cns.Count()} neurons.");
        }

        private async Task AnalyzeCandidate(Candidate value)
        {
            // --- Precreate
            //[Heart rate], if not yet existent	
            // -> Appropriate Range
            // 60-89 Relaxed
            // 90-99 Warning
            // 100-120 Critical

            try
            {
                var candidateData = JsonConvert.DeserializeObject<HeartRateData>(value.Neuron.Tag);

                MainForm.logger.Info($"Analyzing '{candidateData.Timestamp}'...");

                #region Create
                //Get Events [Timestamp]	
                var interneuronId = Guid.NewGuid().ToString();
                await this.neuronApplicationService.CreateNeuron(
                                Properties.Settings.Default.AvatarUrl,
                                interneuronId,
                                HeartRateForm.cleanForJSON($"Get Events {candidateData.Timestamp}"),
                                // -> Original
                                new Terminal[] { new Terminal() { TargetId = value.Neuron.NeuronId, Effect = NeurotransmitterEffect.Excite, Strength = 1f } },
                                Properties.Settings.Default.AnalyzerAuthorId
                            );

                //[Timestamp]
                var timestampId = Guid.NewGuid().ToString();
                await this.neuronApplicationService.CreateNeuron(
                    Properties.Settings.Default.AvatarUrl,
                    timestampId,
                    HeartRateForm.cleanForJSON(candidateData.Timestamp),
                    // -> Get Events[Timestamp]
                    new Terminal[] { new Terminal() { TargetId = interneuronId, Effect = NeurotransmitterEffect.Excite, Strength = 0.33333f } },
                    Properties.Settings.Default.AnalyzerAuthorId
                );
                #endregion

                #region Update
                //Get
                var getNeuron = (await this.neuronQueryService.GetNeuronById(
                    Properties.Settings.Default.AvatarUrl,
                    Properties.Settings.Default.GetId
                    )).First();
                await this.neuronApplicationService.AddTerminalsToNeuron(
                    Properties.Settings.Default.AvatarUrl,
                    getNeuron.NeuronId,
                    // -> Get Events[Timestamp]
                    new Terminal[] { new Terminal() { TargetId = interneuronId, Effect = NeurotransmitterEffect.Excite, Strength = 0.33333f } },
                    Properties.Settings.Default.AnalyzerAuthorId,
                    getNeuron.Version
                    );

                //Events
                var eventsNeuron = (await this.neuronQueryService.GetNeuronById(
                    Properties.Settings.Default.AvatarUrl,
                    Properties.Settings.Default.EventsId
                    )).First();
                await this.neuronApplicationService.AddTerminalsToNeuron(
                    Properties.Settings.Default.AvatarUrl,
                    eventsNeuron.NeuronId,
                    // -> Get Events[Timestamp]
                    new Terminal[] { new Terminal() { TargetId = interneuronId, Effect = NeurotransmitterEffect.Excite, Strength = 0.33333f } },
                    Properties.Settings.Default.AnalyzerAuthorId,
                    eventsNeuron.Version
                    );

                //Original
                var rateNeuron = this.heartRateValues.Where(n => n.Tag == candidateData.HeartRate).FirstOrDefault();
                string rateId = rateNeuron == null ? Properties.Settings.Default.UnknownRateId : rateNeuron.NeuronId;

                await this.neuronApplicationService.AddTerminalsToNeuron(
                    Properties.Settings.Default.AvatarUrl,
                    value.Neuron.NeuronId,
                    new Terminal[] {
                    // -> [Heart rate]
                    new Terminal() { TargetId = rateId, Effect = NeurotransmitterEffect.Excite, Strength = 1 },
                    // -> [Timestamp]            
                    new Terminal() { TargetId = timestampId, Effect = NeurotransmitterEffect.Excite, Strength = 1 },
                    // ->Analyzed
                    new Terminal() { TargetId = Properties.Settings.Default.AnalyzedId, Effect = NeurotransmitterEffect.Excite, Strength = 1 },
                    },
                    Properties.Settings.Default.AnalyzerAuthorId,
                    value.Neuron.Version
                    );
                #endregion

                value.Status = Candidate.StatusValue.Analyzed;
            }
            catch (Exception ex)
            {
                MainForm.logger.Error($"Error: {ex.ToString()}");
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var t = new Thread(new ThreadStart(() =>
            {
                string filename = Path.Combine(Environment.CurrentDirectory, "logs", DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    //start at the end of the file
                    long lastMaxOffset = reader.BaseStream.Length;

                    while (true)
                    {
                        Thread.Sleep(100);

                        //if the file size has not changed, idle
                        if (reader.BaseStream.Length == lastMaxOffset)
                            continue;

                        //seek to the last max offset
                        reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                        //read out of the file until the EOF
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            this.Invoke(new MethodInvoker(() =>
                                {
                                    this.logTextBox.AppendText(line + Environment.NewLine);
                                    this.logTextBox.ScrollToCaret();
                                }
                            )
                            );
                            //Console.WriteLine(line);
                        }

                        //update the last max offset
                        lastMaxOffset = reader.BaseStream.Position;
                    }
                }
            }
            ));
            t.Start();
        }
    }
}

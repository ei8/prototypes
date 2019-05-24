using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aga.Controls.Tree;
using Reactor.Neurons;

namespace Reactor
{
    public class TreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        private NeuronCollection neurons;

        public TreeModel(NeuronCollection neurons)
        {
            this.neurons = neurons;
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            IEnumerable result = null;

            if (treePath.FirstNode == null)
            {
                if (string.IsNullOrWhiteSpace(this.RootId))
                {
                    var list = this.neurons.ToList();
                    list.Sort(delegate (Neuron x, Neuron y)
                    {
                        if (x.Data == null && y.Data == null) return 0;
                        else if (x.Data == null) return -1;
                        else if (y.Data == null) return 1;
                        else return x.Data.CompareTo(y.Data);
                    }
                    );
                    result = list;
                }
                else
                    result = new Neuron[] { this.neurons[this.RootId] };
            }
            else
            {
                result = ((Neuron)treePath.LastNode).Terminals.Select(t => this.neurons[t.TargetId]);
            }

            return result;
        }

        public bool IsLeaf(TreePath treePath)
        {
            bool result = true;
            if (treePath.LastNode != null)
            {
                result = ((Neuron)treePath.LastNode).Terminals.Count() == 0;
            }
            return result;
        }

        public string RootId { get; set; }
    }
}

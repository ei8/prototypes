using Reactor.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reactor
{
    public class Helper
    {
        public static string GetNewShortGuid()
        {
            return Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "").Substring(0, 5);
        }

        public static Neuron GetNeuronByData(string value, NeuronCollection neurons)
        {
            return neurons.First(n => n.Data == value);
        }

        internal static bool IsSelectionNeuron(ISelectionService selectionService)
        {
            return (selectionService.SelectedObjects.FirstOrDefault() as Neuron) != null;
        }
    }
}

using System.Text.RegularExpressions;

namespace HelloWorm.Spiker.Neurons
{
    public class NeuronHelper
    {
        public static string GetNewShortGuid()
        {
            return Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "").Substring(0, 5);
        }

        public static Neuron GetNeuronByData(string value, NeuronCollection neurons)
        {
            return neurons.First(n => n.Data == value);
        }
    }
}

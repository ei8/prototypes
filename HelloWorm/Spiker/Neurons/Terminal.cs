using System.ComponentModel;

namespace ei8.Prototypes.HelloWorm.Spiker.Neurons
{
    public class Terminal
    {
        public Terminal() : this(string.Empty)
        {
        }

        public Terminal(string targetId) : this(targetId, Constants.Spiker.NeurotransmitterEffect.Excite, 1f)
        {
        }

        public Terminal(string targetId, float strength) : this(targetId, Constants.Spiker.NeurotransmitterEffect.Excite, strength)
        {
        }

        public Terminal(string targetId, Constants.Spiker.NeurotransmitterEffect effect, float strength)
        { 
            this.TargetId = targetId;
            this.Effect = effect;
            this.Strength = strength;
        }

        [ParenthesizePropertyName(true)]
        public string TargetId { get; set; }

        public Constants.Spiker.NeurotransmitterEffect Effect { get; set; }

        public float Strength { get; set; }
    }
}


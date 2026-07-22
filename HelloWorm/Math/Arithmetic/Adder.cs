using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Math.Arithmetic
{ 
    public class Adder : IAdder
    {
        public Adder(
            int exponent = 0, 
            BinaryNeuronInfo? precedingCarryOver = null
        ) : 
        this(
            InputInfo.Create($"{nameof(Adder)}{exponent + 1}.Addend1", $"{nameof(Adder)}{exponent + 1}.Addend2"),
            BinaryNeuronInfo.Create($"{nameof(Adder)}{exponent + 1}.{nameof(Sum)}"),
            BinaryNeuronInfo.Create($"{nameof(Adder)}{exponent + 1}.{nameof(CarryOver)}"),
            exponent,
            precedingCarryOver
        )
        {
        }

        public Adder(
            InputInfo addends,
            BinaryNeuronInfo sum,
            BinaryNeuronInfo carryOver,
            int exponent = 0,
            BinaryNeuronInfo? precedingCarryOver = null
        )
        {
            this.Network = new();
            this.Network.AddReplaceItems(
                // Inputs
                this.Addends = addends,
                // Outputs
                this.Sum = sum,
                this.CarryOver = carryOver
            );

            string adderName = $"Adder{exponent + 1}";

            if (
                // is not least significant bit
                precedingCarryOver != null &&
                BinaryNeuronInfo.TryCreate(out var half1_XOR_Result, adderName) &&
                BinaryNeuronInfo.TryCreate(out var half1_CarryOver, adderName) &&
                BinaryNeuronInfo.TryCreate(out var half2_CarryOver, adderName)
            )
            {
                this.Network.AddReplaceItems(
                    half1_XOR_Result,
                    half1_CarryOver,
                    half2_CarryOver
                );

                string precedingAdderName = $"Adder{exponent}";

                CreateAdderHalf1Interneurons(
                    this.Network,
                    addends,
                    half1_XOR_Result,
                    half1_CarryOver,
                    adderName
                );

                // half2
                if (
                    TruthTableInterneuronInfo.TryCreate(
                        out var half2_XOR___CarryOver__Half1_XOR_Result,
                        TruthTableInterneuronInfo.LogicGateType.Xor,
                        this.Sum,
                        new(
                            precedingAdderName,
                            adderName,
                            adderName
                        )
                    ) &&
                    TruthTableInterneuronInfo.TryCreate(
                        out var half2_AND___CarryOver__Half1_XOR_Result,
                        TruthTableInterneuronInfo.LogicGateType.And,
                        half2_CarryOver,
                        new(
                            precedingAdderName,
                            adderName,
                            adderName
                        )
                    ) &&
                    // OR carryOvers
                    TruthTableInterneuronInfo.TryCreate(
                        out var OR___Half1_CarryOver__Half2_CarryOver,
                        TruthTableInterneuronInfo.LogicGateType.Or,
                        this.CarryOver,
                        new(adderName)
                    )
                )
                {
                    this.Network.AddReplaceItems(
                        half2_XOR___CarryOver__Half1_XOR_Result,
                        half2_AND___CarryOver__Half1_XOR_Result,
                        OR___Half1_CarryOver__Half2_CarryOver
                    );
                    this.Network.AddReplaceItems(
                        // Link Half1 interneurons and precedingCarryOver to Half2 interneurons
                        half2_XOR___CarryOver__Half1_XOR_Result.LinkInputNeurons(
                            new(
                                precedingCarryOver,
                                half1_XOR_Result
                            )
                        ),
                        half2_AND___CarryOver__Half1_XOR_Result.LinkInputNeurons(
                            new(
                                precedingCarryOver,
                                half1_XOR_Result
                            )
                        ),
                        // OR carryOvers
                        OR___Half1_CarryOver__Half2_CarryOver.LinkInputNeurons(
                            new(
                                half1_CarryOver,
                                half2_CarryOver
                            )
                        )
                    );
                }
            }
            else
            {
                CreateAdderHalf1Interneurons(
                    this.Network,
                    addends,
                    this.Sum,
                    this.CarryOver,
                    adderName
                );
            }
        }

        private static void CreateAdderHalf1Interneurons(
            Network network,
            InputInfo addends,
            BinaryNeuronInfo xorOutput,
            BinaryNeuronInfo andOutput,
            string prefix
        )
        {
            // Link half1 interneurons
            if (
                TruthTableInterneuronInfo.TryCreate(
                    out var half1_XOR___Addend1__Addend2,
                    TruthTableInterneuronInfo.LogicGateType.Xor,
                    xorOutput,
                    new(prefix)
                ) &&
                TruthTableInterneuronInfo.TryCreate(
                    out var half1_AND___Addend1__Addend2,
                    TruthTableInterneuronInfo.LogicGateType.And,
                    andOutput,
                    new(prefix)
                )
            )
            {
                network.AddReplaceItems(
                    half1_XOR___Addend1__Addend2,
                    half1_AND___Addend1__Addend2
                );
                // Link Input Neurons to Half1 Interneuron
                network.AddReplaceItems(
                    half1_XOR___Addend1__Addend2.LinkInputNeurons(addends),
                    half1_AND___Addend1__Addend2.LinkInputNeurons(addends)
                );
            }
        }

        public Network Network { get; }

        public InputInfo Addends { get; }

        public BinaryNeuronInfo CarryOver { get; }

        public BinaryNeuronInfo Sum { get; }
    }
}

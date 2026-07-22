using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Math.Arithmetic
{
    public class Adder : ICircuit
    {
        public enum Input
        {
            Addend1,
            Addend2
        }

        public enum Output
        {
            Sum,
            CarryOver
        }

        public Adder(
            int exponent = 0,
            BinaryNeuronInfo? precedingCarryOver = null
        ) :
        this(
            new(
                [
                    BinaryNeuronInfo.Create($"{nameof(Adder)}{exponent + 1}.{nameof(Input.Addend1)}"),
                    BinaryNeuronInfo.Create($"{nameof(Adder)}{exponent + 1}.{nameof(Input.Addend2)}"),
                ],
                [
                    BinaryNeuronInfo.Create($"{nameof(Adder)}{exponent + 1}.{nameof(Output.Sum)}"),
                    BinaryNeuronInfo.Create($"{nameof(Adder)}{exponent + 1}.{nameof(Output.CarryOver)}")
                ]
            ),
            exponent,
            precedingCarryOver
        )
        {
        }

        public Adder(
            ParameterInfo parameters,
            int exponent = 0,
            BinaryNeuronInfo? precedingCarryOver = null
        )
        {
            this.Network = new();
            this.Network.AddReplaceItems(
                this.Parameters = parameters
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
                    this.Parameters.Inputs,
                    half1_XOR_Result,
                    half1_CarryOver,
                    adderName
                );

                // half2
                if (
                    TruthTableInterneuronInfo.TryCreate(
                        out var half2_XOR___CarryOver__Half1_XOR_Result,
                        TruthTableInterneuronInfo.LogicGateType.Xor,
                        this.Parameters.Outputs[(int) Output.Sum],
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
                        this.Parameters.Outputs[(int) Output.CarryOver],
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
                            [
                                precedingCarryOver,
                                half1_XOR_Result
                            ]
                        ),
                        half2_AND___CarryOver__Half1_XOR_Result.LinkInputNeurons(
                            [
                                precedingCarryOver,
                                half1_XOR_Result
                            ]
                        ),
                        // OR carryOvers
                        OR___Half1_CarryOver__Half2_CarryOver.LinkInputNeurons(
                            [
                                half1_CarryOver,
                                half2_CarryOver
                            ]
                        )
                    );
                }
            }
            else
            {
                CreateAdderHalf1Interneurons(
                    this.Network,
                    this.Parameters.Inputs,
                    this.Parameters.Outputs[(int) Output.Sum],
                    this.Parameters.Outputs[(int) Output.CarryOver],
                    adderName
                );
            }
        }

        private static void CreateAdderHalf1Interneurons(
            Network network,
            BinaryNeuronInfo[] addends,
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

        public ParameterInfo Parameters { get; }
    }
}

using ei8.Cortex.Coding;

namespace ei8.Prototypes.HelloWorm.Math.Arithmetic
{
    public class Subtractor : ICircuit
    {
        public enum Input
        {
            Minuend,
            Subtrahend
        }

        public enum Output
        {
            Difference,
            Borrow
        }

        public Subtractor(
            int exponent = 0,
            BinaryNeuronInfo? precedingBorrow = null
        ) :
        this(
            new(
                [
                    BinaryNeuronInfo.Create($"{nameof(Subtractor)}{exponent + 1}.{nameof(Input.Minuend)}"), 
                    BinaryNeuronInfo.Create($"{nameof(Subtractor)}{exponent + 1}.{nameof(Input.Subtrahend)}"),
                ],
                [
                    BinaryNeuronInfo.Create($"{nameof(Subtractor)}{exponent + 1}.{nameof(Output.Difference)}"),
                    BinaryNeuronInfo.Create($"{nameof(Subtractor)}{exponent + 1}.{nameof(Output.Borrow)}")
                ]
            ),
            exponent,
            precedingBorrow
        )
        {
        }

        public Subtractor(
            ParameterInfo parameters,
            int exponent = 0,
            BinaryNeuronInfo? precedingBorrow = null
            )
        {
            this.Network = new();
            this.Network.AddReplaceItems(
                this.Parameters = parameters
            );

            string subtractorName = $"Subtractor{exponent + 1}";

            // Declare Outputs
            if (BinaryNeuronInfo.TryCreate(out var half1_OUT___Half1_NOT___Minuend, subtractorName))
            {
                this.Network.AddReplaceItems(half1_OUT___Half1_NOT___Minuend);
                if (
                    // is not least significant bit
                    precedingBorrow != null &&
                    BinaryNeuronInfo.TryCreate(out var half1_XOR_Result, subtractorName) &&
                    BinaryNeuronInfo.TryCreate(out var half2_OUT___Half2_NOT___Half1_XOR_Result, subtractorName) &&
                    BinaryNeuronInfo.TryCreate(out var half1_Borrow, subtractorName) &&
                    BinaryNeuronInfo.TryCreate(out var half2_Borrow, subtractorName)
                )
                {
                    this.Network.AddReplaceItems(
                        half1_XOR_Result,
                        half2_OUT___Half2_NOT___Half1_XOR_Result,
                        half1_Borrow,
                        half2_Borrow
                    );

                    string precedingSubtractorName = $"Subtractor{exponent}";

                    // half1 interneurons
                    CreateSubtractorHalf1Interneurons(
                        this.Network,
                        this.Parameters.Inputs,
                        half1_XOR_Result,
                        half1_OUT___Half1_NOT___Minuend,
                        half1_Borrow,
                        subtractorName
                    );

                    // half2 interneurons
                    if (
                        TruthTableInterneuronInfo.TryCreate(
                            out var half2_XOR___Borrow__Half1_XOR_Result,
                            TruthTableInterneuronInfo.LogicGateType.Xor,
                            this.Parameters.Outputs[(int) Output.Difference],
                            new(
                                precedingSubtractorName,
                                subtractorName,
                                subtractorName
                            )
                        ) &&
                        InverterInterneuronInfo.TryCreate(
                            out var half2_NOT___Half1_XOR_Result,
                            half2_OUT___Half2_NOT___Half1_XOR_Result,
                            subtractorName,
                            subtractorName
                        ) &&
                        TruthTableInterneuronInfo.TryCreate(
                            out var half2_AND___Borrow__Half2_OUT___Half2_NOT___Half1_XOR_Result,
                            TruthTableInterneuronInfo.LogicGateType.And,
                            half2_Borrow,
                            new(
                                precedingSubtractorName,
                                subtractorName,
                                subtractorName
                            )
                        ) &&
                        // OR Borrows
                        TruthTableInterneuronInfo.TryCreate(
                            out var OR___Half1_Borrow__Half2_Borrow,
                            TruthTableInterneuronInfo.LogicGateType.Or,
                            this.Parameters.Outputs[(int) Output.Borrow],
                            new(subtractorName)
                        )
                    )
                    {
                        this.Network.AddReplaceItems(
                            half2_XOR___Borrow__Half1_XOR_Result,
                            half2_NOT___Half1_XOR_Result,
                            half2_AND___Borrow__Half2_OUT___Half2_NOT___Half1_XOR_Result,
                            OR___Half1_Borrow__Half2_Borrow
                        );

                        this.Network.AddReplaceItems(
                            // Link Half1 interneurons and precedingBorrow to Half2 interneurons
                            half2_XOR___Borrow__Half1_XOR_Result.LinkInputNeurons(
                                [
                                    precedingBorrow,
                                    half1_XOR_Result
                                ]
                            ),
                            half2_NOT___Half1_XOR_Result.LinkInputNeurons(
                                half1_XOR_Result
                            ),
                            half2_AND___Borrow__Half2_OUT___Half2_NOT___Half1_XOR_Result.LinkInputNeurons(
                                [
                                    precedingBorrow,
                                    half2_OUT___Half2_NOT___Half1_XOR_Result
                                ]
                            ),
                            // OR Borrows
                            OR___Half1_Borrow__Half2_Borrow.LinkInputNeurons(
                                [
                                    half1_Borrow,
                                    half2_Borrow
                                ]
                            )
                        );
                    }
                }
                else
                {
                    // half1
                    CreateSubtractorHalf1Interneurons(
                        this.Network,
                        this.Parameters.Inputs,
                        this.Parameters.Outputs[(int)Output.Difference],
                        half1_OUT___Half1_NOT___Minuend,
                        this.Parameters.Outputs[(int)Output.Borrow],
                        subtractorName
                    );
                }
            }
        }

        private static void CreateSubtractorHalf1Interneurons(
            Network network,
            BinaryNeuronInfo[] inputs,
            BinaryNeuronInfo xorOutput,
            BinaryNeuronInfo notOutput,
            BinaryNeuronInfo andOutput,
            string prefix
        )
        {
            // Link half1 interneurons
            if (
                TruthTableInterneuronInfo.TryCreate(
                    out var half1_XOR___Minuend__Subtrahend,
                    TruthTableInterneuronInfo.LogicGateType.Xor,
                    xorOutput,
                    new(prefix)
                ) &&
                InverterInterneuronInfo.TryCreate(
                    out var half1_NOT___Minuend,
                    notOutput,
                    prefix,
                    prefix
                ) &&
                TruthTableInterneuronInfo.TryCreate(
                    out var half1_AND___Subtrahend__Half1_OUT___Half1_NOT___Minuend,
                    TruthTableInterneuronInfo.LogicGateType.And,
                    andOutput,
                    new(prefix)
                )
            )
            {
                network.AddReplaceItems(
                    half1_XOR___Minuend__Subtrahend,
                    half1_NOT___Minuend,
                    half1_AND___Subtrahend__Half1_OUT___Half1_NOT___Minuend
                );

                network.AddReplaceItems(
                    // Input Neurons to Interneurons
                    half1_XOR___Minuend__Subtrahend.LinkInputNeurons(inputs),
                    half1_NOT___Minuend.LinkInputNeurons(inputs[(int) Input.Minuend]),
                    // Intermediate Results to Interneurons
                    half1_AND___Subtrahend__Half1_OUT___Half1_NOT___Minuend.LinkInputNeurons(
                        [
                            notOutput,
                            inputs[(int) Input.Subtrahend]
                        ]
                    )
                );
            }
        }

        public Network Network { get; }

        public ParameterInfo Parameters { get; }
    }
}

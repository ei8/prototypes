using ei8.Cortex.Coding;
using ei8.Prototypes.HelloWorm.Math.Logic;

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
                        LogicGateBase.TryCreate(
                            out XorGate? half2_XOR___Borrow__Half1_XOR_Result,
                            new(
                                [
                                    precedingBorrow,
                                    half1_XOR_Result
                                ],
                                [
                                    this.Parameters.Outputs[(int) Output.Difference]
                                ]
                            ),
                            new(
                                [
                                    precedingSubtractorName,
                                    subtractorName,
                                ],
                                subtractorName
                            )
                        ) &&
                        LogicGateBase.TryCreate(
                            out NotGate? half2_NOT___Half1_XOR_Result,
                            new(
                                [half1_XOR_Result],
                                [half2_OUT___Half2_NOT___Half1_XOR_Result]
                            ),
                            LogicGateInterneuronTagInfo.CreateSameTagForSingleInput(subtractorName)
                        ) &&
                        LogicGateBase.TryCreate(
                            out AndGate? half2_AND___Borrow__Half2_OUT___Half2_NOT___Half1_XOR_Result,
                            new(
                                [
                                    precedingBorrow,
                                    half2_OUT___Half2_NOT___Half1_XOR_Result
                                ],
                                [ 
                                    half2_Borrow 
                                ]
                            ),
                            new(
                                [
                                    precedingSubtractorName,
                                    subtractorName
                                ],
                                subtractorName
                            )
                        ) &&
                        // OR Borrows
                        LogicGateBase.TryCreate(
                            out OrGate? OR___Half1_Borrow__Half2_Borrow,
                            new(
                                [
                                    half1_Borrow,
                                    half2_Borrow
                                ],
                                [
                                    this.Parameters.Outputs[(int) Output.Borrow]
                                ]
                            ),
                            LogicGateInterneuronTagInfo.CreateSameTagForDualInput(subtractorName)
                        )
                    )
                    {
                        this.Network.AddReplaceItems(
                            half2_XOR___Borrow__Half1_XOR_Result,
                            half2_NOT___Half1_XOR_Result,
                            half2_AND___Borrow__Half2_OUT___Half2_NOT___Half1_XOR_Result,
                            OR___Half1_Borrow__Half2_Borrow
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
                LogicGateBase.TryCreate(
                    out XorGate? half1_XOR___Minuend__Subtrahend,
                    new(
                        inputs,
                        [
                            xorOutput
                        ]
                    ),
                    LogicGateInterneuronTagInfo.CreateSameTagForDualInput(prefix)
                ) &&
                LogicGateBase.TryCreate(
                    out NotGate? half1_NOT___Minuend,
                    new(
                        [
                            inputs[(int)Input.Minuend]
                        ],
                        [
                            notOutput
                        ]
                    ),
                    LogicGateInterneuronTagInfo.CreateSameTagForSingleInput(prefix),
                    prefix
                ) &&
                LogicGateBase.TryCreate(
                    out AndGate? half1_AND___Subtrahend__Half1_OUT___Half1_NOT___Minuend,
                    new(
                        [
                            notOutput,
                            inputs[(int) Input.Subtrahend]
                        ],
                        [
                            andOutput
                        ]
                    ),
                    LogicGateInterneuronTagInfo.CreateSameTagForDualInput(prefix)
                )
            )
            {
                network.AddReplaceItems(
                    half1_XOR___Minuend__Subtrahend,
                    half1_NOT___Minuend,
                    half1_AND___Subtrahend__Half1_OUT___Half1_NOT___Minuend
                );
            }
        }

        public Network Network { get; }

        public ParameterInfo Parameters { get; }
    }
}

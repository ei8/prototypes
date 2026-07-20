using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;

namespace ei8.Prototypes.HelloWorm.Math.Arithmetic
{ 
    public class Adder : IneurULized
    {
        //public Adder() : this(this.Network.CreateTruthTableInputNeurons($"{adderName}.Addend1", $"{adderName}.Addend2"))
        //{
        //}

        //public Adder(
        //    InputInfo addends,
        //    int base2Exponent = 0,
        //    BinaryNeuronInfo? precedingCarryOver = null
        //)
        //{
        //    this.Network = new();

        //    carryOver = null;
        //    string adderName = $"Adder{base2Exponent + 1}";


        //    // Declare Outputs
        //    if (
        //        this.Network.TryCreateBinaryNeurons(out var result, adderName) &&
        //        this.Network.TryCreateBinaryNeurons(out carryOver, adderName)
        //    )
        //    {
        //        if (
        //            // is not least significant bit
        //            precedingCarryOver != null &&
        //            this.Network.TryCreateBinaryNeurons(out var half1_XOR_Result, adderName) &&
        //            this.Network.TryCreateBinaryNeurons(out var half1_CarryOver, adderName) &&
        //            this.Network.TryCreateBinaryNeurons(out var half2_CarryOver, adderName)
        //        )
        //        {
        //            string precedingAdderName = $"Adder{base2Exponent}";

        //            CreateAdderHalf1Interneurons(
        //                this.Network,
        //                addends,
        //                half1_XOR_Result,
        //                half1_CarryOver,
        //                adderName
        //            );

        //            // half2
        //            if (
        //                this.Network.TryCreateTruthTableInterneurons(
        //                    out var half2_XOR___CarryOver__Half1_XOR_Result,
        //                    ExtensionMethods.LogicGateType.Xor,
        //                    result,
        //                    new(
        //                        precedingAdderName,
        //                        adderName,
        //                        adderName
        //                    )
        //                ) &&
        //                this.Network.TryCreateTruthTableInterneurons(
        //                    out var half2_AND___CarryOver__Half1_XOR_Result,
        //                    ExtensionMethods.LogicGateType.And,
        //                    half2_CarryOver,
        //                    new(
        //                        precedingAdderName,
        //                        adderName,
        //                        adderName
        //                    )
        //                ) &&
        //                // OR carryOvers
        //                this.Network.TryCreateTruthTableInterneurons(
        //                    out var OR___Half1_CarryOver__Half2_CarryOver,
        //                    ExtensionMethods.LogicGateType.Or,
        //                    carryOver,
        //                    new(adderName)
        //                )
        //            )
        //            {
        //                // Link Half1 interneurons and precedingCarryOver to Half2 interneurons
        //                this.Network.LinkTruthTableInputNeuronsToInterneurons(
        //                    new(
        //                        precedingCarryOver,
        //                        half1_XOR_Result
        //                    ),
        //                    half2_XOR___CarryOver__Half1_XOR_Result
        //                );

        //                this.Network.LinkTruthTableInputNeuronsToInterneurons(
        //                    new(
        //                        precedingCarryOver,
        //                        half1_XOR_Result
        //                    ),
        //                    half2_AND___CarryOver__Half1_XOR_Result
        //                );

        //                // OR carryOvers
        //                this.Network.LinkTruthTableInputNeuronsToInterneurons(
        //                    new(
        //                        half1_CarryOver,
        //                        half2_CarryOver
        //                    ),
        //                    OR___Half1_CarryOver__Half2_CarryOver
        //                );
        //            }
        //        }
        //        else
        //        {
        //            CreateAdderHalf1Interneurons(
        //                this.Network,
        //                addends,
        //                result,
        //                carryOver,
        //                adderName
        //            );
        //        }
        //    }
        //}

        //private static void CreateAdderHalf1Interneurons(
        //    Network network,
        //    InputInfo addends,
        //    BinaryNeuronInfo xorOutput,
        //    BinaryNeuronInfo andOutput,
        //    string prefix
        //)
        //{
        //    // Link half1 interneurons
        //    if (
        //        network.TryCreateTruthTableInterneurons(
        //            out var half1_XOR___Addend1__Addend2,
        //            ExtensionMethods.LogicGateType.Xor,
        //            xorOutput,
        //            new(prefix)
        //        ) &&
        //        network.TryCreateTruthTableInterneurons(
        //            out var half1_AND___Addend1__Addend2,
        //            ExtensionMethods.LogicGateType.And,
        //            andOutput,
        //            new(prefix)
        //        )
        //    )
        //    {
        //        // Link Input Neurons to Half1 Interneuron
        //        network.LinkTruthTableInputNeuronsToInterneurons(addends, half1_XOR___Addend1__Addend2);
        //        network.LinkTruthTableInputNeuronsToInterneurons(addends, half1_AND___Addend1__Addend2);
        //    }
        //}

        public Network Network { get; }
    }
}

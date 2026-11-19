using ei8.Cortex.Coding;
using ei8.Cortex.Coding.d23;
using Xunit;

namespace ei8.Prototypes.HelloWorm.Tests;

public class LogicGateTests
{
    private enum LogicGateKind
    {
        Not,
        And,
        Or,
        Nand,
        Nor,
        Xor,
        Xnor,
        Imply,
        Nimply
    }

    public static IEnumerable<object[]> BinaryGateCases
    {
        get
        {
            var gates = new (LogicGateKind Gate, Func<bool, bool, bool> Evaluate)[]
            {
                (LogicGateKind.And, (a, b) => a && b),
                (LogicGateKind.Or, (a, b) => a || b),
                (LogicGateKind.Nand, (a, b) => !(a && b)),
                (LogicGateKind.Nor, (a, b) => !(a || b)),
                (LogicGateKind.Xor, (a, b) => a != b),
                (LogicGateKind.Xnor, (a, b) => a == b),
                (LogicGateKind.Imply, (a, b) => !a || b),
                (LogicGateKind.Nimply, (a, b) => a && !b)
            };

            foreach (var gate in gates)
            foreach (var input1 in new[] { false, true })
            foreach (var input2 in new[] { false, true })
                yield return [(int)gate.Gate, input1, input2, gate.Evaluate(input1, input2)];
        }
    }

    [Theory]
    [MemberData(nameof(BinaryGateCases))]
    public void Binary_gate_maps_each_input_pair_to_the_expected_result(
        int gateValue,
        bool input1,
        bool input2,
        bool expectedResult
    )
    {
        var gate = (LogicGateKind)gateValue;
        var network = LogicGatesNetworkFactory.CreateLogicGates();
        var gateName = gate.ToString().ToUpperInvariant();
        var interneuronTag =
            $"{gateName}(Input1 = {ToBinaryDigit(input1)},Input2 = {ToBinaryDigit(input2)})";

        AssertGateMapping(
            network,
            interneuronTag,
            [
                $"Input1 = {ToTagValue(input1)}",
                $"Input2 = {ToTagValue(input2)}",
                gateName
            ],
            expectedResult
        );
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Not_gate_maps_each_input_to_the_inverted_result(bool input, bool expectedResult)
    {
        var network = LogicGatesNetworkFactory.CreateLogicGates();
        var interneuronTag = $"NOT(Input1 = {ToBinaryDigit(input)})";

        AssertGateMapping(
            network,
            interneuronTag,
            [
                $"Input1 = {ToTagValue(input)}",
                nameof(LogicGateKind.Not)
            ],
            expectedResult
        );
    }

    [Fact]
    public void Complete_logic_gates_worksheet_contains_all_gates_and_boolean_io()
    {
        var network = LogicGatesNetworkFactory.CreateLogicGates();
        var tags = network.GetItems<Neuron>()
            .Select(neuron => neuron.Tag)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("Input1 = TRUE", tags);
        Assert.Contains("Input1 = FALSE", tags);
        Assert.Contains("Input2 = TRUE", tags);
        Assert.Contains("Input2 = FALSE", tags);
        Assert.Contains("result = TRUE", tags);
        Assert.Contains("result = FALSE", tags);

        foreach (var gate in Enum.GetNames<LogicGateKind>())
            Assert.Contains(gate.ToUpperInvariant(), tags);
    }

    private static void AssertGateMapping(
        ReadOnlyNetwork network,
        string interneuronTag,
        IEnumerable<string> expectedInputTags,
        bool expectedResult
    )
    {
        var neurons = network.GetItems<Neuron>().ToDictionary(neuron => neuron.Id);
        var interneuron = neurons.Values.Single(neuron =>
            string.Equals(neuron.Tag, interneuronTag, StringComparison.OrdinalIgnoreCase)
        );
        var terminals = network.GetItems<Terminal>().ToArray();

        var actualInputTags = terminals
            .Where(terminal => terminal.PostsynapticNeuronId == interneuron.Id)
            .Select(terminal => neurons[terminal.PresynapticNeuronId].Tag)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var inputTag in expectedInputTags)
            Assert.Contains(inputTag, actualInputTags);

        var actualOutputTags = terminals
            .Where(terminal => terminal.PresynapticNeuronId == interneuron.Id)
            .Select(terminal => neurons[terminal.PostsynapticNeuronId].Tag)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains($"Result = {ToTagValue(expectedResult)}", actualOutputTags);
        Assert.DoesNotContain($"Result = {ToTagValue(!expectedResult)}", actualOutputTags);
    }

    private static string ToTagValue(bool value) =>
        value ? Boolean.TrueString.ToUpperInvariant() : Boolean.FalseString.ToUpperInvariant();

    private static string ToBinaryDigit(bool value) => value ? "1" : "0";
}

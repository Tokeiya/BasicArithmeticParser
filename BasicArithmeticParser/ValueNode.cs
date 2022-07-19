using System.Text;

namespace BasicArithmeticParser;

public sealed class ValueNode : ExpressionNode
{
	public ValueNode(string value) => TextValue = value;
	public string TextValue { get; }
	public override void Dump(StringBuilder builder) => builder.Append(TextValue);

	public override int ToDot(StringBuilder builder)
	{
		builder.Append($"{Id} [label=\"{TextValue}\",shape= box];\n");
		return Id;
	}
}
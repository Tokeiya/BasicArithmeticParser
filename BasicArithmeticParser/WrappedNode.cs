using System.Text;

namespace BasicArithmeticParser;

public sealed class WrappedNode : ExpressionNode
{
	public WrappedNode(ExpressionNode expression) => Expression = expression;
	public ExpressionNode Expression { get; }

	public override void Dump(StringBuilder builder)
	{
		builder.Append("(");
		Expression.Dump(builder);
		builder.Append(")");
	}

	public override int ToDot(StringBuilder builder)
	{
		builder.Append($"{Id} [label=\"bracket\",shape=polygon,sides=8];\n");

		var child = Expression.ToDot(builder);
		builder.Append($"{Id} -> {child};\n");

		return Id;
	}
}
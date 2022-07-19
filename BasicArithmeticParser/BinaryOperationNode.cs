using System.Text;

namespace BasicArithmeticParser;

public enum BinaryOperators
{
	Add = 1,
	Subtract,
	Multiply,
	Divide,
	Modulo
}

public sealed class BinaryOperationNode : ExpressionNode
{
	public BinaryOperationNode(BinaryOperators? op, ExpressionNode? left, ExpressionNode? right)
	{
		Operator = op;
		Left = left;
		Right = right;
	}

	public BinaryOperators? Operator { get; set; }
	public ExpressionNode? Left { get; set; }
	public ExpressionNode? Right { get; set; }

	public override void Dump(StringBuilder builder)
	{
		void AppendLeft()
		{
			if (Left is BinaryOperationNode)
			{
				builder.Append('(');
				Left.Dump(builder);
				builder.Append(')');
			}
			else
			{
				Left.Dump(builder);
			}
		}

		void AppendRight()
		{
			if (Right is BinaryOperationNode)
			{
				builder.Append('(');
				Right.Dump(builder);
				builder.Append(')');
			}
			else
			{
				Right.Dump(builder);
			}
		}

		if (Left == null) throw new InvalidOperationException("Left is null");
		if (Right == null) throw new InvalidOperationException("Right is null");

		AppendLeft();
		builder.Append(GetOperatorSymbol());

		AppendRight();
	}

	private char GetOperatorSymbol()
	{
		return Operator switch
		{
			BinaryOperators.Add => '+',
			BinaryOperators.Subtract => '-',
			BinaryOperators.Multiply => '*',
			BinaryOperators.Divide => '/',
			BinaryOperators.Modulo => '%',
			_ => throw new InvalidOperationException()
		};
	}

	public override int ToDot(StringBuilder builder)
	{
		var left = Left!.ToDot(builder);
		var right = Right!.ToDot(builder);
		builder.AppendLine($"{Id} [label=\"{GetOperatorSymbol()}\"];\n");
		builder.AppendLine($"{Id} -> {left};\n");
		builder.AppendLine($"{Id} -> {right};\n");
		return Id;
	}
}
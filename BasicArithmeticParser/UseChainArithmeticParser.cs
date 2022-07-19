using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;
using ArithmeticParser = ParsecSharp.Parser<char, BasicArithmeticParser.ExpressionNode>;

namespace BasicArithmeticParser;

public static class UseChainArithmeticParser
{
	public static ArithmeticParser ChainParser(ChainDirection multitiveConfig, ChainDirection additiveConfig)
	{
		ArithmeticParser? impl = default;

		ArithmeticParser expression = Delay(() => impl!);

		ArithmeticParser number = Many1(Digit()).Select(ExpressionNode.Value);

		ArithmeticParser wrapped =
			from open in Char('(')
			from expr in expression
			from close in Char(')')
			select (ExpressionNode)new WrappedNode(expr);

		ArithmeticParser primary = wrapped.Or(number);

		Parser<char, BinaryOperators> additiveOp = Char('+').Select(_ => BinaryOperators.Add)
			.Or(Char('-').Select(_ => BinaryOperators.Subtract));

		Parser<char, BinaryOperators> multitiveOp = Choice(Char('*').Select(_ => BinaryOperators.Multiply),
			Char('/').Select(_ => BinaryOperators.Divide), Char('%').Select(_ => BinaryOperators.Modulo));

		Parser<char, Func<ExpressionNode, ExpressionNode, ExpressionNode>> multitiveChain = multitiveOp.Select(o =>
			(Func<ExpressionNode, ExpressionNode, ExpressionNode>)((left, right) =>
				new BinaryOperationNode(o, left, right)));

		ArithmeticParser multitive = multitiveConfig switch
		{
			ChainDirection.Left => primary.ChainLeft(multitiveChain),
			ChainDirection.Right => primary.ChainRight(multitiveChain),
			_ => throw new InvalidOperationException($"{multitiveConfig} is unexpected.")
		};


		Parser<char,Func<ExpressionNode,ExpressionNode,ExpressionNode>> additiveChain = additiveOp.Select(o =>
			(Func<ExpressionNode, ExpressionNode, ExpressionNode>)((left, right) =>
				new BinaryOperationNode(o, left, right)));

		ArithmeticParser additive = additiveConfig switch
		{
			ChainDirection.Left => multitive.ChainLeft(additiveChain),
			ChainDirection.Right => multitive.ChainRight(additiveChain),
			_ => throw new InvalidOperationException($"{additiveConfig} is unexpected.")
		};

		impl = additive;
		return expression;
	}

}
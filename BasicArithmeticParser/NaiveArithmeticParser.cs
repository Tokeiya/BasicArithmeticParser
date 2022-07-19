using static ParsecSharp.Parser;
using static ParsecSharp.Text;
using ArithmeticParser = ParsecSharp.Parser<char, BasicArithmeticParser.ExpressionNode>;


namespace BasicArithmeticParser;

public enum ChainDirection
{
	Left = 1,
	Right
}

static class NaiveArithmeticParser
{
	private static ExpressionNode LeftBuild(ExpressionNode left,
		IEnumerable<(BinaryOperators, ExpressionNode)> following)
	{
		if (!following.Any()) return left;

		var piv = following.First();
		var current = ExpressionNode.BinaryOperation(piv.Item1, left, piv.Item2);

		foreach (var elem in following.Skip(1)) current = new BinaryOperationNode(elem.Item1, current, elem.Item2);

		return current;
	}

	private static ExpressionNode RightBuild(ExpressionNode left,
		IEnumerable<(BinaryOperators, ExpressionNode)> following)
	{
		if (!following.Any()) return left;

		var piv = new BinaryOperationNode(null, left, null);
		var recent = piv;
		var ret = piv;

		foreach (var elem in following)
		{
			piv.Operator = elem.Item1;
			piv.Right = new BinaryOperationNode(null, elem.Item2, null);

			recent = piv;
			piv = (BinaryOperationNode)piv.Right;
		}

		recent.Right = piv.Left;

		return ret;

	}


	public static ArithmeticParser NaiveParser(ChainDirection multitiveDirection, ChainDirection additiveDirection)
	{
		Func<ExpressionNode, IEnumerable<(BinaryOperators, ExpressionNode)>, ExpressionNode> multChain =
			multitiveDirection switch
			{
				ChainDirection.Left => LeftBuild,
				ChainDirection.Right => RightBuild,
				_ => throw new ArgumentException("multitiveDirection")
			};

		Func<ExpressionNode, IEnumerable<(BinaryOperators, ExpressionNode)>, ExpressionNode> addChain =
			additiveDirection switch
			{
				ChainDirection.Left => LeftBuild,
				ChainDirection.Right => RightBuild,
				_ => throw new ArgumentException("additiveDirection")
			};




		ArithmeticParser? impl = default;

		var expression = Delay(() => impl!);

		var number = Many1(Digit()).Select(ExpressionNode.Value);

		var wrapped =
			from open in Char('(')
			from expr in expression
			from close in Char(')')
			select (ExpressionNode)new WrappedNode(expr);

		var primary = wrapped.Or(number);

		var additiveOp = Char('+').Select(_ => BinaryOperators.Add).Or(Char('-').Select(_ => BinaryOperators.Subtract));

		var multitiveOp = Choice(Char('*').Select(_ => BinaryOperators.Multiply),
			Char('/').Select(_ => BinaryOperators.Divide), Char('%').Select(_ => BinaryOperators.Modulo));

		var tmpA = from op in multitiveOp
			from expr in primary
			select (op, expr);

		var multitive =
			from left in primary
			from following in Many(tmpA)
			select multChain(left, following);

		var tmp = from op in additiveOp
			from expr in multitive
			select (op, expr);

		var additive =
			from left in multitive
			from following in Many(tmp)
			select addChain(left, following);

		impl = additive;

		return expression;
	}
}
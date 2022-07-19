using BasicArithmeticParser;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;
using ArithmeticParser = ParsecSharp.Parser<char, BasicArithmeticParser.ExpressionNode>;

public static class MainEntry
{

	public static ArithmeticParser ChainParser()
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

		ArithmeticParser multitive = primary.ChainLeft(multitiveChain);


		Parser<char,Func<ExpressionNode,ExpressionNode,ExpressionNode>> additiveChain = additiveOp.Select(o =>
			(Func<ExpressionNode, ExpressionNode, ExpressionNode>)((left, right) =>
				new BinaryOperationNode(o, left, right)));

		ArithmeticParser additive = multitive.ChainLeft(additiveChain);
		
		impl = additive;
		return expression;
	}
	
	private static void Main()
	{
		var parser=Parser.Fix<char, ExpressionNode>(p =>
		{
			ArithmeticParser number = Many1(Digit()).Select(ExpressionNode.Value);

			ArithmeticParser wrapped =
				from open in Char('(')
				from expr in p
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

			ArithmeticParser multitive = primary.ChainLeft(multitiveChain);


			Parser<char, Func<ExpressionNode, ExpressionNode, ExpressionNode>> additiveChain = additiveOp.Select(o =>
				(Func<ExpressionNode, ExpressionNode, ExpressionNode>)((left, right) =>
					new BinaryOperationNode(o, left, right)));

			ArithmeticParser additive = multitive.ChainLeft(additiveChain);

			return additive;
		});
		
		var p=parser.Parse("1+2*3");
		Console.WriteLine(p.Value.Dump());
		

	}
}
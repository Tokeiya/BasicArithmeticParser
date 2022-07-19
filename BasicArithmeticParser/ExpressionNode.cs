using System.Text;

namespace BasicArithmeticParser;

public abstract class ExpressionNode
{
	private static int _indexSeed = -1;


	protected ExpressionNode() => Id = GetId();

	public int Id { get; }

	public static ExpressionNode Value(IEnumerable<char> source)
	{
		var bld = new StringBuilder();
		foreach (var c in source) bld.Append(c);

		return new ValueNode(bld.ToString());
	}

	public static ExpressionNode Wrapped(ExpressionNode inner) => new WrappedNode(inner);

	public static ExpressionNode BinaryOperation(BinaryOperators binaryOperator, ExpressionNode? lhs,
		ExpressionNode? rhs) => new BinaryOperationNode(binaryOperator, lhs, rhs);

	public static int GetId() => Interlocked.Increment(ref _indexSeed);

	public abstract void Dump(StringBuilder builder);

	public abstract int ToDot(StringBuilder builder);

	public string Dump()
	{
		var bld = new StringBuilder();
		Dump(bld);
		return bld.ToString();
	}

	public string ToDot()
	{
		var bld = new StringBuilder();

		bld.Append("digraph expression_tree{\n");

		bld.Append("subgraph clusterHeader {\n    margin=0\n    style=\"invis\"\n    ");
		bld.Append($"HEADER [shape=\"plain\" label=\"{Dump()}\"]; \n");
		bld.Append(" }\n ");

		bld.Append("subgraph tree{\n");
		ToDot(bld);

		bld.Append($"HEADER -> {Id} [style=\"invis\"];\n");
		bld.Append("}\n");

		bld.Append("}\n");

		return bld.ToString();
	}

	public override string ToString()
	{
		return Dump();
	}
}
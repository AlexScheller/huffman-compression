public class LeafNode : SymbolNode
{
	public char symbol;
	public override bool isLeaf => true;

	public LeafNode(char symbol, double weight)
	{
		this.weight = weight;
		this.symbol = symbol;
		this.parent = null;
	}

	// For debugging and testing purposes.
	public override string ToString()
	{
		return "{" + symbol + ":" + weight.ToString("F5") + "}";
	}
}
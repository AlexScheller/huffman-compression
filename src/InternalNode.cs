public class InternalNode : SymbolNode
{
	public SymbolNode leftChild;
	public SymbolNode rightChild;
	public override bool isLeaf => false;

	public InternalNode(SymbolNode left, SymbolNode right)
	{
		this.weight = left.weight + right.weight;
		this.leftChild = left;
		this.rightChild = right;
		leftChild.isLeftChild = true;
		leftChild.parent = this;
		rightChild.parent = this;
	}

	// For debugging and testing purposes.
	public override string ToString()
	{
		return "{" + weight.ToString("F5") + "}";
	}
}
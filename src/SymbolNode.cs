/*
 * SymbolNode.cs is the abstract base class meant to be
 * overridden by LeafNode.c and InternalNode.cs for usage
 * in construction of the Huffman tree.
 */
public abstract class SymbolNode
{
	public double weight;
	public InternalNode parent;
 
	public bool isLeftChild = false;

	public abstract bool isLeaf { get; }

	// For testing and debugging purposes.
	public abstract override string ToString();
}
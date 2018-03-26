using System.Collections.Generic;
using SymbolToLeafNode = System.Collections.Generic.Dictionary<char, LeafNode>;

public class HuffmanTree
{
	private SymbolToLeafNode leaves;

	public HuffmanTree(Dictionary<char, int> symbolCounts)
	{
		// create a new leaf node for each symbol and assign
		// it's weight based on count.
		this.leaves = GenerateLeaves(symbolCounts);
		// build the tree from the generated leaves.
		BuildTree();
	}

	private SymbolToLeafNode GenerateLeaves(Dictionary<char, int> symbolCounts)
	{
		int total = 0;
		foreach (int val in symbolCounts.Values) {
			total += val;
		}

		SymbolToLeafNode ret = new SymbolToLeafNode();
		foreach (var entry in symbolCounts) {
			ret.Add(entry.Key, new LeafNode(entry.Key, (double) entry.Value / total));
		}
		return ret;
	}

	// Note that this depopulates the priority queue.
	private void BuildTree()
	{
		SymbolPriorityQueue spq = new SymbolPriorityQueue(leaves);
		
		// Coalesce the leaves into internal nodes
		// with combined weights until there is only
		// one left.

		// The creation of an internal node from two other
		// nodes assigns the proper parent-child relationships
		// to be used later in code generation. This results
		// in a tree where every leaf has a unique path from
		// the root (the last node left), and leaves with
		// higher weights, i.e. higher prevalence in the
		// document to compress, have shorter paths.
		while (spq.Count > 1) {
			SymbolNode left = spq.ExtractMin();
			SymbolNode right = spq.ExtractMin();
			spq.Add(new InternalNode(left, right));
		}
		spq.ExtractMin();
	}

	public CodeBook GenerateCodeBook()
	{
		return new CodeBook(leaves);
	}

}
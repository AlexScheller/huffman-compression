using System.Collections.Generic;
using SymbolToLeafNode = System.Collections.Generic.Dictionary<char, LeafNode>;

public class HuffmanTree
{

	// private readonly int MAX_UNIQUE_SYMBOLS;
	private SymbolToLeafNode leaves;

	public HuffmanTree(Dictionary<char, int> symbolCounts)
	{
		// this.MAX_UNIQUE_SYMBOLS = MAX_UNIQUE_SYMBOLS;
		this.leaves = GenerateLeaves(symbolCounts);
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
		// Coalesce the leaves into internal nodes, until
		// There is only one left.
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
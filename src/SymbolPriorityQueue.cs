/*
 * SymbolPriorityQueue.cs is a heap implementation of a priority
 * queue for usage in the construction of the Huffman tree.
 */
using System;
using System.Text;
using System.Collections.Generic;

public class SymbolPriorityQueue
{
	private SymbolNode[] heap;
	private int bottom;

	// Note that the size of the heap will always be equal to
	// or less than the size of the initial number of leaves,
	// so there is no need to worry about reallocating/increasing
	// the size of the SymbolNode array.
	public int Count => bottom;

	public SymbolPriorityQueue(Dictionary<char, LeafNode> leaves)
	{
		this.bottom = leaves.Count;
		this.heap = new SymbolNode[leaves.Count];
		int i = 0;
		foreach (KeyValuePair<char, LeafNode> entry in leaves) {
			heap[i] = entry.Value;
			i++;
		}
		BuildMinHeap();
	}

	private void Swap(ref SymbolNode a, ref SymbolNode b)
	{
		SymbolNode temp = a;
		a = b;
		b = temp;
	}

	private int Parent(int i) {
		return i / 2;
	}

	private int LeftChild(int i) {
		return (i * 2) + 1;
	}

	private int RightChild(int i) {
		return (i * 2) + 2;
	}

	private void BuildMinHeap()
	{
		int middle = bottom / 2;
		for (int i = middle; i >= 0; i--) {
			MinHeapify(i);
		}
	}

	private void MinHeapify(int i)
	{
		int smallest = i;
		int left = LeftChild(i);
		int right = RightChild(i);
		if (left < bottom && heap[left].weight < heap[smallest].weight) {
			smallest = left;
		}
		if (right < bottom && heap[right].weight < heap[smallest].weight) {
			smallest = right;
		}
		if (smallest != i) {
			Swap(ref heap[i], ref heap[smallest]);
			MinHeapify(smallest);
		}
	}

	public void Add(SymbolNode sn) {
		heap[bottom] = sn;
		int dex = bottom;
		while (dex > 0 && heap[Parent(dex)].weight > heap[dex].weight) {
			Swap(ref heap[Parent(dex)], ref heap[dex]);
			dex = Parent(dex);
		}
		bottom++;
	}

	public SymbolNode ExtractMin() {
		Swap(ref heap[0], ref heap[bottom - 1]);
		bottom--;
		MinHeapify(0);
		return heap[bottom];
	}

	// The following methods are for testing and debugging

	private void ValidateWeights()
	{
		double total = 0.0;
		foreach (SymbolNode sn in heap) {
			total += sn.weight;
		}
		// should equal 100.0
		Console.WriteLine(total.ToString("F5"));
	}

	// Note: there is no need to account for the edge cases
	// where the size of the heap is 1 or 0, as files with those
	// symbols counts are compressed differently and don't use this
	// data structure.
	public override string ToString() {
		StringBuilder sb = new StringBuilder();
		sb.Append('[');
		sb.Append(heap[0].ToString());
		for (int i = 1; i < bottom; i++) {
			sb.Append(", ");
			sb.Append(heap[i].ToString());
		}
		sb.Append(']');
		return sb.ToString();
	}
}
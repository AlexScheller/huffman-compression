// TODO: finish
/*
 * The codebook class handles generation of the codebook from
 * the original Huffman tree build in Compressor.cs
 *
 * Canonical codes are used to make storage of the codebook,
 * as well as decoding of the compressed file much more efficient
 * than rebuilding and traversing the tree again.
 *
 */
using System;
using System.Text;
using System.Collections.Generic;
using SymbolsToLeaves = System.Collections.Generic.Dictionary<char, LeafNode>;

public class CodeBook
{

	private char[] symbols;
	private string[] codes;
	private byte[] lengths;
	private uint[] canonicalCodes;

	public int Count => symbols.Length;

	public CodeBook(SymbolsToLeaves leaves)
	{
		this.symbols = new char[leaves.Count];
		this.codes = new string[leaves.Count];
		this.lengths = new byte[leaves.Count];
		this.canonicalCodes = new uint[leaves.Count];
		GenHuffmanCodes(leaves);
		SortHuffmanCodes();
		GenCanonicalCodes();
	}

	public Dictionary<char, Tuple<byte, uint>> GetCodeDict()
	{
		Dictionary<char, Tuple<byte, uint>> ret = new Dictionary<char, Tuple<byte, uint>>();
		for (int i = 0; i < codes.Length; i++) {
			ret.Add(symbols[i], Tuple.Create(lengths[i], canonicalCodes[i]));
		}
		return ret;
	}

	public char GetSymbol(int i)
	{
		return symbols[i];
	}

	public byte GetLength(int i)
	{
		return lengths[i];
	}

	private void GenCanonicalCodes()
	{
		uint code = 0; // canonical code
		int i;
		for (i = 0; i < codes.Length - 1; i++) {
			byte codeLength = (byte) codes[i].Length;
			canonicalCodes[i] = code;
			lengths[i] = codeLength;
			code = code + 1 << (codes[i + 1].Length - codeLength);
		}
		canonicalCodes[i] = code;
		lengths[i] = (byte) codes[i].Length;
	}

	// Generate the original Huffman codes from the tree,
	// stores the characters in the class level array
	// symbols.
	private void GenHuffmanCodes(SymbolsToLeaves leaves)
	{
		StringBuilder sb = new StringBuilder();
		int i = 0;
		foreach (KeyValuePair<char, LeafNode> entry in leaves) {
			SymbolNode curr = (SymbolNode) entry.Value;
			while (curr.parent != null) {
				if (curr.isLeftChild) {
					sb.Append('1');
				} else {
					sb.Append('0');
				}
				curr = curr.parent;
			}
			symbols[i] = entry.Key;
			codes[i] = sb.ToString();
			sb.Clear();
			i++;
		}
	}

	// for usage with the sorting algorithm.
	private void Swap<T>(ref T a, ref T b)
	{
		T temp = a;
		a = b;
		b = temp;
	}

	// The following three methods comprise QuickSort.
	// Note that although QuickSort is used here, some
	// form of bucket sort could be used to sort in O(n)
	// time.
	private void SortHuffmanCodes()
	{
		QuickSort(0, codes.Length - 1);
	}

	private void QuickSort(int beg, int end)
	{
		if (beg < end) {
			int partition = Partition(beg, end);
			QuickSort(beg, partition - 1);
			QuickSort(partition + 1, end);
		}
	}

	private int Partition(int beg, int end)
	{
		string pivotCode = codes[end];
		char pivotSymbol = symbols[end];
		// rightmost index of the left partition.
		int lesserEnd = beg - 1;
		for (int i = beg; i < end; i++) {
			if (codes[i].Length <= pivotCode.Length) {
				// Following the algorithm for generating
				// canonical codes, ties must be broken
				// by symbol precedence.
				if (codes[i].Length != pivotCode.Length ||
					symbols[i] < pivotSymbol) {
					lesserEnd++;
					Swap(ref codes[lesserEnd], ref codes[i]);
					Swap(ref symbols[lesserEnd], ref symbols[i]);
				}
			}
		}
		Swap(ref codes[lesserEnd + 1], ref codes[end]);
		Swap(ref symbols[lesserEnd + 1], ref symbols[end]);
		return lesserEnd + 1;
	}

	// The following methods are for testing and debugging purposes
	
	private void ValidateSorted()
	{
		for (int i = 0; i < codes.Length - 1; i++) {
			if (codes[i].Length >= codes[i + 1].Length) {
				if (codes[i].Length == codes[i + 1].Length) {
					if (symbols[i] > symbols[i + 1]) {
						Console.WriteLine("improper sorting");
						Environment.Exit(1);
					}
				} else {
					Console.WriteLine("improper sorting");
					Environment.Exit(1);
				}
			}
		}
		Console.WriteLine("sort order is valid");
	}

	private void PrintSorted()
	{
		for (int i = 0; i < codes.Length; i++) {
			Console.WriteLine("{0}: {1}", symbols[i], codes[i]);
		}
	}

	private void PrintFinished()
	{
		Console.WriteLine("\nfinished:");
		for (int i = 0; i < codes.Length; i++) {
			Console.Write("{0}: ({1})", symbols[i], lengths[i]);
			string bitString = Convert.ToString(canonicalCodes[i], 2);
			Console.WriteLine(" - {0} - {1}", codes[i], bitString);
		}
	}

}
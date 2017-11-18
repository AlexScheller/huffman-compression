using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using SymbolsToCounts = System.Collections.Generic.Dictionary<char, int>;
using SymbolsToLeaves = System.Collections.Generic.Dictionary<char, LeafNode>;

public class Compressor
{

	private static readonly int MAX_UNIQUE_SYMBOLS = 255;

	public static void Main(string[] args)
	{
		if (args.Length > 0) {
			Compress(args[0]);
		} else {
			ExitOnError("no file specified");
		}
	}

	private static void PrintUsage()
	{
		Console.WriteLine("huff <file-to-compress>");
	}

	private static void ExitOnError(string msg)
	{
		Console.WriteLine("Error: {0}", msg);
		PrintUsage();
		Environment.Exit(1);
	}

	// // Determines an appearance count for each symbol.
	private static SymbolsToCounts GenCounts(char[] symbols)
	{
		SymbolsToCounts ret = new SymbolsToCounts();
		foreach (char c in symbols) {
			if (ret.ContainsKey(c)) {
				ret[c]++;
			} else {
				ret.Add(c, 1);
			}
		}
		return ret;
	}

	// If for some reason there exists only one unique symbol
	// compression consists of the symbol, and its count.
	private static void CountCompression(BinaryWriter bw, char symbol,
										 int count)
	{
		// A zero is written to indicate count compression
		bw.Write((byte)0);
		bw.Write(symbol);
		bw.Write(count);
	}

	private static void WriteCodeBook(BinaryWriter bw, CodeBook cb)
	{
		for (int i = 0; i < cb.Count; i++) {
			bw.Write(cb.GetSymbol(i));
			bw.Write(cb.GetLength(i));
		}
	}

	private static void HuffmanCompression(BinaryWriter bw, char[] inputSymbols,
									CodeBook cb)
	{
		// write the header
		bw.Write((byte)1); // one for Huffman
		bw.Write(inputSymbols.Length); // chars in file
		bw.Write(cb.Count);

		WriteCodeBook(bw, cb);
		
		// encode the file
		Dictionary<char, Tuple<byte, uint>> canonicalCodes = cb.GetCodeDict();
		int bitCount = 0;
		byte currByte = 0;
		uint LAST_BIT_MASK = 0x80000000;
		for (int i = 0; i < inputSymbols.Length; i++) {
			Tuple<byte, uint> tup = canonicalCodes[inputSymbols[i]];
			int length = tup.Item1;
			uint code = tup.Item2;
			code <<= (32 - length); // subtract size of a uint
			while (length > 0) {
				currByte <<= 1;
				bitCount++;
				if ((code & LAST_BIT_MASK) > 0) {
					currByte |= 1;
				}
				code <<= 1;
				if (bitCount == 8) {
					bw.Write(currByte);
					currByte = 0;
					bitCount = 0;
				}
				length--;
			}
		}
		// write last byte;
		if (bitCount > 0) {
			// Align it.
			while (bitCount < 8) {
				bitCount++;
				currByte <<= 1;
			}
			bw.Write(currByte);
		}
	}

	public static void Compress(string filename)
	{
		StreamReader sr = new StreamReader(filename);
		if (sr.BaseStream.Length == 0) {
			ExitOnError("empty file");
		}

		// The file must be read character by character
		// due to the fact that characters may differ in
		// byte length, so sr.Read(char[], int32, int32)
		// cannot be reliably used unless we know all
		// characters are encoded with a single byte.
		List<char> symbolList = new List<char>();
		while (sr.Peek() > -1) { // still symbols left
			symbolList.Add((char)sr.Read());
		}
		char[] symbols = symbolList.ToArray();

		SymbolsToCounts symbolCounts = GenCounts(symbols);
		if (symbolCounts.Count > MAX_UNIQUE_SYMBOLS) {
			ExitOnError("too many unique symbols to encode");
		}
		using (BinaryWriter bw = new BinaryWriter
			(new FileStream(filename + ".huf", FileMode.Create))) {
			if (symbolCounts.Count == 1) {
				CountCompression(bw, symbols[0], symbols.Length);
			} else {
			 	HuffmanTree ht = new HuffmanTree(symbolCounts);
			 	CodeBook cb = ht.GenerateCodeBook();
			 	HuffmanCompression(bw, symbols, cb);
			}
		}
	}

}
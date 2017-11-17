// TODO: finish
using System;
using System.IO;

public class Decompressor
{

	private static readonly int MAX_CODE_LENGTH = 65535;

	public static void Main(string[] args)
	{
		if (args.Length == 1) {
			string filename = args[0];
			int extIndex = filename.Length - 4;
			string extension = filename.Substring(extIndex, 4);
			if (extension == ".huf") {
				Decompress(filename);
			} else {
				Console.WriteLine("Not a .huf file");
				Environment.Exit(1);
			}
		} else {
			Console.WriteLine("Please specify a single \".huf\" file");
		}
	}

	private static char DecodeNextChar(BitBuffer bb, int[] counts,
									   char[] symbols)
	{
		int code = 0;
		int count = 0;
		int first = 0;
		int index = 0;
		for (int len = 1; len <= MAX_CODE_LENGTH; len++) {
			// Console.Write("variables - code: {0}, count: {1}", code, count);
			// Console.WriteLine(", first: {0}, index: {1}", first, index);
			if (bb.IsEmpty) {
				// Console.WriteLine("end of bit stream");
				break;
			}
			code |= bb.NextBit();
			count = counts[len];
			if (code - count < first) {
				char symbol = symbols[index + (code - first)];
				// Console.WriteLine("symbol found: {0}", symbol);
				return symbol;
			}
			index += count;
			first += count;
			first <<= 1;
			code <<= 1;
		}
		return 'ß'; // signals an error
	}

	private static void HuffmanDecompress(BinaryReader br, StreamWriter sw)
	{
		int numSymbols = (int)br.ReadUInt32();
		int numUniqueSymbols = (int)br.ReadUInt16();
		char[] symbols = new char[numUniqueSymbols];
		byte[] lengths = new byte[numUniqueSymbols];
		for (int i = 0; i < numUniqueSymbols; i++) {
			symbols[i] = br.ReadChar();
			lengths[i] = br.ReadByte();
		}
		// Console.WriteLine("symbols and lengths discovered:");
		// for (int i = 0; i < numUniqueSymbols; i++) {
		// 	Console.WriteLine("{0}: {1}", symbols[i], lengths[i]);
		// }
		
		// an array mapping code length to number of symbols
		// represented with that code length, the first index
		// is unused.
		int[] counts = new int[lengths[lengths.Length - 1] + 1];
		int currLength = 1;
		int currCount = 0;
		for (int i = 0; i < lengths.Length; i++) {
			if (lengths[i] > currLength) {
				counts[currLength] = currCount;
				currCount = 1;
				currLength = lengths[i];
			} else {
				currCount++;
			}
		}
		counts[currLength] = currCount;
		long bytesLeft = br.BaseStream.Length - br.BaseStream.Position;
		BitBuffer bb = new BitBuffer(br.ReadBytes((int)bytesLeft));
		char curr = DecodeNextChar(bb, counts, symbols);
		while (numSymbols > 0) {
			// Console.Write(curr);
			sw.Write(curr);
			curr = DecodeNextChar(bb, counts, symbols);
			numSymbols--;
		}
		// Console.WriteLine();
		// for (int i = 1; i < counts.Length; i++) {
		// 	Console.WriteLine("length: {0}, count: {1}", i, counts[i]);
		// }


	}

	private static void Decompress(string filename)
	{
		using (BinaryReader br = new BinaryReader(
			new FileStream(filename, FileMode.Open)))
		using (StreamWriter sw = new StreamWriter(
			new FileStream(filename + ".dehuf", FileMode.Create))) {
			bool huffmanEncoded = br.ReadBoolean();
			if (huffmanEncoded) {
				// Console.WriteLine("huffman");
				HuffmanDecompress(br, sw);
			} else {
				// Console.WriteLine("count");
				char symbol = br.ReadChar();
				int num = br.ReadInt32();
				for (int i = 0; i < num; i++) {
					// Console.Write(symbol);
					sw.Write(symbol);	
				}
				// Console.WriteLine();
			}
		}
	}

}
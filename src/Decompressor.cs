using System;
using System.IO;

public class Decompressor
{

	private static readonly int MAX_CODE_LENGTH = 255;

	public static void Main(string[] args)
	{
		if (args.Length == 1) {
			string filename = args[0];
			int extIndex = filename.Length - 4;
			string extension = filename.Substring(extIndex, 4);
			if (extension == ".huf") {
				Decompress(filename);
			} else {
				ExitOnError("not a .huf file");
			}
		} else {
			ExitOnError("Please specify a single \".huf\" file");
		}
	}

	private static void PrintUsage()
	{
		Console.WriteLine("dehuff <file-to-decompress>");
	}

	private static void ExitOnError(string msg)
	{
		Console.WriteLine("Error: {0}", msg);
		PrintUsage();
		Environment.Exit(1);
	}

	private static char DecodeNextChar(BitBuffer bb, int[] counts,
									   char[] symbols)
	{
		int code = 0;
		int count = 0;
		int first = 0;
		int index = 0;
		for (int len = 1; len <= MAX_CODE_LENGTH; len++) {
			if (bb.IsEmpty) {
				ExitOnError("bit buffer emptied without decoding symbol");
				// break;
			}
			code |= bb.NextBit();
			count = counts[len];
			if (code - count < first) {
				char symbol = symbols[index + (code - first)];
				return symbol;
			}
			index += count;
			first += count;
			first <<= 1;
			code <<= 1;
		}
		ExitOnError("max code length reached while decoding without a matching symbol");
		return 'ß'; // unreachable
	}

	private static void HuffmanDecompress(BinaryReader br, StreamWriter sw)
	{
		// process the header
		int numSymbols = (int)br.ReadUInt32();
		int numUniqueSymbols = (int)br.ReadByte();
		
		// read in the codebook
		char[] symbols = new char[numUniqueSymbols];
		byte[] lengths = new byte[numUniqueSymbols];
		for (int i = 0; i < numUniqueSymbols; i++) {
			symbols[i] = br.ReadChar();
			lengths[i] = br.ReadByte();
		}

		// an array mapping code length to number of symbols
		// represented with that code length, the first index
		// is unused as no code can be of length 0.

		// lengths[lengths.Length - 1] refers to the longest
		// code length, therefore counts only has to be this long.
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
		
		// fill the bit buffer with the rest of the file
		long bytesLeft = br.BaseStream.Length - br.BaseStream.Position;
		BitBuffer bb = new BitBuffer(br.ReadBytes((int)bytesLeft));
		
		// decode the file
		while (numSymbols > 0) {
			sw.Write(DecodeNextChar(bb, counts, symbols));
			numSymbols--;
		}
	}

	private static void Decompress(string filename)
	{
		// remove the ".huf" extension
		string outputName = filename.Substring(0, filename.Length - 4);
		using (BinaryReader br = new BinaryReader(
			new FileStream(filename, FileMode.Open)))
		using (StreamWriter sw = new StreamWriter(
			new FileStream(outputName, FileMode.Create))) {
			// first byte indicates which compression was used
			bool huffmanEncoded = br.ReadBoolean();
			if (huffmanEncoded) {
				HuffmanDecompress(br, sw);
			} else {
				// count compression was used
				char symbol = br.ReadChar();
				int num = br.ReadInt32();
				for (int i = 0; i < num; i++) {
					sw.Write(symbol);	
				}
			}
		}
	}

}
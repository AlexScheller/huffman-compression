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

	private static void ExitOnError(string error)
	{
		Console.WriteLine("Error: {0}", msg);
		PrintUsage();
		Environment.Exit(1);
	}

	// TODO: write an explanation
	private static char DecodeNextChar(BitBuffer bb, int[] counts,
									   char[] symbols)
	{
		int code = 0;
		int count = 0;
		int first = 0;
		int index = 0;
		for (int len = 1; len <= MAX_CODE_LENGTH; len++) {
			if (bb.IsEmpty) {
				break;
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
		ExitOnError("unable to decode character");
		return 'ß';
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
		
		// fill he bit buffer with the rest of the file
		long bytesLeft = br.BaseStream.Length - br.BaseStream.Position;
		BitBuffer bb = new BitBuffer(br.ReadBytes((int)bytesLeft));
		
		// decode the file
		char curr = DecodeNextChar(bb, counts, symbols);
		while (numSymbols > 0) {
			sw.Write(curr);
			curr = DecodeNextChar(bb, counts, symbols);
			numSymbols--;
		}

	}

	private static void Decompress(string filename)
	{
		using (BinaryReader br = new BinaryReader(
			new FileStream(filename, FileMode.Open)))
		using (StreamWriter sw = new StreamWriter(
			new FileStream(filename + ".dehuf", FileMode.Create))) {
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
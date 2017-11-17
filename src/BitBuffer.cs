/*
 * BitBuffer.cs manages accessing a single bit at a time from
 * the bytes read by the BinaryReader.
 */
using System;
public class BitBuffer
{
	
	private byte[] bytes;
	private int curr; // index into bytes
	private int bitsLeftInCurr;
	
	// for accessing the last bit in a byte
	private readonly byte LAST_BIT_MASK = 0x80;

	public bool IsEmpty => curr == bytes.Length;

	public BitBuffer(byte[] bytes)
	{
		this.bytes = bytes;
		this.curr = 0;
		this.bitsLeftInCurr = 8;
	}

	// Returns a byte with either a 1 or a 0 in the first
	// digit, based on the next bit in the stream.
	public byte NextBit()
	{
		if (curr < bytes.Length) {
			byte ret = (byte)((bytes[curr] & LAST_BIT_MASK) >> 7);
			bitsLeftInCurr--;
			if (bitsLeftInCurr == 0) {
				bitsLeftInCurr = 8;
				curr++;
			} else {
				bytes[curr] <<= 1;
			}
			return ret;
		}
		// In theory this line should never be reached, as the calling
		// function in Compressor.cs "DecodeNextChar" is only called when
		// there are still characters to decode.
		Console.WriteLine("Error: call to NextBit on exhausted BitBuffer.");
		Environment.Exit(1);
		return 3;
	}
}
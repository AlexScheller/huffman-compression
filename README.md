# Canonical Huffman Compression

**An implementation of file compression with Canonical Huffman Encoding.**

Format for the outputted file:

section format: [(byte count)contents]

if Huffman compression is used:

```[(1) 1, indicating Huffman compression][(4) number of input symbols (not necessarily the length of the file in bytes however)][(2) length of the codebook (and therefore number of unique symbols)][(var)code book][(var)encoded file]```

The codebook is stored as follows:

```(...[(1-2)Unicode Character][(1)length of the character's code]...)```

Note that because the length of the characters code is expressed
by a single byte, the maximum length (and therefore maximum count
of unique symbols) is limited to 65,535.

if count compression is used:

```[(1) 0, indicating count compression][(1-2)symbol][(4)count]```
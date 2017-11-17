# Canonical Huffman Compression

**An implementation of file compression with Canonical Huffman Encoding.**

Format for the outputted file:

section format: [(byte count)contents]

if Huffman compression is used:

```[(2)header][(var)extension][(var)code book][(var)encoded file]```

The codebook is stored as follows:

```(...[(2)Unicode Character][(1)length of the character's code]...)```

Note that because the length of the characters code is expressed
by a single byte, the maximum length (and therefore maximum count
of unique symbols) is limited to 65,535, so please don't abuse the
compressor with files with more unique characters than it can handle.

if count compression is used:

```[(3)header][(var)extension][(2)symbol][(var)count]```

either way the header format is:

```[]```
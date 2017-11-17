all: compressor decompressor

compressor:
	mcs -out:huff Compressor.cs SymbolNode.cs InternalNode.cs LeafNode.cs \
	HuffmanTree.cs SymbolPriorityQueue.cs CodeBook.cs

decompressor:
	mcs -out:dehuff Decompressor.cs BitBuffer.cs

clean:
	-rm ../test-data/*.huf
	-rm ../test-data/*.dehuf
	-rm huff
	-rm dehuff

clena: clean

todo:
	grep -n -r -A 1 "TODO" --exclude=makefile
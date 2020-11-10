using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2018.Day08
{
    class Day08
    {
        public static void Puzzle1()
        {
            /*
             * --- Day 8: Memory Maneuver ---
             * The sleigh is much easier to pull than you'd expect for something its weight. 
             * Unfortunately, neither you nor the Elves know which way the North Pole is from here.
             * 
             * You check your wrist device for anything that might help. It seems to have some kind 
             * of navigation system! Activating the navigation system produces more bad news: "Failed 
             * to start navigation system. Could not read software license file."
             * 
             * The navigation system's license file consists of a list of numbers (your puzzle input). 
             * The numbers define a data structure which, when processed, produces some kind of tree 
             * that can be used to calculate the license number.
             * 
             * The tree is made up of nodes; a single, outermost node forms the tree's root, and it 
             * contains all other nodes in the tree (or contains nodes that contain nodes, and so on).
             * 
             * Specifically, a node consists of:
             * 
             * A header, which is always exactly two numbers:
             * The quantity of child nodes.
             * The quantity of metadata entries.
             * Zero or more child nodes (as specified in the header).
             * One or more metadata entries (as specified in the header).
             * Each child node is itself a node that has its own header, child nodes, and metadata. 
             * For example:
             * 
             * 2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2
             * A----------------------------------
             *     B----------- C-----------
             *                      D-----
             * In this example, each node of the tree is also marked with an underline starting with 
             * a letter for easier identification. In it, there are four nodes:
             * 
             * A, which has 2 child nodes (B, C) and 3 metadata entries (1, 1, 2).
             * B, which has 0 child nodes and 3 metadata entries (10, 11, 12).
             * C, which has 1 child node (D) and 1 metadata entry (2).
             * D, which has 0 child nodes and 1 metadata entry (99).
             * The first check done on the license file is to simply add up all of the metadata entries. 
             * In this example, that sum is 1+1+2+10+11+12+2+99=138.
             * 
             * What is the sum of all metadata entries?
             */

            var numbers = ReadInput();
            var root = ParseNode(numbers);
            Console.WriteLine("The sum of all metadata entries is {0}.", root.SubTreeMetadataSum());
        }

        private static List<int> ReadInput()
        {
            var numbers = new List<int>();

            foreach (var number in File.ReadAllText(@"..\..\..\Day08\input.txt").Split(" "))
                numbers.Add(Convert.ToInt32(number));

            return numbers;
        }
        
        private static Node ParseNode(List<int> numbers)
        {
            var childrenCount = numbers[0];
            var metadataCount = numbers[1];
            var node = new Node();
            // index where to start processing metadata/another node
            var offset = 2;

            if (childrenCount == 0)
            {
                // use the offset and add metadata to the node
                for (var i = offset; i < offset + metadataCount; i++)
                {
                    var metadata = numbers[i];
                    node.AddMetadata(metadata);
                }
            }
            else
            {
                // use the offset and create the first child (we know children and metadata count, 
                // which are on indexes offset and offset + 1)
                var child = ParseNode(numbers.GetRange(offset, numbers.Count - offset));
                node.AddChild(child);

                for (var i = 1; i < childrenCount; i++)
                {
                    var previousChild = node.Children[i - 1];
                    // use data length of the previous child to shift the offset
                    offset += previousChild.DataLength();
                    var nextChild = ParseNode(numbers.GetRange(offset, numbers.Count - offset));
                    node.AddChild(nextChild);
                }

                // use data length of the last child to shift the offset
                offset += node.Children[node.Children.Count - 1].DataLength();

                // use the offset and add metadata to the node
                for (var i = offset; i < offset + metadataCount; i++)
                    node.AddMetadata(numbers[i]);
            }
            return node;
        }

        public static void Puzzle2()
        {
            /*
             * --- Part Two ---
             * The second check is slightly more complicated: you need to find the value 
             * of the root node (A in the example above).
             * 
             * The value of a node depends on whether it has child nodes.
             * 
             * If a node has no child nodes, its value is the sum of its metadata entries. 
             * So, the value of node B is 10+11+12=33, and the value of node D is 99.
             * 
             * However, if a node does have child nodes, the metadata entries become indexes which 
             * refer to those child nodes. A metadata entry of 1 refers to the first child node, 
             * 2 to the second, 3 to the third, and so on. The value of this node is the sum of the 
             * values of the child nodes referenced by the metadata entries. If a referenced child 
             * node does not exist, that reference is skipped. A child node can be referenced multiple 
             * time and counts each time it is referenced. A metadata entry of 0 does not refer to 
             * any child node.
             * 
             * For example, again using the above nodes:
             * 
             * - Node C has one metadata entry, 2. Because node C has only one child node, 2 references 
             * a child node which does not exist, and so the value of node C is 0.
             * - Node A has three metadata entries: 1, 1, and 2. The 1 references node A's first child 
             * node, B, and the 2 references node A's second child node, C. Because node B has a value 
             * of 33 and node C has a value of 0, the value of node A is 33+33+0=66.
             * 
             * So, in this example, the value of the root node is 66.
             * 
             * What is the value of the root node?
             */

            var numbers = ReadInput();
            var root = ParseNode(numbers);
            Console.WriteLine("The value of the root node is {0}.", root.Value());
        }
    }

    public class Node
    {
        public List<Node> Children { get; } = new List<Node>();
        public List<int> Metadata { get; } = new List<int>();

        public void AddChild(Node child)
        {
            this.Children.Add(child);
        }

        public void AddMetadata(int metadata)
        {
            this.Metadata.Add(metadata);
        }

        public int DataLength()
        {
            return 2 + this.Metadata.Count + this.Children.Sum(child => child.DataLength());
        }

        public int SubTreeMetadataSum()
        {
            var sum = this.Metadata.Sum();

            foreach (var child in this.Children)
                sum += child.SubTreeMetadataSum();

            return sum;
        }

        public int Value()
        {
            if (this.Children.Count == 0)
            {
                return this.Metadata.Sum();
            }
            else
            {
                var value = 0;
                foreach (var metadata in this.Metadata)
                {
                    var i = metadata - 1;

                    if (i >= 0 && i < this.Children.Count)
                        value += this.Children[i].Value();
                }
                return value;
            }
        }
    }
}

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
    }
}

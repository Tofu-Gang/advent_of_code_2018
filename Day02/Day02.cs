﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2018.Day02
{
    public class Day02
    {
        public static void Puzzle1()
        {
            /*
             * --- Day 2: Inventory Management System ---
             * You stop falling through time, catch your breath, and check the 
             * screen on the device. "Destination reached. Current Year: 1518. 
             * Current Location: North Pole Utility Closet 83N10." You made it! 
             * Now, to find those anomalies.
             * 
             * Outside the utility closet, you hear footsteps and a voice. 
             * "...I'm not sure either. But now that so many people have chimneys, 
             * maybe he could sneak in that way?" Another voice responds, "Actually, 
             * we've been working on a new kind of suit that would let him fit 
             * through tight spaces like that. But, I heard that a few days ago, 
             * they lost the prototype fabric, the design plans, everything! Nobody 
             * on the team can even seem to remember important details of the project!"
             * 
             * "Wouldn't they have had enough fabric to fill several boxes in the 
             * warehouse? They'd be stored together, so the box IDs should be 
             * similar. Too bad it would take forever to search the warehouse for 
             * two similar box IDs..." They walk too far away to hear any more.
             * 
             * Late at night, you sneak to the warehouse - who knows what kinds of 
             * paradoxes you could cause if you were discovered - and use your 
             * fancy wrist device to quickly scan every box and produce a list of 
             * the likely candidates (your puzzle input).
             * 
             * To make sure you didn't miss any, you scan the likely candidate boxes 
             * again, counting the number that have an ID containing exactly two of 
             * any letter and then separately counting those with exactly three of 
             * any letter. You can multiply those two counts together to get a 
             * rudimentary checksum and compare it to what your device predicts.
             * 
             * For example, if you see the following box IDs:
             * 
             * abcdef contains no letters that appear exactly two or three times.
             * bababc contains two a and three b, so it counts for both.
             * abbcde contains two b, but no letter appears exactly three times.
             * abcccd contains three c, but no letter appears exactly two times.
             * aabcdd contains two a and two d, but it only counts once.
             * abcdee contains two e.
             * ababab contains three a and three b, but it only counts once.
             * 
             * Of these box IDs, four of them contain a letter which appears exactly 
             * twice, and three of them contain a letter which appears exactly three 
             * times. Multiplying these together produces a checksum of 4 * 3 = 12.
             * 
             * What is the checksum for your list of box IDs?
             */

            var lines = File.ReadLines(@"..\..\..\Day02\input.txt");
            var count2 = 0;
            var count3 = 0;

            foreach (var line in lines)
            {
                if (HasNSameChars(line, 2))
                    count2++;
                if (HasNSameChars(line, 3))
                    count3++;
            }

            Console.WriteLine("The checksum for the list of box IDs is {0}.", count2 * count3);
        }

        public static void Puzzle2()
        {
            /*
             * --- Part Two ---
             * Confident that your list of box IDs is complete, you're ready to find the boxes 
             * full of prototype fabric.
             * 
             * The boxes will have IDs which differ by exactly one character at the same position 
             * in both strings. For example, given the following box IDs:
             * 
             * abcde
             * fghij
             * klmno
             * pqrst
             * fguij
             * axcye
             * wvxyz
             * 
             * The IDs abcde and axcye are close, but they differ by two characters (the second and 
             * fourth). However, the IDs fghij and fguij differ by exactly one character, the third 
             * (h and u). Those must be the correct boxes.
             * 
             * What letters are common between the two correct box IDs? (In the example above, this 
             * is found by removing the differing character from either ID, producing fgij.)
             */

            var lines = File.ReadAllLines(@"..\..\..\Day02\input.txt");

            for (var i = 0; i < lines.Count(); i++)
            {
                for (var j = 0; j < lines.Count(); j++)
                {
                    var id1 = lines[i];
                    var id2 = lines[j];
                    var common = CommonPart(id1, id2);
                    if (common.Length == id1.Length - 1 && common.Length == id2.Length - 1)
                    {
                        Console.WriteLine("The letters that are common between the two correct box IDs are {0}.", common);
                        return;
                    }
                }
            }
        }

        private static bool HasNSameChars(string id, int n)
        {
            var uniques = new String(id.Distinct().ToArray());
            foreach (var uniqueChar in uniques)
                if (id.Count(c => c == uniqueChar) == n)
                    return true;

            return false;
        }

        private static string CommonPart(string id1, string id2)
        {
            string common = "";

            for (var i = 0; i < id1.Length; i++)
                if (id1[i] == id2[i])
                    common += id1[i];

            return common;
        }
    }
}

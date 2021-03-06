﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2018.Day03
{
    public class Day03
    {

        private static readonly string EMPTY = ".";
        private static readonly string OVERLAP = "X";

        public static void Puzzle1()
        {
            /*
             * --- Day 3: No Matter How You Slice It ---
             * The Elves managed to locate the chimney-squeeze prototype fabric 
             * for Santa's suit (thanks to someone who helpfully wrote its box IDs 
             * on the wall of the warehouse in the middle of the night). Unfortunately, 
             * anomalies are still affecting them - nobody can even agree on how to 
             * cut the fabric.
             * 
             * The whole piece of fabric they're working on is a very large square - 
             * - at least 1000 inches on each side.
             * 
             * Each Elf has made a claim about which area of fabric would be ideal for 
             * Santa's suit. All claims have an ID and consist of a single rectangle 
             * with edges parallel to the edges of the fabric. Each claim's rectangle 
             * is defined as follows:
             * 
             * The number of inches between the left edge of the fabric and the left 
             * edge of the rectangle.
             * The number of inches between the top edge of the fabric and the top 
             * edge of the rectangle.
             * The width of the rectangle in inches.
             * The height of the rectangle in inches.
             * 
             * A claim like #123 @ 3,2: 5x4 means that claim ID 123 specifies a rectangle 
             * 3 inches from the left edge, 2 inches from the top edge, 5 inches wide, 
             * and 4 inches tall. Visually, it claims the square inches of fabric 
             * represented by # (and ignores the square inches of fabric represented by .) 
             * in the diagram below:
             * 
             * ...........
             * ...........
             * ...#####...
             * ...#####...
             * ...#####...
             * ...#####...
             * ...........
             * ...........
             * ...........
             * 
             * The problem is that many of the claims overlap, causing two or more claims 
             * to cover part of the same areas. For example, consider the following claims:
             * 
             * #1 @ 1,3: 4x4
             * #2 @ 3,1: 4x4
             * #3 @ 5,5: 2x2
             * 
             * Visually, these claim the following areas:
             * 
             * ........
             * ...2222.
             * ...2222.
             * .11XX22.
             * .11XX22.
             * .111133.
             * .111133.
             * ........
             * 
             * The four square inches marked with X are claimed by both 1 and 2. (Claim 3, 
             * while adjacent to the others, does not overlap either of them.)
             * 
             * If the Elves all proceed with their own plans, none of them will have enough 
             * fabric. How many square inches of fabric are within two or more claims?
             */

            var fabric = CreateFabric();
            fabric = EnterClaims(fabric);
            Console.WriteLine("Number of square inches of fabric that are within two or more claims: {0}", GetCount(fabric, OVERLAP));
        }

        private static string GetId(string claim) => claim.Split("@")[0].Trim();
        private static int GetLeftPadding(string claim) => Convert.ToInt32(claim.Split("@")[1].Split(",")[0].Trim());
        private static int GetTopPadding(string claim) => Convert.ToInt32(claim.Split("@")[1].Split(",")[1].Split(":")[0].Trim());
        private static int GetWidth(string claim) => Convert.ToInt32(claim.Split(":")[1].Split("x")[0].Trim());
        private static int GetHeight(string claim) => Convert.ToInt32(claim.Split(":")[1].Split("x")[1].Trim());

        private static int GetFabricWidth()
        {
            var width = 0;
            var claims = File.ReadAllLines(@"..\..\..\Day03\input.txt");

            foreach (var claim in claims)
            {
                var claim_width = GetLeftPadding(claim) + GetWidth(claim);
                width = claim_width > width ? claim_width : width;
            }
            return width;
        }

        private static int GetFabricHeight()
        {
            var height = 0;
            var claims = File.ReadAllLines(@"..\..\..\Day03\input.txt");

            foreach(var claim in claims)
            {
                var claim_height = GetTopPadding(claim) + GetHeight(claim);
                height = claim_height > height ? claim_height : height;
            }
            return height;
        }

        private static string[,] CreateFabric()
        {
            var fabricWidth = GetFabricWidth();
            var fabricHeight = GetFabricHeight();
            var fabric = new string[fabricHeight, fabricWidth];

            for (var i = 0; i < fabricHeight; i++)
            {
                for (var j = 0; j < fabricWidth; j++)
                    fabric[i, j] = EMPTY;
            }
            return fabric;
        }

        private static string[,] EnterClaims(string[,] fabric)
        {
            var claims = File.ReadLines(@"..\..\..\Day03\input.txt");
            foreach (var claim in claims)
            {
                var topPadding = GetTopPadding(claim);
                var height = GetHeight(claim);
                var leftPadding = GetLeftPadding(claim);
                var width = GetWidth(claim);
                for (var i = topPadding; i < topPadding + height; i++)
                {
                    for (var j = leftPadding; j < leftPadding + width; j++)
                    {
                        if (fabric[i, j] == EMPTY)
                            fabric[i, j] = GetId(claim);
                        else if (fabric[i, j] != OVERLAP)
                            fabric[i, j] = OVERLAP;
                    }
                }
            }
            return fabric;
        }

        private static int GetCount(string[,] fabric, string value)
        {
            var count = 0;
            for (var i = 0; i < fabric.GetLength(0); i++)
            {
                for (var j = 0; j < fabric.GetLength(1); j++)
                {
                    if (fabric[i, j] == value)
                        count++;
                }
            }
            return count;
        }

        public static void Puzzle2()
        {
            /*
             * --- Part Two ---
             * Amidst the chaos, you notice that exactly one claim doesn't overlap by even a single square 
             * inch of fabric with any other claim. If you can somehow draw attention to it, maybe the 
             * Elves will be able to make Santa's suit after all!
             * 
             * For example, in the claims above, only claim 3 is intact after all claims are made.
             * 
             * What is the ID of the only claim that doesn't overlap?
             */

            var fabric = CreateFabric();
            fabric = EnterClaims(fabric);
            Console.WriteLine("ID of the only claim that doesn't overlap: {0}", GetNonOverlappingClaim(fabric));
        }

        private static string GetNonOverlappingClaim(string[,] fabric)
        {
            var claims = File.ReadLines(@"..\..\..\Day03\input.txt");
            foreach (var claim in claims)
            {
                var id = GetId(claim);
                var count = GetCount(fabric, id);
                var width = GetWidth(claim);
                var height = GetHeight(claim);

                if (count == width * height)
                    return id;
            }
            return null;
        }
    }
}

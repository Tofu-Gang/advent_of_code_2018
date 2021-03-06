﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2018.Day06
{
    public class Day06
    {
        private static readonly int TOTAL_DISTANCE_LIMIT = 10000;

        public static void Puzzle1()
        {
            /*
             * --- Day 6: Chronal Coordinates ---
             * The device on your wrist beeps several times, and once again you feel 
             * like you're falling.
             * 
             * "Situation critical," the device announces. "Destination indeterminate. 
             * Chronal interference detected. Please specify new target coordinates."
             * 
             * The device then produces a list of coordinates (your puzzle input). Are 
             * they places it thinks are safe or dangerous? It recommends you check 
             * manual page 729. The Elves did not give you a manual.
             * 
             * If they're dangerous, maybe you can minimize the danger by finding the 
             * coordinate that gives the largest distance from the other points.
             * 
             * Using only the Manhattan distance, determine the area around each 
             * coordinate by counting the number of integer X,Y locations that are 
             * closest to that coordinate (and aren't tied in distance to any other 
             * coordinate).
             * 
             * Your goal is to find the size of the largest area that isn't infinite. 
             * For example, consider the following list of coordinates:
             * 
             * 1, 1
             * 1, 6
             * 8, 3
             * 3, 4
             * 5, 5
             * 8, 9
             * 
             * If we name these coordinates A through F, we can draw them on a grid, putting 
             * 0,0 at the top left:
             * 
             * ..........
             * .A........
             * ..........
             * ........C.
             * ...D......
             * .....E....
             * .B........
             * ..........
             * ..........
             * ........F.
             * 
             * This view is partial - the actual grid extends infinitely in all directions. Using 
             * the Manhattan distance, each location's closest coordinate can be determined, shown 
             * here in lowercase:
             * 
             * aaaaa.cccc
             * aAaaa.cccc
             * aaaddecccc
             * aadddeccCc
             * ..dDdeeccc
             * bb.deEeecc
             * bBb.eeee..
             * bbb.eeefff
             * bbb.eeffff
             * bbb.ffffFf
             * 
             * Locations shown as . are equally far from two or more coordinates, and so they don't 
             * count as being closest to any.
             * 
             * In this example, the areas of coordinates A, B, C, and F are infinite - while not shown 
             * here, their areas extend forever outside the visible grid. However, the areas of 
             * coordinates D and E are finite: D is closest to 9 locations, and E is closest to 17 
             * (both including the coordinate's location itself). Therefore, in this example, the size 
             * of the largest area is 17.
             * 
             * What is the size of the largest area that isn't infinite?
             */
            
            var coords = GetCoords();
            // boundaries of the area with all the coordinates
            var left = GetLeft(coords);
            var right = GetRight(coords);
            var top = GetTop(coords);
            var bottom = GetBottom(coords);
            var neighbours = GetClosestNeighbours(coords, left, right, top, bottom);
            // +1 because we count the coordinates itselves to the area
            var largestAreaSize = GetLargestAreaSize(neighbours, coords) + 1;
            Console.WriteLine("The largest area that isn't infinite: {0}", largestAreaSize);
        }

        private static List<int[]> GetCoords()
        {
            var coords = new List<int[]>();

            foreach (var coord in new List<string>(File.ReadLines(@"..\..\..\Day06\input.txt")))
            {
                coords.Add(new int[] { 
                    Convert.ToInt32(coord.Split(",")[0].Trim()), 
                    Convert.ToInt32(coord.Split(",")[1].Trim()) 
                });
            }

            return coords;
        }

        private static int GetManhattanDistance(int[] coord1, int[] coord2) => Math.Abs(coord1[0] - coord2[0]) + Math.Abs(coord1[1] - coord2[1]);

        private static int GetLeft(List<int[]> coords)
        {
            var left = Int32.MaxValue;

            foreach (var coord in coords)
                left = coord[0] < left ? coord[0] : left;

            return left;
        }

        private static int GetRight(List<int[]> coords)
        {
            var right = Int32.MinValue;

            foreach (var coord in coords)
                right = coord[0] > right ? coord[0] : right;

            return right;
        }

        private static int GetTop(List<int[]> coords)
        {
            var top = Int32.MaxValue;

            foreach (var coord in coords)
                top = coord[1] < top ? coord[1] : top;

            return top;
        }

        private static int GetBottom(List<int[]> coords)
        {
            var bottom = Int32.MinValue;

            foreach (var coord in coords)
                bottom = coord[1] > bottom ? coord[1] : bottom;

            return bottom;
        }

        private static int[] GetClosestCoord(List<int[]> coords, int[] neighbour)
        {
            var minDistance = Int32.MaxValue;
            var closestCoord = coords[0];
            var isTiedToOtherCoord = false;

            foreach (var coord in coords)
            {
                var distance = GetManhattanDistance(coord, neighbour);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCoord = coord;
                    isTiedToOtherCoord = false;
                }
                else if (distance == minDistance)
                    isTiedToOtherCoord = true;
            }
            // we do not count neighbours that have the same min distance to more than one coordinate
            return isTiedToOtherCoord ? null : closestCoord;
        }

        private static Dictionary<int[], List<int[]>> GetClosestNeighbours(List<int[]> coords, int left, int right, int top, int bottom)
        {
            var neighbours = new Dictionary<int[], List<int[]>>();

            foreach (var coord in coords)
                neighbours.Add(coord, new List<int[]>());

            for (var y = top; y <= bottom; y++)
            {
                for (var x = left; x <= right; x++)
                {
                    var neighbour = new int[] { x, y };

                    // coords.Contain(neighbour) does not work here
                    if (!coords.Any(p => p.SequenceEqual(neighbour)))
                    {
                        var coord = GetClosestCoord(coords, neighbour);

                        if (coord != null)
                            neighbours[coord].Add(neighbour);
                    }
                }
            }
            
            return neighbours;
        }

        private static int GetLargestAreaSize(Dictionary<int[], List<int[]>> neighbours, List<int[]> coords)
        {
            var maxNeighboursCount = 0;
            var globalLeft = GetLeft(coords);
            var globalRight = GetRight(coords);
            var globalTop = GetTop(coords);
            var globalBottom = GetBottom(coords);

            foreach (var coord in neighbours.Keys)
            {
                if (neighbours[coord].Count > maxNeighboursCount)
                {
                    var areaLeft = GetLeft(neighbours[coord]);
                    var areaRight = GetRight(neighbours[coord]);
                    var areaTop = GetTop(neighbours[coord]);
                    var areaBottom = GetBottom(neighbours[coord]);

                    // do not count the infinite areas behind the global boundaries
                    if (areaLeft > globalLeft && areaRight < globalRight && areaTop > globalTop && areaBottom < globalBottom)
                        maxNeighboursCount = neighbours[coord].Count;
                }
            }

            return maxNeighboursCount;
        }

        public static void Puzzle2()
        {
            /*
             * --- Part Two ---
             * On the other hand, if the coordinates are safe, maybe the best you can do 
             * is try to find a region near as many coordinates as possible.
             * 
             * For example, suppose you want the sum of the Manhattan distance to all of 
             * the coordinates to be less than 32. For each location, add up the distances 
             * to all of the given coordinates; if the total of those distances is less than 
             * 32, that location is within the desired region. Using the same coordinates as 
             * above, the resulting region looks like this:
             * 
             * ..........
             * .A........
             * ..........
             * ...###..C.
             * ..#D###...
             * ..###E#...
             * .B.###....
             * ..........
             * ..........
             * ........F.
             * 
             * In particular, consider the highlighted location 4,3 located at the top middle 
             * of the region. Its calculation is as follows, where abs() is the absolute value 
             * function:
             * 
             * Distance to coordinate A: abs(4-1) + abs(3-1) =  5
             * Distance to coordinate B: abs(4-1) + abs(3-6) =  6
             * Distance to coordinate C: abs(4-8) + abs(3-3) =  4
             * Distance to coordinate D: abs(4-3) + abs(3-4) =  2
             * Distance to coordinate E: abs(4-5) + abs(3-5) =  3
             * Distance to coordinate F: abs(4-8) + abs(3-9) = 10
             * Total distance: 5 + 6 + 4 + 2 + 3 + 10 = 30
             * 
             * Because the total distance to all coordinates (30) is less than 32, the location 
             * is within the region.
             * 
             * This region, which also includes coordinates D and E, has a total size of 16.
             * 
             * Your actual region will need to be much larger than this example, though, instead 
             * including all locations with a total distance of less than 10000.
             * 
             * What is the size of the region containing all locations which have a total distance 
             * to all given coordinates of less than 10000?
             */
            
            var coords = GetCoords();
            // boundaries of the area with all the coordinates
            var left = GetLeft(coords);
            var right = GetRight(coords);
            var top = GetTop(coords);
            var bottom = GetBottom(coords);
            Console.WriteLine("Safest region size: {0}", GetSafeRegionSize(coords, left, right, top, bottom));
        }

        private static int GetSafeRegionSize(List<int[]> coords, int left, int right, int top, int bottom)
        {
            var regionSize = 0;

            for (var y = top; y <= bottom; y++)
            {
                for (var x = left; x <= right; x++)
                {
                    var totalDistance = 0;

                    foreach (var coord in coords)
                        totalDistance += GetManhattanDistance(new int[] { x, y }, coord);

                    if (totalDistance < TOTAL_DISTANCE_LIMIT)
                        regionSize++;
                }
            }

            return regionSize;
        }
    }
}

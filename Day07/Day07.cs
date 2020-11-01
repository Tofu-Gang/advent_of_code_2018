using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace AdventOfCode2018.Day07
{
    public class Day07
    {
        private readonly static string REQUIREMENT_SPLIT_LEFT = "Step";
        private readonly static string REQUIREMENT_SPLIT_RIGHT = "must";
        private readonly static string INSTRUCTION_SPLIT_LEFT = "step";
        private readonly static string INSTRUCTION_SPLIT_RIGHT = "can";
        
        public static void Puzzle1()
        {
            Console.WriteLine(GetSolution(GetInstructions()));
        }

        private static Dictionary<char, List<char>> GetInstructions()
        {
            var instructions = new Dictionary<char, List<char>>();

            foreach (var instruction in File.ReadLines(@"..\..\..\Day07\input.txt"))
            {
                var requirement = instruction.Split(REQUIREMENT_SPLIT_LEFT)[1].Trim().Split(REQUIREMENT_SPLIT_RIGHT)[0].Trim()[0];
                var step = instruction.Split(INSTRUCTION_SPLIT_LEFT)[1].Trim().Split(INSTRUCTION_SPLIT_RIGHT)[0].Trim()[0];

                try
                {
                    instructions.Add(requirement, new List<char>());
                }
                catch (ArgumentException) { }

                try
                {
                    var requirements = new List<char>
                    {
                        requirement
                    };
                    instructions.Add(step, requirements);
                }
                catch (ArgumentException) 
                {
                    instructions[step].Add(requirement);
                }
            }
            return instructions;
        }

        private static List<char> GetAvailableSteps(Dictionary<char, List<char>> instructions)
        {
            var available = new List<char>();

            foreach (var instruction in instructions)
            {
                if (instruction.Value.Count == 0)
                {
                    available.Add(instruction.Key);
                }
            }
            return available;
        }

        private static char[] GetSolution(Dictionary<char, List<char>> instructions)
        {
            var solution = new List<char>();

            while (instructions.Count > 0)
            {
                var available = GetAvailableSteps(instructions);
                available.Sort();
                var step = available[0];
                solution.Add(step);
                instructions.Remove(step);

                foreach (var instruction in instructions)
                {
                    instruction.Value.Remove(step);
                }
            }

            return solution.ToArray();
        }

        public static void Puzzle2()
        {

        }
    }
}

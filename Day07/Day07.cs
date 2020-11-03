using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventOfCode2018.Day07
{
    public class Day07
    {
        private readonly static string REQUIREMENT_SPLIT_LEFT = "Step";
        private readonly static string REQUIREMENT_SPLIT_RIGHT = "must";
        private readonly static string INSTRUCTION_SPLIT_LEFT = "step";
        private readonly static string INSTRUCTION_SPLIT_RIGHT = "can";
        private readonly static int STEP_DURATION = 60;
        private readonly static int WORKERS_COUNT = 5;
        
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
            var availableSteps = new List<char>();

            foreach (var instruction in instructions)
                if (instruction.Value.Count == 0)
                    availableSteps.Add(instruction.Key);

            availableSteps.Sort();
            return availableSteps;
        }

        private static List<char> GetAvailableSteps(Dictionary<char, List<char>> instructions, List<Worker> workers)
        {
            var availableSteps = GetAvailableSteps(instructions);

            // remove the available steps which are already assigned to workers
            foreach (var worker in workers)
                if (!worker.IsIdle())
                    availableSteps.Remove(worker.Job);

            return availableSteps;
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
                instructions = RemoveStep(instructions, step);
            }

            return solution.ToArray();
        }

        private static Dictionary<char, List<char>> RemoveStep(Dictionary<char, List<char>> instructions, char step)
        {
            instructions.Remove(step);

            foreach (var instruction in instructions)
                instruction.Value.Remove(step);

            return instructions;
        }

        public static void Puzzle2()
        {
            /*
             * --- Part Two ---
             * As you're about to begin construction, four of the Elves offer to help. 
             * "The sun will set soon; it'll go faster if we work together." Now, you 
             * need to account for multiple people working on steps simultaneously. If 
             * multiple steps are available, workers should still begin them in alphabetical 
             * order.
             * 
             * Each step takes 60 seconds plus an amount corresponding to its letter: 
             * A=1, B=2, C=3, and so on. So, step A takes 60+1=61 seconds, while step Z takes 
             * 60+26=86 seconds. No time is required between steps.
             * 
             * To simplify things for the example, however, suppose you only have help from one 
             * Elf (a total of two workers) and that each step takes 60 fewer seconds (so that 
             * step A takes 1 second and step Z takes 26 seconds). Then, using the same 
             * instructions as above, this is how each second would be spent:
             * 
             * Second   Worker 1   Worker 2   Done
             *    0        C          .        
             *    1        C          .        
             *    2        C          .        
             *    3        A          F       C
             *    4        B          F       CA
             *    5        B          F       CA
             *    6        D          F       CAB
             *    7        D          F       CAB
             *    8        D          F       CAB
             *    9        D          .       CABF
             *   10        E          .       CABFD
             *   11        E          .       CABFD
             *   12        E          .       CABFD
             *   13        E          .       CABFD
             *   14        E          .       CABFD
             *   15        .          .       CABFDE
             * Each row represents one second of time. The Second column identifies how many 
             * seconds have passed as of the beginning of that second. Each worker column shows 
             * the step that worker is currently doing (or . if they are idle). The Done column 
             * shows completed steps.
             * 
             * Note that the order of the steps has changed; this is because steps now take time 
             * to finish and multiple workers can begin multiple steps simultaneously.
             * 
             * In this example, it would take 15 seconds for two workers to complete these steps.
             * 
             * With 5 workers and the 60+ second step durations described above, how long will it 
             * take to complete all of the steps?
             */

            var instructions = GetInstructions();
            var workers = new List<Worker>();
            // OKBNLPHCSVWAIRDGUZEFMXYTJQ

            for (var i = 0; i < WORKERS_COUNT; i++)
                workers.Add(new Worker());

            var assembly_duration = 0;
            
            while (instructions.Count > 0)
            {
                var availableSteps = GetAvailableSteps(instructions, workers);

                // get idle workers and give them the available jobs
                foreach (var availableStep in availableSteps)
                {
                    try
                    {
                        var idleWorker = workers.First(worker => worker.IsIdle());
                        idleWorker.GiveJob(availableStep, GetStepDuration(availableStep));
                    }
                    catch (InvalidOperationException)
                    {
                        // no idle worker left
                        break;
                    }
                }

                // get the shortest work time left and make the workers work for this time
                var minWorkTime = workers.Min(worker => worker.WorkTimeLeft);
                assembly_duration += minWorkTime;
                foreach (var worker in workers)
                {
                    if (!worker.IsIdle())
                    {
                        worker.DoWork(minWorkTime);

                        if (worker.IsJobDone())
                        {
                            instructions = RemoveStep(instructions, worker.Job);
                            worker.JobDone();
                        }
                    }
                }
            }

            Console.WriteLine(assembly_duration);
        }

        private static int GetStepDuration(char step)
        {
            return (int)step - (int)'A' + 1 + STEP_DURATION;
        }
    }

    public class Worker
    {
        private static readonly char IDLE = '0';
        private static readonly int IDLE_TIME_LEFT = int.MaxValue;

        public char Job { get; private set; }
        public int WorkTimeLeft { get; private set; }

        public Worker()
        {
            this.Job = IDLE;
            this.WorkTimeLeft = IDLE_TIME_LEFT;
        }

        public void GiveJob(char step, int workTime)
        {
            this.Job = step;
            this.WorkTimeLeft = workTime;
        }

        public void DoWork(int workTime)
        {
            this.WorkTimeLeft -= workTime;
        }

        public bool IsJobDone()
        {
            return this.WorkTimeLeft == 0 && !this.IsIdle();
        }

        public bool IsIdle()
        {
            return this.Job == IDLE;
        }

        public void JobDone()
        {
            this.Job = IDLE;
            this.WorkTimeLeft = IDLE_TIME_LEFT;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventOfCode2018.Day05
{
    public class Day05
    {
        /*
         * --- Day 5: Alchemical Reduction ---
         * You've managed to sneak in to the prototype suit manufacturing lab. The Elves are making 
         * decent progress, but are still struggling with the suit's size reduction capabilities.
         * 
         * While the very latest in 1518 alchemical technology might have solved their problem eventually, 
         * you can do better. You scan the chemical composition of the suit's material and discover 
         * that it is formed by extremely long polymers (one of which is available as your puzzle input).
         * 
         * The polymer is formed by smaller units which, when triggered, react with each other such that 
         * two adjacent units of the same type and opposite polarity are destroyed. Units' types are 
         * represented by letters; units' polarity is represented by capitalization. For instance, r and R 
         * are units with the same type but opposite polarity, whereas r and s are entirely different 
         * types and do not react.
         * 
         * For example:
         * 
         * In aA, a and A react, leaving nothing behind.
         * In abBA, bB destroys itself, leaving aA. As above, this then destroys itself, leaving nothing.
         * In abAB, no two adjacent units are of the same type, and so nothing happens.
         * In aabAAB, even though aa and AA are of the same type, their polarities match, and so nothing happens.
         * Now, consider a larger example, dabAcCaCBAcCcaDA:
         * 
         * dabAcCaCBAcCcaDA  The first 'cC' is removed.
         * dabAaCBAcCcaDA    This creates 'Aa', which is removed.
         * dabCBAcCcaDA      Either 'cC' or 'Cc' are removed (the result is the same).
         * dabCBAcaDA        No further actions can be taken.
         * After all possible reactions, the resulting polymer contains 10 units.
         * 
         * How many units remain after fully reacting the polymer you scanned? 
         */
        public static void Puzzle1()
        {
            var polymer = File.ReadAllText(@"..\..\..\Day05\input.txt");
            Console.WriteLine("Units remaining after fully reacting the polymer you scanned: {0}", GetFullyTriggeredPolymerLength(polymer));
        }

        private static int GetFullyTriggeredPolymerLength(string polymer)
        {
            while (true)
            {
                var triggered = TriggerPolymer(polymer);
                if (String.IsNullOrEmpty(triggered))
                {
                    return polymer.Length;
                }
                else
                {
                    polymer = triggered;
                }
            }
        }

        private static string TriggerPolymer(string polymer)
        {
            for (var i = 0; i < polymer.Length - 1; i++)
            {
                var unit1 = polymer[i];
                var unit2 = polymer[i + 1];

                if (((Char.IsUpper(unit1) && Char.IsLower(unit2)) 
                     || (Char.IsLower(unit1) && Char.IsUpper(unit2))) 
                    && Char.ToUpper(unit1) == Char.ToUpper(unit2))
                {
                    return polymer.Substring(0, i) + polymer.Substring(i + 2);
                }
            }
            return null;
        }

        public static void Puzzle2()
        {
            /*
             * --- Part Two ---
             * Time to improve the polymer.
             * 
             * One of the unit types is causing problems; it's preventing the polymer from collapsing 
             * as much as it should. Your goal is to figure out which unit type is causing the most 
             * problems, remove all instances of it (regardless of polarity), fully react the remaining 
             * polymer, and measure its length.
             * 
             * For example, again using the polymer dabAcCaCBAcCcaDA from above:
             * 
             * Removing all A/a units produces dbcCCBcCcD. Fully reacting this polymer produces dbCBcD, which has length 6.
             * Removing all B/b units produces daAcCaCAcCcaDA. Fully reacting this polymer produces daCAcaDA, which has length 8.
             * Removing all C/c units produces dabAaBAaDA. Fully reacting this polymer produces daDA, which has length 4.
             * Removing all D/d units produces abAcCaCBAcCcaA. Fully reacting this polymer produces abCBAc, which has length 6.
             * 
             * In this example, removing all C/c units was best, producing the answer 4.
             * 
             * What is the length of the shortest polymer you can produce by removing all units of 
             * exactly one type and fully reacting the result?
             */

            var polymer = File.ReadAllText(@"..\..\..\Day05\input.txt");
            var minLength = Int32.MaxValue;

            for (var i = (int)'a'; i <= (int)'z'; i++)
            {
                var unit = (char)i;
                var unitOppositePolarity = Char.ToUpper(unit);
                var modifiedPolymer = polymer.Replace(unit.ToString(), "").Replace(unitOppositePolarity.ToString(), "");
                var triggeredPolymerLength = GetFullyTriggeredPolymerLength(modifiedPolymer);

                if (triggeredPolymerLength < minLength)
                    minLength = triggeredPolymerLength;

                Console.WriteLine("removing unit {0}/{1}; polymer length after reaction: {2}", unit, unitOppositePolarity, triggeredPolymerLength);
            }

            Console.WriteLine("The length of the shortest polymer you can produce by removing all units of exactly one type and fully reacting the result: {0}", minLength);
        }
    }
}

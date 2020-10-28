using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AdventOfCode2018.Day04
{
    public class Day04
    {
        private static readonly int DATE_START_INDEX = 1;
        private static readonly int DATE_END_INDEX = 16;
        private static readonly string SHIFT_START = "begins shift";
        private static readonly string FALLS_ASLEEP = "falls asleep";
        private static readonly string WAKES_UP = "wakes up";
        private static readonly string GUARD = "Guard";

        public static void Puzzle1()
        {
            /*
             * --- Day 4: Repose Record ---
             * You've sneaked into another supply closet - this time, it's across from the prototype 
             * suit manufacturing lab. You need to sneak inside and fix the issues with the suit, but 
             * there's a guard stationed outside the lab, so this is as close as you can safely get.
             * 
             * As you search the closet for anything that might help, you discover that you're not the 
             * first person to want to sneak in. Covering the walls, someone has spent an hour starting 
             * every midnight for the past few months secretly observing this guard post! They've been 
             * writing down the ID of the one guard on duty that night - the Elves seem to have decided 
             * that one guard was enough for the overnight shift - as well as when they fall asleep or 
             * wake up while at their post (your puzzle input).
             * 
             * For example, consider the following records, which have already been organized into 
             * chronological order:
             * 
             * [1518-11-01 00:00] Guard #10 begins shift
             * [1518-11-01 00:05] falls asleep
             * [1518-11-01 00:25] wakes up
             * [1518-11-01 00:30] falls asleep
             * [1518-11-01 00:55] wakes up
             * [1518-11-01 23:58] Guard #99 begins shift
             * [1518-11-02 00:40] falls asleep
             * [1518-11-02 00:50] wakes up
             * [1518-11-03 00:05] Guard #10 begins shift
             * [1518-11-03 00:24] falls asleep
             * [1518-11-03 00:29] wakes up
             * [1518-11-04 00:02] Guard #99 begins shift
             * [1518-11-04 00:36] falls asleep
             * [1518-11-04 00:46] wakes up
             * [1518-11-05 00:03] Guard #99 begins shift
             * [1518-11-05 00:45] falls asleep
             * [1518-11-05 00:55] wakes up
             * 
             * Timestamps are written using year-month-day hour:minute format. The guard falling asleep 
             * or waking up is always the one whose shift most recently started. Because all asleep/awake 
             * times are during the midnight hour (00:00 - 00:59), only the minute portion (00 - 59) is 
             * relevant for those events.
             * 
             * Visually, these records show that the guards are asleep at these times:
             * 
             * Date   ID   Minute
             *             000000000011111111112222222222333333333344444444445555555555
             *             012345678901234567890123456789012345678901234567890123456789
             * 11-01  #10  .....####################.....#########################.....
             * 11-02  #99  ........................................##########..........
             * 11-03  #10  ........................#####...............................
             * 11-04  #99  ....................................##########..............
             * 11-05  #99  .............................................##########.....
             * 
             * The columns are Date, which shows the month-day portion of the relevant day; ID, which shows 
             * the guard on duty that day; and Minute, which shows the minutes during which the guard was 
             * asleep within the midnight hour. (The Minute column's header shows the minute's ten's digit 
             * in the first row and the one's digit in the second row.) Awake is shown as ., and asleep is 
             * shown as #.
             * 
             * Note that guards count as asleep on the minute they fall asleep, and they count as awake on 
             * the minute they wake up. For example, because Guard #10 wakes up at 00:25 on 1518-11-01, 
             * minute 25 is marked as awake.
             * 
             * If you can figure out the guard most likely to be asleep at a specific time, you might be able 
             * to trick that guard into working tonight so you can have the best chance of sneaking in. You 
             * have two strategies for choosing the best guard/minute combination.
             * 
             * Strategy 1: Find the guard that has the most minutes asleep. What minute does that guard spend 
             * asleep the most?
             * 
             * In the example above, Guard #10 spent the most minutes asleep, a total of 50 minutes (20+25+5), 
             * while Guard #99 only slept for a total of 30 minutes (10+10+10). Guard #10 was asleep most 
             * during minute 24 (on two days, whereas any other minute the guard was asleep was only seen on 
             * one day).
             * 
             * While this example listed the entries in chronological order, your entries are in the order 
             * you found them. You'll need to organize them before they can be analyzed.
             * 
             * What is the ID of the guard you chose multiplied by the minute you chose? (In the above 
             * example, the answer would be 10 * 24 = 240.)
             */

            var entries = GetEntries();
            var mostSleepyGuardId = GetMostSleepyGuardId(entries);
            var mostSleepyGuardEntries = GetGuardEntries(entries, mostSleepyGuardId);
            var mostSleepyMinute = GetMostSleepyMinute(mostSleepyGuardEntries);
            Console.WriteLine("The ID of the guard you chose multiplied by the minute you chose: {0}", 
                Convert.ToInt32(mostSleepyGuardId.Substring(1)) * mostSleepyMinute);
        }

        private static List<string> GetEntries()
        {
            var entries = new List<string>(File.ReadLines(@"..\..\..\Day04\input.txt"));
            entries.Sort(new EntryComparer());
            return entries;
        }

        private static string GetMostSleepyGuardId(List<string> entries)
        {
            var guards = new Dictionary<string, int>();
            var currentGuardId = "";

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (entry.Contains(GUARD) && entry.Contains(SHIFT_START))
                {
                    currentGuardId = GetGuardId(entry);
                    try
                    {
                        guards.Add(currentGuardId, 0);
                    }
                    catch (ArgumentException) { }
                }
                else if (entry.Contains(WAKES_UP))
                {
                    var sleepEntry = entries[i - 1];
                    var sleepMinutes = (GetDateTime(entry) - GetDateTime(sleepEntry)).Minutes;
                    guards[currentGuardId] += sleepMinutes;
                }
            }

            return guards.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }

        private static List<string> GetGuardEntries(List<string> entries, string guardId)
        {
            var mostSleepyGuardEntries = new List<string>();
            var guardFlag = false;

            foreach (var entry in entries)
            {
                if (entry.Contains(GUARD) && entry.Contains(SHIFT_START))
                    guardFlag = GetGuardId(entry) == guardId;
                if (guardFlag)
                    mostSleepyGuardEntries.Add(entry);
            }

            return mostSleepyGuardEntries;
        }

        private static int GetMinuteSleepCount(int minute, List<string> entries)
        {
            var sleepCount = 0;

            for (var i = 0; i < entries.Count; i++)
            {
                if (entries[i].Contains(WAKES_UP))
                {
                    var fallsAsleepMinute = GetDateTime(entries[i - 1]).Minute;
                    var wakesUpMinute = GetDateTime(entries[i]).Minute;

                    if (minute >= fallsAsleepMinute && minute < wakesUpMinute)
                        sleepCount++;
                }
            }

            return sleepCount;
        }

        private static int GetMostSleepyMinute(List<string> entries)
        {
            var mostSleepyMinute = 0;
            var maxMinuteSleepCount = 0;

            for (var minute = 0; minute < 60; minute++)
            {
                var minuteSleepCount = GetMinuteSleepCount(minute, entries);
                if (maxMinuteSleepCount < minuteSleepCount)
                {
                    maxMinuteSleepCount = minuteSleepCount;
                    mostSleepyMinute = minute;
                }
            }

            return mostSleepyMinute;
        }

        private static void PrintLog(List<string> entries)
        {
            Console.WriteLine("Date   ID     Minute                                                      ");
            Console.WriteLine("              000000000011111111112222222222333333333344444444445555555555");
            Console.WriteLine("              012345678901234567890123456789012345678901234567890123456789");
            var lastMinute = 0;
            var logBegins = true;

            foreach (var entry in entries)
            {
                if (entry.Contains(GUARD) && entry.Contains(SHIFT_START))
                {
                    if (logBegins)
                    {
                        logBegins = false;
                    }
                    else
                    {
                        Console.WriteLine(new String('.', 60 - lastMinute));
                    }
                        
                    Console.Write("{0, 2}-{1, 2}  {2}  ", GetDateTime(entry).Month, GetDateTime(entry).Day, GetGuardId(entry));
                    lastMinute = 0;
                }
                else if (entry.Contains(FALLS_ASLEEP))
                {
                    Console.Write(new String('.', GetDateTime(entry).Minute - lastMinute));
                    lastMinute = GetDateTime(entry).Minute;
                }
                else if (entry.Contains(WAKES_UP))
                {
                    Console.Write(new String('#', GetDateTime(entry).Minute - lastMinute));
                    lastMinute = GetDateTime(entry).Minute;
                }
            }
            Console.Write(new String('.', 60 - lastMinute));
        }

        private static DateTime GetDateTime(string entry) => DateTime.Parse(entry.Substring(DATE_START_INDEX, DATE_END_INDEX));

        private static string GetGuardId(string entry) => entry.Split(GUARD)[1].Split(SHIFT_START)[0].Trim();

        private class EntryComparer : IComparer<string>
        {
            /*
             * Used to sort guard shifts log entries by date and time.
             */
            public int Compare([AllowNull] string entry1, [AllowNull] string entry2)
            {
                var date1 = GetDateTime(entry1);
                var date2 = GetDateTime(entry2);
                return date1.CompareTo(date2);
            }
        }

        public static void Puzzle2()
        {
            
        }
    }
}

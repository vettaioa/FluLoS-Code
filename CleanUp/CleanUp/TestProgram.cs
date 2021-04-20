using System;
using System.Collections.Generic;

namespace Cleanup
{
    class TestProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TEST Cleanup");

            (new List<string> {
                TextCleaner.Clean("speedbird five eight zero descend to flight level two one zero"),
                "speedbird 580 descend to flight level 210  [expected]",
                "---",
                TextCleaner.Clean("tarom three seven two continue with rhein one three two decimal four"),
                "tarom 372 continue with rhein 132.4  [expected]",
                "---",
                TextCleaner.Clean("airfrans three two six one bonjour identified maintain flight level two nine zero two passeiry"),
                "airfrans 3261 bonjour identified maintain flight level 290 to passeiry  [expected]",
                "---",
                TextCleaner.Clean("airline six double three two with aircraft boeing triple seven"),
                "airline 6332 with aircraft boeing 777  [expected]",
                "---",
                TextCleaner.Clean("good morning lufthansa four four one six ah radar contact maintain for the time two nine zero further climb in ah twenty five miles trasadingen canne"),
                "good morning lufthansa 4416 radar contact maintain for the time 290 further climb in 25 miles trasadingen canne  [expected]",
            }).ForEach(s => Console.WriteLine(s));

        }
    }
}

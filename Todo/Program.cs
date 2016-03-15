using System;
using System.IO;

using HumDrum.Collections;
using System.Collections.Generic;

namespace Todo
{
	/// <summary>
	/// The starting point of Todo.
	/// </summary>
	class MainClass
	{
		public static void Help()
		{
			Console.WriteLine ("TODO quick command manual");
			Console.WriteLine ("--------------------------");
			Console.WriteLine ("todo (contract|detailed|remove) (TASKNAME|all) dir1 dir2 dir3...");
			Console.WriteLine ("Subsitute TASKNAME for a named task for contract and remove, or ALL for detailed");
			Console.WriteLine ("Any number of directories can be listed at the end of the commands");
		}

		/// <summary>
		/// The Main method. 
		/// Expects "todo (contract|detailed|remove) (TASKNAME|ALL|) dir1 dir2 dir3...
		/// </param>
		public static void Main (string[] args)
		{
			if (args.Length () < 2) {
				Help ();
				return;
			}

			// Build the scanner table from the directory specified in args[0]
			TaskScan scanner = new TaskScan (Transformations.Subsequence(args, (args[0].Equals("detailed")?1:2), args.Length()), "TODO", SearchOption.AllDirectories);
			scanner.BuildTable ();



			switch (args [0]) {
			case "contract":
				Console.WriteLine ("Your TODO contract is as follows: \n");

				foreach (string task in scanner.Tasks.GenerateContract(scanner.Tasks.GetTaskByName(args[1]))) 
					Console.WriteLine (task);
				
				break;
			
			case "detailed":
				Console.WriteLine ("Your detailed TODO contract is as follows: \n");
				foreach (string s in scanner.Tasks.DetailedContract())
					Console.WriteLine (s);
				break;

			case "remove":
				try {
					scanner.Tasks.GetTaskByName (args [1]).AttemptMark ();
				} catch (Exception e) {
					Console.WriteLine ("Task could not be properly removed from the file");
				}
				break;
			default:
				Help ();
				break;
			}
		}
	}
}

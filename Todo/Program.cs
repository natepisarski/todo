using System;
using HumDrum.Collections;
using System.Collections.Generic;

namespace Todo
{
	/// <summary>
	/// The starting point of Todo.
	/// </summary>
	class MainClass
	{

		/// <summary>
		/// Begins the program, with given command-line arguments.
		/// 
		/// A quick sumamry of a TODO:
		/// Todo is a productivity utility for organizing how work should be done.
		/// The anatomy of a TODO comment is
		/// 
		/// KEY NAME {DEPENDENCY1, DEPENDENCY2, ...}: TEXT
		/// 
		/// Where KEY is the TODO keyword (default: TODO), dependency is an optional
		/// dependency. It is a task which must be completed before the first.
		/// 
		/// For example:
		/// TODO groceries: Go get eggs and milk
		/// TODO {groceries}: Come back home
		/// 
		/// The second TODO Task (which is anonymous) depends on the named Task "groceries", meaning
		/// it should be completed before.
		/// 
		/// In order to allow for multi-line and multi-file TODOs, TODOs with the same name can be condensed
		/// into one directive.
		/// 
		/// Any text before the TODO is ignored by the TODO generator
		/// </summary>
		/// <param name="args">Acceptable command-line arguments include:
		/// 
		/// --dir, -d DIR: Add DIR to the search path
		/// -s, --strike NAME: Strike a named TODO from its occuring place.
		/// -c, --contract: Print a contract of all TODOS in a human-readable format.
		/// -p, --pack: Condense TODOs with the same name into one directive.
		/// -n, --number: Show only the first n todos found.
		/// -s, --skip: Skip the first n todos found
		/// 
		/// if -d isn't specified, the current working directory and all nested directories are scanned
		/// for TODO comments. If -s or -c aren't listed, all of the TODO comments as they appear, and
		/// which todo they are dependant on, and which file they are listed in.
		/// </param>
		public static void Main (string[] args)
		{
			var c = new Task ("TODO", "TODO dependencies{a, b} name{c}: Performs Action C");
			var b = new Task ("TODO", "TODO dependencies{a} name{b}: Performs Action B");
			var a = new Task ("TODO", "TODO name{a}: Perofrms Action A");
			TaskTable t = new TaskTable (a, b, c);

			foreach (string s in t.GenerateContract("c")) {
				Console.WriteLine (s);
			}
			for (;;) {
				;
			}
		}
	}
}

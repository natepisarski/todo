using System;
using System.Linq;
using System.Collections.Generic;

using HumDrum.Collections;
using HumDrum.Recursion;

namespace Todo
{
	/// <summary>
	/// The task table contains a list of tasks and manages
	/// their organization and output.
	/// </summary>
	public class TaskTable
	{
		/// <summary>
		/// The list of tasks found in the directory hierarchy.
		/// </summary>
		/// <value>The tasks</value>
		public List<Task> Tasks { get; set; }

		/// <summary>
		/// Initializes the Tasks with an empty list
		/// </summary>
		public TaskTable ()
		{
			Tasks = new List<Task> ();
		}

		/// <summary>
		/// Creates a TaskTable with one initial task.
		/// </summary>
		/// <param name="initialTask">The initial task</param>
		public TaskTable(Task initialTask)
		{
			Tasks = new List<Task> ();
			Tasks.Add (initialTask);
		}

		/// <summary>
		/// Multivariate constructor
		/// </summary>
		/// <param name="initialTasks">A list of the initial tasks to use in this tasktable</param>
		public TaskTable(params Task[] initialTasks)
		{
			Tasks = new List<Task> ();

			foreach (Task t in initialTasks)
				Tasks.Add (t);
		}

		/// <summary>
		/// Attempts to fetch a Task by its name metadata if it has it.
		/// </summary>
		/// <returns>The task</returns>
		/// <param name="name">The name of the named task to search for</param>
		public Task GetTaskByName(string name)
		{
			foreach (Task item in Tasks)
				if (item.Has ("name") && item.Fetch ("name").Get (0).Equals (name))
					return item;
			throw new Exception ("No task with name \"" + name + "\" has been found in TaskTable Object " + this);
		}

		/// <summary>
		/// Attempts to read one level of dependencies from a named task
		/// </summary>
		/// <returns>The task dependencies.</returns>
		/// <param name="name">The name of the task to check the dependencies of</param>
		public List<Task> GetTaskDependencies(string name)
		{
			var attempt = new List<Task>();
			try{
				attempt = (from x in GetTaskByName (name).Fetch (name) select GetTaskByName(x)).ToList();
			}catch(Exception e){
				return new List<Task> ();
			}
			return attempt;
		}

		/// <summary>
		/// Check to see whether or not a task has a circular dependency.
		/// </summary>
		/// <returns><c>true</c> if this instance is circular the specified taskName topLevelDependencies; otherwise, <c>false</c>.</returns>
		/// <param name="taskName">Task name.</param>
		/// <param name="topLevelDependencies">Top level dependencies.</param>
		private bool NotCircular(string taskName, List<Task> topLevelDependencies)
		{
			try {
				var t = GetTaskByName (taskName);
					
				if (topLevelDependencies.Contains (t))
					return false;
				else if (!t.Has ("dependencies"))
					return true;
				else 
					return // If anything below this contains this or anything above it, it's circular
						Predicates.All (new List<bool>((from dependant in t.Fetch ("dependencies")
							select NotCircular (dependant, topLevelDependencies.Tack(t)))));
				
			}catch(Exception e) {
				return true;
			}
		}

		// Make NotCircular more logical
		public bool IsCircular(string taskName)
		{
			return !(NotCircular (taskName, new List<Task> ()));
		}

		/// <summary>
		/// Resolves the dependencies. Meaning, return a list of all of the tasks that this one is
		/// dependant on
		/// </summary>
		/// <returns>The dependencie</returns>
		/// <param name="taskName">Task name.</param>
		public List<Task> ResolveDependencies(string taskName)
		{			
			if (IsCircular(taskName))
				throw new Exception (taskName + " seems to incur a circular dependency.");

			Task t;

			try{
				t = GetTaskByName (taskName);
			}catch(Exception e) {
				return new List<Task> ();
			}

			var totalDependencies = new List<Task>();

			if (t.Has ("dependencies")) {
				var localDependencies = new List<Task> ();

				foreach (string depname in t.Fetch("dependencies"))
				{
					try
					{
						localDependencies.Add (GetTaskByName (depname));
					}catch(Exception e){
						
					}
				}
				 totalDependencies = new List<Task> (localDependencies);

				// After adding the local dependencies, recursively do the same for every other dependency.
				foreach (Task dependency in localDependencies)
					totalDependencies.AddRange(ResolveDependencies (dependency.GetName()));
				
			} else
				return new List<Task> ();

			// Once the recursion is complete, remove duplicate entries.
			return Transformations.RemoveDuplicates<Task>(totalDependencies).ToList();
		}

		/// <summary>
		/// GenerateContract will generate a list of tasks that
		/// need to be completed in order for the provided task with
		/// dependencies provides to be considered complete
		/// </summary>
		/// <returns>The contract</returns>
		/// <param name="t">The task to generate the contract of.</param>
		public string[] GenerateContract(Task t)
		{
			List<string> contract = new List<string> ();

			List<Task> dependencies = ResolveDependencies (t.Fetch("name").Get(0));

			//Reversing the dependencies should get the dependencies with the least
			// amount of dependencies themself
			dependencies.Reverse ();

			foreach (Task task in dependencies)
				contract.Add (task.Text);
			
			contract.Add (t.Text);
			return contract.ToArray();
		}

		/// <summary>
		/// Generates the contract for everything in this TaskTable
		/// </summary>
		/// <returns>The contract, as an array of strings</returns>
		public string[] GenerateContract()
		{
			var tasks = new List<string> ();

			foreach (Task t in Tasks) 
				tasks.AddRange (GenerateContract (t));

			return Transformations.RemoveDuplicates (tasks).AsArray();

		}

		/// <summary>
		/// Detailed contract which shows all metadata members of all tasks, but 
		/// does not resolve their dependencies.
		/// </summary>
		/// <returns>The contract.</returns>
		public string[] DetailedContract()
		{
			List<string> contract = new List<string> ();

			foreach (Task t in Tasks) {
				contract.Add ("Entry from file " + t.Fetch ("file").Get(0) + ":");

				foreach (string key in t.Metadata.Keyset())
					if(!key.Equals("file"))
						contract.Add (t.Consolidated(key));
				
				contract.Add ("Task: " + t.Text);
				contract.Add ("\n");
			}

			return contract.ToArray();
		}
	}
}


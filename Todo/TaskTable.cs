using System;
using System.Linq;
using System.Collections.Generic;

using HumDrum.Collections;
using HumDrum.Recursion;

namespace Todo
{
	public class TaskTable
	{
		List<Task> Tasks { get; set; }


		public TaskTable ()
		{
			Tasks = new List<Task> ();
		}

		public TaskTable(Task initialTask)
		{
			Tasks = new List<Task> ();
			Tasks.Add (initialTask);
		}

		public TaskTable(params Task[] initialTasks)
		{
			Tasks = new List<Task> ();
			foreach (Task t in initialTasks)
				Tasks.Add (t);
		}

		public Task GetTaskByName(string name)
		{
			foreach (Task item in Tasks)
				if (item.Has ("name") && item.Fetch ("name").Get (0).Equals (name))
					return item;
			throw new Exception ("No task with name \"" + name + "\" has been found in TaskTable Object " + this);
		}

	
		public IEnumerable<Task> GetTaskDependencies(string name)
		{
			return (from x in GetTaskByName (name).Fetch (name) select GetTaskByName(x));
		}

		/// <summary>June 3rd
		/// Check to see whether or not a task has a circular dependency.
		/// </summary>
		/// <returns><c>true</c> if this instance is circular the specified taskName topLevelDependencies; otherwise, <c>false</c>.</returns>
		/// <param name="taskName">Task name.</param>
		/// <param name="topLevelDependencies">Top level dependencies.</param>
		private bool NotCircular(string taskName, List<Task> topLevelDependencies)
		{
			var t = GetTaskByName (taskName);

			if (topLevelDependencies.Contains (t))
				return false;
			else if (!t.Has ("dependencies"))
				return true;
			else 
				return 
					Predicates.All (new List<bool>((from dependant in t.Fetch ("dependencies")
						select NotCircular (dependant, topLevelDependencies.Tack(t)))));
		}

		public bool IsCircular(string taskName)
		{
			return !(NotCircular (taskName, new List<Task> ()));
		}

		public List<Task> ResolveDependencies(string taskName)
		{			
			if (IsCircular(taskName))
				throw new Exception (taskName + " seems to incur a circular dependency.");

			var t = GetTaskByName (taskName);
			var totalDependencies = new List<Task>();

			if (t.Has ("dependencies")) {
				var localDependencies = new List<Task> ();

				foreach (string depname in t.Fetch("dependencies"))
					localDependencies.Add (GetTaskByName (depname));

				 totalDependencies = new List<Task> (localDependencies);

				foreach (Task dependency in localDependencies)
					totalDependencies.AddRange(ResolveDependencies (dependency.GetName()));
				
			} else
				return new List<Task> ();
			
			return Transformations.RemoveDuplicates<Task>(totalDependencies).ToList();
		}

		public string[] GenerateContract(Task t)
		{
			List<string> contract = new List<string> ();

			List<Task> dependencies = ResolveDependencies (t.Fetch("name").Get(0));
			dependencies.Reverse ();

			foreach (Task task in dependencies)
				contract.Add (task.Text);
			contract.Add (t.Text);
			return contract.ToArray();
		}
	}
}


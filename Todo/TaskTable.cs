using System;
using System.Collections.Generic;

using HumDrum.Collections;
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

		public TaskTable(Task[] initialTasks)
		{
			Tasks = new List<Task> ();
			foreach (Task t in initialTasks)
				Tasks.Add (t);
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
		}

	
		public Task[] GetTaskDependencies(string name)
		{
			return GetTaskByName (name).Fetch (name);
		}

		/// <summary>June 3rd
		/// Check to see whether or not a task has a circular dependency.
		/// </summary>
		/// <returns><c>true</c> if this instance is circular the specified taskName topLevelDependencies; otherwise, <c>false</c>.</returns>
		/// <param name="taskName">Task name.</param>
		/// <param name="topLevelDependencies">Top level dependencies.</param>
		private bool IsCircular(string taskName, List<Task> topLevelDependencies)
		{
			var t = GetTaskByName (taskName);

			if (topLevelDependencies.Contains (t))
				return false;
			else if (!t.Has ("dependencies"))
				return true;
			else 
				return 
					Predicates.All (from Task dependant in t.Fetch ("dependencies")
					               select IsCircular (t.Fetch ("name"), topLevelDependencies.Add (t)));
		}

		public bool IsCircular(string taskName)
		{
			return IsCircular (taskName, new List<Task> ());
		}

		public List<Task> ResolveDependencies(string taskName)
		{
			if (!(GetTaskByName (taskName).Length > 0))
				throw new Exception ("Task by this name not found");
			
			if (IsCircular)
				throw new Exception (taskName + " seems to incur a circular dependency.");

			var t = GetTaskByName (taskName);

			if (t.Has ("dependencies")) {
				var localDependencies = new List<Task> ();

				foreach (string depname in t.Fetch("dependencies"))
					localDependencies.AddRange (GetTaskByName (depname));

				foreach (Task dependency in localDependencies)
					localDependencies.AddRange (ResolveDependencies (dependency.Fetch ("name")));
			} else
				return new List<Task> ();
		}

		public string[] GenerateContract(Task t)
		{
			List<string> contract = new List<string> ();

			List<Task> dependencies = ResolveDependencies (t.Fetch("name").Get(0));
			dependencies.Reverse ();

			foreach (string line in dependencies.ForEach(x => x.Text))
				contract.Add (line);
			contract.Add (t.Text);
		}
	}
}


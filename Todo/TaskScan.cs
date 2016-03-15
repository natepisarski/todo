using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using HumDrum.Operations;
using HumDrum.Operations.Files;

namespace Todo
{
	/// <summary>
	/// Scans directories for Tasks and builds a table from
	/// the discovered lines.
	/// </summary>
	public class TaskScan
	{
		/// <summary>
		/// The DirectorySearch containing the files from the directories
		/// </summary>
		private DirectorySearch TaskDir;

		/// <summary>
		/// Gets or sets the key used to identify Task lines.
		/// </summary>
		/// <value>The key.</value>
		public string Key {get; private set;}

		/// <summary>
		/// A TaskTable representing all of the discovered lines.
		/// </summary>
		/// <value>The tasks.</value>
		public TaskTable Tasks { get; set; }

		/// <summary>
		/// Creates a TaskScan with a list of directories to scan,
		/// a key to tell the TaskScan what text to look for to begin a task,
		/// and a SearchOption.
		/// </summary>
		/// <param name="directory">The directories to search for tasks</param>
		/// <param name="key">The key for task lookup</param>
		/// <param name="option">Whether or not to delve deeper into repositories</param>
		public TaskScan (IEnumerable<string> directory, string key, SearchOption option)
		{
			TaskDir = new DirectorySearch (directory, option);
			Tasks = new TaskTable ();

			Key = key;
		}
			
		/// <summary>
		/// Go through the directories in the DirectorySearch
		/// to build a TaskTable of all the tasks found.
		/// </summary>
		public void BuildTable()
		{
			var tasks = new List<Task> ();

			// Every file
			foreach (string x in TaskDir.Files) {
				//Every line
				foreach (string line in File.ReadAllLines(x)) {

					//If it's a task
					if (line.Contains (Key) && line.Contains (":")) {

						//Make it a task, remember its file, and add it to the list
						var temp = new Task (Key, line);
						temp.Metadata.Associate ("file", x);
						tasks.Add (temp);
					}
				}
			}

			//Add these tasks to the existing TaskTable
			Tasks.Tasks.AddRange (tasks);
		}
	}
}


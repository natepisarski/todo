using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using HumDrum.Operations;

namespace Todo
{
	public class TaskScan
	{
		private DirectorySearch TaskDir;
		string Key;
		public TaskTable Tasks { get; set; }

		public TaskScan (string directory, string key, SearchOption option)
		{
			TaskDir = new DirectorySearch (directory, option);
			Tasks = new TaskTable ();
			Key = key;
		}
			
		public void BuildTable()
		{
			var tasks = new List<Task> ();

			//TODO: Here is a task
			//TODO dependencies{thing1,thing2}: Here we go

			foreach (string x in TaskDir.Files) {
				foreach (string line in File.ReadAllLines(x)) {
					
					if (line.Contains (Key) && line.Contains (":")) {
						var temp = new Task (Key, line);
						temp.Metadata.Associate ("file", x);
						tasks.Add (temp);
					}
				}
			}

			Tasks.Tasks.AddRange (tasks);
		}
	}
}


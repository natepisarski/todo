using System;
using System.IO;
using System.Collections.Generic;

using HumDrum.Collections;
using HumDrum.Structures;

namespace Todo
{
	/// <summary>
	/// Tasks represent each individual task specified
	/// by TODO lines.
	/// 
	/// KEY metaName{data1, data2} metaName2{data1, data2}: Text here is the format
	/// </summary>
	public class Task
	{
		/// <summary>
		/// Represents the data that a task can contain, such as its name or a list of its dependencies 
		/// </summary>
		/// <value>The metadata.</value>
		public BindingsTable<string, string> Metadata { get; set; }

		/// <summary>
		/// The body of the Task.
		/// </summary>
		public string Text;

		/// <summary>
		/// The key that sets this task apart from its content.
		/// This is TODO by default, and there is currently no way to configure this.
		/// </summary>
		public string Key;

		/// <summary>
		/// Create a task from a string
		/// </summary>
		/// <param name="taskLine">Task line.</param>
		public Task(string key, string taskLine)
		{
			Key = key;
			Metadata = new BindingsTable<string, string> ();

			// The "treated" line starts with the key and just contains the metadata and the text
			var treatedLine = new string (Transformations.StartingWith (taskLine, key).AsArray());

			// Treats anything{with brackets} like one word.
			var meta = Transformations.WhileInclusive (
				           Sections.Globs (treatedLine, '{', '}'),
				x => (! (x.Contains (":"))));

			// meta[0] should actually be "KEY:" or "KEY"
			if (!meta.Get(0).Contains (Key))
				return;

			// Read the metadata text into this Task's metadata structure.
			foreach (string metadataField in Transformations.Subsequence(meta, 1, meta.Length())) {
				Metadata.Associate (
					// All text before {
					new string(Transformations.While (metadataField, x => (!x.Equals ('{'))).AsArray()),
					// Everything in between { and }, split up based on commas
					Sections.Internal (metadataField, '{', '}').Split (','));
			}

			// Alright, so taskLine.Length is used instead of an abitrarily high integer because I don't know what is being thrown at this
			Text = Sections.RepairString (Transformations.Subsequence (Sections.EscapeSplit (taskLine, ':'), 1, taskLine.Length).AsArray());
		}

		/// <summary>
		/// Try to return all of the values associated with this key in this task's metadata
		/// </summary>
		/// <param name="item">The key to search for</param>
		public string[] Fetch(string item)
		{
			return Metadata.Lookup (item);
		}

		/// <summary>
		/// Determines whether or not this Task defines the metaIdentifier as a key
		/// </summary>
		/// <returns><c>true</c> if this instance has metadataIdentifier; otherwise, <c>false</c>.</returns>
		/// <param name="metadataIdentifier">The key to search for</param>
		public bool Has(string metadataIdentifier)
		{
			return Metadata.Lookup (metadataIdentifier).Length > 0;
		}

		/// <summary>
		/// Return the metadata in the format that it is written in.
		/// </summary>
		/// <param name="metadataIdentifier">The key to search for</param>
		public string Consolidated(string metadataIdentifier)
		{
			var containing = Metadata.Lookup (metadataIdentifier);
			string final = metadataIdentifier + ": ";

			foreach (string item in containing)
				final += (item + ",");

			return new string(Transformations.Subsequence (final, 0, final.Length () - 1).AsArray());
		}

		/* Special Metadata */

		/// <summary>
		/// Attempt to get the name metadata
		/// </summary>
		/// <returns>The name.</returns>
		public string GetName()
		{
			if (Has ("name"))
				return Fetch ("name").Get (0);
			else
				throw new Exception ("Something has called GetName(), but task " + this + " does not have one.");
		}

		/// <summary>
		/// Attempt to get the depdendencies metadata
		/// </summary>
		/// <returns>The dependencies.</returns>
		public string[] GetDependencies()
		{
			return Fetch ("dependencies");
		}

		/// <summary>
		/// Attempts to remove this Task from its host file.
		/// </summary>
		public void AttemptMark()
		{
			string filename = Fetch ("file").Get(0);
			var outputLines = new List<string> ();

			foreach (string line in File.ReadAllLines(filename)) {
				if (line.Contains (Key) && line.Contains(Text))
					outputLines.Add (line.Remove (Transformations.SequencePosition (line, Key)));
				else
					outputLines.Add (line);
			}

			File.WriteAllLines (filename, outputLines);
		}

		/// <summary>
		/// Get the task body of this object.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Todo.Task"/>.</returns>
		public string ToString()
		{
			return Text;
		}
	}
}


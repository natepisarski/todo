using System;
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
		public BindingsTable<string, string> Metadata { get; set; }

		public string Text;
		public string Key;

		/// <summary>
		/// Create a task from a string
		/// </summary>
		/// <param name="taskLine">Task line.</param>
		public Task(string key, string taskLine)
		{
			Key = key;
			Metadata = new BindingsTable<string, string> ();
			var treatedLine = new string (Transformations.StartingWith (taskLine, key).ToArray ());

			// Treats anything{with brackets} like one word.
			var meta = Transformations.WhileInclusive (
				           Sections.Globs (treatedLine, '{', '}'),
				x => (! (x.Contains (":"))));

			if (!meta[0].Contains (Key))
				return;

			foreach (string metadataField in Transformations.Subsequence(meta, 1, meta.Length())) {
				Metadata.Associate (
					new string(Transformations.While (metadataField, x => (!x.Equals ('{'))).ToArray()),
					Sections.Internal (metadataField, '{', '}').Split (','));
			}

			// Alright, so taskLine.Length is used instead of an abitrarily high integer because I don't know what is being thrown at this
			Text = Sections.RepairString (Transformations.Subsequence (Sections.EscapeSplit (taskLine, ':'), 1, taskLine.Length).ToArray());
		}

		public string[] Fetch(string item)
		{
			return Metadata.Lookup (item);
		}

		public bool Has(string metadataIdentifier)
		{
			return Metadata.Lookup (metadataIdentifier).Length > 0;
		}

		public string Consolidated(string metadataIdentifier)
		{
			var containing = Metadata.Lookup (metadataIdentifier);
			string final = metadataIdentifier + ": ";

			foreach (string item in containing)
				final += (item + ",");

			return new string(Transformations.Subsequence (final, 0, final.Length () - 1).ToArray());
		}
		/* Special Metadata */
		public string GetName()
		{
			if (Has ("name"))
				return Fetch ("name").Get (0);
			else
				throw new Exception ("Something has called GetName(), but task " + this + " does not have one.");
		}

		public string[] GetDependencies()
		{
			return Fetch ("dependencies");
		}

		public string ToString()
		{
			return Text;
		}
	}
}


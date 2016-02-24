﻿using System;
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
		public BindingsTable<string, string> Metadata;

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

			// Treats anything{with brackets} like one word.
			var meta = Transformations.WhileInclusive (
				           Sections.Globs (taskLine, '{', '}'),
				x => (! (x.Contains (":"))));

			if (!meta [0].Equals (Key))
				return;

			foreach (string metadataField in Transformations.Subsequence(meta, 1, meta.Length())) {
				Metadata.Associate (
					new string(Transformations.While (metadataField, x => (!x.Equals ('{'))).ToArray()),
					Sections.Internal (metadataField, '{', '}').Split (','));
			}

			// Alright, so taskLine.Length is used instead of an abitrarily high integer because I don't know what is being thrown at this
			Text = Sections.RepairString (Transformations.Subsequence (Sections.EscapeSplit (taskLine, ':'), 1, taskLine.Length));
		}

		public string[] Fetch(string item)
		{
			return Metadata.Get (item);
		}

		public bool Has(string metadataIdentifier)
		{
			return Metadata.Get (metadataIdentifier).Length > 0;
		}
	}
}


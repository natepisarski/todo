# Todo
Hey! Welcome to the Todo repository. Let's just get right down to business on what this project is.

Todo is a command-line productivity tool for scraping TODO comments out of your code. The code itself is a lot more flexible than the tool, so TODO comments must be formatted exactly like this:
    TODO: thing
Or, alternatively,
    TODO name{itsName} dependencies{name,of,dependencies,in,comma,list,spaces allowed}: Blarg

Name, dependencies, and file are the only 3 metadata tags that will mean anything. But you already know how to write TODO comments. So here is the tool:

# The Tool
The command line tool parses all the text in the directories given to it and can listen to any of the following commands:
* contract
* detailed
* remove

Contract provides a list of all the tasks that must be completed in order to satisfy a named task's dependencies. Detailed gives information on ALL of the tasks found in a directory. Remove will remove tasks with that name from all of the files it's found in.

# License
Everything in this repository is BSD 3 clause. Do with it as you will. 

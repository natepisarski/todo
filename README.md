# Todo - Source Code TODO comment tracker
**This is not a normal TODO application**, this is something else.

TODO applications have become the "advanced" version of the "Hello World" app in recent years. Despite the name, that's not what this app is.

This is a TODO manager for `// TODO:` comments in source code, with dependency tracking.

You can build a dependency graph just like a package manager would.

**Example**
```javascript
export const getAccountProfile = () => {
// TODO name{profile} dependencies{api}: Need the ability to call API endpoints to work
};

export const showUserPage = () => {
// TODO name{user} dependencies{profile,api}: Need the profile page to continue
}
```

# The Tool
The following 3 commands can be used:
* `contract` provides a list of all tasks, and the order they need to be completed in
* `detailed` gives information about every task, in the entire directory
* `remove` will actually strike the TODO comment from the directory

# License
Everything in this repository is BSD 3 clause. Do with it as you will. 

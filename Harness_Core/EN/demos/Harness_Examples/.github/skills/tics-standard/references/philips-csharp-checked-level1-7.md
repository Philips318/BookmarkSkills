# Philips C# Checked Rules Levels 1-7

Source summary:

- Ruleset: Philips C# Coding Standard 5.33
- Set ID: `4T_Jr6-VSX6fp6egDIhGow`
- Filter: `Status = CHECKED`
- Suggested default must-fix range for agent-delivered code: levels 1 through 7

Checked rule counts by severity:

- Level 1: 9
- Level 2: 12
- Level 3: 5
- Level 4: 8
- Level 5: 5
- Level 6: 2
- Level 7: 4
- Level 8: 1
- Level 9: 7
- Level 10: 1

## Checked Rules By Severity 1-7

| Level | Rule | Category | Synopsis |
|---|---|---|---|
| 1 | 10@406 | Data types | When using composite formatting, do supply all objects referenced in the format string |
| 1 | 5@121 | Object lifecycle | Don't use "using" variables outside the scope of the "using" statement |
| 1 | 6@191 | Control flow | Do not dereference null |
| 1 | 7@502 | Object oriented | Do not modify the value of any of the operands in the implementation of an overloaded operator |
| 1 | 7@520 | Object oriented | Override the GetHashCode method whenever you override the Equals method. |
| 1 | 7@521 | Object oriented | Override the Equals method whenever you implement the == operator, and make them do the same thing |
| 1 | 8@102 | Exceptions | Do not throw exceptions from unexpected locations |
| 1 | 8@110 | Exceptions | Do not silently ignore exceptions |
| 1 | 9@113 | Delegates and events | Always check an event handler delegate for null |
| 2 | 10@401 | Data types | Floating point values shall not be compared using the == nor the != operators nor the Equals method. |
| 2 | 4@101 | Comments | Each file shall contain a header block |
| 2 | 5@108 | Object lifecycle | Do not re-declare a visible name in a nested scope |
| 2 | 5@113 | Object lifecycle | Implement IDisposable if a class uses unmanaged resources, owns disposable objects or subscribes to other objects |
| 2 | 5@114 | Object lifecycle | Do not access any reference type members in the finalizer |
| 2 | 6@101 | Control flow | Do not change a loop variable inside a for loop block |
| 2 | 6@105 | Control flow | Ensure switch statements are exhaustive |
| 2 | 7@101 | Object oriented | Declare all fields (data members) private |
| 2 | 7@531 | Object oriented | Overload the equality operator (==), when you overload the addition (+) operator and/or subtraction (-) operator |
| 2 | 7@532 | Object oriented | Implement all relational operators (<, <=, >, >=) if you implement any |
| 2 | 9@110 | Delegates and events | Each subscribe must have a corresponding unsubscribe |
| 2 | 9@114 | Delegates and events | Do not use return values of callbacks in events |
| 3 | 4@111 | Comments | Don't comment out code |
| 3 | 7@105 | Object oriented | Explicitly define a protected constructor on an abstract base class |
| 3 | 7@530 | Object oriented | Implement operator overloading for the equality (==), not equal (!=), less than (<), and greater than (>) operators when you implement IComparable |
| 3 | 7@533 | Object oriented | Do NOT use the Equals method to compare diffferent value types, but use the equality operators instead. |
| 3 | 8@107 | Exceptions | Use standard exceptions |
| 4 | 10@407 | Data types | When using composite formatting, do not supply any object unless it is referenced in the format string |
| 4 | 2@107 | General | Do not suppress compiler warnings in the code |
| 4 | 5@111 | Object lifecycle | Avoid implementing a finalizer |
| 4 | 6@119 | Control flow | Avoid locking on a public type |
| 4 | 6@201 | Control flow | The cyclomatic complexity of a method should not exceed its configured maximum. |
| 4 | 7@106 | Object oriented | Make all types internal by default |
| 4 | 7@107 | Object oriented | Limit the contents of a source code file to one type |
| 4 | 7@404 | Object oriented | Don't hide inherited members with the new keyword |
| 5 | 5@119 | Object lifecycle | Return interfaces to unchangeable collections |
| 5 | 6@115 | Control flow | Do not access a modified object more than once in an expression |
| 5 | 7@102 | Object oriented | Prevent instantiation of a class if it contains only static members |
| 5 | 7@608 | Object oriented | Use pattern matching instead of the "as" keyword |
| 5 | 7@611 | Object oriented | Use generic constraints if applicable |
| 6 | 7@303 | Object oriented | If you must provide the ability to override a method, make only the most complete overload virtual and define the other operations in terms of it |
| 6 | 7@501 | Object oriented | Do not overload any 'modifying' operators on a class type |
| 7 | 10@301 | Data types | Do not use 'magic numbers' |
| 7 | 3@204 | Naming | Do not use letters that can be mistaken for digits, and vice versa |
| 7 | 3@504 | Naming | Name the source file to the main class |
| 7 | 7@609 | Object oriented | Use the correct way of casting |

This file is a team reference only. The source of truth remains CSViewer and the TICS or CI rerun on the current branch.
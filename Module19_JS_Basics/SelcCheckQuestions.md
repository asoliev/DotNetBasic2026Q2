# Questions for the self-check

1. What is JavaScript? What are the advantages of JS?

JavaScript is a high-level, interpreted programming language used mainly for web development. It runs in browsers and on servers. Its advantages include wide platform support, easy integration with HTML and CSS, support for interactive user interfaces, and a large ecosystem of libraries and frameworks.

2. What are JS data types?

JavaScript data types include primitive types and object types. Primitive types are `string`, `number`, `bigint`, `boolean`, `undefined`, `symbol`, and `null`. Objects include plain objects, arrays, functions, dates, and many other built-in or custom types.

3. What are the pros of including 'use strict' at the beginning of JS source file?

`'use strict'` enables strict mode, which makes JavaScript safer and easier to debug. It catches common mistakes, prevents accidental creation of global variables, disallows some unsafe syntax, and makes `this` behave more predictably in functions.

4. What is variable scope? What are global variables? How are these variables declared?

Variable scope is the part of the program where a variable can be accessed. Global variables are variables that can be accessed from anywhere in the script. They are usually declared outside any function or block, for example with `var`, `let`, or `const` at the top level of a script; in browsers, top-level `var` declarations also become properties of `window`.

5. What is the difference between == and === operators?

`==` compares values after type coercion, so it may convert one operand before comparing. `===` compares both value and type without coercion, so it is usually the safer choice.

6. What would be the result of 3+2+”7″?

The result is `57`. JavaScript evaluates left to right: `3 + 2` gives `5`, then `5 + "7"` converts `5` to a string and concatenates, producing `"57"`.

7. What is the difference between null and undefined in JS?

`undefined` usually means a value has not been assigned or returned. `null` is an explicit assignment meaning "no value" or "empty value". In loose comparison, `null == undefined` is `true`, but they are different values.

## Plans

### :triangular_flag_on_post: Syntax Specification and Documentation

- 2023.8.10

We need to discuss, organize, and standardize the syntax of Acss, creating normative documentation. The syntax of the current version might change in subsequent updates.

### :triangular_flag_on_post: Base Style Library

> Nlnet.Avalonia.Css.Controls

- 2023.8.10

A themed library for Avalonia's basic controls based on the Acss pattern, not providing any visual styles, only retaining standard control template structure. Apart from minor local optimizations, it does not extend functionality.

### :triangular_flag_on_post: Fluent Visual Style Library

> Nlnet.Avalonia.Css.Fluent

- 2023.8.10

This is a visual style library supported by Acss, based on the Fluent design language.

### :triangular_flag_on_post: Extended Control Library

> Nlnet.Avalonia.Senior

- 2023.8.10

This library offers advanced feature extensions. It will provide some advanced controls, such as smooth-scrolling scrollbars, MessageBox, Toast, etc. Note that the visual style support is currently planned from Nlnet.Avalonia.Css.Fluent, and independent visual styles will not be provided for now.

### :triangular_flag_on_post: Core

> Nlnet.Avalonia.Css

- 2023.8.10

This is the core of Acss, responsible for language parsing, object generation, and lifecycle control.

### :triangular_flag_on_post: Built-in Behaviors

> Nlnet.Avalonia.Css.Behaviors

- 2023.8.10

The purpose of this component is to provide some commonly used behavioral features, enhancing Acss's ease of use.

### :triangular_flag_on_post: Built-in Features

- 2023.8.10

Providing some commonly used features, such as effects, animations, etc.

### :triangular_flag_on_post: Debugger

- 2023.8.10

Support for monitoring and modifying Acss throughout its lifecycle, and support for code rewriting.

### :triangular_flag_on_post: Usability

- 2023.8.10

Enhancing the convenience of using Acss, providing richer extension APIs, including context configuration APIs, hot update APIs, lifecycle management APIs, etc.

### :triangular_flag_on_post: Sample Library

- 2023.8.10

Mainly Nlnet.Avalonia.Css.Fluent and Nlnet.Avalonia.Css.App.

### :triangular_flag_on_post: Security

- 2023.8.10

This part needs to consider security issues with Acss. Currently, Acss provides external static text files without any security protection. This issue needs comprehensive consideration and the provision of solutions and explanations for various scenarios.

### :triangular_flag_on_post: Editor

- 2023.8.10

Acss needs support from mainstream editors for development. Currently, we plan to provide language plugins for Rider and VS Code. A rudimentary version of the VS Code plugin has already been released. Rider currently supports it through language configuration, also in a rudimentary form.
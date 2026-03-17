# Infrastructure Project

This project is the [infrastructure](https://github.com/jasontaylordev/CleanArchitecture/tree/main/src/Infrastructure) layer.

- Handles external concerns such as
  - Database provider implementations and persistence format
  - Messaging systems
  - Eamil services

This is depends on both [Domain](../Domain) and [Application](../Application) but it should not depend on [Api](../Api) or [web](../web).
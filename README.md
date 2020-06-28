# Mockasin
Like the shoe, but with the spelling changed because it's a mocking tool.

## Overview
This is the second iteration of a tool that I wrote for integration projects. We often found ourselves writing code to interact with systems that we don't yet have connectivity to (or weren't even built yet), so we needed something to call.

I write the first one using [Node](https://nodejs.org/en/) and [Express](https://expressjs.com) (in a rush), so this will (hopefully) be a more polished version that same thing. In the end, the goals of this project are:

- Make it easy enough to mock real endpoints
- Move to C# and .NET (because that's what I use for the bulk of the work I do)
- Give me a project that I can use to explore some of the more complex parts of ASP.NET

_Yes, I know that building against a mock is not the same as building against the real thing. But this is for the case when we don't have the real thing._

## Down the road
These are things that I'd consider doing in the future, which are somewhat guiding the design:

- Run in process (in a test project) to set up endpoints to test against. Can then write some tests for a full system without having any dependencies on anything else
	- And be able to query if specific endpoints were called
- Build a UI (somewhat like [Insomnia](https://insomnia.rest) or [Postman](https://www.postman.com)) to change endpoints, change responses, or select a specific response to send when a request comes in
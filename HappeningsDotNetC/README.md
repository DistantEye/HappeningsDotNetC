# Happenings App



## Requirements

The project is built of .NET Core and is best run in Visual Studio with the proper support for ASP.NET Core and related installed

Secondary components are NPM, Grunt, Bootstrap, Jquery, Moment

Assuming Node/NPM is functional on system, all dependencies should correctly load in via running 

```npm install```

## Project Requirements Documentation

The project was properly constructed with requirements writeups first, and the associated notes from that are availible [here](Requirements.md)

### Folder Notes

A compressed explanation of the project structure is availible below:

* Top level
  * Contains config files for Grunt, npm, and the .NET application
	* In additional to the usual Program.cs/Startup.cs, there's DependencyInjectionMappings.cs which separates out the DI mapping
* wwwroot
  * Primary public folder. Contains css and javascript libaries (node modules are copied into wwwroot/lib via grunt command)
* Controllers
  * Holds controllers for all endpoints as well as an extra abstract parent for common code across nearly all controllers
  * Any of the controllers using the abstract parent have a primary service associated to them which is defined as the CRUD service they most commonly operate on
  * LoginController is the exception for inheritance since it isn't tightly bound to the conventional CRUD Api structure like the other controllers
  * Glancing through the Controller files should give a good idea about the endpoints/functions of the project, since they're intended to be descriptive without doing much heavy lifting functionally
	* Whenever possible, functional churn is handled in the service layer as it should be
* Database
  * Contains EF Migrations and the DatabaseContext implementation for the project
* Dtos
  * Contains all Dtos for the project, organized into subfolders
  * Entity Dtos are Dtos that directly tie to a particular Model from the database
  * Intermediary Dtos are secondary structures not directly tied, and used for communication between controllers and services
* Helpers
  * General helper classes/structures, more directly tied to the project infrastructure than a particular area (such as Service-specific helpers)
* Interfaces
  * Holds all interfaces used for the project, organized by functional purpose
  * DtoInterfaces is a stub folder holding any common interfaces across Dtos : currently only IDbDto is needed to define the bare bones of what makes a EntityDto an EntityDto
  * EntityInterfaces is an equivalent stub for defining base behavior for all Model classes/structures
  * ServiceInterfaces holds all the service relevant interfaces used for DependencyInjection
* Models
  * Holds all Database mapped Model entities, as well as ErrorViewModel, which is not a true Db entity but used for part of the error handling structure
  * JoinEntities holds mapping table type entities like HappeningUser (handles the Many<->Many mapping between Happening and User)
* Services
  * Holds all project services
  * Most services follow the structure defined by AbstractApiService, which handles all CRUD-centric services in a unified structure
    * AbstractApiService tries to hide the means of Database access as best as it can, but can't do a complete job of it
	* While the main outward facing Create/Update/Delete calls can be overriden by child classes, the presence of hooks for Pre/Post Create|Update|Delete behavior are intended to make that unnecessary
	  * Overriding those functions should only be done in cases of unavoidable needed
	* Implementing services focus mostly on overloads/extra functions, as well as defining the exacts of how a new Model is created or updated (since the generics of the abstract service can't handle that part)
  * LoginService doesn't follow the pattern, as it deals more with helper functions and accessing the HttpContext than dealign with the database itself
* Views
  * Standard views folder, with subfolders organized to match the different project Controllers, plus shared files

## Explanatory Notes

Here is collected any major notes about project choices or oddities that are intentional and may cause confusion if not explained.

* The pure .NET MVC application (as opposed to the ones that only use .NET for backend) is coded with as minimal/simple javascript as possible and sometimes dated methologies to highlight the difference between more modern design practices.
  * This is philosophically intentional : prototype projects should use older, even ancient concepts, rather than newer stuff, since the point isn't to build a wonderful looking site, just to build a proof of concept as quickly as possible
  * This means using techniques every engineer should know, or lacking knowledge, can find countless examples online of
  * Previous career experience has demosntrated that an urge to make each page look the best as possible before the project is complete is an over optimization that can viciously backfire if the project goes to user/customer review and it turns out a core functional requirement was not met
* The application is laid out to have both an MVC frontend as well as REST service type endpoints for an external front end to use
  * Whenever possible the MVC endpoints rely on the Api endpoints, so code isn't duplicated and backend errors will be as consistent as possible between the MVC project and any frontend projects
* The goal is to make a backend that's interchangable across different front ends seamlessly (both because it makes doing multiple projects easier as well as it being a good tech demo)
* The project may well change based on feedback post making it public : is ultimately a prototype and attempts to adapt it to better match standard practices are desirable

## Pitfalls

Similar to the previous section but this discusses shortcommings of the project that would ideally be fixed but are beyond the scope of the project to fix. 

Essentially a compromise between there being only so much time to work on a side project, but the need to make clear that the designers are aware of what things wouldn't be acceptible for an official work.

* Issue : Users can override visibility settings so if someone sees a Hidden Happening made visible on another user's calendar, or somehow guesses the view link otherwise : they can still join that Happening. 
  * Backend could do a final check of user privledges during the service call (making sure edit event requests are being done by someone with the authority to do them)
* Design Gap : Timezones are always fun to deal with and the current simple model does not do anything to try and adjust for timezone differences between users
* The full level of abstraction as seen in larger projects is not used as it unnecessarily complicates the example.
  * In a fully professional setup, a lot of the individual top level folders would be abstracted into their own project inside the greater VS Solution with reference controls in place.
  * However, even starting a project like this professionally I'd likely use the same setup initially : early segregation into different projects can lead to bad code structures that are frustrating to manage and repair into the desired end form.
  * Other similar issues of something being a great production practice but overkill for compact, readable, sample projects, are denoted in comments
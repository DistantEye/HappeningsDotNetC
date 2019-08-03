# Project Requirements

While not conforming to any exact standard, this document is an approximation of the kinds of requirements documents professional projects should have, at minimum. Some works will be more detailed in their writeup than others.

The writeup is split into major sections: a high level concept section and a more detailed semi-granular functional requirements section for the primary sections of the site. The general idea is that the first section should be something akin to what an initial meeting/meetings might have come up with and recorded down, and the second section is the result of someone parsing those requirements into a more implementation-friendly list of tasks. Since in this case the project is a one person job, there isn't the full granularity of splitting things into front and backend concerns, but it should be pretty easy to expect how the second section could morph into such a thing.

While some changes were updated post project start, overall the majority of these requirements were successfully kept from original writeup to implementation, which should be the goal as what counts for a sufficiently well fleshed out requirements writeup. Making changes is fine and to be expected, but making too many changes suggests that the initial planning was probably inadequate.

## Conceptual Requirements

### Visual Form

Single Page App/Pseudo SPA
  * Main Page is the Calendar itself, with top bar buttons
  * In True SPA form, the buttons trigger modals to appear
  * In Classical App form, the sub pages take the place of the Calendar, but the Calendar screen is Home, at the layout is structured in a way that makes it seem SPA-like with consistent structure/framing

### Main Feature Needs

  * Calendar => Usually the logged in user. View a Monthly Calendar that displays happenings, presumably repeating if they extend across days. Option to change months. Can click on happenings to Happening View
  * Login => Need some kind of login page to throw people to first
  * Happening Create => Through menu button or proper click action on slots in the calendar : Create some thing on the calendar with various settings
  * Happening Administration Page => Edit form of Create, with also options
  * Clock => Mostly a menubar widget but also clickable to acknowledge upcoming alarms/happenings so it removes the "impending thing" icon on the clock bar
  * Happening View => Users can see happenings they're in (or have been invited to) and read necessary info, manage their status, etc.
  * Invitations Page => Although the Calendar/Happening View page also gives options for this, a more table/list version of things so people can quick manage stuff.
    * This should have some kind of number display in the top bar so people know if they have still pending invitations
    * The page should only list invitations that haven't been responded to
  * View Other User's Calendars => Table/List/Search Page to try and look at another user's calendar.
    * Some users deny anyone being able to look at all. Anyone else can override the default visibility options for an happening to hide or show it against normal settings
  * Admin
    * Admins need some way to manage users/reset passwords, etc.

### Concepts/Major Objects

These are either the direct database elements or other supporting elements for the app's data structure.

  * Happening => Particular date/time, description, primary user, secondary users, list of user RVSP status, visibility info*, reminder settings (we use some kind of app global thing that can annoy you with a modal)
    * visibility info determines the default visibility setting for if someone can see it on someone else's calendar calendar
    * There will need to be an HappeningsToUsers table DB in terms of implementation, and that will also contain fields for the RVSP status, as well as any User visibility overrides for their status on the happening
  * Invitations should not exist as a specific object, they can be parsed by seeing what Happenings a user has and whether they've responded or not
    * (Post-work note : requirement rejected for EF Core implementations since the mapping table has to be explicit anyways)
  * RSVP status is hardcoded as as far as "NoResponse", "Yes", "No", "Maybe"
  * Users => Normal/Admin Status, User name, friendly name, calendar visible to others flag

## Hard Requirements

  * Login Page
    * User shall be redirected to the Login page regardless of intended destination whenever login information is not stored
    * Login page should hide/blur all other features beyond a simple user/password box and, at most, an application name
    * If feasible, the user's original destination should be stored
    * Once logged in, navigational flow should return to the original intended page, or the Calendar Main Page, if none intended

  * Top Menu Bar
    * Unless otherwise specified , top menu bar should always be visible
    * Menu bar should consist of links to: Home(Calendar), Happening Create, Pending Invitations, View Other Calendars, Logout
    * Pending Invitations and Clock may have relevant number prefixes to show pending reminders/invitations
    * If in "View Other" mode, there should be an extra [] note with the Friendly Name of the User Being Viewed, with an X box for canceling out the view other mode

  * Calendar Main Page
    * User should default to seeing a calendar month view for the current month
    * The top of the page should include options for changing the Month/Year setting to some time other than present
    * Next to the date controls should be a toggle for 'List View' (see below set)
    * Suggested visual style is a traditional black lines and text on a white background.
    * Happenings should appear in boxes as appropriate to what day they are on, with Happening Name and Time fitting as best visually appropriate
    * If some happenings take place over multiple days, the happening should appear in all relevant day boxes
    * If multiple happenings take place in one day they should appear in the box according to start time order
    * If multiday happenings overlap the start time ordering should still persist across successive date boxes (this will give visibility priority to older multiday happenings)
    * Each happening row should have some form of clickable means to navigate to View/Edit Happening Information in full

  * Calendar List View
    * Similar to above, the top page should have a toggle for 'Calendar View'
    * Instead of Month/Year controls, there should be options to filter the Happening lists to a certain start and end date range
    * Default Filter should be start day as present date, no ending date
    * This page, if unfiltered, shall show every Happening the User has in chronological order, obeying filters otherwise
    * Happenings should be presented in a pseudo-tabular form with columns Happening Name, Happening Date, Happening RVSP status
    * Each happening row should have some form of clickable means to navigate to View/Edit Happening Information in full

  * Happening Create
    * Aside from visual indicators of the page being Create rather than edit, this should be requirements identical to a blank version of Happening Administration (editing)

  * Happening Administration
    * Should include text/date fields for setting the following information : date/time, name, description, primary user, secondary users, list of user RVSP status***, visibility info***, reminder settings***
      * RVSP Status is a dropdown for "NoResponse", "Yes", "No", "Maybe"
      * Visibility info determines the default setting for if other users can see an event on their calendar, and if new users can add themselves to the event.
        * Options should be Public, Hidden, Closed. Hidden can be openly joined but won't be visible by default
      * reminder settings is when to create a clock alert for the happening
      * Some of these settings are obviously local user specific, and are not required if for some reason the administrative user is not a member of the happening
    * Happening Name and Happening Date are required fields
    * All available preexisting information for the happening should fill fields as appropriate
    * At the bottom of the page (or as a subpage/modal) there should be a User control option for removing users already attached to the happening
    * Should also be a control for inviting users

  * Clock
    * When navigating to the clock page there should be a simple headerless table showing active alarms, and an option to dismiss them

  * Happening View
    * Note : if user has administrative rights over the happening being viewed, this page should redirect to Happening Administration
    * If not administrative rights, the appearance of the page should be similar to Happening Administration, but with no invite/user management controls, and most fields beyond RVSP status, visibility info, and reminder settings forced to read only
    * If the happening isn't a Closed happening, there should be an option to join the happening if not a member of it already

  * Invitations Page
    * Should be a simple headerless table showing Happening Name, Date, and a dropdown to change RVSP status
    * Depending on UX implementation, either changing the dropdown triggers an auto submit, or there's an explicit submit changes button
    * Only happenings where RSVP is "NoResponse" should be listed and changing off that status removes them from the page's list

  * View Other Calendar Page
    * Both versions of this page (the calendar and list form) should function identically to those pages except with a session tag (or equivalent) to override which user to fetch information for
    * The backend will handle the presence of that tag and the necessary restraining behaviors (visibility settings should be respected/checked closer, no edits should be possible)
    * Going to any page beyond the Calendar/List View pages should drop the user back to their own user context, prhappeninging improper modifications from being made

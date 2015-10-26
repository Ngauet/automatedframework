# Introduction #

- 3 years experience in automation testing.
- 3 years experience in software developing.


# Details #
## 1.	Starting Automation ##

a)	Describe more exect what software do.

b)	Developers have to design the system to make to testable.

c)	Avoid automating existing manual test scripts

d)	If the test is described as a sequence of interdependent steps,
it will be very hard to understand what exactly caused the problem, because the context changes throughout the script.

e)	Don't treat automation code as second-grade code.

## 2.	Managing the automation ##
a)	The testing framework needs to have as good a design as the actual product because it needs to be maintainable. Part of the reason why the test system succeeded was that I knew about the structure and I could read the code.

b)	Describe validation process in the automation layer.

c)	Don't replicate business logic in the test automation layer.

d)	Automate along system boundaries.
> When: complex integration.

e)	Don't check business logic through the use interface.

f)	Automate below the skin of the application. When: Checking session and workflow constraints.

## 3.	Automating user interface: ##
a)	Specify user interface functionality at a higher level of abstraction.

b)	Check only UI functionality with UI specifications. When: User interface contains complex logic.

c)	Avoid recorded UI tests

d)	Set up context in a database.

## 4.	Test data managemnt ##

a)	Avoid using prepopulated data. When: Specifying logic that's not data-driven

b)	Try using prepopulated reference data.When: Data-driven systems

c)	Pull prototypes from the database. When: Legacy data-driven systems.
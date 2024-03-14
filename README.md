# asctool

=================
ASCTOOL 
=================

1.0 Documentation

	asctool is use to automate all the necessary steps upon deployment, which includes performing the following: 

	1.1 connecting to database and perform changes.
	1.2 stopping the information internet services.
	1.3 performing a backup the recent website file system.
	1.4 copying the new website file system.
	1.5 starting the information internet services.

--------------------------------

2.0 Switches

	2.1 -d Switch

		The -d switch is used for connecting to database and perform changes. Reads a .txt file (e.g. SQLquery.txt) to perform a SQL query.

		asctool -d


	2.2 -stop Switch

		The -stop switch is used to manually stop all information internet services.
	
		asctool -stop


	2.3 -start Switch

		The -start switch is used to manually stop all information internet services.
	
		asctool -start


	2.4 -b Switch

		The -b switch is used to backup the recent website file to a specific directory.

		asctool -b	


	2.5 -c Switch

		The -c switch is used to backup the recent website file to a specific directory.
	
		asctool -c


	2.6 -i Switch

		The -initialize switch is used to automate all the steps necessary for upon deployment.

		asctool -i
	
	2.7 -h Switch

		The -h switch stands for help is used to help the user of asctool.

		asctool -h

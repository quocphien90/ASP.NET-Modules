**************************************
**************************************
INSTRUCTION : Modules create report section in Umbraco Section, user can create sql for select data and filter it
			  Modules upload files, it will display all files in server and user can upload new file if it has write permission on that folder

**************************************
**************************************
		INSTALL ALL REPORT/UPLOAD MODULE
**************************************
**************************************
1/ Run 2 SQL scripts 
	- TablesReport.sql
	- dsFileManager.sql
	
2/ Copy and paste all files in folder /Web Resources Deploy into root of umbraco site

3/ Grant permission admin for Report and Configure Section in Users Section

**************************************
**************************************
		USAGE REPORT MODULE
**************************************
**************************************

1/ Create Select SQL Scripts in Report/Config node
Example : 
SELECT * FROM (
	 SELECT *,ROW_NUMBER() OVER (ORDER BY [TablesReport].[ID] DESC) AS [RowNum] 
	 FROM [TablesReport]
	 WHERE (@PropertiesAlias IS NULL OR [Properties] = @PropertiesAlias)
	 ) [T]
WHERE [T].[RowNum] BETWEEN @Begin AND @End

** @PropertiesAlias Is Properties Alias when we Add Properties in Properties TablesReport
** @Begin, @End must be in the end of the queries for pagination in SELECT above

2/ Create Select Count for Pagination
Example : 
SELECT COUNT(*) FROM [TablesReport]
WHERE (@PropertiesAlias IS NULL OR [Properties] = @PropertiesAlias)

** @PropertiesAlias Is Properties Alias when we Add Properties in Properties TablesReport

3/ Add Properties For Add filter condition for SQL above, please describe Name, Alias , select Type of filter , then prevalue if have, and click Save Properties

4/ Choose your SQL Row in Report/Config and Approved it

5/ Reload Reports/Report => Your new report will display

**************************************
**************************************
		USAGE UPLOAD FILE MODULE
**************************************
**************************************

1/ After grant permission for user, go Config/File Manager

2/ Add new path for display , please fill Name, Path ( include ~ in prefix , eg: ~/css), how many items will display in view, and active it

3/ Reload File Manager, new node will display

4/ View files in this path and using Delete / Upload new file / Create folder / Download file in Section




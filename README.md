# FetchDataFromWikipedia
Console App for Fetching Wikipedia Data
This console application retrieves data from Wikipedia using either a company name (string) or an ID.

Implementation Steps:
Read Input from Console

The application first reads user input from the console.
Determine Input Type

It checks whether the input is a string (company name) or an ID (starting with the letter 'Q').
Call searchWikiData Function

If the input is a string (e.g., Skyscanner, Google), it is passed as a parameter to the searchWikiData function.
Inside this function:
The input is appended to the API URL.
An HTTP request is sent using HttpClient.
If the request fails (status code is not successful), an error message is displayed.
Otherwise, the response is read as a string and stored in the variable jsonResponse.
The JSON response is deserialized into another variable.
If the deserialized data is not null and has a count greater than zero:
A foreach loop iterates through the data to display the ID, title, and description.
The first found ID is returned.
Otherwise, an error message is displayed (either due to deserialization failure or no data found).
Retrieve Company Hierarchy

After fetching the search data, control returns to the Main method, where the result is stored in a variable.
If the result is not null, the GetCompanyHierarchy function is called with the search data as a parameter.
Inside GetCompanyHierarchy:
A SPARQL query is written to fetch the parent company of the searched entity.
Another API URL is constructed using this query.
An HTTP request is sent using HttpClient, and the response is processed similarly.
The JSON response is deserialized and displayed accordingly.

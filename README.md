# FetchDataFromWikipedia
A console app which fetches the data from Wikipedia either by using company name(any string) or ID

Used the httpClient, 
1 firstly reading the data(input) from the console
2. Checks whether it's a string or an ID starts with letter 'Q'
3. Then goes into the function called searchWikiData by taking a string as a parameter(i.e. Skyscanner, google)
4. In that function we append this tring input with apiurl, using HTTPClient we get the reponse from the url and checks if statuscode in false then simply display the message otherwise, read that response as string and stores into string variable called, jsonResponse then simple deserialize and store into another avriable and if its not null and count > 0 then using for each loop, we display the id, title  and description and returning the first id that found else deserialize failed or no data found
5.now we have search data and came back to main method and stores that into another variable and it its not null then call another function called GetCompanyHierarchy where we take search data as a parameter, now in that we write the sparqlQuery to show the parent company of that searched data, now we use another url to where we use sparqlQuery repeated the process of HttpClient and deserialise it and all

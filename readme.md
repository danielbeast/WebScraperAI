# Web Scraper AI
I developed Web Scraper AI, an AI-powered tool designed to extract any type of data from a website, delivering only the specific information you need.

## Dependencies
Selenium Web Driver in Docker Container

## Technologies Utilized
Google Gemini AI: For AI capabilities  
Selenium (4.27): For automating web browsing  
.Net Core with Razor (6.0LTS): For building the web API and frontend  
HtmlAgilityPack (1.11.72): For parsing and extracting data from HTML  
Mscc.GenerativeAI.Google (2.0.1): For integrating AI capabilities into scraper  
Axios (1.7.9): HTTP client for frontend to call web API  

## Quick Start
```
docker run -d -p 5000:5000 -e "SBR_DRIVER={SELENIUM URL}" -e "GEMINI_API_KEY={APIKEY}" danielgoddard/webscraperai:latest
```

## How to use
1. Input the website URL and a description of the data you want to extract.
2. The scraper fetches and parses the data, returning only the relevant results.
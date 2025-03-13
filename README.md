# TCGCardScraper

### Summary
This is an application for use with TCGPlayer.com, a trading card game marketplace.  

The site has an order optimizer where it takes the items in your cart and tries to find a combination of them with the lowest total price.  However, I like to utilize the smaller sellers on the market and the optimizer often seems to grab from the biggest stores.  Many sellers have free shipping at a $5 order.  This app will take an input of a number of URLs of market searches for cards, scrape each listing, and report which sellers have the largest number of listings from your desired card pool.

Basically, this application will help you optimize your order by fewest total packages, rather than lowest total price.

### Requirements
The application uses Microsoft Playwright for web scraping and currently is using Chromium, so Chrome must be installed.  A feature request would be allowing Playwright to run with whichever browser you have installed.

### Usage
- Edit any settings as desired in appsettings.json. The default directory for files is the application root.
  
- Go to tcgplayer.com and search for the listing for the specific card printing you want.  Set the filter for condition, language, etc.
  
- Copy the URL and paste it into urls.txt.
  
- Repeat for each card you want to include in the comparison (at least two).

- Run the application.  The application will scrape each individual listing for each card URL in the input file.

- The application will generate a report of which sellers (if any) have the most cards available from the input.

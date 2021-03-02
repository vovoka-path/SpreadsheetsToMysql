> **I'm looking for a job in Minsk, Belarus. My name is Vladimir Polansky.**

# SpreadsheetsToMysql

## Description

WPF application to export from Google Spreadsheets to MySQL.

## How to use

1. Create a file 'accessSQL.txt' in the '/bin/debug' folder with an access string to your mySQL as:
- `server=<yourserver>;username=<yourusername>;password=<yourpassword>;database=<yournameDB>;CharSet=utf8;convert zero datetime=true;allow user variables=true;`
2. Replace with your data (class SheetToSQL):
- `public static string nameNewTable = "customers10" // Name your mySQL table.;`
- `string sql = $@"CREATE TABLE `<nameNewTable>` (<your sql-request>)`
- `string password = "password"; // For AesCrypt.`
3. Change formatting way your data for mySQL in method SheetToSQL.ValuesToTable().
4. Get file 'credentials.json' from [.NET Quickstart](https://developers.google.com/sheets/api/quickstart/dotnet) and save in app root.
5. Rename your export sheet in Google Spreadsheets as 'final'. Change the range "!A2:P500" (class GoogleSheetsAPI), if necessary.
5. Launch app, insert Google spreadsheets link and press big button!

## Reason for creation

The client base of the photo agency is stored in Google Spreadsheets. The task is to transfer to MySQL DB. Part of the data needs to be formatted and encrypted.

This is a one-time operation and is intended to ensure the security of the transfer and data restructuring. 

## What I have learned

- Google Sheets API v.4
- AES Encryption/Decryption
- WPF

## Study way

Comprehensive study by searching on the Internet.

## How much time is spent

About 45 hours.

## Screenshot

![screenshot](https://github.com/vovoka-path/SpreadsheetsToMysql/blob/master/images/screenshot-SpreadsheetsToMysql.png)

## Decomposition

![DECOMPOSITION](https://github.com/vovoka-path/SpreadsheetsToMysql/blob/master/images/DECOMPOSITION-SpreadsheetsToMysql.png)
![INCOME DATA](https://github.com/vovoka-path/SpreadsheetsToMysql/blob/master/images/INCOME%20DATA-SpreadsheetsToMysql.png)

SpreadsheetsToMysql is maintained by [vovoka-path](https://github.com/vovoka-path/)
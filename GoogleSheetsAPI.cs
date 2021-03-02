using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using UserCredential = Google.Apis.Auth.OAuth2.UserCredential;
using System.Threading;
using System.Collections.Generic;
using Google.Apis.Sheets.v4.Data;

namespace SpreadsheetsToMysql
{
    public class GoogleSheetsAPI
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        [STAThread]


        public static SheetsService GetService()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
                
            return service;
        }


        public static IList<IList<Object>> GetSheet(string spreadsheetId)
        {
            string nameSheet = "final"; // default
            string range = "!A2:P500"; // default
            
            SheetsService service = GetService();
            
            SpreadsheetsResource.ValuesResource.GetRequest request = 
                service.Spreadsheets.Values.Get(spreadsheetId, nameSheet + range);

            ValueRange response = request.Execute();

            IList<IList<Object>> values = response.Values;

            return values;
        }

    }
}

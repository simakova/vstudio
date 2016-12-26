﻿/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//add
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
*/


//ex2
using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace DocumentDB
{
    class Program
    {
        // private const string EndpointUri = "https://imagenet.documents.azure.com:443/";
        // private const string PrimaryKey = "SHuQiB5Kb6ZTIUXp5OjWUJjgax2UpX1qo1uYloTuaqf1Ro6NhWp2wwdQqdDpiyHKz0yZ5NcHUUMTBkDwN6UCBg==";
        //private DocumentClient client;


        // The DocumentClient instance that allows the code to run methods that interact with the DocumentDB service
        private static DocumentClient client;
        // Retrieve the desired database id (name) from the configuration file
        private static readonly string databaseId = ConfigurationManager.AppSettings["DatabaseId"];
        // Retrieve the desired collection id (name) from the configuration file
        private static readonly string collectionId = ConfigurationManager.AppSettings["CollectionId"];
        // Retrieve the DocumentDB URI from the configuration file
        private static readonly string endpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        // Retrieve the DocumentDB Authorization Key from the configuration file
        private static readonly string authorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];

        static void Main(string[] args)
        {
            try
            {
                using (client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
                {
                    RunAsync().Wait();
                    Console.ReadKey();
                }
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("Status code {0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("Please, press any key.");
                Console.ReadKey();
            }
        }


            private static async Task<Database> RetrieveOrCreateDatabaseAsync(string id)
        {
            // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
            var database = client.CreateDatabaseQuery().Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();

            // If the previous call didn't return a Database, it is necessary to create it
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = databaseId });
                Console.WriteLine("Created Database: id - {0} and selfLink - {1}", database.Id, database.SelfLink);
            }

            return database;
        }



        private static async Task<DocumentCollection> RetrieveOrCreateCollectionAsync(string databaseSelfLink, string id)
        {
            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(databaseSelfLink).Where(c => c.Id == id).ToArray().FirstOrDefault();

            // If the previous call didn't return a Collection, it is necessary to create it
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(databaseSelfLink, new DocumentCollection { Id = id });
            }

            return collection;
        }



        private static async Task CreateGameDocumentsAsync(string collectionSelfLink)
        {
            // Create a dynamic object
            dynamic dynamicGame1 = new
            {
                gameId = "1",
                name = "Cookie Crush in Mars",
                releaseDate = new DateTime(2014, 8, 10),
                categories = new string[] { "2D", "puzzle", "addictive", "mobile", "in-game purchase" },
                played = true,
                scores = new[]
                {
                    new {
                        playerName = "KevinTheGreat",
                        score = 10000
                    },
                    new {
                        playerName = "BrandonGamer",
                        score = 5800
                    },
                    new {
                        playerName = "VanessaWonderWoman",
                        score = 10000
                    },
                }
            };

            var document1 = await client.CreateDocumentAsync(collectionSelfLink, dynamicGame1);



            // Create a dynamic object
            dynamic dynamicGame2 = new
            {
                gameId = "2",
                name = "Flappy Parrot in Wonderland",
                releaseDate = new DateTime(2014, 7, 10),
                categories = new string[] { "mobile", "completely free", "arcade", "2D" },
                played = true,
                scores = new[]
                {
                    new {
                        playerName = "KevinTheGreat",
                        score = 300
                    }
                },
                levels = new[]
                {
                    new {
                        title = "Stage 1",
                        parrots = 3,
                        rocks = 5,
                        ghosts = 1
                    },
                    new {
                        title = "Stage 2",
                        parrots = 5,
                        rocks = 7,
                        ghosts = 2
                    }
                }
            };

            var document2 = await client.CreateDocumentAsync(collectionSelfLink, dynamicGame2);
        }


        private static async Task RunAsync()
        {
            // Try to retrieve a Database if exists, else create the Database
            var database = await RetrieveOrCreateDatabaseAsync(databaseId);

            // Try to retrieve a Document Collection, else create the Document Collection
            var collection = await RetrieveOrCreateCollectionAsync(database.SelfLink, collectionId);

            // Create two documents within the recently created or retrieved Game collection
            await CreateGameDocumentsAsync(collection.SelfLink);

            // Use DocumentDB SQL to query the documents within the Game collection
            var game1 = client.CreateDocumentQuery(collection.SelfLink, "SELECT * FROM Games g WHERE g.gameId = \"1\"").ToArray().FirstOrDefault();

            if (game1 != null)
            {
                Console.WriteLine("Game with Id == \"1\": {0}", game1);
            }
            
        }

        /*




            try
            {
                Program p = new Program();
                p.GetStartedDemo().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        private async Task GetStartedDemo()
        {
            this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);

            await this.CreateDatabaseIfNotExists("FamilyDB_oa");
        }

        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        private async Task CreateDatabaseIfNotExists(string databaseName)
        {
            // Check to verify a database with the id=FamilyDB does not exist
            try
            {
                await this.client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
                this.WriteToConsoleAndPromptToContinue("Found {0}", databaseName);
            }
            catch (DocumentClientException de)
            {
                // If the database does not exist, create a new database
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDatabaseAsync(new Database { Id = databaseName });
                    this.WriteToConsoleAndPromptToContinue("Created {0}", databaseName);
                }
                else
                {
                    throw;
                }
            }
        }*/
    }
}

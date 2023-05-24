using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net.Http;
using Microsoft.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Xrm.Tooling.Connector;

namespace EnhancedQuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;
            try
            {
                //Get configuration data from App.config connectionStrings
                using (HttpClient client = SampleHelpers.GetHttpClient(connectionString, SampleHelpers.clientId, SampleHelpers.redirectUrl))
                {
                    // Use the WhoAmI function
                    var response = client.GetAsync("WhoAmI").Result;
                    

                    if (response.IsSuccessStatusCode)
                    {
                        //Get the response content and parse it.  
                        JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        Guid userId = (Guid)body["UserId"];
                        Console.WriteLine("Your UserId is {0}", userId);





                        CrmServiceClient service = new CrmServiceClient(connectionString);
                        // Check if the connection to Dataverse is successful
                        if (service.IsReady)
                        {
                            // Create a new FetchXML query
                            string fetchXml = @"
                        <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                            <entity name='account'>
                                <attribute name='name' />
                                <attribute name='emailaddress1' />
                                <filter type='and'>
                                    <condition attribute='address1_city' operator='eq' value='Redmond' />
                                </filter>
                            </entity>
                        </fetch>";

                            // Execute the FetchXML query
                            EntityCollection result = service.RetrieveMultiple(new FetchExpression(fetchXml));

                            // Process the retrieved records
                            foreach (Entity entity in result.Entities)
                            {
                                string name = entity.GetAttributeValue<string>("name");
                                string email = entity.GetAttributeValue<string>("emailaddress1");

                                Console.WriteLine("Name: {0}, Email: {1}", name, email);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to connect to Dataverse.");
                        }

                    }
                    else
                    {
                        Console.WriteLine("The request failed with a status of '{0}'",
                                    response.ReasonPhrase);
                    }

                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.DisplayException(ex);
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
            finally
            {
                
            }
        }
    }
}

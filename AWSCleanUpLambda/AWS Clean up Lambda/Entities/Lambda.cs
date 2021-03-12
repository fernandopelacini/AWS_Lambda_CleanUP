using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;

namespace AWS_Clean_up_Lambda.Entities
{
    public class Lambda
    {
        /// <summary>
        /// Clean up old versions from each Lambda leaving only the $LATEST version
        /// </summary>
        /// <param name="environment">AWS Environment details</param>
        public static async void CleanUpLambdaOlderVersions(AWSEnvironment environment)
        {
            Console.WriteLine("Process started...");
            var lambdas = await GetLambdaVersions(FunctionVersion.ALL, environment);
            var lambdasToDelete = lambdas.Where(x => x.Version != Constants.LambdaLastVersion).ToList();

            Console.WriteLine($"Lambda functions count: {lambdasToDelete.Count}");


            if (lambdasToDelete.Count > 0)
            {
                foreach (var lambda in lambdasToDelete)
                {
                   await DeleteLambda(lambda, environment);
                }
            }
            else
            {
                Console.WriteLine("No lambda functions found!!");
            }
            Console.WriteLine("Process completed");

        }

        private static async Task<bool> DeleteLambda(FunctionConfiguration lambda, AWSEnvironment environment)
        {
            try
            {
                var awsConfiguration = new AmazonLambdaConfig()
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(environment.Region)
                };

                var awsCredentials = new BasicAWSCredentials(environment.AccessKey, environment.SecretKey);

                using (var awsClient = new AmazonLambdaClient(awsCredentials, awsConfiguration))
                {
                    var response = await awsClient.DeleteFunctionAsync(new DeleteFunctionRequest
                    {
                        FunctionName = lambda.FunctionName,
                        Qualifier = lambda.Version //ARN
                    });
                    Console.WriteLine($"Lamba {lambda.FunctionName} deleted.");
                    return response.HttpStatusCode == HttpStatusCode.NoContent; //204
                };
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                
            }
            return false;
        }

        private static async Task<List<FunctionConfiguration>> GetLambdaVersions(FunctionVersion functionVersion, AWSEnvironment environment)
        {
            var versionNumber = functionVersion != null ? Constants.LambdaAll : Constants.LambdaLastVersion;

            Console.WriteLine($"Reading lambda function versions: {versionNumber}");

            var result = new List<FunctionConfiguration>();

            var awsConfiguration = new AmazonLambdaConfig()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(environment.Region)
            };

            var awsCredentials = new BasicAWSCredentials(environment.AccessKey, environment.SecretKey);

            string marker = null;
            using (var awsClient = new AmazonLambdaClient(awsCredentials, awsConfiguration))
            {
                do
                {
                    var response = await awsClient.ListFunctionsAsync(new ListFunctionsRequest
                    {
                        Marker = marker,
                        FunctionVersion = functionVersion
                    });
                    //marker =Task<List<ListFunctionsRequest>> response.Result.NextMarker;
                    marker = response.NextMarker;
                    result.AddRange(response.Functions.Where(x => x.FunctionName.StartsWith($"{environment.Name.ToString()}")));
                } while (!string.IsNullOrEmpty(marker));

                return result;
            }

        }

    }
}

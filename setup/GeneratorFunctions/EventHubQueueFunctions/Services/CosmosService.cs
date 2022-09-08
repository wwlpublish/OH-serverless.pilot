using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Host;
using ServerlessOpenhack.Models;

namespace ServerlessOpenhack.Services
{
    public static class CosmosService
    {
        private static HttpClient client = new HttpClient();

        public static async Task<List<OpenHackTeam>> GetTeamListAsync()
        {
            string region = Environment.GetEnvironmentVariable("REGION");
            string path = $"https://serverlessoh{region}-managementapi.azurewebsites.net/api/team/findAll";
            HttpResponseMessage response = await client.GetAsync(path);

            return response.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<OpenHackTeam>>(await response.Content.ReadAsStringAsync())
                : HandleCosmosFailureStatusCode(response);
        }

        public static async Task<List<OpenHackTeam>> GetRegisteredStorageAccountTeams(TraceWriter log)
        {
            var teams = await GetTeamListAsync();
            List<OpenHackTeam> registeredStorageAccountTeams = teams.Where(t => t.registeredStorageAccount == true).ToList();
            return registeredStorageAccountTeams;
        }

        public static async Task<List<OpenHackTeam>> GetRegisteredEventHubTeams(TraceWriter log)
        {
            var teams = await GetTeamListAsync();
            List<OpenHackTeam> registeredEventHubTeams = teams.Where(t => t.registeredEventHub == true).ToList();
            return registeredEventHubTeams;
        }

        public static async Task<List<OpenHackTeam>> GetRegisteredRatingsTeams(TraceWriter log)
        {
            var teams = await GetTeamListAsync();
            List<OpenHackTeam> registeredRatingsTeams = teams.Where(t => t.registeredRatings).ToList();
            return registeredRatingsTeams;
        }

        private static List<OpenHackTeam> HandleCosmosFailureStatusCode(HttpResponseMessage response)
        {
            throw new ExternalException(response.ReasonPhrase);
        }
    }
}

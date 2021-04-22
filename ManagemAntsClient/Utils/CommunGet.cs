using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;


namespace ManagemAntsClient.Utils
{
    public class CommunGet
    {
        public static async Task<List<Models.Task>> GetTaskByProjectId(string id, string filter, string myTask, long userId)
        {
            var filterVal = -1;
            switch (filter)
            {
                case "À faire":
                    filterVal = 0;
                    break;
                case "En cours":
                    filterVal = 1;
                    break;
                case "Fait":
                    filterVal = 2;
                    break;
                case "Rendu":
                    filterVal = 3;
                    break;
                default:
                    filterVal = -1;
                    break;
            }
            var client = Client.SetUpClient("task/" + id + "?filter=" + filterVal);
            HttpResponseMessage response = await Client.GetAsync(client, "");
            var tasks = new List<Models.Task>();
            if (response.IsSuccessStatusCode)
            {
                tasks = await JsonSerializer.DeserializeAsync<List<Models.Task>>(await response.Content.ReadAsStreamAsync());
            }
            return tasks = tasks.Where(x => !bool.Parse(myTask) || x.collaborators.Any(y => y.id == userId)).Reverse().ToList();
        }




        public static async Task<List<Models.User>> GetCollaboratorsByRole(string projectId, int roleValue)
        {
            var client = Client.SetUpClient("project/" + projectId + "/users/role/" + roleValue);
            HttpResponseMessage response = await Client.GetAsync(client, "");

            var collaborators = new List<Models.User>();
            if (response.IsSuccessStatusCode)
            {
                collaborators = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }
            return collaborators;
        }

        public static async Task<Models.Project> GetProjectById(string id)
        {
            var client = Client.SetUpClient("Project/" + id);
            HttpResponseMessage responce = await Client.GetAsync(client, "");
            var project = new List<Models.Project>();
            if (responce.IsSuccessStatusCode)
            {
                var p = (await responce.Content.ReadAsStringAsync());
                project = await JsonSerializer.DeserializeAsync<List<Models.Project>>(await responce.Content.ReadAsStreamAsync());
            }
            return project.FirstOrDefault();
        }

    }
}

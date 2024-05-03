using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using TeamContext = Microsoft.TeamFoundation.Core.WebApi.Types.TeamContext;

namespace BoardsLibrary;

public class AzureBoardsService
{
    private readonly string _pat;
    private readonly string _organizationUrl;
    private readonly string _projectName;

    public AzureBoardsService(string personalAccessToken, string organizationUrl, string projectName)
    {
        _pat = personalAccessToken;
        _organizationUrl = organizationUrl;
        _projectName = projectName;
    }

    private async Task<WorkItemTrackingHttpClient> GetWorkItemTrackingClient()
    {
        var connection = new VssConnection(new Uri(_organizationUrl), new VssBasicCredential(string.Empty, _pat));
        return connection.GetClient<WorkItemTrackingHttpClient>();
    }

    public async Task<List<WorkItem>> ListAllWorkItems()
    {
        var witClient = await GetWorkItemTrackingClient();

        var query = "SELECT [System.Id], [System.Title] FROM WorkItems";
        var result = await witClient.QueryByWiqlAsync(new Wiql { Query = query }, _projectName);
        var workItems = new List<WorkItem>();

        foreach (var workItemReference in result.WorkItems)
        {
            var workItem = await witClient.GetWorkItemAsync(workItemReference.Id);
            workItems.Add(workItem);
        }

        return workItems;
    }

    public async Task<List<WorkItem>> ListWorkItemsByType(string workItemType)
    {
        var witClient = await GetWorkItemTrackingClient();

        var query = $"SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.WorkItemType] = '{workItemType}'";
        var result = await witClient.QueryByWiqlAsync(new Wiql { Query = query }, _projectName);
        var workItems = new List<WorkItem>();

        foreach (var workItemReference in result.WorkItems)
        {
            var workItem = await witClient.GetWorkItemAsync(workItemReference.Id);
            workItems.Add(workItem);
        }

        return workItems;
    }

    public async Task<WorkItem> GetWorkItem(int workItemId)
    {
        var witClient = await GetWorkItemTrackingClient();
        return await witClient.GetWorkItemAsync(workItemId);
    }

    public async Task UpdateWorkItem(int workItemId, JsonPatchDocument patchDocument)
    {
        var witClient = await GetWorkItemTrackingClient();
        await witClient.UpdateWorkItemAsync(patchDocument, workItemId);
    }

    public async Task DeleteWorkItem(int workItemId)
    {
        var witClient = await GetWorkItemTrackingClient();
        await witClient.DeleteWorkItemAsync(workItemId);
    }

    // Sprint related methods

    private async Task<TeamHttpClient> GetTeamHttpClient()
    {
        var connection = new VssConnection(new Uri(_organizationUrl), new VssBasicCredential(string.Empty, _pat));
        return connection.GetClient<TeamHttpClient>();
    }
    
    private async Task<WorkHttpClient> GetWorkHttpClient()
    {
        var connection = new VssConnection(new Uri(_organizationUrl), new VssBasicCredential(string.Empty, _pat));
        return connection.GetClient<WorkHttpClient>();
    }

    public async Task<List<TeamSettingsIteration>> GetSprints()
    {
        var workClient = await GetWorkHttpClient();
        TeamContext teamContext = new(_projectName);
        return await workClient.GetTeamIterationsAsync(teamContext);
    }

    public async Task<TeamSettingsIteration> GetCurrentSprint()
    {
        var iterations = await GetSprints();
        return iterations.Find(iteration => iteration.Attributes.TimeFrame.ToString().Equals("current"));
    }

    public async Task<TeamSettingsIteration> CreateSprint(string sprintName, DateTime startDate, DateTime endDate)
    {
        var workClient = await GetWorkHttpClient();

        var newIteration = new TeamSettingsIteration
        {
            Name = sprintName,
            Attributes = new Microsoft.TeamFoundation.Work.WebApi.TeamIterationAttributes()
            {
                StartDate = startDate,
                FinishDate = endDate
            }
        };
        TeamContext teamContext = new(_projectName);
        return await workClient.PostTeamIterationAsync(newIteration, teamContext);
    }

    // public async Task UpdateSprint(TeamSettingsIteration sprint)
    // {
    //     var workClient = await GetWorkHttpClient();
    //     await workClient.UpdateTeamIterationAsync(sprint, _projectName, sprint.Id);
    // }
    //
    // public async Task AddWorkItemToSprint(int workItemId, TeamSettingsIteration sprint)
    // {
    //     var workClient = await GetWorkHttpClient();
    //
    //     var patchDocument = new JsonPatchDocument
    //     {
    //         new JsonPatchOperation
    //         {
    //             Operation = Operation.Add,
    //             Path = "/relations/-",
    //             Value = new
    //             {
    //                 rel = "ArtifactLink",
    //                 url = $"{_organizationUrl}/{_projectName}/_apis/wit/workItems/{workItemId}",
    //                 attributes = new { comment = "Added to sprint" }
    //             }
    //         }
    //     };
    //
    //     await workClient.UpdateTeamIterationAsync(patchDocument, _projectName, sprint.Id);
    // }
    //
    // public async Task AddWorkItemToCurrentSprint(int workItemId)
    // {
    //     var currentSprint = await GetCurrentSprint();
    //     await AddWorkItemToSprint(workItemId, currentSprint);
    // }
}
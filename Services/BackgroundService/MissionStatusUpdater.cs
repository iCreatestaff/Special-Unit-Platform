using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using sp_backend_March4.Models;
using WeatherApi;

public class MissionStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MissionStatusUpdater> _logger;
    private readonly TimeZoneInfo _localTimeZone;
    private readonly HttpClient _httpClient;

    public MissionStatusUpdater(IServiceProvider serviceProvider, ILogger<MissionStatusUpdater> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _localTimeZone = TimeZoneInfo.CreateCustomTimeZone("UTC+1", TimeSpan.FromHours(1), "UTC+1", "UTC+1");
        _httpClient = new HttpClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MissionStatusUpdater is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var nowUtc = DateTime.UtcNow;
                    var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, _localTimeZone);
                    var thresholdTime = nowLocal.AddMinutes(30);

                    _logger.LogInformation($"NowLocal: {nowLocal}, ThresholdTime: {thresholdTime}");

                    // 1. Find upcoming missions
                    var upcomingMissions = await _context.Missions
                        .Where(m => m.StartTime <= thresholdTime && m.StartTime > nowLocal)
                        .ToListAsync(stoppingToken);

                    _logger.LogInformation($"Found {upcomingMissions.Count} upcoming missions.");

                    foreach (var mission in upcomingMissions)
                    {
                        _logger.LogInformation($"Processing upcoming mission ID: {mission.Id}");

                        bool alreadyNotified = await _context.Notifications
                            .AnyAsync(n => n.Type == "mission" &&
                                           n.ReferenceId == mission.Id,
                                           stoppingToken);

                        if (alreadyNotified)
                        {
                            _logger.LogInformation($"Mission {mission.Id} already notified.");
                            continue;
                        }

                        var assignedAgentIds = await _context.AccountMissions
    .Where(am => am.MissionId == mission.Id)
    .Select(am => am.AccountId)
    .ToListAsync(stoppingToken);

                        if (assignedAgentIds != null && assignedAgentIds.Any())
                        {
                            _logger.LogInformation($"Mission {mission.Id} has {assignedAgentIds.Count} assigned agents.");

                            var agents = await _context.Accounts
                                .Where(a => assignedAgentIds.Contains(a.Id) && a.Latitude != null && a.Longitude != null)
                                .Select(a => new
                                {
                                    agent_id = a.Id,
                                    location = new double[] { a.Latitude!.Value, a.Longitude!.Value }
                                })
                                .ToListAsync(stoppingToken);

                            _logger.LogInformation($"Found {agents.Count} agents with valid locations for mission {mission.Id}.");

                            if (!agents.Any())
                            {
                                _logger.LogWarning($"No valid agent locations for mission {mission.Id}.");
                                continue;
                            }

                            if (string.IsNullOrEmpty(mission.Location))
                            {
                                _logger.LogWarning($"Mission {mission.Id} has no location specified.");
                                continue;
                            }

                            var locationParts = mission.Location.Split(',');

                            if (locationParts.Length != 2 ||
                                !double.TryParse(locationParts[0], out var latitude) ||
                                !double.TryParse(locationParts[1], out var longitude))
                            {
                                _logger.LogWarning($"Invalid location format for mission {mission.Id}: {mission.Location}");
                                continue;
                            }

                            var missionLocation = new double[] { latitude, longitude };

                            var data = new
                            {
                                mission = missionLocation,
                                agents = agents
                            };

                            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

                            try
                            {
                                _logger.LogInformation($"Sending agent assignment request for mission {mission.Id} to Flask server.");

                                var response = await _httpClient.PostAsync("http://127.0.0.1:5006/assign_agents", content, stoppingToken);
                                response.EnsureSuccessStatusCode();

                                var responseString = await response.Content.ReadAsStringAsync(stoppingToken);
                                var responseData = JsonSerializer.Deserialize<FlaskResponse>(responseString);

                                var agentIds = responseData?.fastest_agents?.Select(a => a.agent_id).ToList() ?? new List<int>();

                                _logger.LogInformation($"Flask response for mission {mission.Id}: Closest agents {string.Join(", ", agentIds)}");

                                var notification = new Notification
                                {
                                    Type = "mission",
                                    Details = $"Mission ID: {mission.Id} scheduled at {mission.StartTime} is due in 30 minutes. Closest agents: {string.Join(", ", agentIds)}",
                                    RecipientId = mission.AdminId,
                                    ReferenceId = mission.Id
                                };

                                _context.Notifications.Add(notification);
                                _logger.LogInformation($"Notification created for mission {mission.Id}.");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error contacting Flask server for mission {MissionId}", mission.Id);
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"No agents assigned to mission {mission.Id}.");
                        }

                    }

                    // 2. Start missions
                    var missionsToStart = await _context.Missions
                        .Where(m => m.StartTime <= nowLocal && m.Status != "Started" && m.Status != "Accomplished")
                        .ToListAsync(stoppingToken);

                    _logger.LogInformation($"Found {missionsToStart.Count} missions to start.");

                    foreach (var mission in missionsToStart)
                    {
                        mission.Status = "Started";
                        _logger.LogInformation($"Mission {mission.Id} started.");
                    }

                    // 3. Accomplish missions
                    var missionsToFinish = await _context.Missions
                        .Where(m => m.EndTime <= nowLocal && m.Status == "Started")
                        .ToListAsync(stoppingToken);

                    _logger.LogInformation($"Found {missionsToFinish.Count} missions to accomplish.");

                    foreach (var mission in missionsToFinish)
                    {
                        mission.Status = "Accomplished";
                        _logger.LogInformation($"Mission {mission.Id} accomplished.");
                    }

                    if (missionsToStart.Any() || missionsToFinish.Any() || upcomingMissions.Any())
                    {
                        await _context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Changes saved to database.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mission statuses.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private class FlaskResponse
    {
        public List<FastestAgent>? fastest_agents { get; set; }
    }

    private class FastestAgent
    {
        public int agent_id { get; set; }
    }
}

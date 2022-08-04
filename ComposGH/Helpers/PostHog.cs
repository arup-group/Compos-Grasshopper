﻿using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Newtonsoft.Json;

namespace ComposGH.Helpers
{
  public class PostHog
  {
    private static HttpClient _phClient = new HttpClient();
    internal static User CurrentUser = new User();

    public static async Task<HttpResponseMessage> SendToPostHog(string eventName, Dictionary<string, object> additionalProperties = null)
    {
      // posthog ADS plugin requires a user object
      User user = CurrentUser;

      Dictionary<string, object> properties = new Dictionary<string, object>() {
        { "distinct_id", user.userName },
        { "user", user },
        { "pluginName", ComposGHInfo.PluginName },
        { "version", ComposGHInfo.Vers },
        { "isBeta", ComposGHInfo.isBeta },
      };

      if (additionalProperties != null)
      {
        foreach (string key in additionalProperties.Keys)
          properties.Add(key, additionalProperties[key]);
      }

      var container = new PhContainer(eventName, properties);
      var body = JsonConvert.SerializeObject(container);
      var content = new StringContent(body, Encoding.UTF8, "application/json");
      var response = await _phClient.PostAsync("https://posthog.insights.arup.com/capture/", content);
      return response;
    }

    public static void AddedToDocument(GH_Component component)
    {
      string eventName = "AddedToDocument";
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        { "componentName", component.Name },
      };
      _ = PostHog.SendToPostHog(eventName, properties);
    }

    public static void ModelIO(string interactionType, int size = 0)
    {
      string eventName = "ModelIO";
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        { "interactionType", interactionType },
        { "size", size },
      };
      _ = PostHog.SendToPostHog(eventName, properties);
    }

    public static void PluginLoaded()
    {
      string eventName = "PluginLoaded";

      Dictionary<string, object> properties = new Dictionary<string, object>()
        {
          { "rhinoVersion", Rhino.RhinoApp.Version.ToString().Split('.')
             + "." + Rhino.RhinoApp.Version.ToString().Split('.')[1] },
          { "rhinoMajorVersion", Rhino.RhinoApp.ExeVersion },
          { "rhinoServiceRelease", Rhino.RhinoApp.ExeServiceRelease },
        };
      _ = PostHog.SendToPostHog(eventName, properties);
    }

    internal static void RemovedFromDocument(GH_Component component)
    {
      if (component.Attributes.Selected)
      {
        string eventName = "RemovedFromDocument";
        Dictionary<string, object> properties = new Dictionary<string, object>()
        {
          { "componentName", component.Name },
          { "runCount", component.RunCount },
        };
        _ = PostHog.SendToPostHog(eventName, properties);
      }
    }

    private class PhContainer
    {
      [JsonProperty("api_key")]
      string api_key { get; set; } = "phc_alOp3OccDM3D18xJTWDoW44Y1cJvbEScm5LJSX8qnhs";
      [JsonProperty("event")]
      string ph_event { get; set; }
      [JsonProperty("timestamp")]
      DateTime ph_timestamp { get; set; }
      public Dictionary<string, object> properties { get; set; }

      public PhContainer(string eventName, Dictionary<string, object> properties)
      {
        this.ph_event = eventName;
        this.properties = properties;
        this.ph_timestamp = DateTime.UtcNow;

      }
    }
  }
  internal class User
  {
    public string email { get; set; }
    public string userName { get; set; }

    internal User()
    {
      userName = Environment.UserName.ToLower();
      try
      {
        var task = Task.Run(() => UserPrincipal.Current.EmailAddress);
        if (task.Wait(TimeSpan.FromSeconds(2)))
        {
          if (task.Result.EndsWith("arup.com"))
            email = task.Result;
          else
          {
            email = task.Result.GetHashCode().ToString();
            userName = userName.GetHashCode().ToString();
          }
          return;
        }
      }
      catch (Exception) { }

      if (Environment.UserDomainName.ToLower() == "global")
        email = userName + "@arup.com";
      else
        userName = userName.GetHashCode().ToString();
    }
  }
}
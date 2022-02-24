using System.Diagnostics;
using HttpClient = System.Net.Http.HttpClient;
using HttpResponseMessage = System.Net.Http.HttpResponseMessage;
#if NET5_0_OR_GREATER
using JsonContent = System.Net.Http.Json.JsonContent;
#endif
using Task = System.Threading.Tasks.Task;
using Thread = System.Threading.Thread;

public class ItemTrackerConnection
{
	private string connectionUrl;
	private HttpClient client;
	public ItemTrackerConnection(string url)
    {
		connectionUrl = url;
	    client = new HttpClient();
	}

	/**
	 * Set a given value in the remote resource.
	 *
	 * The URL depends on how the server was executed, but must it be
	 * something like 'http://localhost:8000/tracker/<game>' (if running
	 * the server locally and on port 8000). '<game>' depends on which
	 * game is getting a resouce set in the tracker.
	 *
	 * @param base_url: The URL of the tracker service.
	 * @param key: Which resource in this tracker will be modified.
	 * @param val: Whether the resource is being set (true) or unset.
	 */
	public void set_bool(string key, bool val)
	{
		string uri = connectionUrl + "/" + key;

		Task.Run(async () => {
			HttpResponseMessage resp;

			if (val)
			{
				resp = await client.PostAsync(uri, null);
			}
			else
			{
				resp = await client.DeleteAsync(uri);
			}

			if (resp.IsSuccessStatusCode)
			{
				Debug.WriteLine($"{key} set to ${val} successfully!");
			}
			else
			{
				Debug.WriteLine($"Failed to set {key} to ${val}...");
			}
		});
	}

#if NET5_0_OR_GREATER
	struct payload {
		string id;
		string value;
	}

	/**
	 * Set a given value in the remote resource.
	 *
	 * The URL depends on how the server was executed, but must it be
	 * something like 'http://localhost:8000/tracker/<game>' (if running
	 * the server locally and on port 8000). '<game>' depends on which
	 * game is getting a resouce set in the tracker.
	 *
	 * The value will be encoded into a JSON object specifying both the
	 * resouce being modified as well as its value. For example:
	 *
	 * {
	 *     "id": "54-text",
	 *     "value": "2"
	 * }
	 *
	 * XXX: This was untested, as I don't have .NET 5 or greater installed
	 * (blame Unity, as I simply used whatever it installed...).
	 *
	 * @param uri: The URL of the tracker service.
	 * @param key: Which resource in this tracker will be modified.
	 * @param val: The new value of the resource.
	 */
	static void set_value(string uri, string key, string val) {
		payload p;

		p.id = key;
		p.value = val;

		JsonContent data = JsonContent.Create(p, typeof(payload));

		Task.Run( async () => {
			HttpResponseMessage resp;

			HttpClient client = new HttpClient();

			resp = await client.PostAsync(uri, data);

			if (resp.IsSuccessStatusCode) {
				Console.WriteLine($"{key} set successfully!");
			}
			else {
				Console.WriteLine($"Failed to set {key}...");
			}
		});
	}
#endif
}
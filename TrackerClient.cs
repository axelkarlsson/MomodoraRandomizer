using Generic = System.Collections.Generic;
using ByteArrayContent = System.Net.Http.ByteArrayContent;
using HttpClient = System.Net.Http.HttpClient;
using HttpResponseMessage = System.Net.Http.HttpResponseMessage;
using Encoding = System.Text.Encoding;
using CancellationTokenSource = System.Threading.CancellationTokenSource;
using Mutex = System.Threading.Mutex;
using Tasks = System.Threading.Tasks;
using Thread = System.Threading.Thread;
using JavaScriptSerializer = System.Web.Script.Serialization.JavaScriptSerializer;

/**
 * TrackerClient wraps and simplifies sending requests to a tracker.
 *
 * A client must be instanced for each desired game. For example, to access
 * Momodora: Reverie Under the Moonlight's tracker running locally, simply
 * instance it with:
 *
 *     TrackerClient tc;
 *     tc = new TrackerClient(http://localhost:8000/tracker/momo4);
 *
 * From this, the following operations are available on the client to
 * modify values in the tracker:
 *
 * - void Set(string resource);
 * - void Unset(string resource);
 * - void SetValue(string resource, string val);
 *
 * All of these operations are instantaneous and handle the request
 * asynchronously. The following operations are available to wait until a
 * resource has been completely set:
 *
 * - bool WaitResource(string resource);
 * - void WaitAll();
 *
 * Additionally, it's possible to cancel any on going (i.e., a initiated
 * but not yet finished) by calling:
 *
 * - void CancelAll();
 *
 * Lastly, to clear the game back to it's initial state, simply call the
 * blocking function:
 *
 * - bool ClearAll();
 *
 * The snippet bellow exemplifies setting the boolean resources "1", "2"
 * and "54", and the text resource "54-text"in the previously instanced
 * client, then it blocks and waits until everything is finished.
 *
 *     // Set the boolean value 1, 2 and 54
 *     tc.Set("1");
 *     tc.Set("2");
 *     tc.Set("54");
 *
 *     // Set the resource "54-text" to "some-value"
 *     tc.SetValue("54-text", "some-value");
 *
 *     // Wait until everything is set
 *     tc.WaitAll();
 */
public class TrackerClient {

	/** A HTTP task and its cancelation token. */
	private struct conn_data {
		public Tasks.Task<HttpResponseMessage> conn;
		public CancellationTokenSource cancel;
	}

	/** Synchronize accesses to conns. */
	private Mutex mut;

	/** Map of connections waiting for a response. */
	private Generic.Dictionary<string, conn_data> conns;

	/** Address where tracker requests shall be made. */
	private string base_url;

	/** Generic JSON encoder. */
	private JavaScriptSerializer json_enc;

	/**
	 * Create a new tracker client.
	 *
	 * The URL depends on how the server was executed, but must it be
	 * something like 'http://localhost:8000/tracker/<game>' (if running
	 * the server locally and on port 8000). '<game>' depends on which
	 * game is getting a resouce set in the tracker.
	 *
	 * @param base_url: The URL of the tracker service.
	 */
	public TrackerClient(string base_url) {
		this.conns = new Generic.Dictionary<string, conn_data>();
		this.base_url = base_url;
		this.mut = new Mutex();
		this.json_enc = new JavaScriptSerializer();
	}

	/**
	 * Set the given resource on the configured tracker.
	 *
	 * If there's already an ongoing connection to this resource, it will
	 * be canceled!
	 *
	 * @param resource: Which resource in this tracker will be modified.
	 */
	public void Set(string resource) {
		this.set_bool(resource, true);
	}

	/**
	 * Unset the given resource on the configured tracker.
	 *
	 * If there's already an ongoing connection to this resource, it will
	 * be canceled!
	 *
	 * @param resource: Which resource in this tracker will be modified.
	 */
	public void Unset(string resource) {
		this.set_bool(resource, false);
	}

	/**
	 * Block until the resource has completed.
	 *
	 * Block until the resource has completed, releasing its resources and
	 * removing it from the internal list of connections.
	 *
	 * @param resource: Which resource in this tracker will be modified.
	 * @return Whether the request existed and was completed successfully.
	 */
	public bool WaitResource(string resource) {
		this.mut.WaitOne();
		if (!this.conns.ContainsKey(resource)) {
			this.mut.ReleaseMutex();
			return false;
		}
		conn_data data = this.conns[resource];
		this.conns.Remove(resource);
		this.mut.ReleaseMutex();

		try {
			HttpResponseMessage resp;

			data.conn.Wait();
			resp = data.conn.Result;
			return resp.IsSuccessStatusCode;
		} catch (System.Exception) {
			return false;
		}
	}

	/**
	 * Block until every ongoing connection is completed.
	 *
	 * The connections are removed from the internal list when this
	 * function finishes running.
	 */
	public void WaitAll() {
		this.exec_all((string resource, conn_data c) => {
			try {
				c.conn.Wait();
			} catch (System.Exception) {
				/* Ignore errors or if the connection was cancelled. */
			}
		}, true);
	}

	/**
	 * Forcefully cancel any ongoing connection.
	 *
	 * The connections are removed from the internal list when this
	 * function finishes running.
	 */
	public void CancelAll() {
		this.exec_all((string resource, conn_data c) => {
			try {
				if (!c.conn.IsCompleted) {
					c.cancel.Cancel();
				}
				c.conn.Wait();
			} catch (System.Exception) {
				/* Ignore errors or if the connection was cancelled. */
			}
		}, true);
	}

	/**
	 * Call a given function on every connection.
	 *
	 * The functions are called with exclusive access to the list, so no
	 * function is able to disrupt calling the function on every
	 * connection. However, the called function SHALL NOT modify the list
	 * of connections!
	 *
	 * @param fn: The action to be called on each connection.
	 * @param clear: Remove every entry from the list of connections.
	 */
	private void exec_all(System.Action<string, conn_data> fn, bool clear) {
		this.mut.WaitOne();
		foreach (Generic.KeyValuePair<string, conn_data> tuple in this.conns) {
			/* Call the action on every value. */
			fn(tuple.Key, tuple.Value);
		}

		if (clear) {
			this.conns.Clear();
		}
		this.mut.ReleaseMutex();
	}

	/**
	 * Add a new connection to the internal list of connections.
	 *
	 * @param key: The resource being modified.
	 * @param data: The connection data.
	 */
	private void add_conn(string key, conn_data data) {
		/* If there's already an ongoing request to this same key, cancel
		 * it so only the new one is sent. */
		this.mut.WaitOne();
		if (this.conns.ContainsKey(key)) {
			conn_data oldData;

			oldData = this.conns[key];
			this.conns.Remove(key);

			if (!oldData.conn.IsCompleted) {
				oldData.cancel.Cancel();
			}
			try {
				/* If the old connection was canceled, this will throw an
				 * exception. */
				oldData.conn.Wait();
			} catch (System.Exception) {
				/* Ignore the exception */
			}
		}
		this.conns.Add(key, data);
		this.mut.ReleaseMutex();
	}

	/**
	 * Set a given value in the remote resource.
	 *
	 * The URL depends on how the server was executed, but must it be
	 * something like 'http://localhost:8000/tracker/<game>' (if running
	 * the server locally and on port 8000). '<game>' depends on which
	 * game is getting a resouce set in the tracker.
	 *
	 * @param key: Which resource in this tracker will be modified.
	 * @param val: Whether the resource is being set (true) or unset.
	 */
	private void set_bool(string key, bool val) {
		conn_data data = new conn_data();
		string uri = this.base_url + "/" + key;

		data.cancel = new CancellationTokenSource();

		HttpClient client = new HttpClient();
		if (val) {
			data.conn = client.PostAsync(uri, null, data.cancel.Token);
		}
		else {
			data.conn = client.DeleteAsync(uri, data.cancel.Token);
		}

		this.add_conn(key, data);
	}

	/** An ID/Value tuple, used in the POST request. */
	private struct payload {
		/** The resource of the ID. */
		public string id;
		/** The new value of the resource. */
		public string value;
	}

	/**
	 * Set a given value in the remote resource.
	 *
	 * The value will be encoded into a JSON object specifying both the
	 * resouce being modified as well as its value. For example:
	 *
	 * {
	 *     "id": "54-text",
	 *     "value": "2"
	 * }
	 *
	 * @param resource: Which resource in this tracker will be modified.
	 * @param val: The new value of the resource.
	 */
	public void SetValue(string resource, string val) {
		/* Encode the payload into a HttpContent. */
		payload p = new payload { id = resource, value = val };

		string json_str = this.json_enc.Serialize(p);
		byte[] json_bytes = Encoding.UTF8.GetBytes(json_str);
		ByteArrayContent content = new ByteArrayContent(json_bytes);

		/* Send the HTTP POST. */
		conn_data data = new conn_data();

		CancellationTokenSource tk = new CancellationTokenSource();
		data.cancel = tk;

		HttpClient client = new HttpClient();
		data.conn = client.PostAsync(this.base_url, content, tk.Token);

		this.add_conn(resource, data);
	}

	/**
	 * Clear every resource from the tracked game.
	 *
	 * This cancels any on going request and then send a new request to
	 * clear the game. Then it blocks until the game is properly cleared.
	 *
	 * @return Whether the game was properly cleared or not.
	 */
	public bool ClearAll() {
		this.CancelAll();

		conn_data data = new conn_data();

		data.cancel = new CancellationTokenSource();

		HttpClient client = new HttpClient();
		data.conn = client.DeleteAsync(this.base_url, data.cancel.Token);

		/* Use an invalid name as the resource. */
		string dummy_res = "../clear";
		this.add_conn(dummy_res, data);
		return this.WaitResource(dummy_res);
	}
}

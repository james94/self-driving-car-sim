using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System.Security.AccessControl;
using System.Globalization;

public class CommandServerFusedTerm2_3 : MonoBehaviour
{
	public CarRemoteControl CarRemoteControl;
	public Camera FrontFacingCamera;

	// Term 3 additions
	public GameObject Car; // assign in Term 3
	private perfect_controller _perfectController;
	private CarTraffic _carTraffic;
	private bool _term3Mode = false;

	private SocketIOComponent _socket;
	private CarController _carController;

	// Use this for initialization
	void Start()
	{
		_socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
		_socket.On("open", OnOpen);
		_socket.On("steer", OnSteer);
		_socket.On("manual", onManual);

		// Term 3-specific events
		_socket.On("control", OnControl);
		_socket.On("close", OnClose);

		// Resolve CarController from Term 1/2 or Term 3 target
		_carController = (CarRemoteControl != null) ? CarRemoteControl.GetComponent<CarController>() : null;
		if (Car != null)
		{
			var cc = Car.GetComponent<CarController>();
			if (cc != null) _carController = cc;

			_perfectController = Car.GetComponent<perfect_controller>();
			_carTraffic = Car.GetComponent<CarTraffic>();
			_term3Mode = (_perfectController != null);
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
		if (_term3Mode)
		{
			_perfectController.OpenScript();
		}
		EmitTelemetry(obj);
	}

	void OnClose(SocketIOEvent obj)
	{
		Debug.Log("Connection Closed");
		if (_term3Mode)
		{
			_perfectController.CloseScript();
		}
	}

	// 
	void onManual(SocketIOEvent obj)
	{
		Debug.Log("Triggered Callback Driving Manually");
		EmitTelemetry (obj);
	}

	void OnSteer(SocketIOEvent obj)
	{
		Debug.Log("Triggered Callback Driving Autonomously via PID Steering");

		Debug.Log("PID Control Active");

		try {
			// Add null check
			if(obj.data == null) {
				Debug.LogError("Empty steering command received");
				return;
			}

			// Add parse error handling
			JSONObject jsonObject = obj.data;

			Debug.Log($"Full JSON Structure: {jsonObject.Print(true)}");

			if(!jsonObject.HasField("steering_angle") || !jsonObject.HasField("throttle")) {
				Debug.LogError($"Missing fields. Actual data: {string.Join(",", jsonObject.keys)}");
				return;
			}

			// Add format verification
			float steeringAngle = jsonObject.GetField("steering_angle").f;
			float throttle = jsonObject.GetField("throttle").f;

			Debug.Log($"Raw steeringAngle = {steeringAngle} (Type: {jsonObject.GetField("steering_angle").type})");
			Debug.Log($"Raw throttle = {throttle} (Type: {jsonObject.GetField("throttle").type})");

			CarRemoteControl.SteeringAngle = Mathf.Clamp(steeringAngle, -1, 1);
			CarRemoteControl.Acceleration = Mathf.Clamp(throttle, 0, 1);

			EmitTelemetry(obj);
		} catch(Exception ex) {
			Debug.LogError($"Steering error: {ex}\nFull JSON: {obj.data}");
		}
	}

	// Term 3: accept planned trajectory from the client
	void OnControl(SocketIOEvent obj)
	{
		if (!_term3Mode || _perfectController == null)
		{
			Debug.LogWarning("Received 'control' but Term 3 mode is not active.");
			return;
		}

		try
		{
			JSONObject jsonObject = obj.data;
			var next_x = jsonObject.GetField("next_x");
			var next_y = jsonObject.GetField("next_y");

			if (next_x == null || next_y == null || next_x.Count != next_y.Count)
			{
				Debug.LogError("Invalid 'control' payload.");
				return;
			}

			List<float> xs = new List<float>();
			List<float> ys = new List<float>();
			for (int i = 0; i < next_x.Count; i++)
			{
				xs.Add(next_x[i].f);
				ys.Add(next_y[i].f);
			}

			_perfectController.setControlPath(xs, ys);
			_perfectController.setSimulatorProcess();

			EmitTelemetry(obj);
		}
		catch (Exception ex)
		{
			Debug.LogError($"Control error: {ex}");
		}
	}

	// Term 3: convert Unity yaw to conventional math orientation
	float convertAngle(float psi) {
		if (psi >= 0 && psi <= 90) {
			return 90 - psi;
		}
		else if (psi > 90 && psi <= 180) {
			return 360 - (psi - 90);
		}
		else if (psi > 180 && psi <= 270) {
			return 270 - (psi - 180);
		}
		return 180 - (psi - 270);
	}

	void EmitTelemetry(SocketIOEvent obj)
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			try {
				// Term 3 telemetry
				if (_term3Mode && _perfectController != null && Car != null)
				{
					if (!_perfectController.isServerProcess())
					{
						_socket.Emit("telemetry", new JSONObject());
					}
					else
					{
						_perfectController.ServerPause();

						// Collect Term 3 Data
						Dictionary<string, JSONObject> data = new Dictionary<string, JSONObject>();
						data["x"] = new JSONObject(Car.transform.position.x);
						data["y"] = new JSONObject(Car.transform.position.z);
						data["yaw"] = new JSONObject(convertAngle(Car.transform.rotation.eulerAngles.y));
						data["speed"] = new JSONObject(_carController != null ? _carController.CurrentSpeed : 0f);

						var carAI = (CarAIControl) Car.GetComponent(typeof(CarAIControl));
						List<float> frenet_values = carAI.getThisFrenetFrame();
						data["s"] = new JSONObject(frenet_values[0]);
						data["d"] = new JSONObject(frenet_values[1]);

						// Previous Path data
						var previous_path_x = _perfectController.previous_path_x();
						var previous_path_y = _perfectController.previous_path_y();

						JSONObject arr_x = new JSONObject(JSONObject.Type.ARRAY);
						JSONObject arr_y = new JSONObject(JSONObject.Type.ARRAY);
						for (int i = 0; i < previous_path_x.Count; i++)
						{
							arr_x.Add(previous_path_x[i]);
							arr_y.Add(previous_path_y[i]);
						}
						data["previous_path_x"] = arr_x;
						data["previous_path_y"] = arr_y;

						// End path S and D values
						var end_path_s = 0.0f;
						var end_path_d = 0.0f;
						if (previous_path_x.Count > 0)
						{
							List<float> fr = carAI.getFrenetFrame(
								previous_path_x[previous_path_x.Count - 1],
								previous_path_y[previous_path_y.Count - 1]);
							end_path_s = fr[0];
							end_path_d = fr[1];
						}
						data["end_path_s"] = new JSONObject(end_path_s);
						data["end_path_d"] = new JSONObject(end_path_d);

						if (_carTraffic == null) _carTraffic = Car.GetComponent<CarTraffic>();
						data["sensor_fusion"] = new JSONObject(_carTraffic.example_sensor_fusion());

						_socket.Emit("telemetry", new JSONObject(data));
					}

					return; // don't fall through to Term 1/2
				}

				// Term 1/2 telemetry (existing behavior)
				print("Attempting to Send...");
				// send only if it's not being manually driven
				if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
					_socket.Emit("telemetry", new JSONObject());
				}
				else {
					// Add CTE calculation (replace with your actual CTE logic)
					float currentCTE = CalculateCTE();

					// Collect Simulated Data from the Car
					JSONObject telemetryData = new JSONObject(JSONObject.Type.OBJECT);

					// Add fields individually
					telemetryData.AddField("cte", currentCTE);
					telemetryData.AddField("steering_angle", _carController != null ? _carController.CurrentSteerAngle : 0f);
					telemetryData.AddField("throttle", _carController != null ? _carController.AccelInput : 0f);
					telemetryData.AddField("speed", _carController != null ? _carController.CurrentSpeed : 0f);
					telemetryData.AddField("image", Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera)));
					_socket.Emit("telemetry", telemetryData);
				}
			} catch(Exception ex) {
				Debug.LogError($"Telemetry error: {ex}");
			}
		});
	}

	// Add this temporary CTE calculation method
	float CalculateCTE()
	{
		// Replace with your actual CTE calculation logic
		return UnityEngine.Random.Range(0.0f, 1.0f);
	}
}